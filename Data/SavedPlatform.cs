using UnityEngine;

[System.Serializable]
public class SavedPlatform
{
    public PlatformDifficulty selectedDifficulty;
    public bool isActive;
    public bool isVertical;
    public bool isMoving;
    public bool isRotatingZAxis;
    public bool isRotatingYAxis;

    public float movementSpeed;
    public float rotateSpeedZAxis;
    public float rotateSpeedYAxis;

    public bool firstPlatform;

    public SavedPlatform() { }
    //Makes an easy platform that doesnt move or rotate
    public SavedPlatform(bool firstPlatform)
    {
        isActive = true; 
        selectedDifficulty = PlatformDifficulty.Easy;
        isMoving = false;
        isVertical = false;
        isRotatingZAxis = false;
        isRotatingYAxis = false;
        movementSpeed = 0f;
        rotateSpeedZAxis = 0f;
        rotateSpeedYAxis = 0f;
    }

    public SavedPlatform(SavedPlatformConfigSO config)
    {
        // Adjust probabilities to ensure they total 100 if needed
        config.AdjustChances();

        // Choose difficulty based on probabilities in the config
        selectedDifficulty = ChooseDifficulty(config);

        // Set boolean properties based on config probabilities
        isActive = GetRandomBool(config.activeChance);
        isVertical = GetRandomBool(config.verticalChance);
        isMoving = GetRandomBool(config.movingChance);
        isRotatingZAxis = GetRandomBool(config.rotatingZChance);
        isRotatingYAxis = GetRandomBool(config.rotatingYChance);

        // Set float properties based on ranges in the config
        movementSpeed = GetRandomFloat(config.movementSpeedRange.x, config.movementSpeedRange.y);
        rotateSpeedZAxis = GetRandomFloat(config.rotateSpeedZRange.x, config.rotateSpeedZRange.y);
        rotateSpeedYAxis = GetRandomFloat(config.rotateSpeedYRange.x, config.rotateSpeedYRange.y);
    }

    private PlatformDifficulty ChooseDifficulty(SavedPlatformConfigSO config)
    {
        int roll = Random.Range(0, 100);

        if (roll < config.difficultyProbabilities.easyChance)
            return PlatformDifficulty.Easy;
        else if (roll < config.difficultyProbabilities.easyChance + config.difficultyProbabilities.mediumChance)
            return PlatformDifficulty.Medium;
        else
            return PlatformDifficulty.Hard;
    }

    private bool GetRandomBool(float chance)
    {
        return Random.value < (chance / 100f); // Convert 0-100 chance to 0-1 probability
    }

    private float GetRandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }
}
