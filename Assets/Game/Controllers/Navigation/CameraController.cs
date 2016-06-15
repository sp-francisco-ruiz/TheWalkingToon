using Game.Managers;
using UnityEngine;

namespace Game.Controllers
{
    public class CameraController : MonoBehaviour
    {
        Camera _camera;
        public Camera Camera
        {
            get
            {
                return _camera;
            }
        }

        GameObject _gameObject;
        public GameObject GameObject
        {
            get
            {
                return _gameObject;
            }
        }

        void Awake()
        {
            _gameObject = gameObject;
            _camera = GetComponent<Camera>();
            CameraManager.Instance.RegisterCamera(this);
            CameraManager.Instance.SetActiveCamera(this);
        }

        void Ondestroy()
        {
            CameraManager.Instance.UnregisterCamera(this);
        }
    }
}