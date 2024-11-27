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
    public bool isStone = false; //������ false ���� true 

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }
    //               | ��� �ִ� ���� O | ��� �ִ� ���� X
    //  ��ó�� ���� O |    swapItem     |   pickupitem
    //  ��ó�� ���� X |    dropitem     |       x
    public void HandleInteraction()
    {
        if (isNearCreateRail)
        {
            //���� ����ִٸ� ������ ��ҿ��� ���� �� �ְ� �Ǿ��ִµ� �̰� ����� �Ǿ����� �ʴ�.
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
    // createRail���� ����ִ� ĭ�� ������Ʈ�� �ִ� �ڵ�
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
