using UnityEngine;
using System.Collections;

namespace Game.Presenters.UI
{
    public class BasePresenter : MonoBehaviour 
    {

    	protected virtual void UnityAwake()
        {

        }

        void Awake()
        {
            UnityAwake();
        }

        public virtual void Show()
        {
            
        }

        public virtual void Hide()
        {
            
        }
    }
}