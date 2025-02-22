using System.Collections;
using UnityEngine;

public class FinalContainerController : MonoBehaviour
{
    [Header("Container Components")]
    public Transform[] containerParts; // Array of parts with children to animate into place
    public Transform finalGate; // Main gate object for special effect

    [Header("Movement Settings")]
    [SerializeField] private float moveToPlaceDuration = 1.5f; // Duration for parts to move into place
    [SerializeField] private float initialDistanceForGate = 50f; // Initial distance for gate approach effect
    [SerializeField] private float partsDistanceFromPlacement = 50f; // Max random distance for parts initial position
    [SerializeField] private float timeBetweenParts = 0.2f; // Time delay between each part's movement start

    private Vector3[] targetPositions; // Final target positions for container parts
    private Quaternion[] targetRotations; // Final target rotations for container parts
    private Vector3 gateTargetPosition; // Final target position for the gate

    void Start()
    {
        // Initialize target positions and rotations
        targetPositions = new Vector3[containerParts.Length];
        targetRotations = new Quaternion[containerParts.Length];

        for (int i = 0; i < containerParts.Length; i++)
        {
            targetPositions[i] = containerParts[i].position;
            targetRotations[i] = containerParts[i].rotation;

            // Set initial random positions for each container part
            containerParts[i].position += new Vector3(
                Random.Range(-partsDistanceFromPlacement, partsDistanceFromPlacement),
                Random.Range(-partsDistanceFromPlacement, partsDistanceFromPlacement),
                Random.Range(-partsDistanceFromPlacement, partsDistanceFromPlacement)
            );
        }

        // Set the gate’s initial distant position
        gateTargetPosition = finalGate.position;
        finalGate.position = gateTargetPosition + Vector3.forward * initialDistanceForGate;

        // Start the animations
        StartCoroutine(MoveContainerPartsIntoPlace());
        StartCoroutine(MoveGateToPosition());
    }

    private IEnumerator MoveContainerPartsIntoPlace()
    {
        for (int i = 0; i < containerParts.Length; i++)
        {
            StartCoroutine(MovePartToPosition(containerParts[i], targetPositions[i], targetRotations[i]));
            yield return new WaitForSeconds(timeBetweenParts); // Wait before moving the next part
        }
    }

    private IEnumerator MovePartToPosition(Transform part, Vector3 targetPosition, Quaternion targetRotation)
    {
        float elapsedTime = 0f;

        Vector3 initialPosition = part.position;
        Quaternion initialRotation = part.rotation;

        while (elapsedTime < moveToPlaceDuration)
        {
            part.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveToPlaceDuration);
            part.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / moveToPlaceDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to final position and rotation
        part.position = targetPosition;
        part.rotation = targetRotation;
    }

    private IEnumerator MoveGateToPosition()
    {
        float elapsedTime = 0f;
        yield return new WaitForSeconds(2); // Delay before moving the gate

        while (elapsedTime < moveToPlaceDuration)
        {
            finalGate.position = Vector3.Lerp(finalGate.position, gateTargetPosition, elapsedTime / moveToPlaceDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        finalGate.position = gateTargetPosition; // Final snap to position
    }
}
