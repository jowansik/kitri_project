using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private float idleTime;

    public override void Enter(Enemy _parent)
    {
        idleTime = 0f;

        parent = _parent;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        idleTime = 0f;
        parent.StopCoroutine(coroutine);
    }

    public override void Update()
    {
        if (parent.type == EEnemyType.Enemy_Archer)
            return;

        idleTime += Time.deltaTime;
        
        if (idleTime > parent.wanderingTime)
            parent.AI.Wander();
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.IsAlive == true)
        {
            yield return new WaitForSeconds(0.2f);

            parent.ResetLocalPos();

            switch (parent.type)
            {
                case EEnemyType.Enemy_Melee:
                    {
                        float dist = Vector3.Distance(parent.NavTR.position, parent.PlayerTR.position);

                        if (dist <= parent.meleeAttackRange)
                        {
                            if (parent.BSkillReady)
                                parent.AI.Skill();
                            else
                                parent.AI.MeleeAttack();
                        }

                        else if (dist < parent.aggroRadius)
                        {
                            parent.AI.Follow();
                        }
                    }
                    break;
                case EEnemyType.Enemy_Archer:
                    {
                        float dist = Vector3.Distance(parent.NavTR.position, parent.PlayerTR.position);

                        if (dist <= parent.meleeAttackRange)
                        {
                            if (parent.BRunaway)
                                parent.AI.Runaway();
                            else
                                parent.AI.MeleeAttack();
                        }

                        else if (dist < parent.arrowAttackRange && parent.BReload == false)
                        {
                            if (parent.BSkillReady)
                                parent.AI.Skill();
                            else
                                parent.AI.ArrowAttack();
                        }
                    }
                    break;
                case EEnemyType.Enemy_Boss:
                    {

                    }
                    break;
                default:
                    break;
            }
        }
    }
}
