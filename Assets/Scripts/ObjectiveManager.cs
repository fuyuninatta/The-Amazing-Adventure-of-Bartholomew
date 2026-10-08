using UnityEngine;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public List<GameObject> Objective = new List<GameObject>();
    public float currentObj;
    public static ObjectiveManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentObj = -1;
        UpdateObjective();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateObjective()
    {
        currentObj++;
        if (currentObj >= Objective.Count)
        {
            currentObj = Objective.Count;
        }

        for (int i = 0; i < Objective.Count; i++)
        {
            if(i == currentObj)
            {
                Objective[i].SetActive(true);
            }
            else
            {
                Objective[i].SetActive(false);
            }
        }
    }
}
