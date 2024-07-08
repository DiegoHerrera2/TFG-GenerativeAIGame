using _Scripts.Systems.Items;
using _Scripts.Systems.Puzzles;
using UnityEngine;
using UnityHFSM;

namespace _Scripts.Units.NPC
{
    public class NpcPatrol : InputProvider
    {
        [SerializeField] private float timeToIdle = 0.5f;
        private QuestHandlerBase _questHandler;
        private StateMachine _stateMachine;
        
        private bool _movedRight;
        private void Awake()
        {
            _questHandler = GetComponent<QuestHandlerBase>();
            _questHandler.QuestCompletedEvent += () => _stateMachine.Trigger("QuestCompleted");
            
            _stateMachine = new StateMachine();

            var idle = new State(onLogic: (s) => Movement = new Vector2(0, 0));
            var moveRight = new State(
                onEnter: (s) => _movedRight = true,
                onLogic: (s) => Movement = new Vector2(1, 0));

                var moveLeft = new State(
                    onEnter: (s) => _movedRight = false,
                    onLogic: (s) => Movement = new Vector2(-1, 0));
            
            var questComplete = new State(onEnter: (s) =>
            {
                Movement = new Vector2(0, 0);
                RequestJump();
            });

            _stateMachine.AddState("Idle", idle);
            _stateMachine.AddState("MoveRight", moveRight);
            _stateMachine.AddState("MoveLeft", moveLeft);
            _stateMachine.AddState("QuestComplete", questComplete);
            
            _stateMachine.AddTransition("Idle", "MoveRight", (t) => idle.timer.Elapsed > 2f && !_movedRight);
            _stateMachine.AddTransition("MoveRight", "Idle", (t) => moveRight.timer.Elapsed > timeToIdle);
            _stateMachine.AddTransition("Idle", "MoveLeft", (t) => idle.timer.Elapsed > 2f && _movedRight);
            _stateMachine.AddTransition("MoveLeft", "Idle", (t) => moveLeft.timer.Elapsed > timeToIdle);
            
            _stateMachine.AddTriggerTransitionFromAny("QuestCompleted", "QuestComplete");

            _stateMachine.SetStartState("Idle");
            
            _stateMachine.Init();

        }

        private void Update()
        {
            _stateMachine.OnLogic();
        }
    }
}