using System;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation
{
    [Serializable]
    // This should be a Interface, but for it to appear in the inspector, it must be a class
    public abstract class TweenAnimation : ScriptableObject
    {
        public abstract Tween PlayAnimation(GameObject target);
        
        public abstract void CancelAnimation(GameObject target);

    }
}
