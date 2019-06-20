using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    GameManager gameManager;
    [HideInInspector] public CharacterController characterController;

    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    float speed;
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;
    public float gravity;
    public float catchDistance;

    Vector3 movement = Vector3.zero;

    private void Start()
    {
        gameManager = GameManager.Instance;
        characterController = GetComponent<CharacterController>();
        
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void Update()
    {
        Move();

        if (Input.GetButtonDown("Fire"))
            Shoot();

    }

    public void Move()
    {
        speed = walkSpeed;

        if (Input.GetButton("Run"))
            speed = runSpeed;

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
            }

            if (Input.GetButtonDown("Jump"))
                movement.y = jumpSpeed;
        }

        movement.y -= gravity * Time.deltaTime;

        characterController.Move(movement * speed * Time.deltaTime);
    }

    public void Shoot()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, catchDistance))
        {
            if (hit.collider.tag == "Ghost")
            {
                hit.collider.gameObject.GetComponent<GhostScript>().Die(); //mantener rayo en fantasmas e ir quitando vida
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Star")
        {
            gameManager.starsCounter++;
            gameManager.starsCounterText.text = "" + gameManager.starsCounter;
            Destroy(other.gameObject);
        }

        if (other.tag == "Key")
        {
            gameManager.key.enabled = true;
            gameManager.keys++;
            if (gameManager.keys == 1) gameManager.door1.SetActive(false);
            if (gameManager.keys == 2) gameManager.door2.SetActive(false);
            if (gameManager.keys == 3) gameManager.door3.SetActive(false);

            Destroy(other.gameObject);
        }

        if (other.tag == "Checkpoint")
        {
            gameManager.key.enabled = false;
            startPos = transform.position;
            startRot = transform.localRotation;

            other.gameObject.SetActive(false);
            gameManager.checkpoints++;

            if (gameManager.checkpoints == 1) gameManager.door1.SetActive(true);
            if (gameManager.checkpoints == 2) gameManager.door2.SetActive(true);
            if (gameManager.checkpoints == 3) gameManager.door3.SetActive(true);
        }
    }
}
