using System;
using UnityEngine;

namespace GameCore {
    public class SelectImplementationAttribute : PropertyAttribute {
        public Type FieldType;

        public SelectImplementationAttribute(Type fieldType) {
            FieldType = fieldType;
        }
    }
}