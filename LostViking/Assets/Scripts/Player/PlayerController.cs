using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;


    [Header("Jump Settings")]
    Vector2 gravityVelocity;
    bool isJumping;
    float jumpCounter;
    [SerializeField] float fallMultiplier; [Tooltip("valor a ser multiplicado na queda do personagem")]
    [SerializeField] float jumpMultiplier; [Tooltip("valor a ser multiplicado no pulo do personagem, influenciando no pulo")]
    [SerializeField] float jumpTime; [Tooltip("contador , que influencia no tempo que o jogador segura o botao de pulo")]
    [SerializeField] int jumpForce; [Tooltip("força do pulo do player")]
    [SerializeField] LayerMask groundLayer; [Tooltip("layer de verificação do chão")]
    [Header("-------------------------------")]

    [Header("Move Settings")]
    [SerializeField] int speed; [Tooltip("velocidade de movimento do jogador")]
    Vector2 moveDirection; 



    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        gravityVelocity = new Vector2(0, -Physics2D.gravity.y);
    }
    private void FixedUpdate()
    {
        onMove();

        if(rigidBody.velocity.y < 0)
        {
            rigidBody.velocity -= gravityVelocity * fallMultiplier * Time.fixedDeltaTime;
        }

        if(rigidBody.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.fixedDeltaTime;
            if(jumpCounter > jumpTime)
            {
                isJumping = false;
            }

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if(t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rigidBody.velocity += gravityVelocity * currentJumpM * Time.fixedDeltaTime;
        }
    }


    public void jump(InputAction.CallbackContext value)
    {
        if (value.started && onGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            jumpCounter = 0;
        }


        if (value.canceled)
        {
            isJumping = false;
            jumpCounter = 0;

            if(rigidBody.velocity.y > 0)
            {
                rigidBody.AddForce(Vector2.up * 0.4f, ForceMode2D.Impulse);
            }
        }
    }

    private bool onGrounded()
    {
        float distance = 0.01f;
        int angle = 0;
        RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, angle, Vector2.down, distance, groundLayer);
        return raycast.collider != null;
    }

    public void Movement(InputAction.CallbackContext value)
    {
        moveDirection = value.ReadValue<Vector2>();
    }

    public void onMove()
    {
        rigidBody.velocity = new Vector2(moveDirection.x * speed * Time.fixedDeltaTime, rigidBody.velocity.y);
    }

}
