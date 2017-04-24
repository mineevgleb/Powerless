using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
    void Update ()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        transform.LookAt(cameraPos);
        Vector3 rotation = transform.localEulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        transform.localEulerAngles = rotation;
    }
}
