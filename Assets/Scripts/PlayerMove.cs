using System.Collections;
using UnityEngine;
using FMODUnity; 
using FMOD.Studio;

public class PlayerMove : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private EventReference saltoEvent;
    [SerializeField] private EventReference dashEvent;
    [SerializeField] private EventReference cargarEvent;

    [Header("Movimiento")]
    public float runSpeed = 2;
    public float jumpSpeed = 3;
    public float wallSlidiningSpeed = 0.5f;

    private bool wallSliding = false;

    [Header("Dash")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Double Jump")]
    public float DoubleJumpSpeed = 3f;
    private bool canDoubleJump;

    [Header("Salto Mejorado")]
    public bool betterJump = false;
    public float fallMultiplier = 0.5f;
    public float lowJumpMultiplier = 1f;

    [Header("Cargar habilidad")]
    public float holdDuration = 1.0f;
    private float holdTime = 0.0f;
    public float cooldownDuration = 0.5f;
    private float cooldownTime = 0.0f;
    private bool isOnCooldown = false;

    [Header("Componentes")]
    private Rigidbody2D rb2D;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Wall Jump")]
    public float wallJumpForceX = 1f;
    public float wallJumpForceY = 1f;
    public float wallJumpDuration = 0.2f;
    private bool isWallJumping = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator.SetBool("DoubleJump", false);
        animator.SetBool("Wall", false);
    }

    private void WallJump(int direction)
    {
        RuntimeManager.PlayOneShot(saltoEvent, transform.position);

        rb2D.velocity = Vector2.zero;
        rb2D.AddForce(new Vector2(direction * wallJumpForceX, wallJumpForceY), ForceMode2D.Impulse);

        if (direction == -1)
            spriteRenderer.flipX = true;
        else if (direction == 1)
            spriteRenderer.flipX = false;

        isWallJumping = true;
        Invoke(nameof(EndWallJump), wallJumpDuration);
    }

    private void EndWallJump()
    {
        isWallJumping = false;
    }

    private void Update()
    {
        // -----------------------------------------
        // GET INPUT FROM INPUT MANAGER
        // -----------------------------------------
        float move = Input.GetAxisRaw("Horizontal");   // teclado + joystick
        float vertical = Input.GetAxisRaw("Vertical"); // teclado + joystick
        bool jumpPressed = Input.GetButton("Jump");    // Space / A button
        bool jumpDown = Input.GetButtonDown("Jump");   // Space down / A button
        bool dashDown = Input.GetButtonDown("Fire3");  // Shift / X button
        bool downHeld = vertical < -0.5f;              // S / stick abajo
        // -----------------------------------------

        // JUMP ---------------
        if (jumpPressed && !isDashing)
        {
            if (CheckGround.isGrounded)
            {
                canDoubleJump = true;
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpSpeed);

                RuntimeManager.PlayOneShot(saltoEvent, transform.position);
            }
            else
            {
                if (jumpDown && (!animator.GetBool("Wall")))
                {
                    if (canDoubleJump && (!animator.GetBool("Navidad")) && (!animator.GetBool("Inicio")))
                    {
                        animator.SetBool("DoubleJump", true);
                        rb2D.velocity = new Vector2(rb2D.velocity.x, DoubleJumpSpeed);

                        RuntimeManager.PlayOneShot(saltoEvent, transform.position);
                        canDoubleJump = false;
                    }
                    else
                    {
                        canDoubleJump = false;
                    }
                }
            }
        }

        // Estados del Animator
        if (!CheckGround.isGrounded)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Run", false);
        }
        else
        {
            animator.SetBool("Jump", false);
            animator.SetBool("DoubleJump", false);
            animator.SetBool("Falling", false);
        }

        if (rb2D.velocity.y < 0 && !(CheckRightSide.RightWall || CheckLeftSide.LeftWall))
        {
            animator.SetBool("Falling", true);
        }
        else if (rb2D.velocity.y > 0)
        {
            animator.SetBool("Falling", false);
        }

        // WALL SLIDE ---------------
        if (!(CheckRightSide.RightWall || CheckLeftSide.LeftWall))
        {
            animator.SetBool("Wall", false);
        }

        if ((CheckRightSide.RightWall || CheckLeftSide.LeftWall) && (!CheckGround.isGrounded))
        {
            animator.SetBool("Wall", true);
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }

        if (wallSliding)
        {
            if (animator.GetBool("Inicio"))
                animator.Play("Wall 1");
            else if (!animator.GetBool("Navidad"))
                animator.Play("Wall");
            else
                animator.Play("Wall 0");

            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -wallSlidiningSpeed, float.MaxValue));

            if (jumpDown && CheckRightSide.RightWall)
                WallJump(-1);
            else if (jumpDown && CheckLeftSide.LeftWall)
                WallJump(1);
        }

        // DASH (Shift o X button)
        if (dashDown && canDash && animator.GetBool("Navidad"))
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        // INPUT Manager
        float move = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool downHeld = vertical < -0.5f;

        // Cooldown de cargar
        if (isOnCooldown)
        {
            cooldownTime += Time.deltaTime;
            if (cooldownTime >= cooldownDuration)
            {
                isOnCooldown = false;
                cooldownTime = 0.0f;
            }
        }

        if (isWallJumping || isDashing) return;

        // Movimiento horizontal (TECLADO + MANDO)
        if (move > 0.1f && !downHeld)
        {
            rb2D.velocity = new Vector2(runSpeed, rb2D.velocity.y);
            spriteRenderer.flipX = false;
            animator.SetBool("Run", true);
        }
        else if (move < -0.1f && !downHeld)
        {
            rb2D.velocity = new Vector2(-runSpeed, rb2D.velocity.y);
            spriteRenderer.flipX = true;
            animator.SetBool("Run", true);
        }
        else
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            animator.SetBool("Run", false);

            // Acción de cargar (S o stick abajo)
            if (downHeld && !isOnCooldown && !(animator.GetBool("Inicio")))
            {
                holdTime += Time.deltaTime;
                animator.SetBool("Loading", true);
                animator.Play("Loading");

                if (holdTime >= holdDuration)
                {
                    RuntimeManager.PlayOneShot(cargarEvent, transform.position);

                    if (animator.GetBool("Navidad"))
                    {
                        animator.SetBool("Navidad", false);
                        isOnCooldown = true;
                        animator.Play("Idle");
                        return;
                    }
                    else
                    {
                        animator.SetBool("Navidad", true);
                        isOnCooldown = true;
                        animator.Play("Idle 0");
                        return;
                    }
                }
            }
            else
            {
                holdTime = 0.0f;
                animator.SetBool("Loading", false);
            }
        }

        // Mejora del salto
        if (betterJump)
        {
            if (rb2D.velocity.y < 0)
                rb2D.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;

            if (rb2D.velocity.y > 0 && !Input.GetButton("Jump"))
                rb2D.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        RuntimeManager.PlayOneShot(dashEvent, transform.position);
        animator.Play("Dash");

        float originalGravity = rb2D.gravityScale;
        rb2D.gravityScale = 0;

        float dashDir = spriteRenderer.flipX ? -1f : 1f;
        rb2D.velocity = new Vector2(dashDir * dashForce, dashForce / 10);

        yield return new WaitForSeconds(dashDuration);

        rb2D.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
