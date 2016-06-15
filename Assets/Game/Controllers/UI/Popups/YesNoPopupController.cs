using UnityEngine;
using System.Collections;
using Game.Presenters.UI.Popups;
using System;
using Game.Localization;

namespace Game.Controllers.UI.Popups
{
    public class YesNoPopupController : PopupController
    {
        YesNoPopupPresenter _yesNoPresenter;

        Action _onYes;
        Action _onNo;
        string _title;
        string _message;

        public YesNoPopupController(string title, string message, Action yesCallback, Action noCallback)
        {
            prefabName = "YesNoPopup";
            _title = title;
            _message = message;
            _onYes = yesCallback;
            _onNo = noCallback;
        }

        public virtual void ShowView()
        {
            base.ShowView();
            _yesNoPresenter = _presenter as YesNoPopupPresenter;
            if(_yesNoPresenter != null)
            {
                _yesNoPresenter.Title = _title;
                _yesNoPresenter.Message = _message;
                _yesNoPresenter.YesText = LocalizationManager.Instance.GetLocalizedString(LocalizationKeys.UI_YES_KEY);
                _yesNoPresenter.NoText = LocalizationManager.Instance.GetLocalizedString(LocalizationKeys.UI_NO_KEY);
                _yesNoPresenter.OnYes =  _onYes;
                _yesNoPresenter.OnNo = _onNo;
            }
            else
            {
                Debug.Assert(false);
                base.HideView();
            }
        }
    }
}