using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 800f;
    [SerializeField] private float dashCooldown = 2f;

    private float lastDashTime = -2f;
    private Vector3 lastMoveDir;
    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleMovement(float horizontalInput, float verticalInput)
    {
        Vector3 moveDir = new Vector3(-verticalInput, 0f, horizontalInput).normalized;

        if (moveDir.magnitude > 0f)
        {
            animator.SetBool("isWalking", true);
            lastMoveDir = moveDir;
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        if (horizontalInput != 0f || verticalInput != 0f)
        {
            Vector3 dir = new Vector3(-horizontalInput, 0f, -verticalInput).normalized;
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }

    public void TryDash()
    {
        if (Time.time > lastDashTime + dashCooldown)
        {
            Vector3 dashDir = new Vector3(lastMoveDir.x, 0f, lastMoveDir.z).normalized;
            transform.Translate(dashDir * dashSpeed * Time.deltaTime, Space.World);
            lastDashTime = Time.time;
        }
    }
}