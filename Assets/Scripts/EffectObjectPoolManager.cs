using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObjectPoolManager : MonoBehaviour
{
    public static EffectObjectPoolManager Instance;

    public List<GameObject> effectPrefabs = new List<GameObject>();
    public int effectPoolSize = 100;

    private List<Queue<GameObject>> effectPools = new List<Queue<GameObject>>();
    private Transform effectPoolParent;
    private bool effectPoolReady;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PrepareEffectPools();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PrepareEffectPools()
    {
        if (effectPoolReady) return;

        //prepare all object pool for all effects
        for (int i = 0; i < effectPrefabs.Count; i++)
        {
            GameObject parentObj = new GameObject(effectPrefabs[i].name + "_ObjectPool");
            effectPoolParent = parentObj.transform;

            Queue<GameObject> pool = new Queue<GameObject>();

            for (int j = 0; j < effectPoolSize; j++)
            {
                GameObject effect = Instantiate(effectPrefabs[i], effectPoolParent);
                effect.gameObject.SetActive(false);
                pool.Enqueue(effect);
            }

            //add to pool
            effectPools.Add(pool);
        }

        effectPoolReady = true;
        Debug.Log(gameObject.name + " effect pools created.");
    }

    public GameObject GetEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        PrepareEffectPools();

        //find specific effect
        int poolIndex = -1;
        for (int i = 0; i < effectPrefabs.Count; i++)
        {
            if (effectPrefabs[i] != null && effectPrefabs[i].name == effectName)
            {
                poolIndex = i;
                break;
            }
        }

        //prevent empty effect
        if (poolIndex == -1)
        {
            Debug.LogWarning("Cant found effects:" + effectName);
            return null;
        }

        //get the founded effects
        GameObject effect;
        if (effectPools[poolIndex].Count > 0)
        {
            effect = effectPools[poolIndex].Dequeue();
        }
        else
        {
            //if not enough effects, create new
            effect = Instantiate(effectPrefabs[poolIndex], effectPoolParent);
        }

        effect.transform.SetPositionAndRotation(position, rotation);
        effect.gameObject.SetActive(true);

        //get particle's life time
        ParticleSystem ps = effect.GetComponentInChildren<ParticleSystem>();
        float lifetime = (ps != null) ? ps.main.duration + ps.main.startLifetime.constantMax : 1.5f;
        
        //return after effect lifetime end
        StartCoroutine(DelayedReturn(effectName, effect, lifetime));
        

        return effect;
    }

    private IEnumerator DelayedReturn(string effectName, GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (effect != null && effect.activeSelf)//prevent error, make sure effect exist
        {
            ReturnEffect(effectName, effect);
        }
    }

    public void ReturnEffect(string effectName, GameObject effect)
    {
        effect.gameObject.SetActive(false);

        //find the objectpool
        int poolIndex = -1;
        for (int i = 0; i < effectPrefabs.Count; i++)
        {
            if (effectPrefabs[i] != null && effectPrefabs[i].name == effectName)
            {
                poolIndex = i;
                break;
            }
        }

        //return back the current effects
        if (poolIndex != -1)
        {
            effectPools[poolIndex].Enqueue(effect);
        }
    }
}
