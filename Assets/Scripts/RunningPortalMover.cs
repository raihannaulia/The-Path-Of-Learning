using UnityEngine;

public class RunningPortalMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform portalTarget;
    public float runSpeed = 4.0f;     // Faster for running
    public float turnSpeed = 15.0f;   // Snappier rotation
    public float stopDistance = 0.8f; // Larger buffer for high speed

    private Animator anim;
    private bool isRunning = false;

    private Vector3 startPos;
    private Quaternion startRot;

    void Awake()
    {
        // Store starting position for the Reset function
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // --- BUTTON TRIGGER ---
    public void StartRunning()
    {
        if (portalTarget != null) isRunning = true;
    }

    // --- AR RESET TRIGGER ---
    public void ResetCharacter()
    {
        isRunning = false;
        transform.localPosition = startPos;
        transform.localRotation = startRot;

        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.Play("idle", 0, 0f);
        }
    }

    void Update()
    {
        if (isRunning && portalTarget != null)
        {
            float distance = Vector3.Distance(transform.position, portalTarget.position);

            if (distance > stopDistance)
            {
                // Rotate and Move
                Vector3 direction = (portalTarget.position - transform.position).normalized;
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
                }

                transform.position = Vector3.MoveTowards(transform.position, portalTarget.position, runSpeed * Time.deltaTime);
                anim.SetBool("isRunning", true);
            }
            else
            {
                // Stop at gate
                isRunning = false;
                anim.SetBool("isRunning", false);
            }
        }
    }
}