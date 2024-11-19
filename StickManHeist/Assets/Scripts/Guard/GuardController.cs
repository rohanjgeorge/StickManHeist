using UnityEngine;

public class GuardController : MonoBehaviour
{
    public enum GuardType { Stationary, Patrolling }
    public GuardType guardType;

    public float patrolDistance = 10f;
    public float speed = 0.1f;
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;

    private void Start()
    {
        startingPosition = transform.position;
        if (guardType == GuardType.Patrolling)
        {
            targetPosition = startingPosition + Vector3.right * patrolDistance;
        }
    }

    private void Update()
    {
        if (guardType == GuardType.Patrolling)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (movingRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                movingRight = false;
                targetPosition = startingPosition - Vector3.right * patrolDistance;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                movingRight = true;
                targetPosition = startingPosition + Vector3.right * patrolDistance;
            }
        }
    }

    public void MoveToPlayerPosition(Vector3 playerPosition)
    {
        // Implement the logic to move towards the player's last known position
    }
}
