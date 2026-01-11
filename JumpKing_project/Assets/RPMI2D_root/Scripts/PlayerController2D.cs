using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{

    [Header("Movement & Jump Configuration")]
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isFacingRight; //Define la orientación del personaje
    [SerializeField] Transform groundCheck; //Posición del detector del suelo
    [SerializeField] float groundCheckRadius; //Define el radio del círculo detector del suelo
    [SerializeField] LayerMask groundLayer; //Define la capa que puede tocar el detector del suelo

    [Header("Shoot Configuration")]
    [SerializeField] GameObject projectile; //ref al prefab de la bala
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootCooldown = 1f;
    bool canShoot;
    
    //Variables de referencia general
    Rigidbody2D playerRb; //Almacén del rigidbody del player
    Animator anim; //Almacén del controlador de animaciones del player
    PlayerInput input; //Almacén del controllador de inputs del player
    Vector2 moveInput; //Almacén del valor de los botones de movimiento
    bool canAttack; //Bool de seguridad que define si se puede atacar o no

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>(); //Autoreferenciar un componente propio
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        canAttack = true;
        canShoot = true;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Lógica de detección del suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //Logica de las animaciones
        AnimationManagement();
        //Logica del flip del personaje
        if (moveInput.x > 0 && !isFacingRight) Flip();
        if (moveInput.x < 0 && isFacingRight) Flip();
    }

    private void FixedUpdate()
    {
        Movement();
    }


    void Movement()
    {
    //Mover el motor de aceleración del rigidbody
    playerRb.linearVelocity = new Vector2(moveInput.x * speed, playerRb.linearVelocity.y);
    }

void Flip()
    {
        Vector3 currentScale = transform.localScale; //Almacén temporal de la escala del objeto
        currentScale.x *= -1; //Invertir el valor en X
        transform.localScale = currentScale; //Le devolvemos la escala al onjeto por el valor en X inverso
        isFacingRight = !isFacingRight; //Decirle al bool que canvie al valor contrario
    }

    void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }

    void AnimationManagement()
    {
        //Acción para gestionar los cambios de animaciñon
        anim.SetBool("Jump", !isGrounded);
        if (moveInput.x != 0) anim.SetBool("Run", true);
        else anim.SetBool("Run", false);
    }
    void Shoot()
    {
        canShoot = false;
        GameObject actualProjectile = Instantiate(projectile, shootPoint.position, Quaternion.identity);
        Projectile projectileScript = actualProjectile.GetComponent<Projectile>();
        projectileScript.isFacingRight = isFacingRight; //Igualar la orientación de la bala a la orientación del player
        Invoke(nameof(ResetShoot), shootCooldown); //Se espera tanto tiempo como shootCooldown.
    }

    void ResetShoot()
    {
        canShoot = true; //Devuelvo la posibilidad de disparar
    }
    IEnumerator Attack()
    {
        canAttack = false; //Quitar la posibilidad de atacar
        float actualSpeed = speed; //Guardamos la velocidad actual para devolverla luego
        speed = 0; //Con velocidad 0, el personaje se queda quieto
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.8f);
        speed = actualSpeed;
        canAttack = true;
        //Devolvemos velocidad y capacidad de ataque al jugador, acaba la corrutina
        yield return null;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

 public void OnJump(InputAction.CallbackContext context)
    {
       if (context.performed && isGrounded) Jump();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && canAttack) StartCoroutine(Attack());
        
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        //logica del inicio del disparo
        if (context.performed && canShoot) Shoot();
    }

  
}
