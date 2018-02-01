using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBaseObejct : MonoBehaviour {

    Dictionary<string, UnityEngine.Component> DicComponent = new Dictionary<string, Component>();

    MyBaseObejct _TargetObject = null;
    public MyBaseObejct TargetComponent
    {
        get { return _TargetObject; }
        set { _TargetObject = value; }
    }

    public GameObject SelfObject
    {
        get
        {
            if (TargetComponent)
            {
                return TargetComponent.SelfObject;
            }
            else
            {
                return this.gameObject;
            }
        }
    }

    public Transform SelfTransform
    {
        get
        {
            if (TargetComponent)
            {
                return TargetComponent.SelfTransform;
            }
            else
            {
                return this.transform;
            }
        }
    }

    public virtual object GetData(string keyData, params object[] datas)
    {
        //params=가변인자

        return null;
    }

    public virtual void ThrowEvent(string keyData, params object[] datas)
    {
        //transform.fin
    }


    public Transform FindTrans(string name)
    {
        return _FindTrans(name, transform);
    }
    public Transform _FindTrans(string name, Transform trans)
    {
        if (name == trans.name)
            return trans;
        if (trans.childCount == 0)
        {
            return null;
        }
        else
        {
            for (int i = 0; i < trans.childCount; ++i)
            {
                Transform reTrans = _FindTrans(name, trans.GetChild(i));
                if (reTrans != null)
                    return reTrans;
            }
        }
        return null;
    }

    public Transform FindInChild(string strName)
    {
        return _FindInChild(strName, SelfTransform);
    }
    public Transform _FindInChild(string strName, Transform trans)
    {
        if (trans.name == strName)
            return trans;
        else
        {
            for (int i = 0; i < trans.childCount; ++i)
            {
                Transform returnTrans = _FindInChild(strName, trans.GetChild(i));
                if (returnTrans != null)
                    return returnTrans;
            }
            return null;
        }
    }

    EBaseObjectState _ObjectState = EBaseObjectState.objectState_Normal;

    public EBaseObjectState ObjectState
    {
        get
        {
            if (TargetComponent != null)
                return TargetComponent.ObjectState;
            else
                return this._ObjectState;
        }
        set
        {
            if (TargetComponent != null)
                TargetComponent.ObjectState = value;
            else
                this.ObjectState = value;
        }
    }


    public T SelfComponent<T>() where T : UnityEngine.Component
    {

        string ObjectName = string.Empty;
        string TypeName = typeof(T).ToString();
        T tempComponent = default(T);
        if (TargetComponent != null)
        {
            ObjectName = TargetComponent.SelfObject.name;
            tempComponent = TargetComponent.SelfComponent<T>();

        }
        else
        {
            ObjectName = this.gameObject.name;
            if (DicComponent.ContainsKey(TypeName) == true)
            {
                //asT = Dynamic_Cast<T>
                tempComponent = DicComponent[TypeName] as T;


            }
            else
            {
                tempComponent = this.GetComponent<T>();
                if (tempComponent == null)
                {
                    //error
                    Debug.LogError("ObjectName : " + ObjectName + ", Missing Component : " + TypeName);
                    tempComponent = this.gameObject.AddComponent<T>();
                }
                else
                {
                    DicComponent.Add(TypeName, tempComponent);
                }
            }
        }

        return tempComponent;
    }
}
