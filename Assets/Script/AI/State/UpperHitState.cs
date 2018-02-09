using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperHitState : IState
{
    private bool bStandUpFlag = false;
    private bool bCoroutineFlag = false;

    private bool bLand = false;
    private float orgCollHeight = 0f;
    private float collHeight = 0.2f;
    private Vector3 orgParentLocalPos;

    public bool BLand { get { return bLand; } set { bLand = value; } }

    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        orgCollHeight = parent.TriggerCollider.height;
        parent.TriggerCollider.height = collHeight;
        orgParentLocalPos = parent.transform.localPosition;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        parent.TriggerCollider.height = orgCollHeight;

        parent.Animator.SetBool("UpperHitEnd", false);
        bStandUpFlag = false;
        bCoroutineFlag = false;
        bLand = false;
        orgCollHeight = 0f;
        collHeight = 0.2f;
    }

    public override void Update()
    {
        if (bLand)
        {
            parent.SwitchPhysicsCollider(); // off

            parent.transform.localPosition = orgParentLocalPos;

            bLand = false;

            parent.RigidBody.isKinematic = true;

            bCoroutineFlag = true;
        }

        else if (bStandUpFlag)
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

            if (bCoroutineFlag)
            {
                parent.Animator.SetBool("UpperHitEnd", true);

                yield return new WaitForSeconds(0.3f);  // normalizedTime 초기화 대기

                bStandUpFlag = true;
            }
        }
    }
}
