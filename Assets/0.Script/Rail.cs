using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    public bool holdAble = true;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train"))
        {
            holdAble = false;
        }
    }
}
