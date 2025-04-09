using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public float v = 0.0f;
    public float h = 0.0f;
    public float moveSpeed = 5.0f;
    private float defaultSpeed = 5.0f;
    private float runSpeed = 10.0f;
    private float crouchSpeed = 2.0f;
    public Transform PlayerTr;
    public Animator animator;
    public bool canRun = false;
    public bool canCrouch = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        moveSpeed = defaultSpeed;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            canCrouch = true;
            canRun = false; // 앉는 중에는 달릴 수 없음
            moveSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && v > 0.01f)
        {
            canRun = true;
            canCrouch = false; // 달리는 중에는 앉을 수 없음
            moveSpeed = runSpeed;
        }
        else
        {
            canRun = false;
            canCrouch = false;
        }

        bool isMoving = v > 0.01f || v < -0.01f;
        animator.SetBool("isMoving", isMoving);

        animator.SetFloat("v", v);
        animator.SetBool("canRun", canRun);
        animator.SetBool("canCrouch", canCrouch);

        PlayerTr.Translate(new Vector3(h, 0, v) * moveSpeed * Time.deltaTime);
        // Vector3 moveDir = new Vector3(h, 0, v);
        // Vector3 move = moveDir * moveSpeed * Time.deltaTime;
        // GetComponent<Rigidbody>().MovePosition(transform.position + move);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter " + collision.gameObject.name);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("OnCollisionStay " + collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit " + collision.gameObject.name);
    }
}
