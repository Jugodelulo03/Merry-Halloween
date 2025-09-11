using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMove : MonoBehaviour
{
    public AudioSource salto;
    public AudioSource dashsound;
    public AudioSource cargar;

    public float runSpeed=2;

    public float jumpSpeed=3;

    public float wallSlidiningSpeed=0.5f;

    bool wallSliding=false;

    public float dashDistance = 1;

    private float steps = 10;

    private float Countsteps = 0;

    private bool isDashing=false;

    public float DoubleJumpSpeed = 3f;

    private bool canDoubleJump;

    Rigidbody2D rb2D;

    public bool betterJump=false;

    public float fallMultiplier=0.5f;

    public float lowJumpMultiplier=1f;

    public float holdDuration = 1.0f; // Tiempo necesario para mantener presionada la tecla

    private float holdTime = 0.0f;    // Tiempo acumulado mientras se mantiene presionada la tecla

    public float cooldownDuration = 0.5f;  // Tiempo de espera después de activar el parámetro

    private float cooldownTime = 0.0f;  // Tiempo acumulado durante el cooldown

    private bool isOnCooldown = false;  // Estado de cooldown

    public SpriteRenderer spriteRenderer;

    public Animator animator;

    void Start()
    {
        rb2D= GetComponent<Rigidbody2D>();
        animator.SetBool("DoubleJump", false);
        animator.SetBool("Wall", false);
    }

    void dash()
    {
        if (spriteRenderer.flipX)
        {
            animator.Play("Dash");
            rb2D.MovePosition(rb2D.position + new Vector2(-1, 0.1f) * dashDistance / steps);
        }
        else
        {
            animator.Play("Dash");
            rb2D.MovePosition(rb2D.position + new Vector2(1, 0.1f) * dashDistance / steps);
        }
        Countsteps++;
    }

    private void Update()
    {
        //JUMP ---------------
        if (Input.GetKey("space") )
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
        else if(rb2D.velocity.y > 0)
        {
            animator.SetBool("Falling", false);
        }

        //WALLJUMP ---------------

        if ( !((CheckRightSide.RightWall) || (CheckLeftSide.LeftWall)))
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
            if (Input.GetKey("space") && (Input.GetKey("a")) && (CheckRightSide.RightWall))
            {
                salto.Play();
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpSpeed*0.7f);
                rb2D.velocity = new Vector2(-runSpeed, rb2D.velocity.y);
            }
            else if (Input.GetKey("space") && (Input.GetKey("d")) && (CheckLeftSide.LeftWall))
            {
                salto.Play();
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpSpeed * 0.7f);
                rb2D.velocity = new Vector2(runSpeed, rb2D.velocity.y);
            }

        }

    }

    void FixedUpdate()
    {
        //COOLDOWN CARGAR
        // Si estamos en cooldown, reducimos el tiempo hasta que pase el cooldown
        if (isOnCooldown)
        {
            cooldownTime += Time.deltaTime;
            if (cooldownTime >= cooldownDuration)
            {
                isOnCooldown = false;
                cooldownTime = 0.0f;  // Reiniciamos el cooldown
            }
        }

        //DASH -------------
        if (isDashing && (Countsteps <= steps))
        {
            dash();
            if (Countsteps == steps)
            {
                isDashing = false;
                Countsteps = 0;
            }
        }

        if (Input.GetKey("left shift") && (animator.GetBool("Navidad")) && (!isOnCooldown))
        {
            dashsound.Play();
            Countsteps = 0;
            isDashing = true;
            isOnCooldown = true;
            dash();
        }

        //Der
        if (Input.GetKey("d") && (!Input.GetKey("s"))){
            rb2D.velocity = new Vector2(runSpeed, rb2D.velocity.y);
            spriteRenderer.flipX=false;
            animator.SetBool("Run", true);


        }
        else if(Input.GetKey("a") && (!Input.GetKey("s")))
        {//Izq
            rb2D.velocity = new Vector2(-runSpeed, rb2D.velocity.y);
            spriteRenderer.flipX=true;
            animator.SetBool("Run", true);
        }
        else{//quieto
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            animator.SetBool("Run", false);

            if (Input.GetKey("s") && !isOnCooldown && !(animator.GetBool("Inicio")) )
            {
                holdTime += Time.deltaTime; // Incrementa el tiempo mientras la tecla está presionada
                animator.SetBool("Loading", true);
                animator.Play("Loading");
                if (holdTime >= holdDuration)
                {
                    cargar.Play();
                    if (animator.GetBool("Navidad"))
                    {
                        animator.SetBool("Navidad", false);
                        isOnCooldown = true;  // Activa el estado de cooldown
                        //Debug.Log("Cargado");
                        animator.Play("Idle");
                        return;
                    }
                    else
                    {
                        animator.SetBool("Navidad", true);
                        isOnCooldown = true;  // Activa el estado de cooldown
                        Debug.Log("Cargado");
                        animator.Play("Idle 0");
                        return;
                    }
                }
            }
            else
            {
                holdTime = 0.0f; // Reinicia el tiempo acumulado cuando se suelta la tecla
                animator.SetBool("Loading", false);
            }
        }

        if(betterJump){
            if(rb2D.velocity.y<0){
                rb2D.velocity += Vector2.up*Physics2D.gravity.y*fallMultiplier*Time.deltaTime;
            }
            if(rb2D.velocity.y>0 && !Input.GetKey("space")){
                rb2D.velocity += Vector2.up*Physics2D.gravity.y*lowJumpMultiplier*Time.deltaTime;                
            }
        }


    }
}