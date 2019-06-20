using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IGhostMaterial
{
    void ChangeMaterialToGlasses();
    void ChangeMaterialToNonGlasses();
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    PlayerScript playerScript;
    GameObject player;

    #region Life
    [Header("Life Settings")]
    public float maxLife;
    float currentLife;
    public Text lifeText;
    #endregion

    #region Power
    [Header("Power Settings")]
    public float maxPower;
    float currentPower;
    public float powerDecrease;
    public float powerIncrease;
    public Image powerBar;
    #endregion

    #region Items
    [Header("Item Settings")]
    public Text starsCounterText;
    [HideInInspector] public int starsCounter;
    [HideInInspector] public float keys = 0;
    public Image key;
    #endregion

    #region Blood
    [Header("Blood Settings")]
    public Image blood;
    public float bloodTime;
    bool bloodActive;
    float bloodCounter = 0.0f;
    #endregion

    public bool glasses = false;

    List<IGhostMaterial> ghostMaterials = new List<IGhostMaterial>();

    [HideInInspector] public float checkpoints = 0;

    public GameObject door1, door2, door3;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        player = playerScript.gameObject;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentLife = maxLife;
        currentPower = maxPower;
        starsCounter = 0;

        lifeText.text = "Life: " + currentLife;
        starsCounterText.text = "" + starsCounter;

        key.enabled = false;
        blood.enabled = false;
        bloodActive = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;

        if (bloodActive)
            UpdateBloodScreen();

        if (Input.GetButtonDown("Glasses"))
        {
            glasses = !glasses;

            if(glasses)
                foreach(IGhostMaterial ghost in ghostMaterials)
                {
                    ghost.ChangeMaterialToGlasses();
                }
            else
                foreach (IGhostMaterial ghost in ghostMaterials)
                {
                    ghost.ChangeMaterialToNonGlasses();
                }

        }

        if (glasses)
            DecreasePower();
        else if (currentPower < 100)
            IncreasePower();
    }

    public void GetHurt(float damage)
    {
        currentLife -= damage;

        if (currentLife <= 0)
            Die();

        else
        {
            blood.enabled = true;
            bloodActive = true;
        }

        lifeText.text = "Life: " + currentLife;
    }

    void UpdateBloodScreen()
    {
        bloodCounter += Time.deltaTime;

        if (bloodCounter >= bloodTime)
        {
            blood.enabled = false;
            bloodActive = false;
            bloodCounter = 0.0f;
        }
    }

    void IncreasePower()
    {
        currentPower += powerIncrease * Time.deltaTime;
        Mathf.Clamp(currentPower, 0, 100);
        powerBar.fillAmount = currentPower / maxPower;
    }

    void DecreasePower()
    {
        if (currentPower <= 0.1)
        {
            glasses = !glasses;
            foreach (IGhostMaterial ghost in ghostMaterials)
            {
                ghost.ChangeMaterialToNonGlasses();
            }
        }

        currentPower -= powerDecrease * Time.deltaTime;
        Mathf.Clamp(currentPower, 0, 100);
        powerBar.fillAmount = currentPower / maxPower;
    }

    void Die()
    {
        playerScript.characterController.enabled = false;
        player.transform.position = playerScript.startPos;
        player.transform.localRotation = playerScript.startRot;
        currentLife = maxLife;
        playerScript.characterController.enabled = true;
    }

    public void AddGhostMaterial(IGhostMaterial ghostMaterial)
    {
        ghostMaterials.Add(ghostMaterial);
    }
}
