using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform playerTransform; // Player의 transform 정보를 담을 변수 선언
    [SerializeField] Vector3 offset = new Vector3(0, 1.6f, 0);
    private float defaultHeight = 1.6f;
    private float crouchHeight = 0.7f;
    private Control playerControl;
    private float heightChangeSpeed = 5f; // 높이 전환 속도

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // GameObject의 메서드인 Find로 Player 오브젝트를 찾은 뒤 Position 정보를 가져온.
        playerControl = playerTransform.GetComponent<Control>(); // Control 스크립트 가져오기
        if (playerControl == null)
        {
            Debug.LogError("Control 스크립트를 찾을 수 없습니다!");
        }
    }

    private void LateUpdate() // 카메라 움직임은 주로 LateUpdate에 적는다.
    {
        // canCrouch에 따라 카메라 높이 조정
        float targetHeight = playerControl.canCrouch ? crouchHeight : defaultHeight;
        offset.y = Mathf.Lerp(offset.y, targetHeight, heightChangeSpeed * Time.deltaTime);

        Vector3 newPosition = playerTransform.position + offset;
        transform.position = newPosition;
        // 카메라의 position에 player의 position 정보를 넣어준다.
    }
}
