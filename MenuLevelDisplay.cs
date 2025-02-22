using Michsky.UI.Heat;
using TMPro;
using UnityEngine;

public class MenuLevelDisplay : MonoBehaviour
{

    public static MenuLevelDisplay Instance { get; private set; }

    public ButtonManager startLevelsButtonText; // Reference to the Start Game button's text
    public ButtonManager worldButtonText;
    public WorldData worldData; // Reference to the shared WorldData ScriptableObject
    public BoolSO isLevels;

    public int currentWorldIndex;
    private int currentLevelIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void Start()
    {
        //ResetProgression();
        LoadPlayerProgress();
        UpdateStartLevelButtonText();
    }

    void ResetProgression()
    {
        PlayerPrefs.SetInt("WorldIndex", 0);
        PlayerPrefs.SetInt("LevelIndex", 0);

    }

    private void LoadPlayerProgress()
    {
        currentWorldIndex = PlayerPrefs.GetInt("WorldIndex", 0);
        currentLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        /*currentWorldIndex = int.Parse(UserManager.Instance.UserData.Data["currentWorld"].Value);
        currentLevelIndex = int.Parse(UserManager.Instance.UserData.Data["lastUnlockedCampaignLevel"].Value);*/
    }

    private void UpdateStartLevelButtonText()
    {
        Color worldcolor = worldData.worlds[currentWorldIndex].horizontalOutPlatformColor;
        string worldcolorHex = ColorToHex(worldcolor);
        string displayText = $"<b><size=125%><color={worldcolorHex}>{worldData.worlds[currentWorldIndex].worldName}</color></size></b> \n\n {GetCurrentWorldLevelDisplayText()}";
        startLevelsButtonText.buttonText = $"{displayText}";
        startLevelsButtonText.UpdateUI();
        worldButtonText.buttonText = $"World: \n         {worldData.worlds[currentWorldIndex].worldName}";
        worldButtonText.UpdateUI();
    }

    private string GetCurrentWorldLevelDisplayText()
    {
        int displayLevelNumber = CalculateDisplayLevelNumber();
        //GameManager.Instance.SetCampaignLevelIndex(displayLevelNumber);
        //return $"{worldData.worlds[currentWorldIndex].worldName} - Level {displayLevelNumber}";
        return $"Level {displayLevelNumber}";
    }

    private int CalculateDisplayLevelNumber()
    {
        LoadPlayerProgress();
        int levelOffset = 1;
        for (int i = 0; i < currentWorldIndex; i++)
        {
            levelOffset += worldData.worlds[i].levels.Count;
        }

        return levelOffset + currentLevelIndex;

    }
    // Sagi
    public int CurrentCampaignLevel()
    {
        //print("Sagi " + CalculateDisplayLevelNumber().ToString());
        return CalculateDisplayLevelNumber();
    }
 
    public void LoadLevels()
    {
        DelegateManager.OnPlayUISound?.Invoke(SoundNamesEnum.ConfirmButton, 0);

        isLevels.value = true;
        //print("  sagi - isLevels.value " + isLevels.value);
    }
    public void LoadInfinty()
    {
        isLevels.value = false;
        //print(" sagi -  isLevels.value " + isLevels.value);
    }

    public void UpdateLevelManualyChanged()
    {
        LoadPlayerProgress();
        UpdateStartLevelButtonText();
        ProgressionNotify.Instance.UpdateProgression();
        FindObjectOfType<UpdateWorldPlanet>().UpdatePlanet();
    }

    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}
