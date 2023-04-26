using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 대상 캐릭터의 Transform
    public float sensitivity = 6f; // 마우스 감도
    public float maxYAngle = 40; // 카메라의 최대 상각도
    public float minYAngle = 0f; // 카메라의 최대 하각도
    public float far = 3;
    private float currentXAngle = 0f;
    private float currentYAngle = 0f;

    void Start()
    {
        // 초기화
        currentXAngle = transform.eulerAngles.x;
        currentYAngle = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        // 마우스 입력에 따른 각도 변화 계산
        float mouseX = Input.GetAxis("Mouse X") * sensitivity *100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity*100 * Time.deltaTime;
        currentXAngle -= mouseY;
        currentYAngle += mouseX;

        // 상하 각도 제한
        currentXAngle = Mathf.Clamp(currentXAngle, minYAngle, maxYAngle);

        // 카메라 회전
        transform.rotation = Quaternion.Euler(currentXAngle, currentYAngle, 0f);

        // 카메라 위치 조정
        transform.position = target.position - transform.forward * far;
    }
}