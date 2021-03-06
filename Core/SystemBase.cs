﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NanoEcs
{
    public interface Iinitializable : ISystem
    {
        void Initialize();
    }

    public interface ILateExecute : ISystem
    {
        void LateExecute();
    }

    public interface IExecutable : ISystem
    {
        void Execute();
    }

    public interface IFixedExecutable : ISystem
    {
        void FixedExecute();
    }

    public interface IPause : ISystem
    {
        void Pause(bool pauseState);
    }

    public interface IStop : ISystem
    {
        void Stop();
    }
    public class SystemEcs : ISystem
    {
    }

    public interface ISystem
    {

    }
}







