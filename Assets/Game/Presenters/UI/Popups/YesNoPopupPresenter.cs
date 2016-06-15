using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.Presenters.UI.Popups
{
    public class YesNoPopupPresenter : PopupPresenter 
    {

        [SerializeField] Text TitleText;
        [SerializeField] Text MessageText;
        [SerializeField] ButtonPresenter YesButton;
        [SerializeField] ButtonPresenter NoButton;

        public Action OnYes;
        public Action OnNo;

        public string Title
        {
            get
            {
                return TitleText.text;
            }

            set
            {
                TitleText.text = value;
            }
        }

        public string Message
        {
            get
            {
                return MessageText.text;
            }

            set
            {
                MessageText.text = value;
            }
        }

        public string YesText
        {
            get
            {
                return YesButton.Text;
            }
            set
            {
                YesButton.Text = value;
            }
        }

        public string NoText
        {
            get
            {
                return NoButton.Text;
            }
            set
            {
                NoButton.Text = value;
            }
        }

        void NoClicked()
        {
            if(OnNo != null)
            {
                OnNo();
            }
        }

        void YesClicked()
        {
            if(OnYes != null)
            {
                OnYes();
            }
        }

        override protected void UnityAwake() 
        {
            base.OnBackgroundClick = NoClicked;

            if(YesButton != null)
                YesButton.OnClick = YesClicked;

            if(NoButton != null)
                NoButton.OnClick = NoClicked;
        }
    }
}