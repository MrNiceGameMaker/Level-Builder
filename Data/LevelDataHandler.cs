using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataHandler", menuName = "LevelBuilding/LevelDataHandler")]
public class LevelDataHandler : ScriptableObject
{
    [Header("List Building")]
    [SerializeField] private List<ContainerConfigSO> containerConfigs; // List of Container Configurations
    [SerializeField] private int repeatCount = 1; // Number of times to repeat the pattern, settable in the Inspector

    public List<SavedContainer> containerData = new List<SavedContainer>();

    // Generates container data based on all container configurations sequentially
    public void GenerateContainerData()
    {
        containerData.Clear();
        bool isFirstPlatform = true; // Track if it's the very first platform of the first config

        // Repeat the generation as many times as specified
        for (int r = 0; r < repeatCount; r++)
        {
            // Loop through each configuration in the list
            for (int i = 0; i < containerConfigs.Count; i++)
            {
                bool isFirstConfig = (isFirstPlatform && i == 0);  // Apply all-easy only on the first platform of the first config
                List<SavedContainer> generatedContainers = containerConfigs[i].GenerateContainers(isFirstConfig);
                containerData.AddRange(generatedContainers);

                // After the first iteration, set isFirstPlatform to false
                if (isFirstPlatform) isFirstPlatform = false;
            }
        }
    }

    // Save the generated level data to a JSON file in a specific path
    public void SaveToJson(string fileName, string folderPath)
    {
        string directoryPath = Path.Combine(Application.dataPath, folderPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, fileName + ".json");
        SavedLevel data = new SavedLevel(containerData);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to " + filePath);
    }

    // Load level data from a JSON file
    public void LoadFromJson(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SavedLevel data = JsonUtility.FromJson<SavedLevel>(json);
            containerData = data.containers;
            Debug.Log("Data loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
        }
    }
}
