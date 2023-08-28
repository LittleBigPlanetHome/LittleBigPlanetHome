using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour
{
    public Transform cam;
    void Update()
    {
        transform.LookAt(cam);
    }
}