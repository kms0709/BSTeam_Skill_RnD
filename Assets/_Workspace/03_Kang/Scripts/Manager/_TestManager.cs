using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TestManager : Singleton<_TestManager>
{
    void Start()
    {
        Debug.Log("_TestManager »ý¼º µÊ.");
    }

    public void TestFN()
    {
        Debug.Log("_TestManager È£Ãâ µÊ.");
    }
}
