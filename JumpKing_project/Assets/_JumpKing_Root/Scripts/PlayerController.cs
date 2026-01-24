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

    [Header("Hard Fall")]
    [SerializeField] float hardFallMinDistance = 6f;

    // =========================
    // 🔊 AUDIO
    // =========================
    [Header("Audio")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip wallHitClip;
    [SerializeField, Range(0f, 0.2f)] float wallHitCooldown = 0.05f;

    [SerializeField] AudioClip footstepClip;
    [SerializeField, Range(0.05f, 1f)] float footstepInterval = 0.33f;
    [SerializeField, Range(0f, 1f)] float footstepVolume = 0.8f;
    [SerializeField] bool footstepsOnlyGrounded = true;

    [Header("Jump SFX")]
    [SerializeField] AudioClip jumpClip;
    [SerializeField, Range(0f, 1f)] float jumpVolume = 1f;

    [Header("Orb Jump SFX")]
    [SerializeField] AudioClip orbJumpClip;                 // arrastra aquí el audio "jump"
    [SerializeField, Range(0f, 1f)] float orbJumpVolume = 1f;


    // ✅ NUEVO: caída fuerte (back_004)
    [Header("Hard Fall SFX")]
    [SerializeField] AudioClip hardFallClip;
    [SerializeField, Range(0f, 1f)] float hardFallVolume = 1f;

    float footstepTimer;
    bool wasWalking;

    float wallHitCooldownTimer;

    // =========================
    // ✅ TORNADO
    // =========================
    [Header("Tornado")]
    [SerializeField] bool enableTornado = true;
    [SerializeField] SpriteRenderer[] renderersToHide;
    [SerializeField] bool disableAnimatorWhileInside = true;

    [Header("Tornado Exit Fix")]
    [SerializeField] float tornadoExitYOffset = 0.6f;        // (YA NO SE USA)
    [SerializeField] float tornadoReenterBlockTime = 0.15f;

    [Header("Tornado Visual Exit")]
    [SerializeField] float tornadoColliderEnableDelay = 0.10f;

    bool insideTornado;
    TornadoZone2D currentTornado;
    float tornadoAngleDeg;

    Vector3 lastTornadoHoldPos;
    Vector3 prevTornadoHoldPos;

    float tornadoReenterBlockTimer;
    TornadoZone2D lastExitedTornado;

    float tornadoColliderEnableTimer;

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

    float jumpAnimBuffer;
    float standUpJumpBuffer;

    float fallStartY;
    bool measuringFall;

    bool hardFallDowned;

    bool jumpHeld;

    bool animatorFrozenByPause;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerCol = GetComponent<Collider2D>();

        if (renderersToHide == null || renderersToHide.Length == 0)
            renderersToHide = GetComponentsInChildren<SpriteRenderer>(true);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
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

        jumpAnimBuffer = 0f;
        standUpJumpBuffer = 0f;

        measuringFall = false;
        fallStartY = transform.position.y;

        hardFallDowned = false;
        anim.SetBool("IsFallingHard", false);

        jumpHeld = false;

        footstepTimer = 0f;
        wasWalking = false;

        insideTornado = false;
        currentTornado = null;

        lastTornadoHoldPos = Vector3.zero;
        prevTornadoHoldPos = Vector3.zero;

        tornadoReenterBlockTimer = 0f;
        lastExitedTornado = null;

        tornadoColliderEnableTimer = 0f;

        animatorFrozenByPause = false;

        wallHitCooldownTimer = 0f;
    }

    void Update()
    {
        if (PauseState.IsPaused)
        {
            moveInput = Vector2.zero;

            if (anim != null && !animatorFrozenByPause)
            {
                anim.speed = 0f;
                animatorFrozenByPause = true;
            }

            return;
        }
        else
        {
            if (anim != null && animatorFrozenByPause)
            {
                anim.speed = 1f;
                animatorFrozenByPause = false;
            }
        }

        if (wallHitCooldownTimer > 0f)
            wallHitCooldownTimer -= Time.deltaTime;

        if (tornadoReenterBlockTimer > 0f)
            tornadoReenterBlockTimer -= Time.deltaTime;

        if (tornadoColliderEnableTimer > 0f)
        {
            tornadoColliderEnableTimer -= Time.deltaTime;
            if (tornadoColliderEnableTimer <= 0f && playerCol != null)
                playerCol.enabled = true;
        }

        if (enableTornado && insideTornado)
        {
            if (jumpAnimBuffer > 0f) jumpAnimBuffer -= Time.deltaTime;
            if (standUpJumpBuffer > 0f) standUpJumpBuffer -= Time.deltaTime;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        AnimationManagement();
        HandleFootsteps();

        if (jumpAnimBuffer > 0f)
            jumpAnimBuffer -= Time.deltaTime;

        if (standUpJumpBuffer > 0f)
            standUpJumpBuffer -= Time.deltaTime;

        if (wasGrounded && !isGrounded)
        {
            measuringFall = true;
            fallStartY = transform.position.y;

            if (isChargingJump)
            {
                isChargingJump = false;
                chargeTimer = 0f;
                jumpHeld = false;
            }
        }

        if (!wasGrounded && isGrounded)
        {
            moveLockedAfterLanding = true;
            landingMoveTimer = landingMoveCooldown;
            hasAirborneLockedX = false;

            if (measuringFall)
            {
                float fallDistance = fallStartY - transform.position.y;

                if (fallDistance >= hardFallMinDistance)
                {
                    hardFallDowned = true;
                    anim.SetBool("IsFallingHard", true);
                    anim.SetTrigger("HardFall");

                    // 🔊 SFX caída fuerte (back_004)
                    if (hardFallClip != null && sfxSource != null)
                        sfxSource.PlayOneShot(hardFallClip, hardFallVolume);
                }

                measuringFall = false;
            }
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

            // ✅ FIX: solo una vez (antes lo tenías duplicado)
            if (autoJumpAtMaxCharge && chargeTimer >= maxChargeTime)
            {
                if (jumpClip != null && sfxSource != null)
                    sfxSource.PlayOneShot(jumpClip, jumpVolume);

                JumpChargedRelease();
            }
        }

        if (!hardFallDowned)
        {
            if (moveInput.x > 0 && !isFacingRight) Flip();
            if (moveInput.x < 0 && isFacingRight) Flip();
        }
    }

    private void FixedUpdate()
    {
        if (PauseState.IsPaused)
            return;

        if (enableTornado && insideTornado && currentTornado != null)
        {
            Transform hp = currentTornado.HoldPoint;

            prevTornadoHoldPos = lastTornadoHoldPos;

            tornadoAngleDeg += currentTornado.OrbitSpeed * Time.fixedDeltaTime;
            float rad = tornadoAngleDeg * Mathf.Deg2Rad;

            Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * currentTornado.OrbitRadius;
            transform.position = hp.position + (Vector3)offset;

            lastTornadoHoldPos = hp.position;

            return;
        }

        WallBounceCheck();
        Movement();
    }

    void Movement()
    {
        if (hardFallDowned && isGrounded)
        {
            playerRb.linearVelocity = new Vector2(0f, playerRb.linearVelocity.y);
            return;
        }

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

        anim.SetTrigger("Jump");
        jumpAnimBuffer = 0.08f;

        airborneLockedX = jumpVelocity.x;
        hasAirborneLockedX = true;

        isChargingJump = false;
        chargeTimer = 0f;
        jumpHeld = false;

        if (hardFallDowned)
        {
            hardFallDowned = false;
            anim.SetBool("IsFallingHard", false);
            standUpJumpBuffer = Mathf.Max(standUpJumpBuffer, 0.25f);
        }
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
        if (PauseState.IsPaused) return;
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        if (PauseState.IsPaused) return;

        if (enableTornado && insideTornado && currentTornado != null)
        {
            ExitTornado();
            return;
        }

        if (!isGrounded && manualOrb != null)
        {
            bool activated = manualOrb.TryActivate(moveInput);
            if (activated)
            {
                // 🔊 SFX salto de ORBE (distinto al salto normal)
                if (orbJumpClip != null && sfxSource != null)
                    sfxSource.PlayOneShot(orbJumpClip, orbJumpVolume);

                return;
            }
        }


        if (!isGrounded) return;

        if (hardFallDowned)
        {
            standUpJumpBuffer = 0.25f;
            anim.SetBool("IsFallingHard", false);
            anim.ResetTrigger("HardFall");
        }

        anim.ResetTrigger("Jump");
        isChargingJump = true;
        chargeTimer = 0f;

        jumpHeld = true;
    }

    void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        if (PauseState.IsPaused) return;
        if (!jumpHeld) return;

        if (isChargingJump && isGrounded)
        {
            // 🔊 SFX salto inmediato al soltar
            if (jumpClip != null && sfxSource != null)
                sfxSource.PlayOneShot(jumpClip, jumpVolume);

            JumpChargedRelease();
        }

        jumpHeld = false;
    }

    public void SetManualOrb(OrbManualJump orb) => manualOrb = orb;

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

        if (wallHitClip != null && sfxSource != null && wallHitCooldownTimer <= 0f)
        {
            sfxSource.PlayOneShot(wallHitClip);
            wallHitCooldownTimer = wallHitCooldown;
        }

        wallBounceLocked = true;
        wallBounceTimer = wallBounceLockTime;
    }

    void AnimationManagement()
    {
        bool forceJumping = standUpJumpBuffer > 0f;

        bool walking =
            Mathf.Abs(moveInput.x) > 0.01f &&
            isGrounded &&
            !isChargingJump &&
            !hardFallDowned &&
            !forceJumping;

        anim.SetBool("Walking", walking);

        bool jumping =
            forceJumping ||
            jumpAnimBuffer > 0f ||
            (!isGrounded && !isChargingJump);

        anim.SetBool("Jumping", jumping);
        anim.SetBool("IsCharging", isChargingJump && isGrounded);
        anim.SetFloat("YVelocity", playerRb.linearVelocity.y);
        anim.SetBool("IsFallingHard", hardFallDowned);
    }

    void HandleFootsteps()
    {
        if (PauseState.IsPaused)
        {
            footstepTimer = 0f;
            wasWalking = false;
            return;
        }

        if (sfxSource == null || footstepClip == null) return;

        bool forceJumping = standUpJumpBuffer > 0f;

        bool walking =
            Mathf.Abs(moveInput.x) > 0.01f &&
            (!footstepsOnlyGrounded || isGrounded) &&
            !isChargingJump &&
            !hardFallDowned &&
            !forceJumping &&
            !(enableTornado && insideTornado);

        if (!walking)
        {
            footstepTimer = 0f;
            wasWalking = false;
            return;
        }

        if (!wasWalking)
        {
            sfxSource.PlayOneShot(footstepClip, footstepVolume);
            footstepTimer = footstepInterval;
            wasWalking = true;
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            sfxSource.PlayOneShot(footstepClip, footstepVolume);
            footstepTimer = footstepInterval;
        }
    }

    // =========================================================
    // ✅ TORNADO API
    // =========================================================

    public void EnterTornado(TornadoZone2D tornado)
    {
        if (!enableTornado) return;
        if (insideTornado) return;
        if (tornado == null) return;

        if (tornadoReenterBlockTimer > 0f && tornado == lastExitedTornado)
            return;

        isChargingJump = false;
        chargeTimer = 0f;
        jumpHeld = false;

        currentTornado = tornado;
        insideTornado = true;

        tornadoAngleDeg = Random.Range(0f, 360f);

        lastTornadoHoldPos = currentTornado.HoldPoint.position;
        prevTornadoHoldPos = lastTornadoHoldPos;

        SetPlayerVisible(false);

        if (playerCol != null) playerCol.enabled = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.simulated = false;
        }
    }

    void ExitTornado()
    {
        if (!insideTornado || currentTornado == null) return;

        lastExitedTornado = currentTornado;
        tornadoReenterBlockTimer = tornadoReenterBlockTime;

        insideTornado = false;

        SetPlayerVisible(true);

        if (playerRb != null)
        {
            playerRb.simulated = true;

            float tornadoVx = (lastTornadoHoldPos.x - prevTornadoHoldPos.x) / Time.fixedDeltaTime;
            float vx = tornadoVx * currentTornado.LaunchSideCarry;
            float vy = currentTornado.LaunchUpSpeed;

            playerRb.linearVelocity = new Vector2(vx, vy);
        }

        if (playerCol != null)
        {
            playerCol.enabled = false;
            tornadoColliderEnableTimer = tornadoColliderEnableDelay;
        }

        wasGrounded = false;
        currentTornado = null;
    }

    void SetPlayerVisible(bool visible)
    {
        if (renderersToHide != null)
        {
            for (int i = 0; i < renderersToHide.Length; i++)
                if (renderersToHide[i] != null) renderersToHide[i].enabled = visible;
        }

        if (disableAnimatorWhileInside && anim != null)
            anim.enabled = visible;
    }
}
