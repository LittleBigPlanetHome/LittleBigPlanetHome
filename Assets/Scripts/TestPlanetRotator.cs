using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlanetRotator : MonoBehaviour
{
    public Vector3 front;

    void Start()
    {
        front = transform.forward;
    }

    void Update()
    {
        transform.eulerAngles = Vector3.RotateTowards(transform.position + front, Camera.main.transform.position, Time.deltaTime, 0f);
    }
}
