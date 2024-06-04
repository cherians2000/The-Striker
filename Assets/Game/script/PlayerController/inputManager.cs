using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    
    PlayerController PlayerControls;
    AnimatorManager animatorManager;
    PlayerMovement playerMovement;


    public Vector2 movementInput;

    public Vector2 cameraMovementInput;

    public float cameraInputX;
    public float cameraInputY;


    public float verticalInput;
    public float horizontalInput;
    public float movementAmount;

    [Header("Inputs Buttons Flags")]

    public bool bInput;
    public bool jumpInput;
    public bool fireInput;
    public bool reloadInput;
    public bool scopeInput;


    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
     
    }
    private void OnEnable()
    {
        if(PlayerControls == null)
        {
            PlayerControls=new PlayerController();
            PlayerControls.PlayerMovement.Movement.performed +=i => movementInput=i.ReadValue<Vector2>();
            PlayerControls.PlayerMovement.CameraMovement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
            PlayerControls.PlayerAction.B.performed += i => bInput = true;
            PlayerControls.PlayerAction.B.canceled += i => bInput = false;
            PlayerControls.PlayerAction.Jump.performed += i => jumpInput = true;
            PlayerControls.PlayerAction.Fire.performed += i => fireInput = true;
            PlayerControls.PlayerAction.Fire.canceled += i => fireInput = false;
            PlayerControls.PlayerAction.Reload.performed += i => reloadInput = true;
            PlayerControls.PlayerAction.Reload.canceled += i => reloadInput = false;
            PlayerControls.PlayerAction.Scope.performed += i => scopeInput = true;
            PlayerControls.PlayerAction.Scope.canceled += i => scopeInput = false;

        }
        PlayerControls.Enable();
    }
    private void OnDisable()
    {
        PlayerControls.Disable();
    }
    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
    }
    private void HandleMovementInput()
    {
        verticalInput=movementInput.y;
        horizontalInput=movementInput.x;  

        cameraInputX=cameraMovementInput.x;
        cameraInputY=cameraMovementInput.y;


        movementAmount=Mathf.Clamp01(Mathf.Abs(horizontalInput)+Mathf.Abs(verticalInput));
        animatorManager.ChangeAnimatorValues(0, movementAmount,playerMovement.isSprinting);
    }
    
    private void HandleSprintingInput()
    {
       if( bInput && movementAmount > 0.5)
        {
            playerMovement.isSprinting = true;
        }
        else
        {
            playerMovement.isSprinting=false;
        }
    }

    private void HandleJumpingInput()
    {
        if(jumpInput)
        {
            jumpInput = false;
            playerMovement.HandleJumping();
        }
    }
}
