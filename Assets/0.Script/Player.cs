using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * 플레이어가 손에 아이템을 들 수 있다.
 * wasd와 방향키로 이동
 * space키와 right_ctrl 아이템 들고 놓기
 * Lshift와 Rshift 대쉬 쿨타임 있는
 * 앞에있는 오브젝트와 상호작용 -> raycast로 감지 -> 플레이어가 접근해서 하는 행동이니 플레이어에서 관리
 * 자원 캐기, 물건 내려놓기, 걷기, 달리기 등의 애니매이션
 */

public class Player : MonoBehaviour
{
    public Animator anim;
    public float moveSeed = 5f;
    public float rotationSpeed = 5f;
    public GameObject Axe;
    public GameObject Pick;
    
    private float X, Y;
    private bool holding = false; //손에 물건을 들고 있는가 확인
    private new string anim_name = " "; //애니메이션 이름

    void Start()
    { 
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-Y, 0, X) * moveSeed * Time.deltaTime;

        

        //움직임이 있다면 walk 애니메이션을 켜주고 이동값이 없다면 idle
        anim_name = (move.magnitude > 0f) ? "Walk": "Idle";
        anim.SetTrigger(anim_name);
        transform.Translate(move, Space.World);
        
        //대쉬 키입력
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 dir = new Vector3(-X,0f, -Y).normalized;
            
        }
        //물건 들고 내릴 키입력
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {

        }

        if (X != 0f || Y != 0f)
        {
            // 플레이어가 움직이는 방향
            Vector3 direction = new Vector3(-X, 0f,-Y).normalized;

            // 움직이는 방향을 기준으로 회전 각도를 설정
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

            // 부드럽게 회전 (현재 각도에서 목표 각도로 회전)
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
