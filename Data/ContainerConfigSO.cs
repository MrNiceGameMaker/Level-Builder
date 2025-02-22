using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ContainerConfig", menuName = "LevelBuilding/ContainerConfig")]
public class ContainerConfigSO : ScriptableObject
{
    [Header("Container and Platform Settings")]
    [Range(1, 2000)] public int minContainers = 1; // Minimum number of containers
    [Range(1, 2000)] public int maxContainers = 15; // Maximum number of containers
    [Range(0, 10)] public int minPlatformsPerContainer = 1;
    [Range(0, 10)] public int maxPlatformsPerContainer = 10;
    [Range(0, 10)] public int minActivePlatformsPerContainer = 1;

    [Header("Platform Configurations")]
    public List<SavedPlatformConfigSO> platformConfigs;

    private void OnValidate()
    {
        // Ensure minContainers does not exceed maxContainers
        if (minContainers > maxContainers)
        {
            minContainers = maxContainers;
        }

        // Ensure minPlatformsPerContainer does not exceed maxPlatformsPerContainer
        if (minPlatformsPerContainer > maxPlatformsPerContainer)
        {
            minPlatformsPerContainer = maxPlatformsPerContainer;
        }

        // Ensure minActivePlatformsPerContainer does not exceed maxPlatformsPerContainer
        if (minActivePlatformsPerContainer > maxPlatformsPerContainer)
        {
            minActivePlatformsPerContainer = maxPlatformsPerContainer;
        }
    }

    public List<SavedContainer> GenerateContainers(bool isFirstConfig)
    {
        List<SavedContainer> containers = new List<SavedContainer>();
        int numberOfContainers = Random.Range(minContainers, maxContainers + 1);

        // Check if this is the first container of the first config
        if (isFirstConfig)
        {
            SavedContainer firstContainer = new SavedContainer();
            int firstContainerPlatforms = maxPlatformsPerContainer;

            for (int j = 0; j < firstContainerPlatforms; j++)
            {
                SavedPlatform easyPlatform = new SavedPlatform(true);  // All easy platforms
                firstContainer.savedPlatforms.Add(easyPlatform);
            }
            containers.Add(firstContainer);
        }

        // Generate remaining containers with random configurations
        for (int i = isFirstConfig ? 1 : 0; i < numberOfContainers; i++)
        {
            SavedContainer newContainer = new SavedContainer();
            int platformCount = Random.Range(minPlatformsPerContainer, maxPlatformsPerContainer + 1);
            int activePlatforms = 0;

            // Generate platforms with random configurations
            for (int j = 0; j < platformCount; j++)
            {
                SavedPlatformConfigSO config = platformConfigs[Random.Range(0, platformConfigs.Count)];
                SavedPlatform platform = new SavedPlatform(config);

                if (platform.isActive)
                {
                    activePlatforms++;
                }

                newContainer.savedPlatforms.Add(platform);
            }

            // Ensure minimum active platforms requirement is met
            if (platformCount < minActivePlatformsPerContainer)
            {
                foreach (var platform in newContainer.savedPlatforms)
                {
                    platform.isActive = true;
                }
            }
            else if (activePlatforms < minActivePlatformsPerContainer)
            {
                List<int> inactiveIndexes = new List<int>();

                for (int j = 0; j < newContainer.savedPlatforms.Count; j++)
                {
                    if (!newContainer.savedPlatforms[j].isActive)
                    {
                        inactiveIndexes.Add(j);
                    }
                }

                while (activePlatforms < minActivePlatformsPerContainer && inactiveIndexes.Count > 0)
                {
                    int randomIndex = Random.Range(0, inactiveIndexes.Count);
                    int platformIndex = inactiveIndexes[randomIndex];

                    newContainer.savedPlatforms[platformIndex].isActive = true;
                    activePlatforms++;

                    inactiveIndexes.RemoveAt(randomIndex);
                }
            }

            containers.Add(newContainer);
        }

        return containers;
    }
}
