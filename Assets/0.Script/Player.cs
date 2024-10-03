using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * 플레이어가 손에 아이템을 들 수 있다.
 * wasd와 방향키로 이동
 * space키와 right_ctrl 아이템 들고 놓기
 * Lshift와 Rshift 대쉬 쿨타임 있는
 * 자원 캐기, 물건 내려놓기, 걷기, 달리기 등의 애니매이션
 */

public class Player : MonoBehaviour
{
    public Animator anim;
    public float moveSpeed = 5f;       // 이동 속도
    public float dashSpeed = 10f;      // 대쉬 속도
    public float rotationSpeed = 5f;   // 회전 속도
    public float dashCooldown = 2f;    // 대쉬 쿨타임
    public GameObject Axe;
    public GameObject Pick;

    private float X, Y;
    private bool holding = false;      // 손에 물건을 들고 있는지 여부
    private Vector3 lastMoveDir;       // 마지막 이동 방향
    private float lastDashTime = -2f;  // 마지막 대쉬 시간 초기화

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        // 이동 방향 계산
        Vector3 moveDir = new Vector3(-Y, 0f, X).normalized;

        // 이동이 있으면 'isWalking' 애니메이션 상태로 전환, 이동이 없으면 'Idle'
        if (moveDir.magnitude > 0f)
        {
            anim.SetBool("isWalking", true);   // 걷기 상태
            lastMoveDir = moveDir;             // 마지막 이동 방향 저장
        }
        else
        {
            anim.SetBool("isWalking", false);  // 걷기 상태 해제 (Idle)
        }

        // 이동 처리
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        // 대쉬 입력 처리 (쿨타임 적용)
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > lastDashTime + dashCooldown)
        {
            Vector3 dashDir = new Vector3(-lastMoveDir.x, 0f, -lastMoveDir.z).normalized;
            transform.Translate(dashDir * dashSpeed * Time.deltaTime, Space.World);

            lastDashTime = Time.time; // 대쉬한 시간 기록
        }

        // 물건 들고/내리기 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (!holding)
            {
                
                holding = true;
            }
            else
            {
                
                holding = false;
            }
        }

        // 플레이어가 움직일 때 회전 처리
        if (X != 0f || Y != 0f)
        {
            Vector3 dir = new Vector3(-X, 0f, -Y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = targetRotation;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe"))
        {
            other.transform.SetParent(transform);
        }

    }

}