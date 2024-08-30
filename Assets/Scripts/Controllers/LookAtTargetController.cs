using UnityEngine;

public class LookAtTargetController : MonoBehaviour
{
    [SerializeField] private Transform targetCharacter; // The character to look at
    [SerializeField] private Transform head; // The head or object that will rotate to look at the target
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask obstacleLayers; // Layers that are considered obstacles
    [SerializeField] private float maxDistance = 100f; // Maximum distance for the raycast

    private void Update()
    {
        // Perform the raycast to check if the view is blocked
        if (IsViewBlocked())
        {
            return; // Do nothing if the view is blocked
        }

        // Rotate to look at the target character
        Vector3 direction = targetCharacter.position - head.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        head.rotation = Quaternion.Slerp(head.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool IsViewBlocked()
    {
        // Calculate the direction to the target character
        Vector3 directionToTarget = targetCharacter.position - head.position;

        // Perform the raycast
        if (Physics.Raycast(head.position, directionToTarget, out RaycastHit hit, maxDistance, obstacleLayers))
        {
            // If the ray hits something before reaching the target, the view is blocked
            if (hit.transform != targetCharacter)
            {
                return true;
            }
        }

        return false; // View is not blocked
    }
}
