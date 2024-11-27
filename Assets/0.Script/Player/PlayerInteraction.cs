using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private string[] pickableTags = { "Axe", "Pick", "Wood", "Stone" };
    [SerializeField] private string[] railTags = { "S_Rail", "R_Rail", "R_Rail" };
    [SerializeField] public Transform[] woodPlaces;
    [SerializeField] public Transform[] stonePlaces;

    private GameObject nearbyItem;
    private Transform nearbyItemParent;
    private PlayerInventory inventory;

    private bool isNearCreateRail;
    public bool isStone = false; //나무는 false 돌은 true 

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }
    //               | 들고 있는 물건 O | 들고 있는 물건 X
    //  근처에 물건 O |    swapItem     |   pickupitem
    //  근처에 물건 X |    dropitem     |       x
    public void HandleInteraction()
    {
        if (isNearCreateRail)
        {
            //돌을 들고있다면 돌놓는 장소에만 놓을 수 있게 되어있는데 이게 제대로 되어있지 않다.
            if (isStone)
            {
                FindEmptyPlace(stonePlaces);
                isStone = false;
            }
            else
            {
                FindEmptyPlace(woodPlaces);
                isStone= true;
            }
        }
        if (nearbyItem != null)
        {
            if (!inventory.HasItem)
            {
                inventory.PickUpItem(nearbyItem);
                if (nearbyItem.tag == "Wood")
                    isStone = false;
                else if (nearbyItem.tag == "Stone")
                    isStone = true;
                nearbyItem = null;
            }
            else
            {
                inventory.SwapItem(nearbyItem);
            }
        }
        else if (inventory.HasItem)
        {

        }
        else if (inventory.HasItem && nearbyItemParent != null)
        {
            inventory.DropItem(nearbyItemParent);
        }
    }
    // createRail에서 비어있는 칸에 오브젝트를 넣는 코드
    private void FindEmptyPlace(Transform[] places)
    {
        foreach (Transform t in places)
        {
            if (t != null && t.childCount == 0)
            {
                inventory.DropItem(t);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.childCount == 0)
        {
            nearbyItemParent = other.transform;
            return;
        }

        Transform child = other.transform.GetChild(0);

        if (IsPickableItem(child.tag))
        {
            nearbyItem = child.gameObject;
        }

        if (other.CompareTag("CreateRail"))
        {
            isNearCreateRail = true;
        }

        if (IsRailItem(child.tag) && other.GetComponent<Rail>()?.holdAble == true)
        {
            nearbyItem = child.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CreateRail"))
        {
            isNearCreateRail = false;
        }

        if (other.transform.childCount > 0)
        {
            Transform child = other.transform.GetChild(0);

            if (IsPickableItem(child.tag))
            {
                nearbyItem = null;
            }
        }
    }

    private bool IsPickableItem(string tag) => System.Array.Exists(pickableTags, t => t == tag);
    private bool IsRailItem(string tag) => System.Array.Exists(railTags, t => t == tag);
}
