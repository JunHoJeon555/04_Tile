using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 3.0f;

    Vector2 inputDir;
    bool isMove = false;

    Animator anim;

    PlayerInputActions inputActions;

    private void Awake()
    {
        anim= GetComponent<Animator>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Attack.performed += OnAttack;
    }

    

    private void OnDisable()
    {
        inputActions.Player.Attack.performed-= OnAttack;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        transform.Translate(Time.fixedDeltaTime * speed * inputDir);       
    }

 

    private void OnMove(InputAction.CallbackContext context)
    {

        inputDir = context.ReadValue<Vector2>();
        isMove = true;

        anim.SetFloat("InputX", inputDir.x);
        anim.SetFloat("InputY", inputDir.y);
        anim.SetBool("IsMove", true);
        

    }


    private void OnStop(InputAction.CallbackContext context)
    {
        inputDir = Vector2.zero;
        isMove = false;
        
        anim.SetBool("IsMove", false);
        
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        anim.SetTrigger("Attack");
    }

}
