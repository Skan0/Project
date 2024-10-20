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
    void TryPlaceItem(Transform[] placeArray, string itemType)
    {
        for (int i = 0; i < placeArray.Length; i++)
        {
            if (placeArray[i].childCount == 0)
            {
                Debug.Log("�����������̴�.");
                Player.instance.PutDownItem(placeArray[i]);
                Debug.Log("�������Ҵ�.");
                return;
            }
            if(i == placeArray.Length-1)
                Debug.Log(itemType + "�� ��ġ�� ������ �����ϴ�.");
        }
        
    }

    void LoadStuff()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl)))
        {
            if (isPlayerHasWood)
            {
                TryPlaceItem(woodPlace, "����");
            }
            else if (isPlayerHasStone)
            {
                TryPlaceItem(stonePlace, "��");  
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
