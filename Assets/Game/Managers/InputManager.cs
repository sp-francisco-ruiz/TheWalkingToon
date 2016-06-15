using UnityEngine;

namespace Game.Managers
{
    public enum MouseButton
    {
        Left,
        Right,
        Center,
        Wheel,
        None
    }

    public enum TouchType
    {
        Up,          //down on object
        Down,        //up on object
        Clicked,     //up and down on the same object
        LongClicked, //down for a long time on an object
        Moved,       //down on other object and moved over another object
        Dragged,     //down on an object and moved
        HoverIn,     //being up and moved over an object
        HoverOut,     //being up and moved over an object
        HoverMoved,     //being up and moved over an object
    }

    public class InputEvent
    {
    }

    public class ClickedAsLeftButtonEvent : InputEvent
    {
        public readonly GameObject TouchedObject;
        public readonly Vector3 TouchedPosition = Vector3.zero;
        public ClickedAsLeftButtonEvent(GameObject touchedObject, Vector3 touchedPos)
        {
            TouchedObject = touchedObject;
            TouchedPosition = touchedPos;
        }
    }

    public class ClickedAsRightButtonEvent :  InputEvent
    {
        public readonly GameObject TouchedObject;
        public readonly Vector3 TouchedPosition = Vector3.zero;
        public ClickedAsRightButtonEvent(GameObject touchedObject, Vector3 touchedPos)
        {
            TouchedObject = touchedObject;
            TouchedPosition = touchedPos;
        }
    }

    public class MouseHoverInEvent :  InputEvent
    {
        public readonly GameObject HoveredObject;
        public MouseHoverInEvent(GameObject hoveredObject)
        {
            HoveredObject = hoveredObject;
        }
    }

    public class MouseHoverOutEvent :  InputEvent
    {
        public readonly GameObject HoveredObject;
        public MouseHoverOutEvent(GameObject hoveredObject)
        {
            HoveredObject = hoveredObject;
        }
    }

    public class InputManager
    {
        public const int TouchableLayerMask = 1 << 8;

        static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new InputManager();
                }
                return _instance;
            }
        }

        public bool Paused;

        GameObject _lastTouchedObject;
        GameObject _lastHoverObject;
        Vector3 _lastMousePosition = Vector3.zero;
        MouseButton _currentMouseButtonDown;

        public InputManager()
        {
            //GameManager.Instance.RegisterForUpdate(Update);
        }

        public void Update()
        {
            if(Paused)
                return;

            var cameraController = CameraManager.Instance.GetActiveCamera();

            if(cameraController == null) 
                return;
            
            Vector3 mousePos = Input.mousePosition;
            if(Input.GetMouseButtonDown(0))
            {
                var hit = CastARayFromCamera(cameraController.Camera);
                var touchedObject = hit.transform != null ? hit.transform.gameObject : null;
                _lastTouchedObject = touchedObject;
                _currentMouseButtonDown = MouseButton.Left;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                var hit = CastARayFromCamera(cameraController.Camera);
                var touchedObject = hit.transform != null ? hit.transform.gameObject : null;
                if(touchedObject != null && touchedObject == _lastTouchedObject && _currentMouseButtonDown == MouseButton.Left)
                {
                    EventDispatcher.Instance.Raise(new ClickedAsLeftButtonEvent(touchedObject, hit.point));
                }
                _currentMouseButtonDown = MouseButton.None;
                _lastTouchedObject = null;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                var hit = CastARayFromCamera(cameraController.Camera);
                var touchedObject = hit.transform != null ? hit.transform.gameObject : null;
                _lastTouchedObject = touchedObject;
                _currentMouseButtonDown = MouseButton.Right;
            }
            else if(Input.GetMouseButtonUp(1))
            {
                var hit = CastARayFromCamera(cameraController.Camera);
                var touchedObject = hit.transform != null ? hit.transform.gameObject : null;
                if(touchedObject != null && touchedObject == _lastTouchedObject && _currentMouseButtonDown == MouseButton.Right)
                {
                    EventDispatcher.Instance.Raise(new ClickedAsRightButtonEvent(touchedObject, hit.point));
                }
                _currentMouseButtonDown = MouseButton.None;
                _lastTouchedObject = null;
            }
            else if((mousePos - _lastMousePosition).sqrMagnitude > 0.1f)
            {
                var hit = CastARayFromCamera(cameraController.Camera);
                var hoverObject = hit.transform != null ? hit.transform.gameObject : null;
                if(hoverObject != null)
                {
                    if(_currentMouseButtonDown == MouseButton.None)
                    {
                        if(_lastHoverObject != hoverObject)
                        {
                            EventDispatcher.Instance.Raise(new MouseHoverOutEvent(_lastHoverObject));
                            EventDispatcher.Instance.Raise(new MouseHoverInEvent(hoverObject));
                            _lastHoverObject = hoverObject;
                        }
                    }
                }
                
                if(_lastTouchedObject != hoverObject)
                {
                    _lastTouchedObject = null;
                }
            }
            _lastMousePosition = mousePos;
        }

        static RaycastHit CastARayFromCamera(Camera camera)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit, Mathf.Infinity, TouchableLayerMask);
            return hit;
        }
    }
}