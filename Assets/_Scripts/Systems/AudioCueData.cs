using UnityEngine;

namespace Systems
{
    [CreateAssetMenu(fileName = "AudioCueData", menuName = "TFG/Audio/AudioCueData")]
    public class AudioCueData : ScriptableObject
    {
        public AudioClip[] clips;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0f, 1f)]
        public float pitch = 1f;
        [Range(0f, 1f)]
        public float spatialBlend = 0f;
        
        public AudioClip GetRandomClip()
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}
