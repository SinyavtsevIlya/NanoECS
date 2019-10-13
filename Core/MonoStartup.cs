using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public abstract class MonoStartup : MonoBehaviour
{

    protected Systems systems;

    public void Initialize()
    {
        systems = Setup();

        // no initialization requared for now
    }

    public abstract Systems Setup();

}
public class Systems : Iinitializable, IExecutable, ILateExecute, IFixedExecutable, IStop, IPause
{
    IContext context;

    public List<ISystem> Values = new List<ISystem>();

    public List<Iinitializable> initializables = new List<Iinitializable>();
    public List<IExecutable> executables = new List<IExecutable>();
    public List<IPause> pauses = new List<IPause>();
    public List<IStop> stops = new List<IStop>();
    public List<ILateExecute> lateExecutes = new List<ILateExecute>();
    public List<IFixedExecutable> fixedExecutes = new List<IFixedExecutable>();

    public Systems(IContext context)
    {
        this.context = context;
    }

    public Systems Add<T>() where T : ISystem, new()
    {
        var system = new T();
        return Add(system);
    }

    public Systems Add(ISystem system)
    {
        if (system is Iinitializable)
        {
            initializables.Add(system as Iinitializable);
        }
        if (system is IExecutable)
        {
            executables.Add(system as IExecutable);
        }
        if (system is IPause)
        {
            pauses.Add(system as IPause);
        }
        if (system is IStop)
        {
            stops.Add(system as IStop);
        }
        if (system is ILateExecute)
        {
            lateExecutes.Add(system as ILateExecute);
        }
        if (system is IFixedExecutable)
        {
            fixedExecutes.Add(system as IFixedExecutable);
        }

        Values.Add(system);
        return this;
    }

    public Systems Remove(ISystem system)
    {
        if (system is Iinitializable)
        {
            initializables.Remove(system as Iinitializable);
        }
        if (system is IExecutable)
        {
            executables.Remove(system as IExecutable);
        }
        if (system is IPause)
        {
            pauses.Remove(system as IPause);
        }
        if (system is IStop)
        {
            stops.Remove(system as IStop);
        }

        if (system is ILateExecute)
        {
            lateExecutes.Remove(system as ILateExecute);
        }

        if (system is IFixedExecutable)
        {
            fixedExecutes.Remove(system as IFixedExecutable);
        }

        Values.Remove(system);
        return this;
    }


    public void Initialize()
    {
        for (int i = 0; i < initializables.Count; i++)
        {
            initializables[i].Initialize();
            context.HandleDalayedOperations();
        }
    }

    public void Execute()
    {
        for (int i = 0; i < executables.Count; i++)
        {
            Profiler.BeginSample(executables[i].ToString());
            executables[i].Execute();
            context.HandleDalayedOperations();
            Profiler.EndSample();
        }
    }

    public void LateExecute()
    {
        for (int i = 0; i < lateExecutes.Count; i++)
        {
            Profiler.BeginSample(lateExecutes[i].ToString());
            lateExecutes[i].LateExecute();
            context.HandleDalayedOperations();
            Profiler.EndSample();
        }
    }

    public void Stop()
    {
        for (int i = 0; i < stops.Count; i++)
        {
            stops[i].Stop();
        }
    }

    public void Pause(bool state)
    {
        for (int i = 0; i < pauses.Count; i++)
        {
            pauses[i].Pause(state);
        }
    }
}