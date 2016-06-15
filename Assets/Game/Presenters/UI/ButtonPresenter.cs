using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.Presenters.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonPresenter : BasePresenter 
    {
        [SerializeField] Text ButtonText;
        [SerializeField] Button Button;

        public string Text
        {
            get
            {
                return ButtonText.text;
            }

            set
            {
                ButtonText.text = value;
            }
        }

        public Action OnClick;

        override protected void UnityAwake()
        {
            Button.onClick.AddListener(Onclicked);
        }

        void Onclicked()
        {
            if(OnClick != null)
            {
                OnClick();
            }
        }
    }
}