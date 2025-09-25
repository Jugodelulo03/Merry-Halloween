using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public AudioSource salto;
    public AudioSource dashsound;
    public AudioSource cargar;

    public float runSpeed = 2;
    public float jumpSpeed = 3;
    public float wallSlidiningSpeed = 0.5f;

    bool wallSliding = false;

    // DASH --------------------
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    private bool isDashing = false;
    private bool canDash = true;

    public float DoubleJumpSpeed = 3f;
    private bool canDoubleJump;

    Rigidbody2D rb2D;

    public bool betterJump = false;
    public float fallMultiplier = 0.5f;
    public float lowJumpMultiplier = 1f;

    public float holdDuration = 1.0f;
    private float holdTime = 0.0f;

    public float cooldownDuration = 0.5f;
    private float cooldownTime = 0.0f;
    private bool isOnCooldown = false;

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    // 🔑 Fuerzas específicas del wall jump
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
        salto.Play();
        rb2D.velocity = Vector2.zero; // limpiar velocidad para consistencia
        rb2D.AddForce(new Vector2(direction * wallJumpForceX, wallJumpForceY), ForceMode2D.Impulse);

        // 🔑 Ajustar orientación del sprite según dirección
        if (direction == -1)
            spriteRenderer.flipX = true;  // mirando a la izquierda
        else if (direction == 1)
            spriteRenderer.flipX = false; // mirando a la derecha

        isWallJumping = true;
        Invoke(nameof(EndWallJump), wallJumpDuration);
    }

    private void EndWallJump()
    {
        isWallJumping = false;
    }

    private void Update()
    {
        // JUMP ---------------
        if (Input.GetKey("space") && !isDashing)
        {
            if (CheckGround.isGrounded)
            {
                canDoubleJump = true;
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpSpeed);
                salto.Play();
            }
            else
            {
                if (Input.GetKeyDown("space") && (!animator.GetBool("Wall")))
                {
                    if (canDoubleJump && (!animator.GetBool("Navidad")) && (!animator.GetBool("Inicio")))
                    {
                        animator.SetBool("DoubleJump", true);
                        rb2D.velocity = new Vector2(rb2D.velocity.x, DoubleJumpSpeed);
                        salto.Play();
                        canDoubleJump = false;
                    }
                    else
                    {
                        canDoubleJump = false;
                    }
                }
            }
        }

        if (CheckGround.isGrounded == false)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Run", false);
        }

        if (CheckGround.isGrounded == true)
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
        if (!((CheckRightSide.RightWall) || (CheckLeftSide.LeftWall)))
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
            {
                animator.Play("Wall 1");
            }
            else if (!animator.GetBool("Navidad"))
            {
                animator.Play("Wall");
            }
            else
            {
                animator.Play("Wall 0");
            }

            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -wallSlidiningSpeed, float.MaxValue));

            // 🔑 Aquí aplicamos wall jump
            if (Input.GetKeyDown(KeyCode.Space) && CheckRightSide.RightWall)
            {
                WallJump(-1); // salto hacia la izquierda
            }
            else if (Input.GetKeyDown(KeyCode.Space) && CheckLeftSide.LeftWall)
            {
                WallJump(1); // salto hacia la derecha
            }
        }

        // DASH input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && animator.GetBool("Navidad"))
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        // COOLdown CARGAR
        if (isOnCooldown)
        {
            cooldownTime += Time.deltaTime;
            if (cooldownTime >= cooldownDuration)
            {
                isOnCooldown = false;
                cooldownTime = 0.0f;
            }
        }

        // 🔑 Si está en wall jump o dash, ignoramos el movimiento lateral normal
        if (isWallJumping || isDashing) return;

        // Movimiento horizontal
        if (Input.GetKey("d") && (!Input.GetKey("s")))
        {
            rb2D.velocity = new Vector2(runSpeed, rb2D.velocity.y);
            spriteRenderer.flipX = false;
            animator.SetBool("Run", true);
        }
        else if (Input.GetKey("a") && (!Input.GetKey("s")))
        {
            rb2D.velocity = new Vector2(-runSpeed, rb2D.velocity.y);
            spriteRenderer.flipX = true;
            animator.SetBool("Run", true);
        }
        else
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            animator.SetBool("Run", false);

            if (Input.GetKey("s") && !isOnCooldown && !(animator.GetBool("Inicio")))
            {
                holdTime += Time.deltaTime;
                animator.SetBool("Loading", true);
                animator.Play("Loading");
                if (holdTime >= holdDuration)
                {
                    cargar.Play();
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
                        Debug.Log("Cargado");
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

        // Mejora de salto
        if (betterJump)
        {
            if (rb2D.velocity.y < 0)
            {
                rb2D.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
            }
            if (rb2D.velocity.y > 0 && !Input.GetKey("space"))
            {
                rb2D.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;
            }
        }
    }

    // --------------------------
    // DASH (corutina limpia)
    // --------------------------
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        dashsound.Play();
        animator.Play("Dash");

        float originalGravity = rb2D.gravityScale;
        rb2D.gravityScale = 0;

        float dashDir = spriteRenderer.flipX ? -1f : 1f;
        rb2D.velocity = new Vector2(dashDir * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb2D.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
