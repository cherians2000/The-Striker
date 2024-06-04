using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    [Header("Player Health")]

    const float maxHealth = 150f;
    public float currentHealth;
    public Slider healthbarSlider;
    public GameObject playerUI;

    [Header("Ref & Physics")]

    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    PlayerControllerManager playerControllerManager;


    Vector3 moveDirection;

    Transform cameragameObject;
    Rigidbody playerRigidbody;

    [Header("Falling And Landing")]

    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;


    [Header("Movement flags")]

    public bool isSprinting;
    public bool isMoving;
    public bool isGrounded;
    public bool isJumping;


    [Header("Movement Values")]

    public float movementSpeed=2;
    public float rotationSpeed = 13f;
    public float sprintingSpeed = 7f;


    [Header("Jump var")]

    public float jumpHeight = 4f;
    public float gravityIntensity = -15f;


    PhotonView view;

    public int playerTeam;
    private void Awake()
    {
        currentHealth = maxHealth;
        healthbarSlider.minValue= 0f; 
        healthbarSlider.maxValue= maxHealth;
        healthbarSlider.value=currentHealth;
      
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameragameObject = Camera.main.transform;

        view = GetComponent<PhotonView>();
        playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();


    }
    private void Start()
    {
        if (!view.IsMine)
        {
            Destroy(playerUI);
            Destroy(playerRigidbody);
        }
        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }

    }
    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting)
            return;
            HandleMovement();
            HandleRotation();
        
        
    }
    void HandleMovement()
    {
        if (isJumping)
            return;

        moveDirection = cameragameObject.forward * inputManager.verticalInput;
        moveDirection=moveDirection+cameragameObject.right*inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if(inputManager.movementAmount >= 0.5f)
            {
                moveDirection = moveDirection * movementSpeed;
                isMoving = true;
            }
             if(inputManager.movementAmount <= 0f)
            {
                isMoving = false;
            }
        }

        Vector3 movementVelcity = moveDirection;
        playerRigidbody.velocity = movementVelcity;
    }
    void HandleRotation()
    {
        if (isJumping)
            return;
        Vector3 targetDirection = Vector3.zero;

        targetDirection=cameragameObject.forward*inputManager.verticalInput;
        targetDirection= targetDirection+ cameragameObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection=transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;

        if (!isMoving && !isSprinting)
        {
            float lookAngle = CameraManager.lookAngle;
            transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);
        }
    }

    void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrgin = transform.position;
        Vector3 targetPosition;
        rayCastOrgin.y = rayCastOrgin.y + rayCastHeightOffset;
        targetPosition =transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }
            inAirTimer =inAirTimer+ Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity*inAirTimer);
        }

        if(Physics.SphereCast(rayCastOrgin,0.2f,-Vector3.up,out hit, groundLayer))
        {
            if(!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            }

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded=false;  
        }
        if(isGrounded && !isJumping)
        {
            if(playerManager.isInteracting || inputManager.movementAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if(isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt( - 2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity= playerVelocity;
            isJumping = false;

        }
    }

    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }

 
    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!view.IsMine)
            return;
        currentHealth-=damage;
        healthbarSlider.value= currentHealth;
        if(currentHealth<=0)
        {
            Die();
        }
        Debug.Log("Damage Taken" + damage);
        Debug.Log(gameObject.name+"current Health" + currentHealth);
    }
    public void ApplyDamage(float damageValue)
    {
        view.RPC("RPC_TakeDamage", RpcTarget.All, damageValue);
    }


    public void Die()
    {
       playerControllerManager.Die();
        ScoreBoard.Instance.PlayerDied(playerTeam);
    }
}
