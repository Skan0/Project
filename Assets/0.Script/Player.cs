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
    public static Player instance;

    public Transform HoldingTrans;      //아이템을 들고 있을 위치
    public Animator anim;               
    
    public float moveSpeed = 5f;        // 이동 속도
    public float dashSpeed = 800f;      // 대쉬 속도
    public float rotationSpeed = 5f;    // 회전 속도
    public float dashCooldown = 2f;     // 대쉬 쿨타임

    private Transform nearItemParent;   //Ground아래 collider
    private GameObject nearItem;        //근처의 아이템을 잠시 담아둘 공간

    private float lastDashTime = -2f;                   // 마지막 대쉬 시간 초기화
    private float X, Y;
    private string[] tagsToCheck = { "Axe", "Pick","Wood","Stone"}; // 손에 들 수 있는 물건들의 태그 목록
    private string[] tagsToBreak = { "Tree", "Rock" };  //이건 스스로 점차 부서지면서 애니메이션이 켜져야 할거 같으니까 다른데서 만들자.
    private bool closeToStuff = false;                  // 물건의 collider에 접촉중인가
    private string holdingstuff =null;                  // 들고있는 물건의 이름으로 행동제어
    private Vector3 lastMoveDir;                        // 마지막 이동 방향

    private void Awake()
    {
        if (this == null)
        {
            instance = this;
        }
       
    }
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
    
    // 플레이어 움직임 
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

    // 앞으로 약간 빠르게 이동시켜줌
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

    // 아이템과 관련된 상호작용을 담당하는 함수
    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("Pressed command");
            if (nearItem != null)//가까운데 아이템이 있다.
            {
                Debug.Log("nearItem != null");
                if (holdingstuff == null)//들고있는 물건이 없는가 확인
                {
                    PickUpItem();
                }
                else
                {
                    SwapItem();
                }
            }
            else if (holdingstuff != null)
            {
                Debug.Log(holdingstuff);
                //현재 충돌중인 콜라이더의 자식오브젝트로 놔줘야 함
                PutDownItem();
            }
        }
    }
    
    // 바닥이 비어있을 때 부를 함수
    void PutDownItem()
    {
        GameObject temp = HoldingTrans.GetChild(0).gameObject;
        temp.transform.SetParent(nearItemParent);
        temp.transform.position = nearItemParent.position;
        temp.transform.rotation = Quaternion.identity;
        temp.transform.localScale = Vector3.one;
        nearItemParent = null;
        holdingstuff = null;
    }

    // 손에 있는 아이템과 바닥에 있는 아이템을 바꿔준다.
    void SwapItem()
    {
        GameObject temp = HoldingTrans.GetChild(0).gameObject;
        Transform Parent = nearItem.transform.parent;
        temp.transform.SetParent(Parent);
        temp.transform.position = nearItem.transform.position;
        temp.transform.rotation=Quaternion.identity;
        temp.transform.localScale = Vector3.one;
        PickUpItem();
    }

    // 손이 비어있을 때 아이템 들기
    void PickUpItem()
    {
        // 부모를 변경 (HoldingTrans로)
        nearItem.transform.SetParent(HoldingTrans);

        // 부모 오브젝트의 로컬 좌표를 기준으로 위치, 회전, 스케일을 초기화
        nearItem.transform.localPosition = Vector3.zero;
        nearItem.transform.localRotation = Quaternion.identity;
        nearItem.transform.localScale = Vector3.one;  // 스케일 초기화 (1, 1, 1)

        holdingstuff = nearItem.tag;
        closeToStuff = false;
        nearItem = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.childCount == 0)
        {
            //Debug.Log(other.transform.position);
            nearItemParent = other.transform;
            return;
        }
        
        Transform child = other.transform.GetChild(0);

        foreach (string tag in tagsToCheck)
        {
            // 자식이 있고 해당 자식의 태그가 tagsToCheck에 있는지 확인
            if (child.CompareTag(tag))
            {
                Debug.Log(tag);
                nearItem = child.gameObject;
                closeToStuff = true;
                return;  // 원하는 아이템을 찾으면 루프 종료
            }
        }
        foreach (string tag in tagsToBreak)
        {
            if (child.CompareTag(tag))
            {
                anim.SetBool("Working", true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.childCount == 0)return;  
       
        Transform child = other.transform.GetChild(0);
        foreach (string tag in tagsToCheck)
        {
            if (child.CompareTag(tag))
            {
                Debug.Log("Exit");
                closeToStuff = false;
                nearItem = null;
                break;
            }
        }
        foreach (string tag in tagsToBreak)
        {
            if (child.CompareTag(tag))
            {
                anim.SetBool("Working", false);
            }
        }
    }
}