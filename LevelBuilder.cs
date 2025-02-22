using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJSON;
using UnityEngine.SceneManagement;


public class LevelBuilder : MonoBehaviour
{
    [Header("Prefab Building")]
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private GameObject easyPlatform;
    [SerializeField] private GameObject mediumPlatform;
    [SerializeField] private GameObject hardPlatform;
    [SerializeField] private GameObject finalContainerPrefab;

    [Header("Settings")]
    [SerializeField] private bool useJsonList = true; // Toggle to choose between JSON list or infinite generation
    [SerializeField] BoolSO isLevels;
    [SerializeField] IntSO currentWorldSO;
    [SerializeField] private int maxActiveContainers = 10;
    [SerializeField] private float containerSpawnInterval = 0.35f;
    [SerializeField] private float playerPassThreshold = 5.0f;

    [Header("Player Reference")]
    //[SerializeField] private PlayerLocationRefference playerLocation;
    [SerializeField] private Transform playerTransform;

    [Header("World and Levels")]
    public WorldData worldData;

    [Header("Data Manager")]
    [SerializeField] private LevelDataHandler levelDataHandler;

    [Header("Current Level Info")]
    [SerializeField] private int currentLevelIndex = 0; // Tracks the current level within the world (visible in inspector)
    [SerializeField] public int CurrentWorldIndex { get; private set; } = 0; // Tracks the current world (visible in inspector)

    private int currentContainerIndex = 0; // Tracks which container is currently loaded
    private List<GameObject> activeContainers = new List<GameObject>(); // List to manage active containers
    private List<SavedContainer> containerData; // Holds the list of containers for the current level
    private bool finalContainerPlaced = false; // Flag to check if the final container is placed
    private bool infiniteModeActive = false; // Flag for infinite level mode
    private float lastContainerPositionZ = 0f; // Tracks the Z position for the next container in infinite mode

    public bool IsNextLevelANewWorld => currentLevelIndex >= worldData.worlds[CurrentWorldIndex].levels.Count;

    void Start()
    {
        useJsonList = isLevels.value;
        LoadPlayerProgress();

        if (useJsonList)
        {
            LoadCurrentLevel();
            StartCoroutine(LoadInitialContainers());
        }
        else
        {
            ActivateInfiniteMode();
        }
    }
/*
    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadPlayerProgress;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadPlayerProgress;
    }*/

    void Update()
    {
        CheckAndDestroyPassedPlatforms();
    }

    private void LoadCurrentLevel()
    {
        if (currentLevelIndex >= worldData.worlds[CurrentWorldIndex].levels.Count && CurrentWorldIndex < worldData.worlds.Count)
        {
            currentLevelIndex = 0;
            CurrentWorldIndex++;
        }
        else if (CurrentWorldIndex >= worldData.worlds.Count)
        {
            CurrentWorldIndex--;
        }
        currentWorldSO.value = CurrentWorldIndex;
        TextAsset levelFile = worldData.worlds[CurrentWorldIndex].levels[currentLevelIndex];
        JSONNode rootNode = JSON.Parse(levelFile.text); // Parse the compact JSON as an array

        containerData = new List<SavedContainer>();

        // Loop through each container
        foreach (JSONNode containerNode in rootNode.AsArray)
        {
            SavedContainer container = new SavedContainer { savedPlatforms = new List<SavedPlatform>() };

            // Loop through each platform in the container
            foreach (JSONNode platformNode in containerNode.AsArray)
            {
                SavedPlatform platform = new SavedPlatform
                {
                    selectedDifficulty = (PlatformDifficulty)platformNode[0].AsInt,
                    isActive = platformNode[1].AsBool,
                    isVertical = platformNode[2].AsBool,
                    isMoving = platformNode[3].AsBool,
                    isRotatingZAxis = platformNode[4].AsBool,
                    isRotatingYAxis = platformNode[5].AsBool,
                    movementSpeed = platformNode[6].AsFloat,
                    rotateSpeedZAxis = platformNode[7].AsFloat,
                    rotateSpeedYAxis = platformNode[8].AsFloat,
                    firstPlatform = platformNode[9].AsBool
                };

                container.savedPlatforms.Add(platform);
            }

            containerData.Add(container);
        }

        currentContainerIndex = 0;
        finalContainerPlaced = false;
        GameManager.Instance.SetVictoryGatePos(containerData.Count*10);
    }


    private void ActivateInfiniteMode()
    {
        infiniteModeActive = true;
        levelDataHandler.GenerateContainerData();
        containerData = levelDataHandler.containerData;
        currentContainerIndex = 0;
        lastContainerPositionZ = 0f; // Start the Z position for infinite mode
        StartCoroutine(GenerateInfiniteContainers());
    }

