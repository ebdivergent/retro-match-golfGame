using UnityEditor;
using UnityEngine;

public class ApCrEnumFlagsAttribute : PropertyAttribute
{
    public ApCrEnumFlagsAttribute() { }
}

[CustomPropertyDrawer(typeof(ApCrEnumFlagsAttribute))]
public class ApCrEnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}