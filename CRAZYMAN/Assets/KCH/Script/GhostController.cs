using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float defaultSpeed = 5f;
    private float runSpeed = 10f;
    public float mouseSensitivity = 250f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public Transform cameraTransform;
    public CharacterController controller;

    public String ghostLayerName = "Ghost";
    public String enemyLayerName = "Enemy";
    // Start is called before the first frame update
    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = transform.GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // 커서 숨기기

        int ghostLayer = LayerMask.NameToLayer(ghostLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);

        for (int i = 0; i < 32; i++)
        {
            if (i != ghostLayer)
            {
                Physics.IgnoreLayerCollision(ghostLayer, i, true);
            }
        }
        Physics.IgnoreLayerCollision(ghostLayer, ghostLayer, true); // 유령끼리도 무시
        gameObject.layer = ghostLayer;
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = ghostLayer; // 자식 오브젝트도 유령 레이어로 설정
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateView();
        Move();
    }

    void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        moveSpeed = defaultSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = runSpeed;
        }

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
