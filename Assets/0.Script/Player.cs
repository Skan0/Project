using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Transform HoldingTrans;      // 아이템을 들고 있을 위치
    public Animator anim;

    public float moveSpeed = 5f;        // 이동 속도
    public float dashSpeed = 800f;      // 대쉬 속도
    public float rotationSpeed = 5f;    // 회전 속도
    public float dashCooldown = 2f;     // 대쉬 쿨타임
    public string holdingstuff = null;  // 들고있는 물건의 이름

    private Transform nearItemParent;   // 바닥의 collider
    private GameObject nearItem;        // 근처의 아이템

    private float lastDashTime = -2f;   // 마지막 대쉬 시간
    private float X, Y;
    private string[] tagsToCheck = { "Axe", "Pick", "Wood", "Stone" }; // 손에 들 수 있는 물건 태그 목록
    private string[] tagsToBreak = { "Tree", "Rock" };  // 부술 수 있는 물건 태그 목록
    private Vector3 lastMoveDir;        // 마지막 이동 방향

    //CreateRail에서 쓰던거 
    public Transform[] woodPlace;
    public Transform[] stonePlace;
    public Transform[] railPlace;
    public GameObject Rail;

    private bool isPlayerHasWood = false;
    private bool isPlayerHasStone = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        holdingstuff = null;
    }

    void Update()
    {
        Movement();
        Dash();
        Interaction();
    }

    // 플레이어 움직임 
    private void Movement()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        // 이동 방향 계산
        Vector3 moveDir = new Vector3(-Y, 0f, X).normalized;

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

    // 앞으로 빠르게 이동시켜줌
    public void Dash()
    {
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > lastDashTime + dashCooldown)
        {
            Vector3 dashDir = new Vector3(lastMoveDir.x, 0f, lastMoveDir.z).normalized;
            transform.Translate(dashDir * dashSpeed * Time.deltaTime, Space.World);

            lastDashTime = Time.time; // 쿨타임을 위한 대쉬 시간 기록
        }
    }

    // 아이템과 관련된 상호작용 담당
    public void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("Pressed command");
            if (nearItem != null)  // 가까운 아이템이 있는 경우
            {
                Debug.Log("nearItem != null");
                if (holdingstuff == null)  // 손에 들고 있는 물건이 없는 경우
                {
                    PickUpItem();
                }
                else
                {
                    SwapItem();
                }
            }
            else if(holdingstuff != null && (isPlayerHasStone || isPlayerHasWood))
            {
                if (isPlayerHasWood)
                {
                    PlaceItemToCreateRail(woodPlace);
                }
                else if (isPlayerHasStone) 
                {
                    PlaceItemToCreateRail(stonePlace);
                }
            }
            else if (holdingstuff != null)  // 들고 있는 물건이 있을 때 내려놓기
            {
                Debug.Log("holdingstuff: " + holdingstuff);

                if (nearItemParent != null)
                {
                    PutDownItem(nearItemParent);
                    nearItemParent = null;
                }
                else
                {
                    Debug.LogWarning("nearItemParent가 null입니다. 아이템을 내려놓을 수 없습니다.");
                }
            }
        }
    }
    void PlaceItemToCreateRail(Transform[] placeArry)
    {
        for (int i = 0; i < placeArry.Length; i++)
        {
            if (placeArry[i].childCount == 0)
            {
                PutDownItem(placeArry[i]);
                return;
            }
            if(i == placeArry.Length - 1)
            {
                Debug.Log("배치할 공간이 없습니다.");
            }
        }
    }
    // 바닥이 비어있을 때 아이템 내려놓기
    public void PutDownItem(Transform nearItemParent)
    {
        if (nearItemParent == null)
        {
            Debug.LogError("nearItemParent가 null입니다.");
            return;
        }

        if (HoldingTrans.childCount > 0)
        {
            GameObject temp = HoldingTrans.GetChild(0).gameObject;
            temp.transform.SetParent(nearItemParent);
            temp.transform.position = nearItemParent.position;
            temp.transform.rotation = Quaternion.identity;
            temp.transform.localScale = Vector3.one;
            holdingstuff = null;
        }
        else
        {
            Debug.LogError("HoldingTrans에 아이템이 없습니다.");
        }
    }

    // 손에 있는 아이템과 바닥에 있는 아이템을 바꿔줌
    public void SwapItem()
    {
        if (HoldingTrans.childCount > 0)
        {
            GameObject temp = HoldingTrans.GetChild(0).gameObject;
            Transform parent = nearItem.transform.parent;
            temp.transform.SetParent(parent);
            temp.transform.position = nearItem.transform.position;
            temp.transform.rotation = Quaternion.identity;
            temp.transform.localScale = Vector3.one;
            PickUpItem();
        }
    }

    // 손이 비어있을 때 아이템 들기
    public void PickUpItem()
    {
        nearItem.transform.SetParent(HoldingTrans);
        nearItem.transform.localPosition = Vector3.zero;
        nearItem.transform.localRotation = Quaternion.identity;
        nearItem.transform.localScale = Vector3.one;  // 스케일 초기화

        holdingstuff = nearItem.tag;
        nearItem = null;
    }

    // 충돌 시작 시 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.childCount == 0)
        {
            nearItemParent = other.transform;
            //Debug.Log("nearItemParent 설정: " + nearItemParent.name);
            return;
        }

        Transform child = other.transform.GetChild(0);

        foreach (string tag in tagsToCheck)
        {
            if (child.CompareTag(tag))
            {
                nearItem = child.gameObject;
                Debug.Log("근처 아이템 발견: " + nearItem.name);
                return;
            }
        }
        if (other.CompareTag("CreateRail")) 
        { 
            if(holdingstuff == "Wood")
            {
                isPlayerHasWood = true;
            }   
            else if(holdingstuff == "Stone")
            {
                isPlayerHasStone = true;
            }
        }
    }
    // 충돌 종료 시 호출
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CreateRail"))
        {
            isPlayerHasStone=false;
            isPlayerHasWood=false;
        }
        if (other.transform.childCount == 0) return;

        Transform child = other.transform.GetChild(0);
        foreach (string tag in tagsToCheck)
        {
            if (child.CompareTag(tag))
            {
                Debug.Log("Exit: " + child.name);
                nearItem = null;
                break;
            }
        }
    }
}
