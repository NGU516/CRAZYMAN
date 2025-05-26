using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 250f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public MentalGauge mentalGauge;

    // Start is called before the first frame update
    void Start()
    {
        if (mentalGauge == null)
        {
            Debug.LogError("MentalGauge is not assigned in Control! Please assign it in the Inspector.");
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (mentalGauge != null && mentalGauge.isDeath)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
