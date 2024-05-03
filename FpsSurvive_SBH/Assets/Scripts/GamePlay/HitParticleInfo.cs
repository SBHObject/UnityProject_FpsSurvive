using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.GamePlay
{
    public enum HitMaterial
    {
        Dirt,
        Stone,
        Metal,
        Wood,
        Body,
        None
    }

    public class HitParticleInfo : MonoBehaviour
    {
        public HitMaterial material;
    }
}