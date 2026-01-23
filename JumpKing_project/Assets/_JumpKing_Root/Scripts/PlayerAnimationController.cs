using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    public bool isGrounded;
    public float moveInput;

    private float fallStartY;
    private bool wasGrounded;

    private void Update()
    {
        // Movimiento
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", rb.linearVelocity.y);

        // Cargar salto (agacharse)
        if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("IsCharging", true);
        }
        else
        {
            animator.SetBool("IsCharging", false);
        }

        // Soltar salto
        if (isGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
        }

        // Detectar inicio de ca�da
        if (!isGrounded && wasGrounded)
        {
            fallStartY = transform.position.y;
        }

        // Detectar ca�da fuerte
        if (isGrounded && !wasGrounded)
        {
            float fallDistance = fallStartY - transform.position.y;

            if (fallDistance > 3f) // ajusta esta distancia
            {
                animator.SetBool("HardFall", true);
            }
            else
            {
                animator.SetBool("HardFall", false);
            }
        }

        wasGrounded = isGrounded;
    }
}
