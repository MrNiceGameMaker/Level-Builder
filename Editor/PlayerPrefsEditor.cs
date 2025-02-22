using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPrefsEditor : EditorWindow
{
    private int worldIndex;
    private int levelIndex;
    private string worldsJson; // Holds the JSON representation of worlds

    [MenuItem("Tools/PlayerPrefs Editor")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsEditor>("PlayerPrefs Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit Player Progress", EditorStyles.boldLabel);

        // Load current PlayerPrefs values
        worldIndex = PlayerPrefs.GetInt("WorldIndex", 0);
        levelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        worldsJson = PlayerPrefs.GetString("Worlds", ""); // Load worlds JSON

        // Display fields to edit PlayerPrefs values
        worldIndex = EditorGUILayout.IntField("World Index", worldIndex);
        levelIndex = EditorGUILayout.IntField("Level Index", levelIndex);

        GUILayout.Label("Worlds (JSON Format)", EditorStyles.boldLabel);
        worldsJson = EditorGUILayout.TextField("Worlds JSON", worldsJson);

        if (GUILayout.Button("Save Changes"))
        {
            SaveChanges();
        }

        if (GUILayout.Button("Reset Progress"))
        {
            ResetProgress();
        }
    }

    private void SaveChanges()
    {
        PlayerPrefs.SetInt("WorldIndex", worldIndex);
        PlayerPrefs.SetInt("LevelIndex", levelIndex);
        PlayerPrefs.SetString("Worlds", worldsJson);
        PlayerPrefs.Save();
        Debug.Log("Player progress and worlds saved.");
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("WorldIndex");
        PlayerPrefs.DeleteKey("LevelIndex");
        PlayerPrefs.DeleteKey("Worlds");
        PlayerPrefs.Save();
        Debug.Log("Player progress and worlds reset.");
    }

    // Utility function to save a List<World> to PlayerPrefs
    public static void SaveWorldsToPrefs(List<World> worlds)
    {
        string json = JsonUtility.ToJson(new WorldList(worlds));
        PlayerPrefs.SetString("Worlds", json);
        PlayerPrefs.Save();
    }

    // Utility function to load a List<World> from PlayerPrefs
    public static List<World> LoadWorldsFromPrefs()
    {
        string json = PlayerPrefs.GetString("Worlds", "");
        if (string.IsNullOrEmpty(json)) return new List<World>();

        WorldList worldList = JsonUtility.FromJson<WorldList>(json);
        return worldList.worlds;
    }

    // Wrapper class to enable JSON serialization of a list
    [System.Serializable]
    private class WorldList
    {
        public List<World> worlds;

        public WorldList(List<World> worlds)
        {
            this.worlds = worlds;
        }
    }
}
