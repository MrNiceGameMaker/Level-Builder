using UnityEngine;

public class PlatformChildCollision : MonoBehaviour
{
    private PlatformController parentController;

    private void Start()
    {
        // Find and store the PlatformController component on the parent
        parentController = GetComponentInParent<PlatformController>();
        if (parentController == null)
        {
            Debug.LogWarning("PlatformController not found on parent.");
        }else if(parentController.isVertical)
        {
            //print("connected");
            gameObject.layer = 7;
        }
    }


}
