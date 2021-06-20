using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class WizardSimulatePhysics : ScriptableWizard {
    struct SimulatedBody {
        public readonly Rigidbody Rigidbody;
        public readonly bool IsSelected;
        readonly Vector3 originalPosition;
        readonly Quaternion originalRotation;
        readonly Transform transform;

        public SimulatedBody(Rigidbody rigidbody, bool selected) {
            Rigidbody = rigidbody;
            IsSelected = selected;
            transform = rigidbody.transform;
            originalPosition = rigidbody.position;
            originalRotation = rigidbody.rotation;
        }

        public void Reset() {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            if (Rigidbody != null) {
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

    [Range(0, 600)]
    public int MaxIterations = 200;
    public Vector3 InitialWorldForce = Vector3.zero;

    List<GameObject> initialSelection;
    SimulatedBody[] simulatedBodies;
    List<Rigidbody> generatedRigidbodies;
    List<Collider> generatedColliders;

    EditorCoroutine previewSimulationCoroutine;

    [MenuItem("Edit/Simulate Physics For Selected #G", false, 142)]
    static void CreateWizard() {
        ScriptableWizard.DisplayWizard<WizardSimulatePhysics>("Simulate Physics For Selected", "Apply");
    }

    [MenuItem("Edit/Simulate Physics For Selected #G", true)]
    public static bool CreateWizardValidate() {
        return Selection.gameObjects.Length > 0;
    }

    void OnWizardCreate() {
        if (previewSimulationCoroutine != null) {
            EditorCoroutineUtility.StopCoroutine(previewSimulationCoroutine);
        }

        var recordedPoses = new List<Pose>();
        foreach (var body in simulatedBodies) {
            if (body.IsSelected) {
                var transform = body.Rigidbody.transform;
                recordedPoses.Add(new Pose(transform.position, transform.rotation));
            }
        }

        // Resimulate to allow for proper undo
        ResetAll();

        int i = 0;
        foreach (var body in simulatedBodies) {
            if (body.IsSelected) {
                Undo.RecordObject(body.Rigidbody.transform, "Simulated Physics");
                var pose = recordedPoses[i++];
                var transform = body.Rigidbody.transform;
                transform.position = pose.position;
                transform.rotation = pose.rotation;
            }
        }

        RemoveAutoGeneratedComponents();

        // Make sure we are not resetting objects when destroying the wizard
        simulatedBodies = new SimulatedBody[0];
    }

    void OnWizardUpdate() {
        if (initialSelection == null) {
            initialSelection = new List<GameObject>();
            foreach (var go in Selection.gameObjects) {
                if (go.scene == null)
                    continue;

                initialSelection.Add(go);
            }

            AutoGenerateComponents();

            simulatedBodies = FindObjectsOfType<Rigidbody>().Select(rb => new SimulatedBody(rb, Selection.Contains(rb.gameObject))).ToArray();
        }

        ResetAll();
        if (previewSimulationCoroutine != null) {
            EditorCoroutineUtility.StopCoroutine(previewSimulationCoroutine);
        }
        previewSimulationCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(SimulateAfterDelay());
    }



    void OnDestroy() {
        if (previewSimulationCoroutine != null) {
            EditorCoroutineUtility.StopCoroutine(previewSimulationCoroutine);
        }

        ResetAll();
        RemoveAutoGeneratedComponents();
    }

    void ResetAll() {
        foreach (SimulatedBody body in simulatedBodies) {
            body.Reset();
        }
    }

    IEnumerator SimulateAfterDelay() {
        // Small delay to make tweaking faster
        for (int i = 0; i < 50; ++i)
            yield return null;

        foreach (var body in simulatedBodies) {
            if (body.IsSelected) {
                body.Rigidbody.velocity = InitialWorldForce;
            }
        }

        Physics.autoSimulation = false;
        for (int i = 0; i < MaxIterations; ++i) {
            Physics.Simulate(Time.fixedDeltaTime);
            if (simulatedBodies.All(body => body.Rigidbody.IsSleeping() || !body.IsSelected))
                break;

            if (i % 10 == 0) {
                foreach (var body in simulatedBodies) {
                    if (!body.IsSelected) {
                        body.Reset();
                    }
                }

                yield return null;
            }
        }
        Physics.autoSimulation = true;

        foreach (var body in simulatedBodies) {
            if (!body.IsSelected) {
                body.Reset();
            }
        }
    }

    void AutoGenerateComponents() {
        generatedRigidbodies = new List<Rigidbody>();
        generatedColliders = new List<Collider>();

        foreach (var go in initialSelection) {
            if (go.GetComponent<Rigidbody>() == null) {
                generatedRigidbodies.Add(go.AddComponent<Rigidbody>());
            }
            if (go.GetComponent<Collider>() == null) {
                generatedColliders.Add(go.AddComponent<BoxCollider>());
            }
        }
    }

    void RemoveAutoGeneratedComponents() {
        foreach (Rigidbody rb in generatedRigidbodies) {
            DestroyImmediate(rb);
        }
        foreach (Collider c in generatedColliders) {
            DestroyImmediate(c);
        }

        generatedRigidbodies.Clear();
        generatedColliders.Clear();
    }
}