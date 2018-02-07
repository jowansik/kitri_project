using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MyBaseObejct
{
    Actor actor;
    public int attackState;
    public int AttackState
    {
        get { return attackState; }
        set { attackState = value; }
    }

    // Use this for initialization
    void Start()
    {
        //actor = FindInParentComp<Actor>();
        actor = GetComponentInParent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hit!");

        if (attackState == 0)
        {
            if (other.gameObject.GetComponent<Actor>() != null)
            {
                other.transform.position += (other.transform.position - transform.position) * 0.1f; // 밀침
                other.SendMessage("onDamaged", actor.POWER);
            }
        }
    }
}
