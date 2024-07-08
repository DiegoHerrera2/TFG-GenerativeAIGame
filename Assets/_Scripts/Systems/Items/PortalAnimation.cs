using _Scripts.Systems.ImageRecognition;
using UnityEngine;
using UnityHFSM;

namespace _Scripts.Systems.Items
{
    public class PortalAnimation : MonoBehaviour
    {
        private SpriteMask _spriteRenderer;
        private Interactable _interactable;
        private ParticleSystem _particleSystem;
        [SerializeField] private AnimationCurve alphaCutoffCurveEnter;
        [SerializeField] private AnimationCurve alphaCutoffCurveExit;

        [SerializeField] private float alphaCutoffSpeed = 0.5f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float enterAnimationSpeed = 1f;
        [SerializeField] private float exitAnimationSpeed = 1f;


        private HybridStateMachine _stateMachine;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteMask>();
            _interactable = GetComponent<Interactable>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            
            _stateMachine = new HybridStateMachine(afterOnLogic: _ =>
            {
                _spriteRenderer.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            });
            
            var idleState = new State(onLogic: (s) => _spriteRenderer.alphaCutoff = Mathf.Lerp(0.1f, 0.3f, Mathf.PingPong(s.timer.Elapsed * alphaCutoffSpeed, 1)));

            var growState = new State(onLogic: (s) =>
                {
                    _spriteRenderer.alphaCutoff = Mathf.Lerp(1, 0.1f, alphaCutoffCurveEnter.Evaluate(s.timer.Elapsed / enterAnimationSpeed));
                }, 
                canExit: (s) => s.timer.Elapsed >= enterAnimationSpeed,
                needsExitTime: true);
            
            var shrinkState = new State(
                onLogic: (s) =>
                {
                    _spriteRenderer.alphaCutoff = Mathf.Lerp(0.1f, 1, alphaCutoffCurveExit.Evaluate(s.timer.Elapsed / exitAnimationSpeed));
                    _particleSystem.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0,0,1), s.timer.Elapsed / exitAnimationSpeed);
                },  
                canExit: (s) => s.timer.Elapsed >= enterAnimationSpeed,
                needsExitTime: true,
                onExit: _ => ImageItemGenerator.Instance.ReleaseItem(gameObject));
            
            _stateMachine.AddState("Idle", idleState);
            _stateMachine.AddState("Grow", growState);
            _stateMachine.AddState("Shrink", shrinkState);
            _stateMachine.AddState("Exit");
            
            _stateMachine.AddTransition("Grow", "Idle");
            _stateMachine.AddTransition("Idle", "Shrink", _ => _interactable.Attached);
            
            _stateMachine.AddTransition("Shrink", "Exit");

            _stateMachine.SetStartState("Grow");
        }

        private void OnEnable()
        {
            _stateMachine.Init();
        }

        private void Update()
        {
            _stateMachine.OnLogic();
        }
    }
}