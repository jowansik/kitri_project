using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttackState : IState
{
    private float normalizedTime;

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

        normalizedTime = parent.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        Quaternion look = Quaternion.identity;
        Vector3 dir = (parent.PlayerTR.position - parent.MobTR.position).normalized;
        dir.y = 0f;

        look.SetFromToRotation(parent.FireDir.position, dir);
        
        parent.MobTR.rotation = look;
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.Life == true)
        {
            yield return new WaitForSeconds(0.2f);

            switch (parent.type)
            {
                case EEnemyType.Enemy_Melee:
                    break;
                case EEnemyType.Enemy_Archor:
                    {
                        if (normalizedTime >= 0.9f)
                        {
                            Debug.Log(normalizedTime);
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
        Debug.Log("Shoot");

        parent.InstantiateArrow();
    }
}
