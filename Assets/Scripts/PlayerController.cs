using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float moveSpeed, runSpeed, gravityModifier, jumpingPower;
    public CharacterController charCon;
    private Vector3 moveInput;
    public Transform camTrans;
    int jumping = 2;
    public Animator anim;

    public float mouseSensitivity;

    //fallen damage
    public float minFallSpeed = -15f;
    public float lethalFallSpeed = -60f;
    public float maxFallSpeed;
    public float fallDamageRatio = 1.0f;

    //dash
    public float dashSpeed = 20f;    
    public float dashDuration = 0.2f, dashTimer = 0.0f;

    //public GameObject bullet;
    public Transform firePoint;

    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public int currentGun, maxGunIndex = 1;

    //Suply amount
    public int BowSupply = 10,CrossbowSupply = 10, ManaSupply = 10;

    //ladder
    public float ladderSpeed = 3.0f;
    private bool isClimbing = false;

    //audio source
    public AudioSource audiosource;
    public AudioClip pickupArrowSFX, pickupCrossbowArrowSFX, pickupManaSFX;
    
    //knockback by boss
    private float knockbackTimer;
    private Vector3 knockbackDir;
    public float knockbackForce = 25f;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        for (int i = 0; i < allGuns.Count; i++)
        {
            allGuns[i].PreparePool();

            //get maxGunIndex
            if(PlayerPrefs.HasKey("maxGunIndex"))
            {
                maxGunIndex = PlayerPrefs.GetInt("maxGunIndex");
            }
            

            //get current ammo
            if(PlayerPrefs.HasKey("Gun_"+i+"_Ammo"))//prevent error
            {
                allGuns[i].currentAmmo = PlayerPrefs.GetInt("Gun_" + i + "_Ammo");
            }
        }

        AmmoUpdate();

        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        float yStore = moveInput.y;

        //move base on player facing direction
        Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

        moveInput = vertMove + horiMove;
        moveInput.Normalize();


        if (Input.GetKey(KeyCode.LeftShift))
        {
            //running speed
            moveInput *= runSpeed;
        }
        else
        {
            //walking speed;
            moveInput *= moveSpeed;
        }

        moveInput.y = yStore;

        if (isClimbing)//climbing ladder
        {
            if (Input.GetKeyDown(KeyCode.Space))//player jump when climbing ladder
            {
                isClimbing = false;
                moveInput.y = jumpingPower;
            }
            else//when climbing ladder, player can only move up and down
            {
                moveInput = new Vector3(0, Input.GetAxis("Vertical") * ladderSpeed, 0);
            }
        }
        else//normal movement
        {
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

            if (charCon.isGrounded)
            {
                jumping = 2;
                moveInput.y = -1f;
                moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

                ApplyFallDamage();
                maxFallSpeed = 0f;
            }
            else
            {
                if (charCon.velocity.y < maxFallSpeed)
                {
                    maxFallSpeed = charCon.velocity.y;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P)|| (maxFallSpeed < lethalFallSpeed && !charCon.isGrounded))
        {
            maxFallSpeed = 0f;
            PlayerHeathController.instance.DamagePlayer(PlayerHeathController.instance.currentHealth);
        }

        if (!isClimbing && Input.GetKeyDown(KeyCode.Space) && jumping > 0)
        {
            moveInput.y = jumpingPower;
            jumping--;
        }

        charCon.Move(moveInput * Time.deltaTime);
        float movemag = new Vector3(charCon.velocity.x, 0, charCon.velocity.z).magnitude;
        anim.SetFloat("MoveSpeed", movemag);

        //player looking rotation(left and right)
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));

        //Handle the shooting
        if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
        {
            fireShot();
        }

        if (Input.GetMouseButton(0) && activeGun.canAutoFire && activeGun.fireCounter <= 0)
        {
            fireShot();
        }

        //Switch gun base on what player press (1-3)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchGun(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchGun(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchGun(2);
        }

        //Press E to Heal if player have healing potions
        if (Input.GetKeyDown(KeyCode.E) && PlayerHeathController.instance.healPotion > 0)
        {
            PlayerHeathController.instance.healPotion--;
            UIController.instance.healingPotionsText.text = "Healing Potions: " + PlayerHeathController.instance.healPotion;
            PlayerHeathController.instance.healPlayer();
        }

        //Dash
        if (Input.GetMouseButtonDown(1) && dashTimer <= 0 &&charCon.isGrounded)
        {
            dashTimer = dashDuration;
        }

        if(dashTimer >0)
        {
            charCon.Move(transform.forward * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
        }

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            charCon.Move(knockbackDir * knockbackForce * Time.deltaTime);
            return;
        }
    }
    public void fireShot()
    {
        if (activeGun.currentAmmo <= 0)
        {
            return;
        }

        RaycastHit hit;

        //play shooting sfx
        audiosource.PlayOneShot(activeGun.Shootsfx, 0.25f);
        //shoot
        if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
        {
            firePoint.LookAt(hit.point);
        }
        else
        {
            firePoint.LookAt(camTrans.position + camTrans.forward * 30f);
        }

        activeGun.currentAmmo--;

        activeGun.GetBullet(firePoint.position, firePoint.rotation);

        activeGun.fireCounter = activeGun.fireRate;

        AmmoUpdate();
    }

    public void SwitchGun(int currentGunIndex)
    {
        activeGun.gameObject.SetActive(false);
        //switch gun when the gun is unlocked
        //maxGunIndex will +1 when player enter new level(each level unlock 1 new weapon)
        if (currentGunIndex < maxGunIndex)
        {
            currentGun = currentGunIndex;
            AmmoUpdate();
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        //load next level
        if(other.gameObject.CompareTag("NextLevel"))
        {
            PlayerPrefs.SetInt("maxGunIndex", maxGunIndex + 1);//unlock new gun
            if(maxGunIndex>3)//PREVENT ERROR
            {
                PlayerPrefs.SetInt("maxGunIndex", 3);
            }
            SaveGunData();
            PlayerHeathController.instance.updateHealth();
            GameManager.instance.LoadNextScene();
        }

        //pick up supply
        if (other.gameObject.CompareTag("BowArrow") || other.gameObject.CompareTag("CrossbowArrow") || other.gameObject.CompareTag("Mana"))
        {
            //add ammo base on what item
            if (other.gameObject.CompareTag("BowArrow"))
            {
                allGuns[0].currentAmmo += BowSupply;
                audiosource.PlayOneShot(pickupArrowSFX, 0.25f);
            }
            else if (other.gameObject.CompareTag("CrossbowArrow"))
            {
                allGuns[1].currentAmmo += CrossbowSupply;
                audiosource.PlayOneShot(pickupCrossbowArrowSFX, 0.25f);
            }
            else if (other.gameObject.CompareTag("Mana"))
            {
                allGuns[2].currentAmmo += ManaSupply;
                audiosource.PlayOneShot(pickupManaSFX, 0.25f);
            }

            //update ui
            AmmoUpdate();

            //remove item
            Destroy(other.gameObject);
        }

        //climbing ladder
        if(other.gameObject.CompareTag("Ladder"))
        {
            if (Input.GetKey(KeyCode.W))//only when player press w when touching ladder will start climbing
            {
                isClimbing = true;
            }
        }

        //Bite damage
        if (other.CompareTag("Boss"))
        {
            PlayerHeathController.instance.DamagePlayer(40.0f);
            knockbackDir= (transform.position - other.transform.position).normalized;
            knockbackDir.y = 0.5f;
            knockbackTimer = knockbackForce * 0.02f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
        }
    }

    public void SaveGunData()//save current ammo and unlock new weapon
    {
        for(int i=0;i<allGuns.Count; i++)//save all ammo
        {
            PlayerPrefs.SetInt("Gun_"+i+"_Ammo", allGuns[i].currentAmmo);
        }
    }

    public void ApplyFallDamage()
    {
        if (maxFallSpeed < minFallSpeed)
        {
            //calculaate damage ratio
            float speedDifference = Mathf.Abs(maxFallSpeed) - Mathf.Abs(minFallSpeed);
            float maxDifference = Mathf.Abs(lethalFallSpeed) - Mathf.Abs(minFallSpeed);
            float damagePercent = Mathf.Clamp01(speedDifference / maxDifference);

            //calculate damage
            float damage = damagePercent * PlayerHeathController.instance.maxHealth * fallDamageRatio;

            PlayerHeathController.instance.DamagePlayer(damage);
        }
    }

    public void AmmoUpdate()
    {
        for (int i = 0; i < UIController.instance.weaponBoxes.Length; i++)
        {
            //decide which gun is active, only smaller than maxGunIndex is unlocked
            if(i < maxGunIndex)
            {
                UIController.instance.weaponBoxes[i].SetActive(true);
            }
            else
            {
                UIController.instance.weaponBoxes[i].SetActive(false);
            }

            //printout ammo if the weapon box is active
            if (UIController.instance.weaponBoxes[i] != null)
            {
                UIController.instance.ammoText[i].text = "" + allGuns[i].currentAmmo;
            }

            //change color and scale for active gun
            //current gun will be larger and brighter
            Image WeaponImage = UIController.instance.weaponBoxes[i].GetComponent<Image>();
            if(i == currentGun)
            {
                UIController.instance.weaponBoxes[i].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                WeaponImage.color = Color.white;
            }
            else
            {
                UIController.instance.weaponBoxes[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                WeaponImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            }
        }
    }
}
