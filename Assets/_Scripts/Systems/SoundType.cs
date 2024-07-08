using System;
using Systems;

namespace _Scripts.Systems
{
    
    [Serializable] public struct AudioData
    {
        public SoundType soundType;
        public AudioCueData audioCueData;
    }
    public enum SoundType
    {
        Footstep,
    }
}