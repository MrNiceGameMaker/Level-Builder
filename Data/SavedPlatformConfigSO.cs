using UnityEngine;

[CreateAssetMenu(fileName = "PlatformConfig", menuName = "LevelBuilding/PlatformConfig")]
public class SavedPlatformConfigSO : ScriptableObject
{
    [System.Serializable]
    public struct DifficultyProbabilities
    {
        [Range(0, 100)] public int easyChance;
        [Range(0, 100)] public int mediumChance;
        [Range(0, 100)] public int hardChance;
    }

    [Header("Difficulty Selection Probabilities (Total = 100%)")]
    [Range(0, 100)] public float activeChance;
    public DifficultyProbabilities difficultyProbabilities;

    [Header("Boolean Probabilities (0-100%)")]
    [Range(0, 100)] public float verticalChance;
    [Range(0, 100)] public float movingChance;
    [Range(0, 100)] public float rotatingZChance;
    [Range(0, 100)] public float rotatingYChance;

    [Header("Float Ranges (Min and Max)")]
    public Vector2 movementSpeedRange = new Vector2(0, 5);
    public Vector2 rotateSpeedZRange = new Vector2(0, 10);
    public Vector2 rotateSpeedYRange = new Vector2(0, 10);

    // Ensures the chances always add up to 100%
    public void AdjustChances()
    {
        int total = difficultyProbabilities.easyChance + difficultyProbabilities.mediumChance + difficultyProbabilities.hardChance;

        if (total == 100) return;

        if (total > 0)
        {
            // Scale each probability proportionally to sum to 100
            float scaleFactor = 100f / total;
            difficultyProbabilities.easyChance = Mathf.RoundToInt(difficultyProbabilities.easyChance * scaleFactor);
            difficultyProbabilities.mediumChance = Mathf.RoundToInt(difficultyProbabilities.mediumChance * scaleFactor);
            difficultyProbabilities.hardChance = Mathf.RoundToInt(difficultyProbabilities.hardChance * scaleFactor);
        }
        else
        {
            // Default values if all are zero (e.g., set easy to 100%)
            difficultyProbabilities.easyChance = 100;
            difficultyProbabilities.mediumChance = 0;
            difficultyProbabilities.hardChance = 0;
        }
    }
}
