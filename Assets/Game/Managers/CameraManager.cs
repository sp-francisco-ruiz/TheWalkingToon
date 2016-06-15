using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Controllers;

namespace Game.Managers
{
    public class CameraManager
    {
        Dictionary<string, CameraController> _cameras;
        CameraController _activeCamera;

        static CameraManager _instance;
        public static CameraManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CameraManager();
                }
                return _instance;
            }
        }

        CameraManager()
        {
            _cameras = new Dictionary<string, CameraController>();
        }

        public void SetActiveCamera(string cameraName)
        {
            foreach(var cameraEntrance in _cameras)
            {
                cameraEntrance.Value.GameObject.SetActive(false);
            }

            CameraController camera;
            if (_cameras.TryGetValue(cameraName, out camera))
            {
                camera.GameObject.SetActive(true);
                _activeCamera = camera;
            }
            else
            {
                Debug.LogWarning("Camera with name " + cameraName + " not found.");
            }
        }

        public void SetActiveCamera(CameraController cameraController)
        {
            if(cameraController != null)
            {
                RegisterCamera(cameraController);
                SetActiveCamera(cameraController.gameObject.name);
            }
        }

        public CameraController GetCameraByName(string cameraName)
        {
            CameraController camera;
            if (_cameras.TryGetValue(cameraName, out camera))
            {
                return camera;
            }
            return null;
        }

        public CameraController GetActiveCamera()
        {
            return _activeCamera;
        }

        public void RegisterCamera(CameraController cameraToRegister)
        {
            string cameraName = cameraToRegister.GameObject.name;
            if (!_cameras.ContainsKey(cameraName))
            {
                _cameras.Add(cameraName, cameraToRegister);
                cameraToRegister.GameObject.SetActive(false);
            }
        }

        public void UnregisterCamera(CameraController cameraToUnegister)
        {
            string cameraName = cameraToUnegister.gameObject.name;
            if (!_cameras.ContainsKey(cameraName))
            {
                _cameras.Remove(cameraName);
            }
        }
    }
}