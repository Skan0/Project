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
    public Transform[] rockPlace;
    public Transform[] railPlace;
    public GameObject Rail;

    // Update is called once per frame
    void Update()
    {
        if (woodPlace.Length != 0 && rockPlace.Length != 0)
        {
            //rail���� �ڵ�
        }
    }
}
