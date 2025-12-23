using UnityEngine;

public class StraightPathMover : MonoBehaviour
{
    public Transform portalTarget; // Drag your single portal here
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 10f;
    public float stopDistance = 0.5f;

    private bool isWalking = false;
    private Animator anim;

    // Variables to store the original "Home" spot
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    void Awake()
    {
        // Save the starting spot relative to the AR Marker/Island
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // --- RESET FUNCTION ---
    // Call this from your AR Tracking event (OnTargetFound)
    public void ResetCharacter()
    {
        isWalking = false; // Stop the movement logic

        // Reset transform back to the saved local starting point
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        // Reset the animation to Idle
        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            // Optional: Force the idle animation to start from the beginning
            anim.Play("idle", 0, 0f);
        }
    }

    public void StartPath()
    {
        if (portalTarget != null) isWalking = true;
    }

    void Update()
    {
        if (isWalking && portalTarget != null)
        {
            float distance = Vector3.Distance(transform.position, portalTarget.position);

            if (distance > stopDistance)
            {
                MoveForward();
            }
            else
            {
                StopWalking();
            }
        }
    }

    void MoveForward()
    {
        Vector3 direction = (portalTarget.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, portalTarget.position, moveSpeed * Time.deltaTime);
        anim.SetBool("isWalking", true);
    }

    void StopWalking()
    {
        isWalking = false;
        anim.SetBool("isWalking", false);
    }
}