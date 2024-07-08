using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace _Scripts.Systems.UI.Animation
{
    public enum SequenceAction
    {
        Append,
        Join,
        Insert,
        Prepend
    }

    [Serializable]
    public struct AnimationData
    {
        public TweenAnimation animation;
        public GameObject target;
        public SequenceAction sequenceAction;
        public float atPosition; // Only used when sequenceAction is Insert
    }
    
    [Serializable]
    public class TweenSequence
    {
        private Sequence _sequence;
        [SerializeField] private List<AnimationData> animations = new List<AnimationData>();
        public void Init(GameObject self = null)
        {
            _sequence = DOTween.Sequence();
            foreach (var animationData in animations)
            {
                var tween = animationData.animation.PlayAnimation(animationData.target ? animationData.target : self);
                switch (animationData.sequenceAction)
                {
                    case SequenceAction.Append:
                        _sequence.Append(tween);
                        break;
                    case SequenceAction.Join:
                        _sequence.Join(tween);
                        break;
                    case SequenceAction.Insert:
                        _sequence.Insert(animationData.atPosition, tween);
                        break;
                    case SequenceAction.Prepend:
                        _sequence.Prepend(tween);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            _sequence.SetUpdate(true);
            _sequence.SetAutoKill(false);
            _sequence.Pause();
        }

        public Sequence Play() { return _sequence.Play(); }
        public Sequence Append(Tween tween) { return _sequence.Append(tween); }
        public Sequence Join(Tween tween) { return _sequence.Join(tween); }
        public Sequence Insert(float atPosition, Tween tween) { return _sequence.Insert(atPosition, tween); }
        public Sequence Prepend(Tween tween) { return _sequence.Prepend(tween); }
        
        public Sequence OnComplete(TweenCallback action) { return _sequence.OnComplete(action); }
        
        public void Kill() { _sequence.Kill(); }
        public void Complete() { _sequence.Complete(); }
        public void Restart() { _sequence.Restart(); }
        public void Rewind() { _sequence.Rewind(); }

        public void Pause() { _sequence.Pause(); }
        public void SetAutoKill(bool autoKillOnCompletion) { _sequence.SetAutoKill(autoKillOnCompletion); }
        
        
    }
}