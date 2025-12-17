using UnityEngine;

public class ARTargetNavigator : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 10f;

    public Transform leftGate;
    public Transform middleGate;
    public Transform rightGate;

    private Transform currentTarget;
    private Animator anim;

    // Variables to store the original "Home" spot
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    void Awake()
    {
        // Save the starting spot relative to the AR Marker
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        currentTarget = null;
    }

    // CALL THIS FUNCTION when the marker is lost or found
    public void ResetCharacter()
    {
        currentTarget = null; // Stop any current walking
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.Play("idle", 0, 0f); // Force animator back to start of idle
        }
    }

    void Update()
    {
        if (currentTarget != null)
        {
            // Use a 2D distance (ignore height) to prevent Y-axis glitches
            Vector3 currentPosFlattened = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetPosFlattened = new Vector3(currentTarget.position.x, 0, currentTarget.position.z);

            float distance = Vector3.Distance(currentPosFlattened, targetPosFlattened);

            // Increase this number (e.g., 0.3f) to stop them BEFORE they go behind the gate
            if (distance > 0.27f)
            {
                MoveTowardsTarget();
            }
            else
            {
                StopWalking();
            }
        }
    }

    void StopWalking()
    {
        currentTarget = null;
        anim.SetBool("isWalking", false);
        // This stops the sliding by forcing the character to the exact target position
        // transform.position = targetPosFlattened; // Only use this if you still see a gap
    }

    void MoveTowardsTarget()
    {
        float step = moveSpeed * Time.deltaTime;
        Vector3 targetPos = new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z);

        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        anim.SetBool("isWalking", true);
    }

    public void GoToLeft() { currentTarget = leftGate; }
    public void GoToMiddle() { currentTarget = middleGate; }
    public void GoToRight() { currentTarget = rightGate; }
}