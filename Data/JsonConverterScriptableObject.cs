#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "JsonConverter", menuName = "Tools/Json Converter")]
public class JsonConverterScriptableObject : ScriptableObject
{
    public List<Object> jsonFiles = new List<Object>(); // List to hold selected JSON files

    public void ConvertAndSave()
    {
        foreach (Object fileObj in jsonFiles)
        {
            if (fileObj == null) continue;

            string filePath = AssetDatabase.GetAssetPath(fileObj);
            string jsonContent = File.ReadAllText(filePath);
            var containers = JsonConvert.DeserializeObject<JsonRoot>(jsonContent);
            var minimalData = new List<string>();

            foreach (var container in containers.containers)
            {
                var containerData = new List<string>(); // Temporary list to hold platforms for each container

                foreach (var platform in container.savedPlatforms)
                {
                    if (!platform.isActive)
                    {
                        // For inactive platforms, include only selectedDifficulty and isActive
                        containerData.Add(JsonConvert.SerializeObject(new List<object>
                    {
                        platform.selectedDifficulty,
                        platform.isActive
                    }));
                        continue;
                    }

                    // Build a single-line JSON array for each active platform
                    string platformData = JsonConvert.SerializeObject(new List<object>
                {
                    platform.selectedDifficulty,
                    platform.isActive,
                    platform.isVertical,
                    platform.isMoving,
                    platform.isRotatingZAxis,
                    platform.isRotatingYAxis,
                    Mathf.Round((float)platform.movementSpeed * 100) / 100f,
                    Mathf.Round((float)platform.rotateSpeedZAxis * 100) / 100f,
                    Mathf.Round((float)platform.rotateSpeedYAxis * 100) / 100f,
                    platform.firstPlatform
                });

                    containerData.Add(platformData);
                }

                // Add each container's platforms as a JSON array and add to the main list
                minimalData.Add("[" + string.Join(",\n", containerData) + "]");
            }

            // Define comment lines
            string comments = "// difficulty scale, is active, is vertical, is moving, rotates on Z axis, rotates on Y axis,// movement speed, rotate speed on Z axis, rotate speed on X axis, first platform\n";

            // Format output JSON with each container's platforms grouped in its own JSON array
            string minimalJson = comments + "[\n" + string.Join(",\n", minimalData) + "\n]";

            // Save the converted JSON in a folder called "smallJson" in the same directory as the original file
            string directoryPath = Path.Combine(Path.GetDirectoryName(filePath), "smallJson");
            Directory.CreateDirectory(directoryPath); // Create folder if it doesn't exist
            string newFilePath = Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(filePath) + "_small.json");

            File.WriteAllText(newFilePath, minimalJson);

            Debug.Log($"Converted and saved JSON file to {newFilePath}");
        }

        // Refresh the AssetDatabase to show new files in Unity
        AssetDatabase.Refresh();
    }



}

// Renamed classes to avoid conflicts
public class JsonRoot
{
    public List<JsonContainer> containers;
}

public class JsonContainer
{
    public List<JsonPlatform> savedPlatforms;
}

public class JsonPlatform
{
    public int selectedDifficulty;
    public bool isActive;
    public bool isVertical;
    public bool isMoving;
    public bool isRotatingZAxis;
    public bool isRotatingYAxis;
    public double movementSpeed;
    public double rotateSpeedZAxis;
    public double rotateSpeedYAxis;
    public bool firstPlatform;
}
#endif