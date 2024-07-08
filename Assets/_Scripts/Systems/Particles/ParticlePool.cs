using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Scripts.Systems.Particles
{
    [Serializable]
    public class ParticleStruct
    {
        public ParticleType type;
        public ParticleSystem prefab;
    }
    
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] private ParticleStruct[] effects;

        private readonly Dictionary<ParticleType, ObjectPool<ParticleSystem>> _effectPools = new();
        private void Awake()
        {
            foreach (var effect in effects)
            {
                _effectPools.Add(effect.type, new ObjectPool<ParticleSystem>(() =>
                    {
                        var instance = Instantiate(effect.prefab);
                        return instance;
                    },
                    instance => instance.gameObject.SetActive(true),
                    instance => instance.gameObject.SetActive(false)));
            }

        }
        public void PlayEffect(ParticleType type, Vector3 position)
        {
            if (!_effectPools.ContainsKey(type))
            {
                Debug.LogError($"EffectType {type} not found in dictionary.");
                return;
            }
            
            var effectPool = _effectPools[type];
            var instance = effectPool.Get();
            instance.transform.position = position;
            instance.Play();
            StartCoroutine(ReturnWithDelay(instance, instance.main.duration, effectPool));
        }

        private static IEnumerator ReturnWithDelay(ParticleSystem instance, float delay, IObjectPool<ParticleSystem> pool)
        {
            yield return new WaitForSecondsRealtime(delay);
            pool.Release(instance);
        }
    }
}