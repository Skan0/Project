using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Train Script에서 처리하고자 하는 것
 * 시간이 지남에 따라 이어진 선로 위로 이동을 해야함
 * 앞에 선로가 없다면 부서진다.
 * 도착지점까지 선로가 이어졌다면 빠른속도로 기차가 이동한다.
 */


//raycast로 앞의 선로를 체크해서 이동하는 방식으로 진행하려 했으나 회전하는 선로가 나왔을 때 멀리서 읽고 돌아버리는 문제
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
//                        //직진
//                        transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime);
//                    }
//                    else if (hit.collider.CompareTag("R_Rail"))
//                    {
//                        //우회전
//                        StartCoroutine(TurnAndMove(Vector3.up, 90f));
//                    }
//                    else if (hit.collider.CompareTag("L_Rail"))
//                    {
//                        //좌회전
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
//        // 회전 처리
//        Quaternion startRotation = transform.rotation;
//        Quaternion endRotation = startRotation * Quaternion.Euler(turnAxis * turnAngle);
//        float elapsedTime = 0f;
//        float targetTime = Mathf.Abs(turnAngle / turnSpeed);  // 회전에 걸리는 시간

//        while (elapsedTime < targetTime)
//        {
//            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / targetTime);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        transform.rotation = endRotation;  // 정확한 최종 각도로 설정
//    }
//    private void OnDrawGizmos()//Ray가 어떻게 나가고 있나 확인하는 용도
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
//    }
//}
public class TrainMovement : MonoBehaviour
{
    public float moveSpeed = 0.2f;               // 기차 이동 속도
    public float rotationSpeed = 50f;           // 회전 속도
    private bool isTurningRight = false;       // 우회전 중인지 여부 확인
    private bool isTurningLeft = false;        // 좌회전 중인지 여부 확인
    private Quaternion targetRotation;         // 목표 회전 각도

    void Update()
    {
        if (GameManager.Instance.State == GameState.Play)
        {
            // 우회전 중일 때 처리
            if (isTurningRight)
            {
                // 부드럽게 우회전
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 회전 완료시 상태 초기화
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    isTurningRight = false;
                }

                // 회전 중에도 전진
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            // 좌회전 중일 때 처리
            else if (isTurningLeft)
            {
                // 부드럽게 좌회전
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 회전 완료시 상태 초기화
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    isTurningLeft = false;
                }

                // 회전 중에도 전진
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                // 좌우회전 중이 아닐 때는 직진
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
        }
    }

    // Collider 충돌 감지
    private void OnTriggerEnter(Collider other)
    {
        // 우회전 레일(R_Rail) 감지
        if (other.CompareTag("R_Rail"))
        {
            if (!isTurningRight)
            {
                isTurningRight = true;
                // 우회전 각도 설정
                targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 90, 0));
            }
        }

        // 좌회전 레일(L_Rail) 감지
        else if (other.CompareTag("L_Rail"))
        {
            if (!isTurningLeft)
            {
                isTurningLeft = true;
                // 좌회전 각도 설정
                targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -90, 0));
            }
        }
    }
}