    private IEnumerator GenerateInfiniteContainers()
    {
        while (true)
        {
            if (activeContainers.Count < maxActiveContainers)
            {
                if (currentContainerIndex >= containerData.Count)
                {
                    // Once all containers are used, repeat the last one indefinitely
                    currentContainerIndex = containerData.Count - 1;
                }

                CreateContainer(currentContainerIndex);
                currentContainerIndex++;

                // Increment position for the next container in infinite mode
                lastContainerPositionZ += 10f;

                yield return new WaitForSeconds(containerSpawnInterval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator LoadInitialContainers()
    {
        int endIndex = Mathf.Min(maxActiveContainers, containerData.Count);

        for (int i = 0; i < endIndex; i++)
        {
            CreateContainer(i);
            currentContainerIndex = i + 1;
            yield return new WaitForSeconds(containerSpawnInterval);
        }
    }

    private void CycleContainer()
    {
        if (infiniteModeActive) return;

        if (currentContainerIndex < containerData.Count)
        {
            if (activeContainers.Count >= maxActiveContainers)
            {
                DestroyOldestContainer();
            }

            CreateContainer(currentContainerIndex);
            currentContainerIndex++;

            if (currentContainerIndex >= containerData.Count)
            {
                PlaceFinalContainer();
            }
        }
    }

    private void PlaceFinalContainer()
    {
        if (finalContainerPrefab == null || finalContainerPlaced || infiniteModeActive) return;

        float endPositionZ = activeContainers.Count > 0
            ? activeContainers[activeContainers.Count - 1].transform.position.z + 10f
            : 0;

        GameObject finalContainerInstance = Instantiate(finalContainerPrefab, transform);
        finalContainerInstance.transform.position = new Vector3(0, 0, endPositionZ);
        activeContainers.Add(finalContainerInstance);
        finalContainerPlaced = true;
        
    }

    public void CycleToNextLevel()
    {
        if (infiniteModeActive) return;

       
        currentLevelIndex++;
        if (currentLevelIndex >= worldData.worlds[CurrentWorldIndex].levels.Count)
        {
            currentLevelIndex = 0;
            CurrentWorldIndex++;
        }
        SavePlayerProgress();

        //loads the new level on the same scene
        /*DestroyCurrentLevel();
        if (currentWorldIndex < worldData.worlds.Count)
        {
            LoadCurrentLevel();
            StartCoroutine(LoadInitialContainers()); 
        }
        else
        {
            Debug.Log("All worlds and levels completed!");
        }*/
    }


    //  Change To Cloud
    private void SavePlayerProgress()
    {
        PlayerPrefs.SetInt("WorldIndex", CurrentWorldIndex);
        PlayerPrefs.SetInt("LevelIndex", currentLevelIndex);
        PlayerPrefs.Save();
    }

/*    private void LoadPlayerProgress(Scene scene, LoadSceneMode loadMode)
    {
        *//*currentWorldIndex = PlayerPrefs.GetInt("WorldIndex", 0);
        currentLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);*//*
        if (scene.buildIndex == 0)
        {
            CurrentWorldIndex = int.Parse(UserManager.Instance.UserData.Data["currentWorld"].Value);
            currentLevelIndex = int.Parse(UserManager.Instance.UserData.Data["lastUnlockedCampaignLevel"].Value); 
        }
    }*/

    private void LoadPlayerProgress()
    {
        CurrentWorldIndex = PlayerPrefs.GetInt("WorldIndex", 0);
        currentLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        /*CurrentWorldIndex = int.Parse(UserManager.Instance.UserData.Data["currentWorld"].Value);
        currentLevelIndex = int.Parse(UserManager.Instance.UserData.Data["lastUnlockedCampaignLevel"].Value);*/
    }

    private void CreateContainer(int index)
    {
        if (index >= containerData.Count) return;

        SavedContainer currentContainer = containerData[index];
        GameObject containerInstance = Instantiate(containerPrefab, transform);

        //------------------------------------------------------------------------------------
        //  Add A class to hold all platforms objetcs by their index
        ContainerPlatformHolder containerHolder = containerInstance.AddComponent<ContainerPlatformHolder>();
        //------------------------------------------------------------------------------------

        // Set position based on lastContainerPositionZ in infinite mode or index in normal mode
        float zPosition = infiniteModeActive ? lastContainerPositionZ : index * 10.0f;
        containerInstance.transform.position = new Vector3(0, 0, zPosition);

        containerInstance.SetActive(true);

        int platformCount = currentContainer.savedPlatforms.Count;
        float halfWidth = (platformCount - 1) / 2.0f;

        for (int j = 0; j < platformCount; j++)
        {
            SavedPlatform platformData = currentContainer.savedPlatforms[j];
            if (!platformData.isActive) continue;

            float platformXPos = (j - halfWidth);
            GameObject platformPrefab = GetPlatformPrefab(platformData.selectedDifficulty);
            GameObject platformInstance = Instantiate(platformPrefab, containerInstance.transform);
            platformInstance.transform.localPosition = new Vector3(platformXPos, 0, 5);
            platformInstance.transform.localRotation = Quaternion.identity;

            //------------------------------------------------------------------------------------
            //  Add platforms objetcs and save their index
            containerHolder.AddPlatformWithIndex(platformInstance, j);
            //------------------------------------------------------------------------------------

            PlatformController controller = platformInstance.GetComponent<PlatformController>();
            if (controller != null)
            {
                controller.InitializePlatform(platformData);
            }

        }

        PositionNeonSignSpawnPoints(containerInstance, platformCount);
        activeContainers.Add(containerInstance);

        //----------------------------------------------------------------------------------------------

        //  OrGrufi - Spawn Collectable On CurrSpawnedContainer
        if(index != 0)
        {
            CollectableManager.Instance.SpawnOnContainer(currentContainer, containerInstance,index);
        }

        //----------------------------------------------------------------------------------------------

    }

    private void DestroyOldestContainer()
    {
        if (activeContainers.Count == 0 || finalContainerPlaced && activeContainers.Count == 1) return;

        GameObject oldestContainer = activeContainers[0];
        activeContainers.RemoveAt(0);
        Destroy(oldestContainer);
    }

    private void DestroyCurrentLevel()
    {
        foreach (GameObject container in activeContainers)
        {
            Destroy(container);
        }
        activeContainers.Clear();
        finalContainerPlaced = false;
    }

    public void SaveLevel(string fileName)
    {
        if (levelDataHandler.containerData == null || levelDataHandler.containerData.Count == 0)
        {
            Debug.LogWarning("No level data available to save.");
            return;
        }
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        levelDataHandler.SaveToJson($"{fileName} - {timestamp}", $"Levels/LevelsJson/Infinity");
        Debug.Log($"Level saved as {fileName}.json");
    }

    private void PositionNeonSignSpawnPoints(GameObject containerInstance, int platformCount)
    {
        float halfWidth = ((platformCount - 1)) + 4.5f;
        if (halfWidth < 11.5f) halfWidth = 11.5f;

        Transform neonSignLeft = containerInstance.transform.Find("Neon Sign Spawn Point - Left");
        if (neonSignLeft != null)
        {
            neonSignLeft.localPosition = new Vector3(-halfWidth, 3, 7.5f);
        }

        Transform neonSignRight = containerInstance.transform.Find("Neon Sign Spawn Point - Right");
        if (neonSignRight != null)
        {
            neonSignRight.localPosition = new Vector3(halfWidth, 3, 7.5f);
        }
    }

    private GameObject GetPlatformPrefab(PlatformDifficulty difficulty)
    {
        switch (difficulty)
        {
            case PlatformDifficulty.Easy:
                return easyPlatform;
            case PlatformDifficulty.Medium:
                return mediumPlatform;
            case PlatformDifficulty.Hard:
                return hardPlatform;
            default:
                return easyPlatform;
        }
    }

    private void CheckAndDestroyPassedPlatforms()
    {
        if (activeContainers.Count == 0 ) return;

        //---------------------------------------------------
        if (playerTransform == null) return; ;
        //---------------------------------------------------


        for (int i = 0; i < activeContainers.Count; i++)
        {
            GameObject container = activeContainers[i];

            //if(container.transform.position.z < playerLocation.position.z - playerPassThreshold)
            if (container.transform.position.z < playerTransform.position.z - playerPassThreshold)
            {
                if (container == activeContainers[activeContainers.Count - 1] && finalContainerPlaced)
                {
                    break;
                }

                Destroy(container);
                activeContainers.RemoveAt(i);
                i--;

                if (infiniteModeActive)
                {
                    if (currentContainerIndex >= containerData.Count)
                    {
                        currentContainerIndex = containerData.Count - 1; // Repeat last container
                    }
                    CreateContainer(currentContainerIndex);
                    currentContainerIndex++;
                    lastContainerPositionZ += 10f;
                }
                else if (currentContainerIndex < containerData.Count)
                {
                    CreateContainer(currentContainerIndex);
                    currentContainerIndex++;
                }
                else if (!finalContainerPlaced)
                {
                    PlaceFinalContainer();
                }
            }
            else
            {
                break;
            }
        }
    }
}

[System.Serializable]
public class CompactSavedLevel
{
    public List<List<object>> containers;
}

