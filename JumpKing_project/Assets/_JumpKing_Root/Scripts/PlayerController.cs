using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Jump Configuration")]
    [SerializeField] float speed = 6f;

    [Header("Jump King Jump (Charge)")]
    [SerializeField] float minJumpForce = 6f;
    [SerializeField] float maxJumpForce = 14f;
    [SerializeField] float maxChargeTime = 1.2f;
    [SerializeField] float horizontalJumpMultiplier = 1.0f;

    [Header("Charge / Landing behavior")]
    [SerializeField] bool autoJumpAtMaxCharge = true;
    [SerializeField] float landingMoveCooldown = 0.12f;

    [SerializeField] bool isGrounded;
    [SerializeField] bool isFacingRight;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Optional - Movement rules")]
    [SerializeField] bool blockMoveWhileCharging = true;
    [SerializeField] bool blockMoveInAir = true;

    [Header("Tap Jump Tuning")]
    [SerializeField, Range(0f, 0.3f)] float tapChargeThreshold = 0.12f;
    [SerializeField] float tapVerticalMultiplier = 0.65f;
    [SerializeField] float tapHorizontalMultiplier = 1.25f;

    [Header("Wall Bounce (Raycast)")]
    [SerializeField] bool enableWallBounce = true;
    [SerializeField] float wallBounceSpeed = 6f;
    [SerializeField] float wallBounceLockTime = 0.08f;
    [SerializeField] float wallCheckDistance = 0.08f;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float minVerticalSpeedForBounce = 0.2f;
    [SerializeField] float minHorizontalSpeedForBounce = 0.5f;

    OrbManualJump manualOrb;

    Rigidbody2D playerRb;
    Animator anim;
    PlayerInput playerInput;
    Collider2D playerCol;

    InputAction moveAction;
    InputAction jumpAction;

    Vector2 moveInput;
    bool isChargingJump;
    float chargeTimer;

    bool moveLockedAfterLanding;
    float landingMoveTimer;
    bool wasGrounded;

    bool wallBounceLocked;
    float wallBounceTimer;

    float airborneLockedX;
    bool hasAirborneLockedX;

    bool ignoreLandingMoveLock;
    public void SetNoLandingLock(bool v) => ignoreLandingMoveLock = v;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerCol = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        jumpAction.started -= OnJumpStarted;
        jumpAction.canceled -= OnJumpCanceled;
    }

    void Start()
    {
        isFacingRight = true;
        wasGrounded = false;

        if (wallLayer.value == 0)
            wallLayer = groundLayer;

        hasAirborneLockedX = false;
        airborneLockedX = 0f;

        ignoreLandingMoveLock = false;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            moveLockedAfterLanding = true;
            landingMoveTimer = landingMoveCooldown;
            hasAirborneLockedX = false;
        }
        wasGrounded = isGrounded;

        if (moveLockedAfterLanding)
        {
            landingMoveTimer -= Time.deltaTime;
            if (landingMoveTimer <= 0f)
                moveLockedAfterLanding = false;
        }

        if (wallBounceLocked)
        {
            wallBounceTimer -= Time.deltaTime;
            if (wallBounceTimer <= 0f)
                wallBounceLocked = false;
        }

        if (isChargingJump && isGrounded)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);

            if (autoJumpAtMaxCharge && chargeTimer >= maxChargeTime)
                JumpChargedRelease();
        }

        if (moveInput.x > 0 && !isFacingRight) Flip();
        if (moveInput.x < 0 && isFacingRight) Flip();
    }

    private void FixedUpdate()
    {
        WallBounceCheck();
        Movement();
    }

    void Movement()
    {
        if (!isGrounded)
        {
            if (blockMoveInAir) return;

            if (hasAirborneLockedX && Mathf.Abs(moveInput.x) < 0.01f)
            {
                playerRb.linearVelocity = new Vector2(airborneLockedX, playerRb.linearVelocity.y);
                return;
            }

            playerRb.linearVelocity = new Vector2(moveInput.x * speed, playerRb.linearVelocity.y);
            return;
        }

        if (moveLockedAfterLanding && isGrounded && !ignoreLandingMoveLock)
        {
            playerRb.linearVelocity = new Vector2(0f, playerRb.linearVelocity.y);
            return;
        }

        if (wallBounceLocked) return;

        if (blockMoveWhileCharging && isChargingJump)
        {
            playerRb.linearVelocity = new Vector2(0f, playerRb.linearVelocity.y);
            return;
        }

        playerRb.linearVelocity = new Vector2(moveInput.x * speed, playerRb.linearVelocity.y);
    }

    void JumpChargedRelease()
    {
        float t = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, t);

        float xDir = Mathf.Clamp(moveInput.x, -1f, 1f);
        if (Mathf.Abs(xDir) < 0.01f)
            xDir = isFacingRight ? 1f : -1f;

        Vector2 jumpVelocity = new Vector2(
            xDir * jumpForce * horizontalJumpMultiplier,
            jumpForce
        );

        if (t <= tapChargeThreshold)
        {
            jumpVelocity.y *= tapVerticalMultiplier;
            jumpVelocity.x *= tapHorizontalMultiplier;
        }

        playerRb.linearVelocity = jumpVelocity;

        airborneLockedX = jumpVelocity.x;
        hasAirborneLockedX = true;

        isChargingJump = false;
        chargeTimer = 0f;
    }

    void Flip()
    {
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
        isFacingRight = !isFacingRight;
    }

    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        if (!isGrounded && manualOrb != null)
        {
            bool activated = manualOrb.TryActivate(moveInput);
            if (activated) return;
        }

        if (!isGrounded) return;
        isChargingJump = true;
        chargeTimer = 0f;
    }

    void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        if (isChargingJump && isGrounded)
            JumpChargedRelease();
    }

    public void SetManualOrb(OrbManualJump orb)
    {
        manualOrb = orb;
    }

    public void ClearManualOrb(OrbManualJump orb)
    {
        if (manualOrb == orb)
            manualOrb = null;
    }

    void WallBounceCheck()
    {
        if (!enableWallBounce) return;
        if (wallBounceLocked) return;

        float vx = playerRb.linearVelocity.x;
        float vy = playerRb.linearVelocity.y;

        if (Mathf.Abs(vy) < minVerticalSpeedForBounce) return;
        if (Mathf.Abs(vx) < minHorizontalSpeedForBounce) return;

        float dirX = Mathf.Sign(vx);

        Vector2 center = playerCol.bounds.center;
        float halfWidth = playerCol.bounds.extents.x;
        float skin = 0.02f;

        Vector2 origin = center + Vector2.right * dirX * (halfWidth + skin);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * dirX, wallCheckDistance, wallLayer);

        Debug.DrawRay(origin, Vector2.right * dirX * wallCheckDistance, Color.magenta);

        if (hit.collider == null) return;
        if (hit.collider.isTrigger) return;

        float newVx = -dirX * wallBounceSpeed;
        playerRb.linearVelocity = new Vector2(newVx, playerRb.linearVelocity.y);

        wallBounceLocked = true;
        wallBounceTimer = wallBounceLockTime;
    }
}
