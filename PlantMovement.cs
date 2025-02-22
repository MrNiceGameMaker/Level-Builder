using UnityEngine;

public class PlantMovement : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] WorldData worldData;
    int currentWorldIndex;
    int currentLevelIndex;
    [SerializeField] Vector3 startPos;
    [SerializeField] float rotationSpeed;
    GameObject currentWorldPrefab;
    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType(typeof(Player)) as Player;
        currentWorldIndex = PlayerPrefs.GetInt("WorldIndex", 0);
        currentLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        currentWorldPrefab = Instantiate(worldData.worlds[currentWorldIndex].worldPrefab);

        int currentWorldSize = worldData.worlds[currentWorldIndex].levels.Count;
        //print(currentWorldSize);
        currentWorldPrefab.transform.position = startPos;
        // Calculate the scale based on the current world index
        float minScale = 35f;
        float maxScale = 70f;

        // Calculate the scale factor based on the current world position
        float scaleFactor = Mathf.Lerp(minScale, maxScale, (float)currentLevelIndex / (currentWorldSize - 1));

        // Set the object's scale
        currentWorldPrefab.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    // Update is called once per frame
    void Update()
    {
        currentWorldPrefab.transform.position = new Vector3(currentWorldPrefab.transform.position.x, currentWorldPrefab.transform.position.y, player.transform.position.z +108);
        currentWorldPrefab.transform.Rotate((rotationSpeed / 2) * Time.deltaTime, rotationSpeed * Time.deltaTime, 0);
    }
}
