using UnityEngine;

public class PlayermController : MonoBehaviour
{
    private Rigidbody playerRb;

    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float sensivity;

    private float yRotation = 0;
    private float xRotation = 0;
    private float mouseXInput = 0;
    private float mouseYInput = 0;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        PlayerMovement();
        PlayerRotation();
    }

    private void PlayerMovement()
    {
        Vector2 userInput = SavedInputKey();
        Vector3 moveDir = new Vector3(userInput.x, 0, userInput.y);
        moveDir = transform.TransformDirection(moveDir);

        if(!(userInput.x == 0 && userInput.y == 0))
        {
            playerRb.AddForce(moveDir * speed * Time.deltaTime);
        }
        else
        {
            playerRb.velocity = Vector3.zero;
        }
    }

    private Vector2 SavedInputKey()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1f;
        }
        if(Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = 1f;
        }

        inputVector = inputVector.normalized;

        return inputVector;
    }

    private void PlayerRotation()
    {
        // Quaternion currentRotation = Quaternion.Euler(0f, yRotation, 0f);

        mouseXInput = Input.GetAxis("Mouse X") * sensivity;
        //mouseYInput = Input.GetAxis("Mouse Y") * sensivity;
        yRotation += mouseXInput;
        //xRotation += mouseYInput;

        //Quaternion destination = Quaternion.Euler(0, yRotation, 0);
        //transform.rotation = Quaternion.Slerp(currentRotation, destination, turnSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

}
