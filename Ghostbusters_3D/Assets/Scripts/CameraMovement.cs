using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    GameObject player;

    public float maxY;
    public float minY;
    public float speed;

    float yaw = 0.0f;
    float pitch = 0.0f;

    private void Start()
    {
        player = transform.parent.gameObject;
        transform.forward = player.transform.forward;
        pitch = transform.rotation.eulerAngles.x;
        yaw = player.transform.eulerAngles.y;
    }

    private void Update()
    {
        CameraRotation();
    }

    void CameraRotation()
    {
        pitch += Input.GetAxis("Camera Y") * speed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minY, maxY);
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);

        yaw += Input.GetAxis("Camera X") * speed * Time.deltaTime;
        player.transform.rotation = Quaternion.Euler(0, yaw, 0);
    }
}
