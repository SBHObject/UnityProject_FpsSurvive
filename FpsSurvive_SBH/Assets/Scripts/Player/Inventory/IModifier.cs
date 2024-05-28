using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    public interface IModifier
    {
        void AddValue(ref int baseValue);
    }
}