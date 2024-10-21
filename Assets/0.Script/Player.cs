using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Transform HoldingTrans;      // �������� ��� ���� ��ġ
    public Animator anim;

    public float moveSpeed = 5f;        // �̵� �ӵ�
    public float dashSpeed = 800f;      // �뽬 �ӵ�
    public float rotationSpeed = 5f;    // ȸ�� �ӵ�
    public float dashCooldown = 2f;     // �뽬 ��Ÿ��
    public string holdingstuff = null;  // ����ִ� ������ �̸�

    private Transform nearItemParent;   // �ٴ��� collider
    private GameObject nearItem;        // ��ó�� ������

    private float lastDashTime = -2f;   // ������ �뽬 �ð�
    private float X, Y;
    private string[] tagsToCheck = { "Axe", "Pick", "Wood", "Stone" }; // �տ� �� �� �ִ� ���� �±� ���
    private string[] tagsToBreak = { "Tree", "Rock" };  // �μ� �� �ִ� ���� �±� ���
    private Vector3 lastMoveDir;        // ������ �̵� ����

    //CreateRail���� ������ 
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

    // �÷��̾� ������ 
    private void Movement()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        // �̵� ���� ���
        Vector3 moveDir = new Vector3(-Y, 0f, X).normalized;

        if (moveDir.magnitude > 0f)
        {
            anim.SetBool("isWalking", true);   // �ȱ� ����
            lastMoveDir = moveDir;             // ������ �̵� ���� ����
        }
        else
        {
            anim.SetBool("isWalking", false);  // �ȱ� ���� ���� (Idle)
        }

        // �̵� ó��
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        // �÷��̾ ������ �� ȸ�� ó��
        if (X != 0f || Y != 0f)
        {
            Vector3 dir = new Vector3(-X, 0f, -Y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = targetRotation;
        }
    }

    // ������ ������ �̵�������
    public void Dash()
    {
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > lastDashTime + dashCooldown)
        {
            Vector3 dashDir = new Vector3(lastMoveDir.x, 0f, lastMoveDir.z).normalized;
            transform.Translate(dashDir * dashSpeed * Time.deltaTime, Space.World);

            lastDashTime = Time.time; // ��Ÿ���� ���� �뽬 �ð� ���
        }
    }

    // �����۰� ���õ� ��ȣ�ۿ� ���
    public void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("Pressed command");
            if (nearItem != null)  // ����� �������� �ִ� ���
            {
                Debug.Log("nearItem != null");
                if (holdingstuff == null)  // �տ� ��� �ִ� ������ ���� ���
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
            else if (holdingstuff != null)  // ��� �ִ� ������ ���� �� ��������
            {
                Debug.Log("holdingstuff: " + holdingstuff);

                if (nearItemParent != null)
                {
                    PutDownItem(nearItemParent);
                    nearItemParent = null;
                }
                else
                {
                    Debug.LogWarning("nearItemParent�� null�Դϴ�. �������� �������� �� �����ϴ�.");
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
                Debug.Log("��ġ�� ������ �����ϴ�.");
            }
        }
    }
    // �ٴ��� ������� �� ������ ��������
    public void PutDownItem(Transform nearItemParent)
    {
        if (nearItemParent == null)
        {
            Debug.LogError("nearItemParent�� null�Դϴ�.");
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
            Debug.LogError("HoldingTrans�� �������� �����ϴ�.");
        }
    }

    // �տ� �ִ� �����۰� �ٴڿ� �ִ� �������� �ٲ���
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

    // ���� ������� �� ������ ���
    public void PickUpItem()
    {
        nearItem.transform.SetParent(HoldingTrans);
        nearItem.transform.localPosition = Vector3.zero;
        nearItem.transform.localRotation = Quaternion.identity;
        nearItem.transform.localScale = Vector3.one;  // ������ �ʱ�ȭ

        holdingstuff = nearItem.tag;
        nearItem = null;
    }

    // �浹 ���� �� ȣ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.childCount == 0)
        {
            nearItemParent = other.transform;
            //Debug.Log("nearItemParent ����: " + nearItemParent.name);
            return;
        }

        Transform child = other.transform.GetChild(0);

        foreach (string tag in tagsToCheck)
        {
            if (child.CompareTag(tag))
            {
                nearItem = child.gameObject;
                Debug.Log("��ó ������ �߰�: " + nearItem.name);
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
    // �浹 ���� �� ȣ��
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
