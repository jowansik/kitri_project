using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonObejct<T> : MyBaseObejct where T : SingletonObejct<T>
{
    private static T _instance = null;

    static bool bShoutdown = false;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if (bShoutdown == false)
                {
                    T newInstace = GameObject.FindObjectOfType<T>() as T;

                    if (newInstace == null)
                    {
                        newInstace = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }
                    InstanceInit(newInstace);
                    //조건이 false일때 실행
                    Debug.Assert(_instance != null, typeof(T).ToString() + " singleton Init Failed.");
                }
            }
            return _instance;
        }
    }
    static void InstanceInit(Object inst)
    {
        _instance = inst as T;
        _instance.Init();
    }

    public virtual void Init()
    {
        DontDestroyOnLoad(_instance);

    }
    private void OnDestroy()
    {
        _instance = null;
    }



    //종료시 예외처리
    private void OnApplicationQuit()
    {
        _instance = null;
        bShoutdown = true;
    }
}
