using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Train Script���� ó���ϰ��� �ϴ� ��
 * �ð��� ������ ���� �̾��� ���� ���� �̵��� �ؾ���
 * �տ� ���ΰ� ���ٸ� �μ�����.
 * ������������ ���ΰ� �̾����ٸ� �����ӵ��� ������ �̵��Ѵ�.
 */


//raycast�� ���� ���θ� üũ�ؼ� �̵��ϴ� ������� �����Ϸ� ������ ȸ���ϴ� ���ΰ� ������ �� �ָ��� �а� ���ƹ����� ����
//public class Train : MonoBehaviour
//{
//    public float moveSpeed = 1f;
//    public float rayDistance = 1f;
//    public float forwardDistance = 0.6f;
//    public float turnSpeed = 90f;

//    public Animator anim;

//    bool isMovingForward = true;

//    private void Start()
//    {
//        anim = GetComponent<Animator>();
//    }


//    void Update()
//    {
//        if (GameManager.Instance != null)
//        {
//            GameState currentState = GameManager.Instance.State;

//            Ray ray = new Ray(transform.position, transform.forward);
//            RaycastHit hit;

//            if (currentState == GameState.Play)
//            {
//                if (Physics.Raycast(ray, out hit, rayDistance))
//                {

//                    if (hit.collider.CompareTag("S_Rail"))
//                    {
//                        //����
//                        transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime);
//                    }
//                    else if (hit.collider.CompareTag("R_Rail"))
//                    {
//                        //��ȸ��
//                        StartCoroutine(TurnAndMove(Vector3.up, 90f));
//                    }
//                    else if (hit.collider.CompareTag("L_Rail"))
//                    {
//                        //��ȸ��
//                        StartCoroutine(TurnAndMove(Vector3.up, -90f));
//                    }
//                }
//                else
//                {
//                    if (isMovingForward)
//                    {
//                        StartCoroutine(MoveMore());
//                    }
//                }
//            }
//        }
//    }
//    IEnumerator MoveMore()
//    {
//        isMovingForward = false;
//        float elapsedTime = 0f;
//        float targetTime = forwardDistance / moveSpeed;
//        while (elapsedTime<targetTime)
//        {
//            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }
//    }
//    IEnumerator TurnAndMove(Vector3 turnAxis, float turnAngle)
//    {
//        // ȸ�� ó��
//        Quaternion startRotation = transform.rotation;
//        Quaternion endRotation = startRotation * Quaternion.Euler(turnAxis * turnAngle);
//        float elapsedTime = 0f;
//        float targetTime = Mathf.Abs(turnAngle / turnSpeed);  // ȸ���� �ɸ��� �ð�

//        while (elapsedTime < targetTime)
//        {
//            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / targetTime);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        transform.rotation = endRotation;  // ��Ȯ�� ���� ������ ����
//    }
//    private void OnDrawGizmos()//Ray�� ��� ������ �ֳ� Ȯ���ϴ� �뵵
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
//    }
//}
public class TrainMovement : MonoBehaviour
{
    public float moveSpeed = 0.2f;               // ���� �̵� �ӵ�
    public float rotationSpeed = 50f;           // ȸ�� �ӵ�
    private bool isTurningRight = false;       // ��ȸ�� ������ ���� Ȯ��
    private bool isTurningLeft = false;        // ��ȸ�� ������ ���� Ȯ��
    private Quaternion targetRotation;         // ��ǥ ȸ�� ����

    void Update()
    {
        if (GameManager.Instance.State == GameState.Play)
        {
            // ��ȸ�� ���� �� ó��
            if (isTurningRight)
            {
                // �ε巴�� ��ȸ��
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // ȸ�� �Ϸ�� ���� �ʱ�ȭ
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    isTurningRight = false;
                }

                // ȸ�� �߿��� ����
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            // ��ȸ�� ���� �� ó��
            else if (isTurningLeft)
            {
                // �ε巴�� ��ȸ��
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // ȸ�� �Ϸ�� ���� �ʱ�ȭ
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    isTurningLeft = false;
                }

                // ȸ�� �߿��� ����
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                // �¿�ȸ�� ���� �ƴ� ���� ����
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
        }
    }

    // Collider �浹 ����
    private void OnTriggerEnter(Collider other)
    {
        // ��ȸ�� ����(R_Rail) ����
        if (other.CompareTag("R_Rail"))
        {
            if (!isTurningRight)
            {
                isTurningRight = true;
                // ��ȸ�� ���� ����
                targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 90, 0));
            }
        }

        // ��ȸ�� ����(L_Rail) ����
        else if (other.CompareTag("L_Rail"))
        {
            if (!isTurningLeft)
            {
                isTurningLeft = true;
                // ��ȸ�� ���� ����
                targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -90, 0));
            }
        }
    }
}

