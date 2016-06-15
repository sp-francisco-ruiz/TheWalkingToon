using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Game.Controllers.Navigation;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            MAIN_MENU,
            NAVIGATION,
            BATTLE
        }

        public static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = UnityEngine.Object.FindObjectOfType<GameManager>();
                    Debug.Assert(_instance != null);
                }
                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }

        PawnController _pawn;
        FloorController _floor;
        List<Action> _updateActions;
        GameState _currentGameState;
        InputManager _inputManager;

        void Awake()
        {
            Instance = this;
            _updateActions = new List<Action>();
            _inputManager = InputManager.Instance;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            StartCoroutine(LoadLevelAdditive("Level1"));
            _currentGameState = GameState.NAVIGATION;
        }

        IEnumerator LoadLevelAdditive(string levelName)
        {
            _inputManager.Paused = true;
            AsyncOperation async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            yield return async;
            yield return new WaitForSeconds(0.1f);
            OnSceneLoaded();
        }

        Game.Controllers.UI.Popups.YesNoPopupController _popup;
        void OnSceneLoaded()
        {
            CameraManager.Instance.SetActiveCamera(Statics.CameraNames.NAVIGATION_CAMERA);
            _popup = new Game.Controllers.UI.Popups.YesNoPopupController(
                Localization.LocalizationManager.Instance.GetLocalizedString(Localization.LocalizationKeys.TEST_YOU_SURE_KEY),
                Localization.LocalizationManager.Instance.GetLocalizedString(Localization.LocalizationKeys.TEST_BEGIN_POPUP_TEST_KEY),
                InitScene,
                ()=>
                {
                    Debug.Log("you said no, but I don't care, this will start anyway");
                    InitScene();
            });

            _popup.ShowView();
        }

        void InitScene()
        {
            _popup.HideView();
            _inputManager.Paused = false;
            switch(_currentGameState)
            {
            case GameState.NAVIGATION:
                    _floor = new FloorController();
                    _floor.ShowView();
                    _pawn = new PawnController();
                    _pawn.ShowView();
                break;
            }
        }

        void Update()
        {
            for(int i = 0; i < _updateActions.Count; ++i)
            {
                var updateAction = _updateActions[i];
                if(updateAction != null)
                {
                    updateAction();
                }
            }
        }

        public void RegisterForUpdate(Action updateAction)
        {
            if(!_updateActions.Contains(updateAction))
            {
                _updateActions.Add(updateAction);
            }
        }

        public void UnregisterForUpdate(Action updateAction)
        {
            if(_updateActions.Contains(updateAction))
            {
                _updateActions.Remove(updateAction);
            }
        }
    }
}