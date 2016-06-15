using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.Presenters.UI.Popups
{
    public class PopupPresenter : BasePresenter 
    {
        [SerializeField] protected Image BackgroundImage;

        public Action OnBackgroundClick;

        public void OnBackgroundClicked()
        {
            if(OnBackgroundClick != null)
            {
                OnBackgroundClick();
            }
        }
    }
}