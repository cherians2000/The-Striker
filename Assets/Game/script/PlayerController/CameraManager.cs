using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    public Transform playerTransform;
    public Transform cameraTransform;

    [Header("Camera Movement")]
    public Transform cameraPivot;

    Vector3 cameraFollowVelocity =Vector3.zero;
    public float cameraFollowSpeed = 0.3f;

    public float camLookSpeed = 2;
    public float camerPivotSpeed = 2;

    public static float lookAngle;//up and down movement of the camera
    public float pivotAngle;//left and right movement of the camera

    public float minimumPiotAngle = -11;
    public float maximumPiotAngle = 30;

    [Header("camera collision")]

    public LayerMask collisionLayer;

    private float defaultPosition;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;

    private Vector3 cameraVectorPosition;

    private PlayerMovement playerMovement;


    [Header("Scope")]

    public GameObject scopeCanvas;
    public GameObject playerUI;
    public Camera mainCamera;
    bool isScoped = false;
    float originalFOV = 60f;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        inputManager = FindObjectOfType<InputManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform =Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
       
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState=CursorLockMode.None; Cursor.visible = true;
        }
    }
    public void HandleAllCameraMovement()
    {
        RotateCamera();
        FollowTarget();
        CameraCollision();
        isPlayerScoped();
    }
     void FollowTarget()
    {
        Vector3 targerPosition =Vector3.SmoothDamp(transform.position,playerTransform.position,ref cameraFollowVelocity,cameraFollowSpeed);
        transform.position = targerPosition;
    }
     void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * camLookSpeed);
        pivotAngle = pivotAngle + (inputManager.cameraInputY * camerPivotSpeed);
        pivotAngle= Mathf.Clamp(pivotAngle,minimumPiotAngle,maximumPiotAngle);
        //up & down 
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation =targetRotation;

        //left and right
        rotation = Vector3.zero;
        rotation.x = -pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

       
    }

    void CameraCollision()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

       if( Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Math.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }
       if(Mathf.Abs(targetPosition)<minCollisionOffset)
        {
            targetPosition =targetPosition - minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    public void isPlayerScoped()
    {
        if (inputManager.scopeInput)
        {
            scopeCanvas.SetActive(true);
            playerUI.SetActive(false);
            mainCamera.fieldOfView = 10f;
        }
        else
        {
            scopeCanvas.SetActive(false);
            playerUI.SetActive(true);
            mainCamera.fieldOfView = originalFOV;
        }
    }
}
