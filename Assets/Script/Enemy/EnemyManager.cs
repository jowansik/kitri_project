using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonObejct<EnemyManager>
{
    private List<IState> listStates = new List<IState>();
    private Dictionary<EEnemyState, IState> dicState = new Dictionary<EEnemyState, IState>();
    private Dictionary<EEnemyType, List<Enemy>> dicEnemyList = new Dictionary<EEnemyType, List<Enemy>>();

    public List<IState> ListStates { get { return listStates; } }
    public Dictionary<EEnemyState, IState> DicState { get { return dicState; } }
    public Dictionary<EEnemyType, List<Enemy>> DicEnemyList { get { return dicEnemyList; } }
    
    private void Start()
    {
        listStates.Add(new IdleState());
        listStates.Add(new AttackState());
        listStates.Add(new HitState());
        listStates.Add(new CriticalHitState());
        listStates.Add(new StunState());
        listStates.Add(new DieState());
        listStates.Add(new FollowState());
        listStates.Add(new RunawayState());
        listStates.Add(new WanderState());

        for (int i = 0; i < (int)EEnemyState.MAX; i++)
        {
            dicState.Add((EEnemyState)i, listStates[i]);
        }
    }    

    public void AddEnemy(Enemy enemy)
    {
        List<Enemy> listEnemy = null;
        EEnemyType type = enemy.type;

        if (dicEnemyList.ContainsKey(type) == false)
        {
            listEnemy = new List<Enemy>();
            dicEnemyList.Add(type, listEnemy);
        }
        else
        {
            dicEnemyList.TryGetValue(type, out listEnemy);
        }

        listEnemy.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy, bool bDelete = false)
    {
        EEnemyType enemyType = enemy.type;

        if (dicEnemyList.ContainsKey(enemyType) == true && dicEnemyList[enemyType].Count != 0)
        {
            List<Enemy> listEnemy = null;
            dicEnemyList.TryGetValue(enemyType, out listEnemy);
            listEnemy.Remove(enemy);
        }
        else
        {
            Debug.LogError("존재 하지 않는 Enemy를 삭제하려고 합니다.");
        }

        if (bDelete)
        {
            if (enemy.gameObject != null)
                Destroy(enemy.gameObject);
        }
    }

    public IState GetState(EEnemyState state)
    {
        if (state == EEnemyState.MAX)
        {
            Debug.LogError("EEnemyState.MAX 를 매개변수로 사용했습니다.");
            return null;
        }

        IState _state;

        dicState.TryGetValue(state, out _state);

        return _state;
    }
}
