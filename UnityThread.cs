using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityThread : MonoBehaviour
{
    private static UnityThread _instance;
    public static UnityThread Instance { get { return _instance; } }
    public readonly ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Update()
    {
        if (!RunOnMainThread.IsEmpty)
        {
            Action action;
            while (RunOnMainThread.TryDequeue(out action))
            {
                action.Invoke();
            }
        }
    }

}