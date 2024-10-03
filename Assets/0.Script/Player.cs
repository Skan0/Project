using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * �÷��̾ �տ� �������� �� �� �ִ�.
 * wasd�� ����Ű�� �̵�
 * spaceŰ�� right_ctrl ������ ��� ����
 * Lshift�� Rshift �뽬 ��Ÿ�� �ִ�
 * �ڿ� ĳ��, ���� ��������, �ȱ�, �޸��� ���� �ִϸ��̼�
 */

public class Player : MonoBehaviour
{
    public Animator anim;
    public float moveSpeed = 5f;       // �̵� �ӵ�
    public float dashSpeed = 10f;      // �뽬 �ӵ�
    public float rotationSpeed = 5f;   // ȸ�� �ӵ�
    public float dashCooldown = 2f;    // �뽬 ��Ÿ��
    public GameObject Axe;
    public GameObject Pick;

    private float X, Y;
    private bool holding = false;      // �տ� ������ ��� �ִ��� ����
    private Vector3 lastMoveDir;       // ������ �̵� ����
    private float lastDashTime = -2f;  // ������ �뽬 �ð� �ʱ�ȭ

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
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

        // �뽬 �Է� ó�� (��Ÿ�� ����)
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > lastDashTime + dashCooldown)
        {
            Vector3 dashDir = new Vector3(-lastMoveDir.x, 0f, -lastMoveDir.z).normalized;
            transform.Translate(dashDir * dashSpeed * Time.deltaTime, Space.World);

            lastDashTime = Time.time; // �뽬�� �ð� ���
        }

        // ���� ���/������ Ű �Է� ó��
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

        // �÷��̾ ������ �� ȸ�� ó��
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