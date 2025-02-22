using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "LevelBuilding/WorldData")]
public class WorldData : ScriptableObject
{
    public List<World> worlds; // List of worlds
}


[System.Serializable]
public class World
{
    [Header("World")]
    public string worldName;
    public GameObject worldPrefab;
    public Vector3 worldRotation = Vector3.up;
    [Header("Collectables")]
    public WorldCollectableChances collectableChances;

    [Header("Colors")]
    [Header("Horizontal")]
    public Color horizontalOutPlatformColor;
    public Color horizontalInPlatformColor;
    [Header("Vertical")]
    public Color verticalOutPlatformColor;
    public Color verticalInPlatformColor;
    [Header("Alpha Value")]
    [Range(0,1f)]
    public float aplhaValue;

    [Header("Levels")]
    public List<TextAsset> levels; // Levels within each world
    


}