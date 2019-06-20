using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public Text lifeText, startsCounterText;
    public Image key;

    CharacterController characterController;
    public float speed;

    public bool glasses = false;

    public Image blood;
    public float bloodTime = 1.0f;
    bool bloodActive;
    float bloodCounter = 0.0f;

    float maxLife = 100;
    public float currentLife;

    public GameObject door1, door2, door3;

    float keys = 0;
    float checkpoints = 0;

    Vector3 startPos;
    Quaternion startRot;

    public float jumpSpeed = 8.0F;
    public float gravity = 10.0F;

    Vector3 movement = Vector3.zero;

    int starsCounter;

    private void Start()
    {
        currentLife = maxLife;
        lifeText.text = "Life: " + currentLife;
        starsCounter = 0;
        startsCounterText.text = "" + starsCounter;

        key.enabled = false;

        startPos = transform.position;
        startRot = transform.rotation;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();

        blood.enabled = false;
        bloodActive = false;
    }

    private void Update()
    {
        Move();

        if (bloodActive)
        {
            bloodCounter += Time.deltaTime;
        }

        if (bloodCounter >= bloodTime)
        {
            blood.enabled = false;
            bloodActive = false;
            bloodCounter = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (glasses)
                glasses = false;
            else
                glasses = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Move()
    {

        if (characterController.isGrounded)
        {
            movement = Vector3.zero;
            float xMovement = Input.GetAxisRaw("Horizontal");
            float zMovement = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(xMovement) > 0 || Mathf.Abs(zMovement) > 0)
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0;
                forward.Normalize();
                Vector3 right = Camera.main.transform.right;
                right.y = 0;
                right.Normalize();

                movement = forward * zMovement + right * xMovement;
                transform.localRotation = Quaternion.LookRotation(movement);

            }

            if (Input.GetButton("Jump"))
                movement.y = jumpSpeed;
        }

        else
            movement.y -= gravity * Time.deltaTime;

        characterController.Move(movement.normalized * speed * Time.deltaTime);
    }

    public void GetHurt()
    {
        currentLife -= 15;

        if (currentLife <= 0)
        {
            Die();
        }
        else
        {
            blood.enabled = true;
            bloodActive = true;
        }

        lifeText.text = "Life: " + currentLife;
    }

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 10.0f))
        {
            if (hit.collider.tag == "Ghost")
                hit.collider.gameObject.GetComponent<GhostScript>().Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Star")
        {
            starsCounter++;
            startsCounterText.text = "" + starsCounter;
            Destroy(other.gameObject);
        }

        if (other.tag == "Key")
        {
            key.enabled = true;
            keys++;
            if (keys == 1) door1.SetActive(false);
            if (keys == 2) door2.SetActive(false);
            if (keys == 3) door3.SetActive(false);

            Destroy(other.gameObject);
        }

        if (other.tag == "Checkpoint")
        {
            key.enabled = false;
            startPos = transform.position;
            startRot = transform.rotation;

            other.gameObject.SetActive(false);
            checkpoints++;

            if (checkpoints == 1) door1.SetActive(true);
            if (checkpoints == 2) door2.SetActive(true);
            if (checkpoints == 3) door3.SetActive(true);
        }
    }

    void Die()
    {
        characterController.enabled = false;
        transform.position = startPos;
        transform.rotation = startRot;
        currentLife = maxLife;
        characterController.enabled = true;
    }

}
