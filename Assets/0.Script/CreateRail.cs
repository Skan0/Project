using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* wood를 손에 들고 collider안에서 space나 rightcontrol을 누르면 
 * CreateRail의 지정된 장소에 이동시키고 scale값으로 약간 줄였다가 CreateRail 뒤쪽에 
 * 제작 완성된 rail을 initiate 
 * rail을 바닥에 놓으면 근처에 rail이라는 tag를 가진 rail -> 이건 rail 스크립트를 만들어서 하자
 * 새로운 rail을 놓을 때 근처에 배치되어 있는 rail 근처에 놓으면 기존의 레일이 curved로 바뀔 수도 있어야 하는데 이건 어케 하지 ㅠㅡㅠ 
 * 이미 나무나 돌이 있다면 리스트형태로 여러개의 자원을 받을 수 있게 만들고 위에 점차 쌓이게 만들자
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
                Debug.Log("내려놓기전이다.");
                Player.instance.PutDownItem(placeArray[i]);
                Debug.Log("내려놓았다.");
                return;
            }
            if(i == placeArray.Length-1)
                Debug.Log(itemType + "를 배치할 공간이 없습니다.");
        }
        
    }

    void LoadStuff()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightControl)))
        {
            if (isPlayerHasWood)
            {
                TryPlaceItem(woodPlace, "나무");
            }
            else if (isPlayerHasStone)
            {
                TryPlaceItem(stonePlace, "돌");  
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
