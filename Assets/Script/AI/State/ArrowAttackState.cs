using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttackState : IState
{
    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        parent.Arrow.SetActive(true);

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        parent.Arrow.SetActive(false);

        parent.BReload = true;
    }

    public override void Update()
    {
        base.Update();

        Quaternion look = Quaternion.identity;
        Vector3 dir = parent.PlayerTR.position - parent.NavTR.position;
        dir.y = 0f;
        dir = dir.normalized;

        look.SetLookRotation(dir);

        parent.NavTR.rotation = look;
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.IsAlive == true)
        {
            yield return new WaitForSeconds(0.2f);

            switch (parent.type)
            {
                case EEnemyType.Enemy_Melee:
                    break;
                case EEnemyType.Enemy_Archer:
                    {
                        float dist = Vector3.Distance(parent.NavTR.position, parent.PlayerTR.position);

                        if (dist <= parent.meleeAttackRange)
                        {
                            parent.AI.MeleeAttack();
                        }

                        else if (normalizedTime >= 0.9f)
                        {
                            Shoot();
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

    public void Shoot()
    {
        parent.InstantiateArrow();
    }
}
