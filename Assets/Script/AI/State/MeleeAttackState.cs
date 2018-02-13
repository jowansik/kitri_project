using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : IState
{
    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);
    }

    public override void Update()
    {
        base.Update();

        // 공격 시작시 방향 전환
        if ((normalizedTime * 10.0f) % 10 < 1f)
        {
            Quaternion look = Quaternion.identity;
            Vector3 dir = (parent.PlayerTR.position - parent.NavTR.position).normalized;
            dir.y = 0f;
            look.SetLookRotation(dir);

            parent.NavTR.rotation = look;
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
                        float dist = Vector3.Distance(parent.NavTR.position, parent.PlayerTR.position);

                        if (parent.BSkillReady)
                        {
                            parent.AI.SkillState();
                        }

                        if (dist > parent.meleeAttackRange)
                        {
                            parent.AI.Idle();
                        }
                    }
                    break;
                case EEnemyType.Enemy_Archer:
                    {
                        float dist = Vector3.Distance(parent.NavTR.position, parent.PlayerTR.position);

                        if (dist > parent.meleeAttackRange)
                        {
                            parent.AI.Idle();
                        }
                    }
                    break;
                case EEnemyType.Enemy_Boss:
                    break;
                default:
                    break;
            }

        }
    }
}
