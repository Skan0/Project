using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*
 * �÷��̾ �տ� �������� �� �� �ִ�.
 * wasd�� ����Ű�� �̵�
 * spaceŰ�� right_ctrl ������ ��� ����
 * Lshift�� Rshift �뽬 ��Ÿ�� �ִ�
 * �ڿ� ĳ��, ���� ��������, �ȱ�, �޸��� ���� �ִϸ��̼�
 * 
 * ���迡 ������ �־���. ���� ��ü�� �ݶ��̴��� �浹�� üũ�� �� �ƴ϶� Ÿ�ϸ��� box�� �浹 üũ�� �ϰ� key�� ������ �� �˻��ؼ� �ڽ� ������Ʈ
 * �� ���ƾ������
 */

public class Player : MonoBehaviour
{
    public static Player instance;

    public Transform HoldingTrans;      //�������� ��� ���� ��ġ
    public Animator anim;               
    
    public float moveSpeed = 5f;        // �̵� �ӵ�
    public float dashSpeed = 800f;      // �뽬 �ӵ�
    public float rotationSpeed = 5f;    // ȸ�� �ӵ�
    public float dashCooldown = 2f;     // �뽬 ��Ÿ��

    private Transform nearItemParent;   //Ground�Ʒ� collider
    private GameObject nearItem;        //��ó�� �������� ��� ��Ƶ� ����

    private float lastDashTime = -2f;                   // ������ �뽬 �ð� �ʱ�ȭ
    private float X, Y;
    private string[] tagsToCheck = { "Axe", "Pick","Wood","Stone"}; // �տ� �� �� �ִ� ���ǵ��� �±� ���
    private string[] tagsToBreak = { "Tree", "Rock" };  //�̰� ������ ���� �μ����鼭 �ִϸ��̼��� ������ �Ұ� �����ϱ� �ٸ����� ������.
    private bool closeToStuff = false;                  // ������ collider�� �������ΰ�
    private string holdingstuff =null;                  // ����ִ� ������ �̸����� �ൿ����
    private Vector3 lastMoveDir;                        // ������ �̵� ����

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
    
    // �÷��̾� ������ 
    void Movement()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        // �̵� ���� ���
        Vector3 moveDir = new Vector3(-Y, 0f, X).normalized;

        // �̵��� ������ 'isWalking' �ִϸ��̼� ���·� ��ȯ, �̵��� ������ 'Idle'
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

    // ������ �ణ ������ �̵�������
    void Dash()
    {
        // �뽬 �Է� ó�� (��Ÿ�� ����)
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > lastDashTime + dashCooldown)
        {
            //Debug.Log("Dash");
            Vector3 dashDir = new Vector3(lastMoveDir.x, 0f, lastMoveDir.z).normalized;
            transform.Translate(dashDir * dashSpeed * Time.deltaTime, Space.World);

            lastDashTime = Time.time; // �뽬�� �ð� ���
        }
    }

    // �����۰� ���õ� ��ȣ�ۿ��� ����ϴ� �Լ�
    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("Pressed command");
            if (nearItem != null)//���� �������� �ִ�.
            {
                Debug.Log("nearItem != null");
                if (holdingstuff == null)//����ִ� ������ ���°� Ȯ��
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
                //���� �浹���� �ݶ��̴��� �ڽĿ�����Ʈ�� ����� ��
                PutDownItem();
            }
        }
    }
    
    // �ٴ��� ������� �� �θ� �Լ�
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

    // �տ� �ִ� �����۰� �ٴڿ� �ִ� �������� �ٲ��ش�.
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

    // ���� ������� �� ������ ���
    void PickUpItem()
    {
        // �θ� ���� (HoldingTrans��)
        nearItem.transform.SetParent(HoldingTrans);

        // �θ� ������Ʈ�� ���� ��ǥ�� �������� ��ġ, ȸ��, �������� �ʱ�ȭ
        nearItem.transform.localPosition = Vector3.zero;
        nearItem.transform.localRotation = Quaternion.identity;
        nearItem.transform.localScale = Vector3.one;  // ������ �ʱ�ȭ (1, 1, 1)

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
            // �ڽ��� �ְ� �ش� �ڽ��� �±װ� tagsToCheck�� �ִ��� Ȯ��
            if (child.CompareTag(tag))
            {
                Debug.Log(tag);
                nearItem = child.gameObject;
                closeToStuff = true;
                return;  // ���ϴ� �������� ã���� ���� ����
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