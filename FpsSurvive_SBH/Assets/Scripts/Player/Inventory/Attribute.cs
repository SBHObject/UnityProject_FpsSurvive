using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    [System.Serializable]
    public class Attribute
    {
        public AttributeType type;
        public ModifiableInt value;
    }
}