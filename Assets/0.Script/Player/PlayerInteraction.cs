using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private string[] pickableTags = { "Axe", "Pick", "Wood", "Stone" };
    [SerializeField] private string[] railTags = { "S_Rail", "R_Rail", "R_Rail" };

    private GameObject nearbyItem;
    private Transform nearbyItemParent;
    private PlayerInventory inventory;

    private bool isNearCreateRail;
    private bool canHoldRail;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }
    //               | ��� �ִ� ���� O | ��� �ִ� ���� X
    //  ��ó�� ���� O |    swapItem     |   pickupitem
    //  ��ó�� ���� X |    dropitem     |       x
    public void HandleInteraction()
    {
        if (nearbyItem != null)
        {
            if (!inventory.HasItem)
            {
                if (canHoldRail) 
                { 
                
                }
                inventory.PickUpItem(nearbyItem);
                nearbyItem = null;
            }
            else
            {
                inventory.SwapItem(nearbyItem);
            }
        }
        else if (inventory.HasItem && nearbyItemParent != null)
        {
            inventory.DropItem(nearbyItemParent);
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
            canHoldRail = true;
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

            if (IsRailItem(child.tag))
            {
                canHoldRail = false;
            }
        }
    }

    private bool IsPickableItem(string tag) => System.Array.Exists(pickableTags, t => t == tag);
    private bool IsRailItem(string tag) => System.Array.Exists(railTags, t => t == tag);
    //rail�� ������ ������������ �����ϰ� bool ���·� �÷��̾ rail�� ����ֳ� ������ ��Ÿ�����Ѵ�.
    //
}