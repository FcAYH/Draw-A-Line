using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    public static bool InInitialized
    {
        get
        {
            return _instance != null;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);

        else _instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
