using UnityEngine;

namespace Game.Presenters.Navigation
{
    public class PawnPresenter : MonoBehaviour
    {
        [SerializeField]
        float cameraFollowSpeed = 2f;

        public float CameraFollowSpeed
        {
            get
            {
                return cameraFollowSpeed;
            }
        }

        NavMeshAgent _agent;

        Transform _transform;
        public Transform Transform
        {
            get
            {
                return _transform;
            }
        }
        
        void Awake()
        {
            _transform = GetComponent<Transform>();
            _agent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 targetPosition)
        {
            _agent.SetDestination(targetPosition);
        }
    }
}
