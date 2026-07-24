using UnityEngine;
using UnityEngine.AI;

public class MiniBossController : MonoBehaviour, IDamagable
{
    public int currentHealth, MaxHealth = 10;

    public AudioClip SummonSfx, GetHitSfx, DeathSfx;

    public NavMeshAgent agent;
    public Animator animator;
    public GameObject WeaponGate;

    private bool action = false;//true:summon, false:free roam
    public float actionTimer = 0f, actionDuration = 5f, freeroamRange = 10f;

    //UI
    private HealthBar healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MaxHealth;
        healthBar = GetComponent<HealthBar>();
        actionTimer = actionDuration;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = PlayerController.instance.transform.position;

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

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                RandomAction();
                if (action)//summon enemies
                {
                    Debug.Log("summon");
                    animator.SetTrigger("Summon");
                    SummonEnemy.instance.spawnEnemy();
                }
                actionTimer = actionDuration;
            }
        }
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            currentHealth -= damage;
            PlayerController.instance.audiosource.PlayOneShot(GetHitSfx, 0.2f);

            float hitChance = Random.value; //0.0 - 1.0
            if (hitChance < 0.3f)
            {
                animator.SetTrigger("GetHit");
            }

            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.2f);
                Died();
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
        agent.enabled = false;
        animator.SetTrigger("Dead");
        Instantiate(WeaponGate,transform.position,transform.rotation);
    }
}
