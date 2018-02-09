using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperHitState : IState
{
    private bool bFlag = false;
    private float orgHeight = 0f;
    private float collHeight = 0.2f;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        orgHeight = parent.PhysicsCollider.height;

        parent.TriggerCollider.height = collHeight;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        parent.TriggerCollider.height = orgHeight;

        parent.Animator.SetBool("UpperHitEnd", false);
        bFlag = false;
    }

    public override void Update()
    {
        if (bFlag)
        {
            base.Update();
            
            if (normalizedTime > 0.9f)
            {
                parent.AI.Idle();
            }
        }
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.IsAlive == true)
        {
            yield return new WaitForSeconds(0.2f);
            
            if (parent.CalcIsFalling() == false && parent.CalcIsGround())
            {
                parent.Animator.SetBool("UpperHitEnd", true);

                yield return new WaitForSeconds(0.2f);  // normalizedTime 초기화 대기

                bFlag = true;

                parent.RigidBody.useGravity = false;
                parent.RigidBody.isKinematic = true;
            }
        }
    }
}
