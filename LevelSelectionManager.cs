using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TMP_Dropdown

public class LevelSelectionManager : MonoBehaviour
{
    public WorldData worldData; // Reference to the ScriptableObject
    public TMP_Dropdown worldDropdown; // Update to TMP_Dropdown
    public TMP_Dropdown levelDropdown; // Update to TMP_Dropdown
    public Button playButton;

    private int currentWorldIndex;
    private int currentLevelIndex;

    void Start()
    {
        PopulateWorldDropdown();
        worldDropdown.onValueChanged.AddListener(OnWorldChanged);
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void PopulateWorldDropdown()
    {
        worldDropdown.ClearOptions();
        foreach (World world in worldData.worlds)
        {
            worldDropdown.options.Add(new TMP_Dropdown.OptionData(world.worldName));
        }

        worldDropdown.value = 0;
        OnWorldChanged(0);
    }

    private void OnWorldChanged(int worldIndex)
    {
        currentWorldIndex = worldIndex;
        PopulateLevelDropdown(worldIndex);
    }

    private void PopulateLevelDropdown(int worldIndex)
    {
        levelDropdown.ClearOptions();
        foreach (TextAsset level in worldData.worlds[worldIndex].levels)
        {
            levelDropdown.options.Add(new TMP_Dropdown.OptionData(level.name));
        }

        levelDropdown.value = 0;
        currentLevelIndex = 0;
        levelDropdown.onValueChanged.AddListener(OnLevelChanged);
    }

    private void OnLevelChanged(int levelIndex)
    {
        currentLevelIndex = levelIndex;
    }

    private void OnPlayButtonClicked()
    {
        PlayerPrefs.SetInt("WorldIndex", currentWorldIndex);
        PlayerPrefs.SetInt("LevelIndex", currentLevelIndex);
        PlayerPrefs.Save();
        LoadSelectedLevel();
        MenuLevelDisplay.Instance.UpdateLevelManualyChanged();
    }

    private void LoadSelectedLevel()
    {
        //Debug.Log($"Loading World: {currentWorldIndex}, Level: {currentLevelIndex}");
    }
}
