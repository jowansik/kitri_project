using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : IState
{
    public override void Enter(Enemy _parent)
    {
        //Debug.Log("스킬");

        parent = _parent;

        if (parent.type == EEnemyType.Enemy_Archer)
            parent.Arrow.SetActive(true);        

        parent.skillPoint = 0f;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        if (parent.type == EEnemyType.Enemy_Archer)
        {
            parent.Arrow.SetActive(false);

            parent.BReload = true;
        }

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
                        {
                            parent.BSkillReady = false;
                            parent.AI.Idle();
                        }
                    }
                    break;
                case EEnemyType.Enemy_Archer:
                    {
                        if (normalizedTime >= 0.9f)
                        {
                            parent.InstantiateArrow();
                            parent.BSkillReady = false;
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
