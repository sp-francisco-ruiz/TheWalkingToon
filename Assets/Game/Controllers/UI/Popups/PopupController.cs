using UnityEngine;
using System.Collections;
using Game.Presenters.UI.Popups;

namespace Game.Controllers.UI.Popups
{

    public class PopupController 
    {
        protected const string kPath = "UI/Popups/";
        protected string prefabName = "";

        protected PopupPresenter _presenter;

        public virtual void ShowView()
        {
            if(_presenter == null)
            {
                var go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(kPath + prefabName));
                _presenter = go.GetComponent<PopupPresenter>();
                Game.Managers.UIManager.Instance.PushPopup(_presenter);
            }
            _presenter.Show();
        }

        public virtual void HideView() 
        {
            if(_presenter != null)
            {
                _presenter.Hide();
                GameObject.Destroy(_presenter.gameObject);
                Game.Managers.UIManager.Instance.PopPopup(_presenter);
            }
        }


    }

}