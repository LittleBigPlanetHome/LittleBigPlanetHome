using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public virtual bool DontDestroy()
	{
		return true;
	}

    private static T _instance;
    public static T Instance
	{
		get
		{
			return Singleton<T>._instance;
		}
	}

	public virtual void OnAwake()
	{
	}

    void Awake()
    {
        if (Singleton<T>._instance == null || Singleton<T>._instance == this)
		{
			Singleton<T>._instance = base.gameObject.GetComponent<T>();
			if (this.DontDestroy())
			{
				DontDestroyOnLoad(base.gameObject.transform.root.gameObject);
			}
			this.OnAwake();
			return;
		}
		if (Singleton<T>._instance != this && base.gameObject.activeInHierarchy)
		{
			if (Application.isEditor)
			{
				Debug.Log("Another instance found for singletons. Destroying new one..." + base.gameObject.name + typeof(T).Name);
			}
			Destroy(base.gameObject);
		}
    }
}
