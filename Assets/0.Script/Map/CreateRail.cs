using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * CreateRail: �տ� �� wood�� stone�� �̿��� rail�� �����ϴ� Ŭ����
 * 1. wood�� stone�� ��� �ִ� ���¿��� ������ ����.
 * 2. ������ ��ҿ� ������ ����, ������ �ڿ����� ����.
 * 3. ������ ��ġ�� ��, �ֺ��� ������ �ִٸ� �׿� �°� � ���·� ��ȯ�� ���� ����.
 * 4. ���� �ڿ��� ���� �� �ְ� �����, �ڿ��� ���̴� ������� Ȯ�� ����.
 */
public class Create_Rail : MonoBehaviour
{
    public Transform[] woodPlace;       // wood�� ���� ��� �迭
    public Transform[] stonePlace;      // stone�� ���� ��� �迭
    public Transform[] railPlace;       // rail�� ��ġ�� ��� �迭
    public GameObject Rail;             // ������ rail ������

    void Update()
    {
        // woodPlace�� stonePlace�� ù ��° ��ġ�� �ڿ��� ��� ���� ��쿡�� ������ �����մϴ�.
        if (woodPlace[0].childCount>0 && stonePlace[0].childCount > 0)
        {
            createRail();
        }
    }

    void createRail()
    {
        int i, j;
        for ( i = woodPlace.Length - 1; i >= 0; i--)
        {
            if (woodPlace[i].childCount > 0)
            {
                break;
            }
        }
        for ( j = stonePlace.Length - 1; j >= 0; j--)
        {
            if (stonePlace[j].childCount > 0)
            {
                break;
            }
        }
        StartCoroutine(DestroyAfterDelay(woodPlace[i].GetChild(0).gameObject,stonePlace[i].GetChild(0).gameObject, 2f));        
    }

    // ���� �ð� �Ŀ� �ڿ��� �ı��ϰ� ������ �����ϴ� �ڷ�ƾ
    IEnumerator DestroyAfterDelay(GameObject obj1, GameObject obj2, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj1);
        Destroy(obj2);
        for (int i = 0; i < railPlace.Length; i++)
        {
            if (railPlace[i].childCount == 0)
            {
                Instantiate(Rail, railPlace[i].transform);
                break;
            }
        }
    }
}
