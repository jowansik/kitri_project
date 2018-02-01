using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenSetting : MonoBehaviour
{
    //private int TransformID;
    
    public GameObject[] Mobs;
    public int MobID;
    public float GenCycle;
    public bool Auto;
    
    public float AreaXLength;
    public float AreaYLength;
    public float AreaZLength;

    [System.NonSerialized]
    public bool GenTrigger;
    private float CurrentTime;

    // Use this for initialization
    void Start()
    {
        CurrentTime = 0f;
        GenTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Auto)
        {
            CurrentTime += Time.deltaTime;

            if (CurrentTime > GenCycle)
            {
                CurrentTime = 0f;
                GenTrigger = true;
            }
        }

        if (GenTrigger)
        {
            if (Mobs.Length > MobID)
            {
                Vector3 pos = new Vector3(Random.Range(-AreaXLength / 2, AreaXLength / 2), Random.Range(-AreaYLength / 2, AreaYLength / 2), Random.Range(-AreaZLength / 2, AreaZLength / 2));

                Instantiate(Mobs[MobID], transform.position + pos, Quaternion.identity);
            }
            else
                Debug.Log("Out of Range, MobID : " + MobID);

            GenTrigger = false;
        }
    }
}
