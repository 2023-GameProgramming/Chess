using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // ī�޶� ���� ��� ĳ������ Transform
    public float sensitivity = 6f; // ���콺 ����
    public float maxYAngle = 40; // ī�޶��� �ִ� �󰢵�
    public float minYAngle = 0f; // ī�޶��� �ִ� �ϰ���
    public float far = 3;
    private float currentXAngle = 0f;
    private float currentYAngle = 0f;
    bool Coll;
    void Start()
    {
        // �ʱ�ȭ
        currentXAngle = transform.eulerAngles.x;
        currentYAngle = transform.eulerAngles.y;
        Coll = false;
    }



    private void Update()
    {
        Coll = false;
        // ī�޶�� �÷��̾� ������ ���� ���͸� ����մϴ�.
        Vector3 direction = target.position - transform.position;
        // ī�޶󿡼� �÷��̾� �������� ����ĳ��Ʈ�� �߻��մϴ�.
        RaycastHit hit;
        if (Physics.Raycast(transform.position - direction.normalized *0.3f, direction, out hit, Vector3.Distance(target.position, transform.position)))
        {
            // �浹�� ��ü�� �ִ� ��� ó���� ������ �ۼ��մϴ�.
            if (hit.collider.CompareTag("CameraWall"))
            {
                Coll = true;
            }
        }
       







        if (Coll)
        {
            if(far>1)
            {
                far -= Time.deltaTime;
                if(far<1)
                {
                    far = 1;
                }
            }
        }
        else
        {
            if (far < 2)
            {
                far += Time.deltaTime;
                if (far > 2)
                {
                    far = 2;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraWall"))
        {
            Coll = false;
        }
    }
    void LateUpdate()
    {
        // ���콺 �Է¿� ���� ���� ��ȭ ���
        float mouseX = Input.GetAxis("Mouse X") * sensitivity *100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity*100 * Time.deltaTime;
        currentXAngle -= mouseY;
        currentYAngle += mouseX;

        // ���� ���� ����
        currentXAngle = Mathf.Clamp(currentXAngle, minYAngle, maxYAngle);

        // ī�޶� ȸ��
        transform.rotation = Quaternion.Euler(currentXAngle, currentYAngle, 0f);

        // ī�޶� ��ġ ����
        transform.position = target.position - transform.forward * far;
    }
}