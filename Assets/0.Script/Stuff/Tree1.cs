using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree1 : MonoBehaviour
{
    public float Hp = 100f;
    public GameObject Wood;
    Transform parent;
    
    private void Start()
    {
        parent = transform.parent;
    }
    private void Update()
    {
        if (Hp <= 0)
        {
            GameObject wood = Instantiate(Wood, transform.position, transform.rotation);
            wood.transform.SetParent(parent);
            Player.Instance.movement.animator.SetBool("Working", false);
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            Transform child = other.transform.GetChild(2).GetChild(0).GetChild(0);
            if (child.childCount==0)
                return;

            child = child.GetChild(0);
            if (child.CompareTag("Axe"))
            {
                Player.Instance.movement.animator.SetBool("Working", true);
                Hp -= 20 * Time.deltaTime;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Player.Instance.movement.animator.SetBool("Working", false);
    }
}
