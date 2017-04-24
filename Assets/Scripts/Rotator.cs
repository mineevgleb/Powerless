using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float RotationSpeedScaler = 1.0f;
    void Update ()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += Time.deltaTime * RotationSpeedScaler;
        transform.eulerAngles = rotation;
    }
}
