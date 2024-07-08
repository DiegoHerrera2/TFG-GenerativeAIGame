using UnityEngine;

namespace _Scripts.Systems
{
    public class AnimationDelay : MonoBehaviour
    {
        [SerializeField] private float delay;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            delay = Random.Range(0.0f, 1.0f);
            _animator.SetFloat("Delay", delay);
        }
    }
}
