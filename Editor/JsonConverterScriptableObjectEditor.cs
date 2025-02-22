using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JsonConverterScriptableObject))]
public class JsonConverterScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        JsonConverterScriptableObject converter = (JsonConverterScriptableObject)target;

        DrawDefaultInspector(); // Draws the default Inspector UI for selecting multiple files

        if (GUILayout.Button("Convert and Save"))
        {
            converter.ConvertAndSave();
        }
    }
}
