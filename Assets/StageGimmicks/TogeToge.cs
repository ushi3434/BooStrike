using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogeToge : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 axis;

    private Vector3 defaultAngle;

    void Start()
    {
        defaultAngle = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        defaultAngle += rotationSpeed * axis * Time.deltaTime;

        transform.eulerAngles = defaultAngle;
    }
}
