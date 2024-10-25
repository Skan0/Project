using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform holdingTransform;

    private GameObject heldItem;
    private string holdingItemTag;

    public bool HasItem => heldItem != null;
    public string HeldItemTag => holdingItemTag;

    public void PickUpItem(GameObject item)
    {
        heldItem = item;
        holdingItemTag = item.tag;

        item.transform.SetParent(holdingTransform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
    }

    public void DropItem(Transform dropLocation)
    {
        if (!HasItem || dropLocation == null) return;

        heldItem.transform.SetParent(dropLocation);
        heldItem.transform.position = dropLocation.position;
        heldItem.transform.rotation = Quaternion.identity;
        heldItem.transform.localScale = Vector3.one;

        heldItem = null;
        holdingItemTag = null;
    }

    public void SwapItem(GameObject newItem)
    {
        if (!HasItem) return;

        Transform oldItemParent = newItem.transform.parent;
        Vector3 oldItemPosition = newItem.transform.position;

        GameObject oldItem = heldItem;
        oldItem.transform.SetParent(oldItemParent);
        oldItem.transform.position = oldItemPosition;
        oldItem.transform.rotation = Quaternion.identity;
        oldItem.transform.localScale = Vector3.one;

        PickUpItem(newItem);
    }
}