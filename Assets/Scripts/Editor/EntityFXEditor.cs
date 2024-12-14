using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityFX))]
public class EntityFXEditor : Editor
{
    SerializedProperty flashDurationProp;

    private void OnEnable()
    {
        flashDurationProp = serializedObject.FindProperty("flashDuration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector
        DrawDefaultInspector();

        EditorGUILayout.Slider(flashDurationProp, 0f, 1f, new GUIContent("Flash Duration"));
        flashDurationProp.floatValue = Mathf.Round(flashDurationProp.floatValue * 10) / 10f;

        serializedObject.ApplyModifiedProperties();
    }
}
