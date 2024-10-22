using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * CreateRail: 손에 든 wood와 stone을 이용해 rail을 생성하는 클래스
 * 1. wood와 stone이 모두 있는 상태에서 레일을 생성.
 * 2. 지정된 장소에 레일을 놓고, 기존의 자원들은 제거.
 * 3. 레일을 배치할 때, 주변에 레일이 있다면 그에 맞게 곡선 형태로 전환될 수도 있음.
 * 4. 여러 자원을 받을 수 있게 만들고, 자원이 쌓이는 방식으로 확장 가능.
 */
public class Create_Rail : MonoBehaviour
{
    public Transform[] woodPlace;       // wood가 놓일 장소 배열
    public Transform[] stonePlace;      // stone이 놓일 장소 배열
    public Transform[] railPlace;       // rail이 배치될 장소 배열
    public GameObject Rail;             // 생성할 rail 프리팹

    void Update()
    {
        // woodPlace와 stonePlace의 첫 번째 위치에 자원이 모두 있을 경우에만 레일을 생성합니다.
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

    // 일정 시간 후에 자원을 파괴하고 레일을 생성하는 코루틴
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
