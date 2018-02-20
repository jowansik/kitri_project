﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : IState
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
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.IsAlive == true)
        {
            yield return new WaitForSeconds(0.2f);            
        }
    }
}
