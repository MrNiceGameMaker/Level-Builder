using System.Collections.Generic;
using System;

[Serializable]
public class CompactSavedPlatform
{
    public int selectedDifficulty;
    public bool isActive;
    public bool isVertical;
    public bool isMoving;
    public bool isRotatingZAxis;
    public bool isRotatingYAxis;
    public float movementSpeed;
    public float rotateSpeedZAxis;
    public float rotateSpeedYAxis;
    public bool firstPlatform;

    // Converts platform data to a compact array for JSON
    public List<object> ToCompactArray()
    {
        var data = new List<object> { selectedDifficulty, isActive };

        if (!isActive)
            return data; // Return only `selectedDifficulty` and `isActive`

        // Add additional properties when `isActive` is true
        data.AddRange(new object[]
        {
            isVertical,
            isMoving,
            isRotatingZAxis,
            Math.Round(movementSpeed, 2),
            Math.Round(rotateSpeedZAxis, 2),
            Math.Round(rotateSpeedYAxis, 2),
            firstPlatform
        });

        return data;
    }
}
