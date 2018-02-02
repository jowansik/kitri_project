using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonObejct<EnemyManager>
{
    private Dictionary<EEnemyType, List<Enemy>> dicEnemyList = new Dictionary<EEnemyType, List<Enemy>>();

    public Dictionary<EEnemyType, List<Enemy>> DicEnemyList { get { return dicEnemyList; } }

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

    public void RemoveEnemy(Enemy enemy, bool bDestroy = false)
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

        if (bDestroy)
        {
            if (enemy.gameObject != null)
                Destroy(enemy.gameObject);
        }
    }

    //public IState GetState(EEnemyState state)
    //{
    //    if (state == EEnemyState.MAX)
    //    {
    //        Debug.LogError("EEnemyState.MAX 를 매개변수로 사용했습니다.");
    //        return null;
    //    }

    //    IState _tmp;
    //    dicState.TryGetValue(state, out _tmp);

    //    return _tmp;
    //}

    //public IState[] CreateArrayStates()
    //{
    //    IState[] arrStates = new IState[listStates.Count];

    //    listStates.CopyTo(arrStates);

    //    return arrStates;
    //}
}
