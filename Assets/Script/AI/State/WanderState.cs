using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : IState
{
    private Vector3 nextPos;
    private bool bEndPos;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        bEndPos = false;
        SetNextPos();
        
        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);
    }

    public override void Update()
    {
        if (bEndPos == false)
        {
            Quaternion look = Quaternion.identity;
            Vector3 dir = ((nextPos - parent.MobTR.position)*1000000/1000000).normalized;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                look.SetLookRotation(dir);

                parent.MobTR.rotation = look;
            }
            else
            {
                parent.NavAgent.SetDestination(Vector3.zero);
                parent.NavAgent.isStopped = true;
                bEndPos = true;

                parent.AI.Idle();
            }
        }
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.Life == true)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(parent.MobTR.position, parent.PlayerTR.position);

            if (dist <= parent.meleeAttackRange)
            {
                parent.AI.Attack();
            }

            else if (dist < parent.aggroRadius)
            {
                parent.AI.Follow();
            }
        }
    }

    public void SetNextPos()
    {
        nextPos = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));

        parent.NavAgent.SetDestination(nextPos);
        parent.NavAgent.isStopped = false;
        bEndPos = false;
    }
}
