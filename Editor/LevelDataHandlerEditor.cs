using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDataHandler))]
public class LevelDataHandlerEditor : Editor
{
    private int startLevelNumber = 1;
    private int numberOfLevelsToCreate = 1;
    private int worldNumber = 1;

    public override void OnInspectorGUI()
    {
        // Draw default inspector for LevelDataHandler fields
        DrawDefaultInspector();

        // Input fields for world number, start level, and number of levels
        worldNumber = EditorGUILayout.IntField("World Number", worldNumber);
        startLevelNumber = EditorGUILayout.IntField("Start Level Number", startLevelNumber);
        numberOfLevelsToCreate = EditorGUILayout.IntField("Number of Levels to Create", numberOfLevelsToCreate);

        // Generate Levels button
        if (GUILayout.Button("Generate and Save Levels"))
        {
            GenerateAndSaveLevels();
        }
    }

    private void GenerateAndSaveLevels()
    {
        LevelDataHandler levelDataHandler = (LevelDataHandler)target;
        string worldFolder = $"Levels/LevelsJson/World {worldNumber}";

        for (int i = 0; i < numberOfLevelsToCreate; i++)
        {
            int currentLevelNumber = startLevelNumber + i;
            string fileName = $"World{worldNumber} - Level {currentLevelNumber}";

            // Generate the container data for each level
            levelDataHandler.GenerateContainerData();

            // Save the generated data to a JSON file in the specified world folder
            levelDataHandler.SaveToJson(fileName, worldFolder);

            Debug.Log($"Generated and saved {fileName}.json in {worldFolder}");
        }

        // Refresh the Asset Database to make sure files appear in the project
        AssetDatabase.Refresh();
    }
}
