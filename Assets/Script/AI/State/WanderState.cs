using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : IState
{
    private float moveTime;
    private Vector3 moveDir;
    private Quaternion look = Quaternion.identity;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        SetTimeAndDir();

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        moveTime = 0f;
        moveDir = Vector3.zero;
        look = Quaternion.identity;
    }

    public override void Update()
    {
        moveTime -= Time.deltaTime;

        if (moveTime > 0)
        {
            if (moveDir == Vector3.zero)
                return;

            look.SetLookRotation(moveDir);

            parent.MobTR.rotation = look;
            
            parent.NavAgent.SetDestination(parent.MobTR.position + moveDir);
            parent.NavAgent.isStopped = false;
        }
        else
        {
            parent.NavAgent.isStopped = true;
            parent.AI.Idle();
        }
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.IsAlive == true)
        {
            yield return new WaitForSeconds(0.2f);

            switch (parent.type)
            {
                case EEnemyType.Enemy_Melee:
                    {
                        float dist = Vector3.Distance(parent.MobTR.position, parent.PlayerTR.position);

                        if (dist <= parent.meleeAttackRange)
                        {
                            parent.AI.MeleeAttack();
                        }

                        else if (dist < parent.aggroRadius)
                        {
                            parent.AI.Follow();
                        }
                    }
                    break;
                case EEnemyType.Enemy_Archer:
                    break;
                case EEnemyType.Enemy_Boss:
                    break;
                default:
                    break;
            }
        }
    }

    public void SetTimeAndDir()
    {
        moveTime = Random.Range(0.3f, 1.0f);
        moveDir = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3)).normalized;

        //Debug.Log("moveTime : " + moveTime);
        //Debug.Log("moveDir : " + moveDir);
    }
}
