using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MiniBossController : MonoBehaviour, IDamagable
{
    public int currentHealth, MaxHealth = 10;

    public AudioClip SummonSfx, GetHitSfx, DeathSfx;

    public NavMeshAgent agent;
    public Animator anim;
    public GameObject WeaponGate;

    private bool action = false;//true:summon, false:free roam
    public float actionTimer = 0f, actionDuration = 5f, freeroamRange = 10f;

    //UI
    private HealthBar healthBar;

    private Collider hitbox;

    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MaxHealth;
        healthBar = GetComponent<HealthBar>();
        actionTimer = actionDuration;
        hitbox = GetComponentInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        Vector3 playerPos = PlayerController.instance.transform.position;

        if (agent.remainingDistance < 0.25f)
        {
            anim.SetBool("isMoving", false);
        }
        else
        {
            anim.SetBool("isMoving", true);
        }

        if (actionTimer > 0)
        {
            actionTimer -= Time.deltaTime;
            if (!action)
            {
                agent.isStopped = false;
                if (!agent.hasPath || agent.remainingDistance < 0.5f)
                {
                    Vector2 randPos = Random.insideUnitCircle * freeroamRange;//random position for freeroam
                    agent.destination = playerPos + new Vector3(randPos.x, 0f, randPos.y);
                }
            }
        }
        else
        {
            agent.ResetPath();

            RandomAction();

                if (action)//summon enemies
                {
                    Debug.Log("summon");
                    anim.SetTrigger("Summon");
                    SummonEnemy.instance.spawnEnemy();
                }
             actionTimer = actionDuration;
        }
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            currentHealth -= damage;
            PlayerController.instance.audiosource.PlayOneShot(GetHitSfx, 0.2f);

            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.2f);
                Died();
                isDead = true;
            }
            else
            {
                float hitChance = Random.value; //0.0 - 1.0
                if (hitChance < 0.3f)
                {
                    anim.SetTrigger("GetHit");
                }
            }
        }
    }

    public void RandomAction()
    {
        float randomVal = Random.value;//0.0 - 1.0

        if (randomVal < 0.7f)//70% free roam
        {
            action = false;
        }
        else//30% chance to summon minions
        {
            action = true;
        }
    }

    public void Died()
    {
        Debug.Log("drop item");
        agent.enabled = false;
        hitbox.enabled = false;
        anim.SetBool("isalive", false);
        anim.SetTrigger("Dead");

        //position of droping item base on boss
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            Vector3 spawnItemPos = transform.position + new Vector3(0.0f, -2.75f, -4.5f);//infront of the tree
            Instantiate(WeaponGate, spawnItemPos, transform.rotation);
        }
        else
        {
            Vector3 spawnItemPos = transform.position + new Vector3(0.0f, -0.25f, 3.0f);//infront of the necromancer
            Instantiate(WeaponGate, spawnItemPos, transform.rotation);
        }
    }
}
