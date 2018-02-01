using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    protected Enemy enemy;
    private IState currentState;

    public Enemy Enemy { set { enemy = value; } }

    public virtual void Idle()          { ChangeState(EEnemyState.State_Idle); }
    public virtual void Attack()        { ChangeState(EEnemyState.State_Attack); }
    public virtual void Hit()           { ChangeState(EEnemyState.State_Hit); }
    public virtual void CriticalHit()   { ChangeState(EEnemyState.State_CriticalHit); }
    public virtual void Stun()          { ChangeState(EEnemyState.State_Stun); }
    public virtual void Die()           { ChangeState(EEnemyState.State_Die); }
    public virtual void Follow()        { ChangeState(EEnemyState.State_Follow); }
    public virtual void Runaway()       { ChangeState(EEnemyState.State_Runaway); }
    public virtual void Wander()        { ChangeState(EEnemyState.State_Wander); }

    public virtual void UpdateAI()
    {
        currentState.Update();
    }

    private void ChangeState(EEnemyState eState)
    {
        IState newState = EnemyManager.Instance.GetState(eState);

        if (currentState != null)
        {
            currentState.Exit();
            Debug.Log("Exit State : " + currentState.ToString());
        }

        currentState = newState;

        currentState.Enter(enemy);
        Debug.Log("Enter State : " + newState.ToString());

        ChangeAnimation(eState);
    }

    private void ChangeAnimation(EEnemyState enemyState)
    {
        enemy.Animator.SetInteger("State", (int)enemyState);
    }
}
