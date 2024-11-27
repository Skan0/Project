using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] public GameObject WoodBox;
    public void PlaceWoodBox()
    {
        GameObject woodbox = Instantiate(WoodBox, transform.position, Quaternion.identity);
        woodbox.transform.SetParent(transform, true);
    }
}
