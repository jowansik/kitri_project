using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : IState
{
    public override void Enter(Enemy _parent)
    {
        Debug.Log("스킬");

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
                        if (normalizedTime >= 0.9f)
                            parent.AI.Idle();
                    }
                    break;
                case EEnemyType.Enemy_Archor:
                    {
                        if (normalizedTime >= 0.9f)
                            parent.AI.Idle();
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
