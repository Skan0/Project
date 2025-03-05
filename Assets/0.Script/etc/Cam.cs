using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    //기차의 이동에 따라 같이 움직이는 카메라
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
