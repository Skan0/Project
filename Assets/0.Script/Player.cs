using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*
 * 플레이어가 손에 아이템을 들 수 있다.
 * wasd와 방향키로 이동
 * space키와 right_ctrl 아이템 들고 놓기
 * Lshift와 Rshift 대쉬 쿨타임 있는
 * 자원 캐기, 물건 내려놓기, 걷기, 달리기 등의 애니매이션
 * 
 * 설계에 문제가 있었다. 물건 자체의 콜라이더와 충돌을 체크할 게 아니라 타일맵의 box와 충돌 체크를 하고 key를 눌렀을 때 검사해서 자식 오브젝트
 * 다 갈아엎어야함
 */

public class Player : MonoBehaviour
{
    public Transform HoldingTrans;
    public Animator anim;
    
    public float moveSpeed = 5f;        // 이동 속도
    public float dashSpeed = 150f;      // 대쉬 속도
    public float rotationSpeed = 5f;    // 회전 속도
    public float dashCooldown = 2f;     // 대쉬 쿨타임

    private GameObject nearItem;

    private float lastDashTime = -2f;   // 마지막 대쉬 시간 초기화
    private float X, Y;
    private string[] tagsToCheck = { "Axe", "Pick","Wood","Stone"}; // 손에 들 수 있는 물건들의 태그 목록
    private string[] tagsToBreak = { "Tree", "Rock" };
    private bool closeToStuff = false;  // 물건의 collider에 접촉중인가
    private string holdingstuff =null;    // 들고있는 물건의 이름으로 행동제어
    private Vector3 lastMoveDir;        // 마지막 이동 방향
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Movement();
        Dash();
        Interaction();
    }
    void Movement()
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

       
        // 플레이어가 움직일 때 회전 처리
        if (X != 0f || Y != 0f)
        {
            Vector3 dir = new Vector3(-X, 0f, -Y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = targetRotation;
        }
    }
    void Dash()
    {
        // 대쉬 입력 처리 (쿨타임 적용)
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > lastDashTime + dashCooldown)
        {
            //Debug.Log("Dash");
            Vector3 dashDir = new Vector3(lastMoveDir.x, 0f, lastMoveDir.z).normalized;
            transform.Translate(dashDir * dashSpeed * Time.deltaTime, Space.World);

            lastDashTime = Time.time; // 대쉬한 시간 기록
        }
    }
    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (nearItem != null)//가까운데 아이템이 있다.
            {
                if (holdingstuff == null)//들고있는 물건이 없는가 확인
                {
                    PickUpItem();
                }
                else
                {
                    SwapItem();
                    PickUpItem();
                }
            }
        }
    }
    void SwapItem()
    {
        GameObject temp = HoldingTrans.GetChild(0).gameObject;
        Transform Parent = nearItem.transform.parent;
        temp.transform.SetParent(Parent);
        temp.transform.position = nearItem.transform.position;
        temp.transform.rotation=Quaternion.identity;
        temp.transform.localScale = Vector3.one;

        holdingstuff = nearItem.tag;
    }
    void PickUpItem()
    {
        // 부모를 변경 (HoldingTrans로)
        nearItem.transform.SetParent(HoldingTrans);

        // 부모 오브젝트의 로컬 좌표를 기준으로 위치, 회전, 스케일을 초기화
        nearItem.transform.localPosition = Vector3.zero;
        nearItem.transform.localRotation = Quaternion.identity;
        nearItem.transform.localScale = Vector3.one;  // 스케일 초기화 (1, 1, 1)

        holdingstuff = nearItem.tag;
    }


    private void OnTriggerEnter(Collider other)
    {
        foreach (string tag in tagsToCheck)
        {
            if (other.CompareTag(tag))
            {
                closeToStuff = true;
                nearItem = other.gameObject;
                break;  
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        foreach (string tag in tagsToCheck)
        {
            if (other.CompareTag(tag))
            {
                closeToStuff = false;
                nearItem = null;
                break;
            }
        }
    }
}