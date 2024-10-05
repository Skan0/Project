using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform Train;

    private float InitZ;
    private float SelfZ;
    void Start()
    {
        SelfZ = transform.position.z;
        InitZ = Train.position.z;
    }
    void Update()
    {
        Vector3 cameraPos = transform.position;
        cameraPos.z = SelfZ + (Train.position.z - InitZ);
        transform.position = cameraPos;
    }
}
