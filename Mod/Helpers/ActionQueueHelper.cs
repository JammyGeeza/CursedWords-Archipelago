using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueueHelper : MonoBehaviour
{
    // Current action being executed
    private Coroutine _currentAction;

    // Thread-safe lock
    private readonly object _lock = new object();

    // Current action queue
    private readonly Queue<(Func<IEnumerator<bool>> Action, string Name)> _queue = new();

    private static ActionQueueHelper _instance;
    public static ActionQueueHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("ActionQueueHelper");
                _instance = gameObject.AddComponent<ActionQueueHelper>();

                DontDestroyOnLoad(gameObject);
            }

            return _instance;
        }
    }

    /// <summary>
    /// Triggered when a queued action completes.
    /// </summary>
    public event Action<bool, string> OnActionCompleted;

    /// <summary>
    /// Add an action to the queue.
    /// </summary>
    /// <param name="action">The action to add to the queue.</param>
    /// <param name="trackingName">The name of the action, if completion tracking required.</param>
    public void Enqueue(Func<IEnumerator<bool>> action, string trackingName = "")
    {
        lock (_lock)
        {
            _queue.Enqueue((action, trackingName));
        }
    }

    /// <summary>
    /// Attempt to process the next action in the queue.
    /// </summary>
    public void ProcessNext()
    {
        // If currently executing or nothing in queue, ignore
        if (_currentAction != null)
        {
            return;
        }

        // Execute next action
        lock (_lock)
        {
            if (_queue.TryDequeue(out (Func<IEnumerator<bool>> Action, string Name) nextAction))
            {
                _currentAction = StartCoroutine(Execute(nextAction.Action, nextAction.Name));
            }
        }
    }

    /// <summary>
    /// Execute an action.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="trackingName">(OPTIONAL) Tracking name to trigger event.</param>
    private IEnumerator Execute(Func<IEnumerator<bool>> actionFactory, string trackingName = "")
    {
        bool success = false;

        IEnumerator<bool> action = actionFactory();

        // Execute action
        while (action.MoveNext())
        {
            success = action.Current;
            yield return null;
        }

        OnActionCompleted?.Invoke(success, trackingName);

        // Clear for next item in queue
        _currentAction = null;
    }
}