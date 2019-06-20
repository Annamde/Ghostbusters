using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform lookAt;
    public float maxY;
    public float minY;
    public float speed;

    float distance = 10;
    float yaw = 0.0f;
    float pitch = 0.0f;

    private void Start()
    {
        yaw = transform.localRotation.eulerAngles.y;
        pitch = transform.localRotation.eulerAngles.x;
    }

    private void Update()
    {
        yaw += Input.GetAxis("Mouse X") * speed * Time.deltaTime;
        pitch += Input.GetAxis("Mouse Y") * speed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minY, maxY);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = lookAt.position + rotation * dir;
        transform.LookAt(lookAt.position);
    }
}
