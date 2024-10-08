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
    public Transform HoldingTrans;
    public Animator anim;
    
    public float moveSpeed = 5f;        // �̵� �ӵ�
    public float dashSpeed = 150f;      // �뽬 �ӵ�
    public float rotationSpeed = 5f;    // ȸ�� �ӵ�
    public float dashCooldown = 2f;     // �뽬 ��Ÿ��

    private GameObject nearItem;

    private float lastDashTime = -2f;   // ������ �뽬 �ð� �ʱ�ȭ
    private float X, Y;
    private string[] tagsToCheck = { "Axe", "Pick","Wood","Stone"}; // �տ� �� �� �ִ� ���ǵ��� �±� ���
    private string[] tagsToBreak = { "Tree", "Rock" };
    private bool closeToStuff = false;  // ������ collider�� �������ΰ�
    private string holdingstuff =null;    // ����ִ� ������ �̸����� �ൿ����
    private Vector3 lastMoveDir;        // ������ �̵� ����
    
    
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
    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (nearItem != null)//���� �������� �ִ�.
            {
                if (holdingstuff == null)//����ִ� ������ ���°� Ȯ��
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
        // �θ� ���� (HoldingTrans��)
        nearItem.transform.SetParent(HoldingTrans);

        // �θ� ������Ʈ�� ���� ��ǥ�� �������� ��ġ, ȸ��, �������� �ʱ�ȭ
        nearItem.transform.localPosition = Vector3.zero;
        nearItem.transform.localRotation = Quaternion.identity;
        nearItem.transform.localScale = Vector3.one;  // ������ �ʱ�ȭ (1, 1, 1)

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