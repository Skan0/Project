using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * �÷��̾ �տ� �������� �� �� �ִ�.
 * wasd�� ����Ű�� �̵�
 * spaceŰ�� right_ctrl ������ ��� ����
 * Lshift�� Rshift �뽬 ��Ÿ�� �ִ�
 * �տ��ִ� ������Ʈ�� ��ȣ�ۿ� -> raycast�� ���� -> �÷��̾ �����ؼ� �ϴ� �ൿ�̴� �÷��̾�� ����
 * �ڿ� ĳ��, ���� ��������, �ȱ�, �޸��� ���� �ִϸ��̼�
 */

public class Player : MonoBehaviour
{
    public Animator anim;
    public float moveSeed = 5f;
    public float rotationSpeed = 5f;
    public GameObject Axe;
    public GameObject Pick;
    
    private float X, Y;
    private bool holding = false; //�տ� ������ ��� �ִ°� Ȯ��
    private new string anim_name = " "; //�ִϸ��̼� �̸�

    void Start()
    { 
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-Y, 0, X) * moveSeed * Time.deltaTime;

        

        //�������� �ִٸ� walk �ִϸ��̼��� ���ְ� �̵����� ���ٸ� idle
        anim_name = (move.magnitude > 0f) ? "Walk": "Idle";
        anim.SetTrigger(anim_name);
        transform.Translate(move, Space.World);
        
        //�뽬 Ű�Է�
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 dir = new Vector3(-X,0f, -Y).normalized;
            
        }
        //���� ��� ���� Ű�Է�
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {

        }

        if (X != 0f || Y != 0f)
        {
            // �÷��̾ �����̴� ����
            Vector3 direction = new Vector3(-X, 0f,-Y).normalized;

            // �����̴� ������ �������� ȸ�� ������ ����
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

            // �ε巴�� ȸ�� (���� �������� ��ǥ ������ ȸ��)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed );
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe"))
        {
            other.transform.SetParent(transform, true);
            holding = true;

        }
        if (other.CompareTag("Axe"))
        {
            other.transform.SetParent(transform, true);
            holding = true;

        }

    }
}
