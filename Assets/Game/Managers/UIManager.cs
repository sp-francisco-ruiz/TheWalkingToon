using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Presenters.UI.Popups;

namespace Game.Managers
{
    public class UIManager : MonoBehaviour 
    {
        [SerializeField] Canvas _canvas;
        RectTransform _canvasTransform;
        readonly List<PopupPresenter> _currentPopups = new List<PopupPresenter>();

        public static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = UnityEngine.Object.FindObjectOfType<UIManager>();
                    Debug.Assert(_instance != null);
                }
                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }

        void Awake()
        {
            Instance = this;
            _canvasTransform = _canvas.GetComponent<RectTransform>();
            DontDestroyOnLoad(gameObject);
        }

        public void PushPopup(PopupPresenter popup)
        {
            if(popup != null && !_currentPopups.Contains(popup))
            {
                var popupTransform = popup.GetComponent<RectTransform>();
                popupTransform.SetParent(_canvasTransform,false);
            }
            if(_currentPopups.Count > 0)
            {
                InputManager.Instance.Paused = true;
            }
        }

        public void PopPopup(PopupPresenter popup)
        {
            if(popup != null && _currentPopups.Contains(popup))
            {
                var popupTransform = popup.GetComponent<RectTransform>();
                popupTransform.SetParent(_canvasTransform);
                popupTransform.localPosition = Vector3.zero;
            }
            if(_currentPopups.Count < 1)
            {
                InputManager.Instance.Paused = false;
            }
        }
    }
}