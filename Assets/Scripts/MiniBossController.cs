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
    public GameObject HealthBarGO;

    public float SummonTimer = 0f, SummonDuration = 15f, freeroamRange = 10f;
    private bool isSummoning = false;
    private AnimationTrigger SummonTrigger;

    //UI
    private HealthBar healthBar;

    private Collider hitbox;

    private bool isDead = false;
    public Transform spawnitemPos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MaxHealth;
        healthBar = GetComponent<HealthBar>();
        hitbox = GetComponentInChildren<Collider>();
        SummonTrigger = GetComponentInChildren<AnimationTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        Vector3 playerPos = PlayerController.instance.transform.position;

        if(SummonTrigger.Trigger)
        {
            isSummoning = false;
            Debug.Log("Stupid");
            SummonTimer = SummonDuration;
            SummonTrigger.Trigger = false;
        }

        if (isSummoning) return;

        if (SummonTimer > 0)
        {
            SummonTimer -= Time.deltaTime;
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                Vector2 randPos = Random.insideUnitCircle * freeroamRange;//random position for freeroam
                agent.destination = playerPos + new Vector3(randPos.x, 0f, randPos.y);
            }
        }
        else
        {
            Debug.Log("summon");
            agent.isStopped = true;
            agent.ResetPath();
            isSummoning = true;
            anim.SetTrigger("Summon");
            PlayerController.instance.audiosource.PlayOneShot(SummonSfx, 0.4f);
            CameraController.Instance.Shake();
            SummonEnemy.instance.spawnEnemy();
        }
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.4f);
                Died();
                isDead = true;
            }
            else
            {
                if (Random.value < 0.035f)
                {
                    anim.SetTrigger("GetHit");
                    PlayerController.instance.audiosource.PlayOneShot(GetHitSfx, 0.6f);
                }
            }
        }
    }

    public void Died()
    {
        Debug.Log("drop item");
        agent.enabled = false;
        hitbox.enabled = false;
        anim.SetBool("isalive", false);
        anim.SetTrigger("Dead");
        HealthBarGO.SetActive(false);

        SummonEnemy.instance.DestroyEnemy();
        SpawnBoss.instance.closeBGM();

        //spawn item
        Instantiate(WeaponGate, spawnitemPos.position, transform.rotation);
    }
}
