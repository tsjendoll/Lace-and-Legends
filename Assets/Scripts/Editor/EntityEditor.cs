using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity), true)]
public class EntityEditor : Editor
{
    SerializedProperty knockbackDurationProp;
    SerializedProperty knockbackDirectionProp;

    private void OnEnable()
    {
        // Find properties
        knockbackDurationProp = serializedObject.FindProperty("knockbackDuration");
        knockbackDirectionProp = serializedObject.FindProperty("knockbackDirection");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector
        DrawDefaultInspector();

        // Add space and header for Knockback Info
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Knockback Info", EditorStyles.boldLabel);

        // Add the slider for knockbackDuration
        EditorGUILayout.Slider(knockbackDurationProp, 0f, 0.1f, new GUIContent("Knockback Duration"));
        knockbackDurationProp.floatValue = Mathf.Round(knockbackDurationProp.floatValue * 100f) / 100f;

        // Add the vector field for knockbackDirection
        knockbackDirectionProp.vector2Value = EditorGUILayout.Vector2Field("Knockback Direction", knockbackDirectionProp.vector2Value);

        serializedObject.ApplyModifiedProperties();
    }
}
