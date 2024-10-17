using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* wood�� �տ� ��� collider�ȿ��� space�� rightcontrol�� ������ 
 * CreateRail�� ������ ��ҿ� �̵���Ű�� scale������ �ణ �ٿ��ٰ� CreateRail ���ʿ� 
 * ���� �ϼ��� rail�� initiate 
 * rail�� �ٴڿ� ������ ��ó�� rail�̶�� tag�� ���� rail -> �̰� rail ��ũ��Ʈ�� ���� ����
 * ���ο� rail�� ���� �� ��ó�� ��ġ�Ǿ� �ִ� rail ��ó�� ������ ������ ������ curved�� �ٲ� ���� �־�� �ϴµ� �̰� ���� ���� �ФѤ� 
 * �̹� ������ ���� �ִٸ� ����Ʈ���·� �������� �ڿ��� ���� �� �ְ� ����� ���� ���� ���̰� ������
 * 
 */
public class Create_Rail : MonoBehaviour
{
    public Transform[] woodPlace;
    public Transform[] stonePlace;
    public Transform[] railPlace;
    public GameObject Rail;

    private bool isPlayerHasWood = false;
    private bool isPlayerHasStone = false;
    // Update is called once per frame
    void Update()
    {
        if (woodPlace.Length != 0 && stonePlace.Length != 0)
        {
            createRail();
        }
        LoadStuff();
    }


    void createRail() { 


    }


    void LoadStuff()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (isPlayerHasWood)
            {
                for (int i = 0; i < woodPlace.Length; i++)
                {
                    if (woodPlace[i].GetChild(0) == null)
                    {
                        Player.instance.PutDownItem(woodPlace[i]);
                        break;
                    }
                    if (i == woodPlace.Length - 1)
                    {
                        Debug.Log("������ ��ġ�� ������ �����ϴ�.");
                    }
                }
            }
            else if (isPlayerHasStone)
            {
                for (int i = 0; i < stonePlace.Length; i++)
                {
                    if (stonePlace[i].GetChild(0) == null)
                    {
                        Player.instance.PutDownItem(stonePlace[i]);
                        break;
                    }
                    if (i == stonePlace.Length - 1)
                    {
                        Debug.Log("���� ��ġ�� ������ �����ϴ�.");
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.instance.holdingstuff =="Wood")
            {
                isPlayerHasWood = true;                
            }
            else if(Player.instance.holdingstuff == "Stone")
            {
                isPlayerHasStone = true; 
            }
        }
    }
}
