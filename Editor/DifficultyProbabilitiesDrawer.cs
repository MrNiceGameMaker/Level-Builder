using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SavedPlatformConfigSO.DifficultyProbabilities))]
public class DifficultyProbabilitiesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get individual properties
        SerializedProperty easyChance = property.FindPropertyRelative("easyChance");
        SerializedProperty mediumChance = property.FindPropertyRelative("mediumChance");
        SerializedProperty hardChance = property.FindPropertyRelative("hardChance");

        // Define initial slider height
        float lineHeight = EditorGUIUtility.singleLineHeight + 2;

        // Easy Chance Slider
        position.height = lineHeight;
        easyChance.intValue = DrawSliderWithAdjustments(position, "Easy", easyChance.intValue, mediumChance, hardChance);

        // Move to the next line
        position.y += lineHeight;

        // Medium Chance Slider
        mediumChance.intValue = DrawSliderWithAdjustments(position, "Medium", mediumChance.intValue, easyChance, hardChance);

        // Move to the next line
        position.y += lineHeight;

        // Hard Chance Slider
        hardChance.intValue = DrawSliderWithAdjustments(position, "Hard", hardChance.intValue, easyChance, mediumChance);

        EditorGUI.EndProperty();
    }

    private int DrawSliderWithAdjustments(Rect position, string label, int value, SerializedProperty prop1, SerializedProperty prop2)
    {
        // Draw the slider with the current value
        value = EditorGUI.IntSlider(position, label, value, 0, 100);

        // Calculate total of all three values
        int total = value + prop1.intValue + prop2.intValue;

        // If the total exceeds 100, adjust other properties to keep the total at 100
        if (total > 100)
        {
            int overflow = total - 100;

            // Calculate how much to reduce from each other property
            int prop1Reduction = Mathf.Min(overflow, prop1.intValue);
            prop1.intValue -= prop1Reduction;
            overflow -= prop1Reduction;

            int prop2Reduction = Mathf.Min(overflow, prop2.intValue);
            prop2.intValue -= prop2Reduction;
        }

        return value;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Three lines, each with its own slider
        return (EditorGUIUtility.singleLineHeight + 2) * 3;
    }
}
