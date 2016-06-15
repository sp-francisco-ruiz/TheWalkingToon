using UnityEngine;
using Game.Presenters.Navigation;
using Game.Managers;

namespace Game.Controllers.Navigation
{
    public class FloorClickedEvent
    {
        public Vector3 clickedPosition;
        public FloorClickedEvent(Vector3 position)
        {
            clickedPosition = position;
        }
    }

    public class FloorController
    {
        GameObject _floorObject;

        public FloorController()
        {

        }

        public void ShowView()
        {
            _floorObject = GameObject.Find("LevelObjects/WhiteBoxFloorPrefab");
            EventDispatcher.Instance.AddListener<ClickedAsRightButtonEvent>(OnFloorClicked);
        }

        public void HideView()
        {
            EventDispatcher.Instance.RemoveListener<ClickedAsRightButtonEvent>(OnFloorClicked);
        }

        void OnFloorClicked(ClickedAsRightButtonEvent e)
        {
            if(e.TouchedObject == _floorObject)
            {
                Vector3 touchedPosition = e.TouchedPosition;
                EventDispatcher.Instance.Raise(new FloorClickedEvent(touchedPosition));
            }
        }
    }
}
