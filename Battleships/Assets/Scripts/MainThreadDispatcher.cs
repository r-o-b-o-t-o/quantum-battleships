using System;
using System.Collections.Concurrent;

public class MainThreadDispatcher : Singleton<MainThreadDispatcher>
{
    private ConcurrentQueue<Action> actions;

    private void Awake()
    {
        instance = this;
        this.actions = new ConcurrentQueue<Action>();
    }

    private void Update()
    {
        while (this.actions.Count > 0)
        {
            Action a;
            if (this.actions.TryDequeue(out a))
            {
                a?.Invoke();
            }
        }
    }

    public void RunOnMainThread(Action a)
    {
        if (a == null) return;
        this.actions.Enqueue(a);
    }

    public void RunOnMainThread<T1>(Action<T1> a, T1 t1)
    {
        if (a == null) return;
        this.actions.Enqueue(() =>
        {
            a?.Invoke(t1);
        });
    }

    public void RunOnMainThread<T1, T2>(Action<T1, T2> a, T1 t1, T2 t2)
    {
        if (a == null) return;
        this.actions.Enqueue(() =>
        {
            a?.Invoke(t1, t2);
        });
    }

    public void RunOnMainThread<T1, T2, T3>(Action<T1, T2, T3> a, T1 t1, T2 t2, T3 t3)
    {
        if (a == null) return;
        this.actions.Enqueue(() =>
        {
            a?.Invoke(t1, t2, t3);
        });
    }
}
