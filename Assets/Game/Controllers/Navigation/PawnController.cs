using Game.Managers;
using Game.Presenters.Navigation;
using Statics;
using UnityEngine;

namespace Game.Controllers.Navigation
{
    public class PawnController
    {
        PawnPresenter _presenter;
        Transform _cameraTransform;
        Vector3 _cameraOffset = Vector3.zero;

        public PawnController()
        {
        }

        public void ShowView()
        {
            var go = Object.Instantiate(Resources.Load("Navigation/WhiteBoxCharacterPrefab"), new Vector3(0f,0.5f,0f), Quaternion.identity) as GameObject;
            _presenter = go != null ? go.GetComponent<PawnPresenter>() : null;

            EventDispatcher.Instance.AddListener<FloorClickedEvent>(OnFloorClicked);
            GameManager.Instance.RegisterForUpdate(Update);

            EnableNavigationCamera();
        }

        void EnableNavigationCamera()
        {
            var cameraManager = CameraManager.Instance;
            var navigationCamera = cameraManager.GetCameraByName(CameraNames.NAVIGATION_CAMERA);
            if(navigationCamera != null)
            {
                cameraManager.SetActiveCamera(navigationCamera);
                var cameraActive = cameraManager.GetActiveCamera();
                if (cameraActive != null)
                {
                    _cameraTransform = cameraActive.transform;
                    _cameraOffset = _cameraTransform.position;
                }
            }
        }

        void OnFloorClicked(FloorClickedEvent e)
        { 
            _presenter.MoveTo(e.clickedPosition);
        }

        void Update()
        {
            if(_cameraTransform != null)
            {
                if(_presenter != null && _presenter.Transform != null)
                {
                    Vector3 newCamPosition = _presenter.Transform.position + _cameraOffset;
                    float speed = Time.deltaTime * _presenter.CameraFollowSpeed;
                    _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, newCamPosition, speed);
                }
            }
            else
            {
                EnableNavigationCamera();
            }
        }
    }
}