using System;
using System.Collections;
using Systems;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace _Scripts.Systems
{
    public class AudioManager : MonoBehaviour
    {
        //Object pool
        private ObjectPool<AudioSource> _audioSources;
        
        // [SerializeField]
        // private AudioMixerGroup sfxMixerGroup;
        // [SerializeField] 
        // private AudioMixer audioMixer;

        public static Action<AudioCueData, Vector3> Play;
        public static Action<AudioCueData> PlayAtCamera;
        public static Action<AudioCueData, Vector3, float> PlayWithPitch;
        public static Action<AudioCueData, Vector3, Vector2> PlayWithPitchRange;

        private void Start()
        {
            // SetAudioVolume();
            _audioSources = new ObjectPool<AudioSource>(() =>
                {
                    var audioSource = Instantiate(new GameObject("AudioSource")).AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSource.loop = false;
                    return audioSource;
                }, 
                audioSource => audioSource.gameObject.SetActive(true),
                audioSource => audioSource.gameObject.SetActive(false), 
                null, 
                true, 
                100);
            }

        // private void SetAudioVolume()
        // {
        //     var masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        //     var musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        //     var sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        //     audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        //     audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        //     audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        // }

        private void OnEnable()
        {
            Play += OnPlay;
            PlayAtCamera += OnPlayAtCamera;
            PlayWithPitch += OnPlayWithPitch;
            PlayWithPitchRange += OnPlayWithPitchRange;
        }
        
        private void OnDisable()
        {
            Play -= OnPlay;
            PlayAtCamera -= OnPlayAtCamera;
            PlayWithPitch -= OnPlayWithPitch;
            PlayWithPitchRange -= OnPlayWithPitchRange;
        }
        
        private void OnPlayAtCamera(AudioCueData audioCueData)
        {
            OnPlay(audioCueData, Camera.main.transform.position);
        }
        
        private void OnPlay(AudioCueData audioCueData, Vector3 position)
        {
            OnPlay(audioCueData, position, 1f);
        }
        
        private void OnPlayWithPitch(AudioCueData audioCueData, Vector3 position, float pitch)
        {
            OnPlay(audioCueData, position, pitch);
        }
        
        private void OnPlayWithPitchRange(AudioCueData audioCueData, Vector3 position, Vector2 pitchRange)
        {
            var pitch = Random.Range(pitchRange.x, pitchRange.y);
            OnPlay(audioCueData, position, pitch);
        }

        private void OnPlay(AudioCueData audioCueData, Vector3 position, float pitch)
        {
            var audioSource = _audioSources.Get();
            audioSource.transform.position = position;
            audioSource.clip = audioCueData.GetRandomClip();
            audioSource.volume = audioCueData.volume;
            audioSource.pitch = pitch;
            audioSource.spatialBlend = audioCueData.spatialBlend;
            audioSource.Play();
            StartCoroutine(ReturnWithDelay(audioSource, audioSource.clip.length));
        }
        
        private IEnumerator ReturnWithDelay(AudioSource audioSource, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            _audioSources.Release(audioSource);
        }
    }
}
