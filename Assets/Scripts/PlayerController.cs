using NUnit.Framework.Interfaces;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float moveSpeed,runSpeed,gravityModifier,jumpingPower;
    public CharacterController charCon;
    private Vector3 moveInput;
    public Transform camTrans;
    int jumping = 2;
    public Animator anim;

    public float mouseSensitivity;

    public GameObject bullet;
    public Transform firepoint;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
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

        if(Input.GetKey(KeyCode.LeftShift))
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

        if(charCon.isGrounded)
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
        float movemag = new Vector3(charCon.velocity.x,0,charCon.velocity.z).magnitude;
        anim.SetFloat("MoveSpeed", movemag);

        //player looking rotation(left and right)
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"))*mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y,0f,0f));

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(camTrans.position,camTrans.forward, out hit,50f))
            {
                firepoint.LookAt(hit.point);
            }
            else
            {
                firepoint.LookAt(camTrans.position + (camTrans.forward * 30f));
            }
               
            Instantiate(bullet, firepoint.position, firepoint.rotation);
        }
    }
}
