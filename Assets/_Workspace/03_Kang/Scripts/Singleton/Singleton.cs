using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// Singleton Class 를 상속받은 T 스크립트를 가져옵니다.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Instance가 없으면 하이어라키에서 찾습니다.
                _instance = FindFirstObjectByType<T>();

                
                if (_instance == null)
                {
                    // 하이어라키에 없으면 오브젝트를 하나 생성합니다.
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
