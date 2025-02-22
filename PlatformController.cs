using PlayFab.ServerModels;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public PlatformDifficulty Difficulty { get; private set; }
    private bool isActive;
    public bool isVertical;
    private bool isMoving;
    private float movementSpeed;
    private bool isRotatingZAxis;
    private float rotateSpeedZAxis;
    private bool isRotatingYAxis;
    private float rotateSpeedYAxis;

    private float baseYPosition;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    [SerializeField] float randomStartPosRange;
    [SerializeField] float randomStartRotRange;
    [SerializeField] float moveToPlaceDuration;

    int rotationSide;

    [SerializeField] Material verticalArrowsMaterial;
    [SerializeField] Material horizontalArrowsMaterial;
    [SerializeField] Material verticalOctagonMaterial;
    [SerializeField] Material horizontalOctagonMaterial;

    // Arrays to store original materials
    private Material[] originalNeonMaterials;
    private Material[] originalOctagonMaterials;

    private Neon[] allNeons;
    private PlatformOctagonColor[] allOctagons;


    [SerializeField] WorldData worldDataSO;
    [SerializeField] IntSO currentWorldSO;
    // Method to initialize platform attributes based on SavedPlatform data
    public void InitializePlatform(SavedPlatform platformData)
    {
        Difficulty = platformData.selectedDifficulty;
        isActive = platformData.isActive;
        isVertical = platformData.isVertical;
        isMoving = platformData.isMoving;
        movementSpeed = platformData.movementSpeed;
        isRotatingZAxis = platformData.isRotatingZAxis;
        rotateSpeedZAxis = platformData.rotateSpeedZAxis;
        isRotatingYAxis = platformData.isRotatingYAxis;
        rotateSpeedYAxis = platformData.rotateSpeedYAxis;

        allNeons = GetComponentsInChildren<Neon>();
        allOctagons = GetComponentsInChildren<PlatformOctagonColor>();

        // Set up the platform state based on isActive
        gameObject.SetActive(isActive);
        baseYPosition = transform.position.y;

        if (Random.Range(0, 2) > 0)
            rotationSide = -1;
        else
            rotationSide = 1;

        if (isVertical) SetToVertical();
        else UpdateMaterials();
        

        // Set initial random position and rotation
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        transform.position = targetPosition + new Vector3(
            Random.Range(-randomStartPosRange, randomStartPosRange),
            Random.Range(-randomStartPosRange, randomStartPosRange),
            Random.Range(-randomStartPosRange, randomStartPosRange)
        );

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x + Random.Range(-randomStartRotRange, randomStartRotRange),
            transform.rotation.eulerAngles.y + Random.Range(-randomStartRotRange, randomStartRotRange),
            transform.rotation.eulerAngles.z + Random.Range(-randomStartRotRange, randomStartRotRange)
        );

        

        // Start moving to target position and rotation
        StartCoroutine(MoveToTargetPosition());
    }

    private IEnumerator MoveToTargetPosition()
    {
        PlatformChildCollision[] children = GetComponentsInChildren<PlatformChildCollision>();
        //turns off all colliders so it wont destroy player on first platfrom build
        foreach (PlatformChildCollision child in children)
        {
            child.GetComponent<BoxCollider>().enabled = false;
        }
        float elapsedTime = 0f;
        float duration = moveToPlaceDuration;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set final position and rotation
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        foreach (PlatformChildCollision child in children)
        {
            child.GetComponent<BoxCollider>().enabled = true;
            if (!isMoving || !isRotatingYAxis || !isRotatingYAxis)
            {
                child.gameObject.isStatic = true;
            }
        }
        if (!isMoving || !isRotatingYAxis || !isRotatingYAxis)
        {
            gameObject.isStatic = true;
        }
    }

    void Update()
    {
        // Apply up and down movement along the global Y-axis if the platform is set to move
        if (isMoving)
        {
            // Calculate the new Y position using a sine wave for smooth oscillation
            float newY = baseYPosition + Mathf.Sin(Time.time * movementSpeed) * 2.0f;
            Vector3 newPosition = new Vector3(transform.position.x, newY, transform.position.z);

            // Update the position to the new Y position, keeping X and Z the same
            transform.position = newPosition;
        }

        if (isRotatingYAxis && !isVertical)
        {
            transform.Rotate(Vector3.left, rotateSpeedYAxis * rotationSide * Time.deltaTime);
        }
        else if (isRotatingZAxis)
        {
            transform.Rotate(Vector3.forward, rotateSpeedZAxis * rotationSide * Time.deltaTime);
        }
        VerticalityHandler();
    }
    private void VerticalityHandler()
    {
        if (isVertical && PowerManager.Instance.IsWalkThoughWallsActive)
        {
            Collider[] allChildPlatformsColliders = GetComponentsInChildren<Collider>();

            foreach (Collider childCollider in allChildPlatformsColliders)
            {
                childCollider.enabled = false;
            }
        }
        else
        {
            Collider[] allChildPlatformsColliders = GetComponentsInChildren<Collider>();

            foreach (Collider childCollider in allChildPlatformsColliders)
            {
                childCollider.enabled = true;
            }
        }
    }

    void SetToVertical()
    {

        gameObject.layer = 7;
        transform.rotation = Quaternion.Euler(90, 0, 0);

        float platformHeight = Random.Range(-3.25f, 5f);
        transform.localPosition = new Vector3(transform.localPosition.x, platformHeight, transform.localPosition.z);

        UpdateMaterials();
    }

    private void UpdateMaterials()
    {
        // Set materials based on whether the platform is vertical or horizontal
        foreach (Neon neon in allNeons)
        {
            neon.GetComponent<MeshRenderer>().material = isVertical ? verticalArrowsMaterial : horizontalArrowsMaterial;
        }

        foreach (PlatformOctagonColor octagon in allOctagons)
        {
            octagon.GetComponent<MeshRenderer>().material = isVertical ? verticalOctagonMaterial : horizontalOctagonMaterial;
            
        }
    }
}
