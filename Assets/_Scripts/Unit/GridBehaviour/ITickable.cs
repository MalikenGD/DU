using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace _Scripts.Unit.GridBehaviour
{
    public interface ITickable
    {
        public void Tick();
    }
}