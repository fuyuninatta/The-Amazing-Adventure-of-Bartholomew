using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

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

    //public GameObject bullet;
    public Transform firePoint;

    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public int currentGun, maxGunIndex = 1;

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
        
        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
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

        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (charCon.isGrounded)
        {
            jumping = 2;
            moveInput.y = -1f;
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumping > 0)
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
    }
    public void fireShot()
    {
        if (activeGun.currentAmmo <= 0)
        {
            return;
        }

        RaycastHit hit;

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

        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
    }

    public void SwitchGun(int currentGunIndex)
    {
        activeGun.gameObject.SetActive(false);

        //switch gun when the gun is unlocked
        //maxGunIndex will +1 when player enter new level(each level unlock 1 new weapon)
        if (currentGunIndex < maxGunIndex)
        {
            currentGun = currentGunIndex;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("NextLevel"))
        {
            SaveGunData();
            GameManager.instance.LoadNextScene();
        }
    }
    
    public void SaveGunData()//save current ammo and unlock new weapon
    {
        PlayerPrefs.SetInt("maxGunIndex", maxGunIndex + 1);//unlock new gun
        for(int i=0;i<allGuns.Count; i++)//save all ammo
        {
            PlayerPrefs.SetInt("Gun_"+i+"_Ammo", allGuns[i].currentAmmo);
        }
    }
}
