using UnityEngine;
using UnityEngine.AI;

namespace StealthShooter.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        // Đổi tên biến để tránh trùng tên lớp StateMachine của Unity
        public StateMachine AiStateMachine { get; private set; }
        public NavMeshAgent Agent { get; private set; }

        [Header("AI Settings")]
        public Transform[] waypoints;
        public Transform player;
        public float detectionRadius = 8f;

        // Các trạng thái
        public PatrolState PatrolState { get; private set; }
        public ChaseState ChaseState { get; private set; }

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            AiStateMachine = new StateMachine();

            PatrolState = new PatrolState(this);
            ChaseState = new ChaseState(this);
        }

        private void Start()
        {
            AiStateMachine.Initialize(PatrolState);
        }

        private void Update()
        {
            AiStateMachine.Update();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

    // --- TRẠNG THÁI 1: ĐI TUẦN (PATROL) ---
    public class PatrolState : IState
    {
        private EnemyAI ai;
        private int currentWaypointIndex = 0;

        public PatrolState(EnemyAI enemyAI) => this.ai = enemyAI;

        public void Enter()
        {
            MoveToNextWaypoint();
        }

        public void Execute()
        {
            if (!ai.Agent.pathPending && ai.Agent.remainingDistance < 0.5f)
            {
                MoveToNextWaypoint();
            }

            if (ai.player != null && Vector3.Distance(ai.transform.position, ai.player.position) <= ai.detectionRadius)
            {
                ai.AiStateMachine.ChangeState(ai.ChaseState);
            }
        }

        public void Exit() { }

        private void MoveToNextWaypoint()
        {
            if (ai.waypoints == null || ai.waypoints.Length == 0) return;
            ai.Agent.SetDestination(ai.waypoints[currentWaypointIndex].position);
            currentWaypointIndex = (currentWaypointIndex + 1) % ai.waypoints.Length;
        }
    }

    // --- TRẠNG THÁI 2: ĐUỔI BẮT (CHASE) ---
    public class ChaseState : IState
    {
        private EnemyAI ai;

        public ChaseState(EnemyAI enemyAI) => this.ai = enemyAI;

        public void Enter()
        {
            Debug.Log("AI: Phát hiện mục tiêu! Đang đuổi bắt...");
        }

        public void Execute()
        {
            if (ai.player == null) return;

            ai.Agent.SetDestination(ai.player.position);

            if (Vector3.Distance(ai.transform.position, ai.player.position) > ai.detectionRadius * 1.5f)
            {
                ai.AiStateMachine.ChangeState(ai.PatrolState);
            }
        }

        public void Exit()
        {
            Debug.Log("AI: Mất dấu mục tiêu, quay lại đi tuần.");
        }
    }
}