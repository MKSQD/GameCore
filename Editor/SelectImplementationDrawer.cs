using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameCore {
    [CustomPropertyDrawer(typeof(SelectImplementationAttribute))]
    public class SelectImplementationDrawer : PropertyDrawer {
        Type[] _implementations;
        int _implementationTypeIndex;



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (_implementations == null) {
                _implementations = GetImplementations((attribute as SelectImplementationAttribute).FieldType)
                    .Where(impl => !impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
            }


            EditorGUI.BeginProperty(position, label, property);

            if (property.managedReferenceValue == null) {
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width - 80, position.height), new GUIContent(label.text + ": "));
                position.x += 80;
                position.width -= 80;

                if (GUI.Button(position, "create")) {
                    void handleItemClicked(object parameter) {
                        property.managedReferenceValue = Activator.CreateInstance((Type)parameter);
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    GenericMenu menu = new();
                    foreach (var impl in _implementations) {
                        menu.AddItem(new GUIContent(impl.FullName), false, handleItemClicked, impl);
                    }
                    menu.DropDown(position);
                }
            } else {
                EditorGUI.PropertyField(position, property, new GUIContent(label.text + ": " + property.managedReferenceValue.GetType()), true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public static Type[] GetImplementations(Type interfaceType) {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
        }
    }
}