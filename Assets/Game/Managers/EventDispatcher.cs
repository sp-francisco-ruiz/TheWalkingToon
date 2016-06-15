using System;
using System.Collections.Generic;

namespace Game.Managers
{
    public class EventDispatcher
    {
        static EventDispatcher _instance;
        public static EventDispatcher Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EventDispatcher();
                }
                return _instance;
            }
        }

        EventDispatcher()
        {
        }

        public delegate void EventDelegate<T>(T e) where T : class;

        readonly Dictionary<Type, List<Delegate>> _delegates = new Dictionary<Type, List<Delegate>>();

        public void AddListener<T>(EventDelegate<T> listener) where T : class
        {
            List<Delegate> d;
            if(!_delegates.TryGetValue(typeof(T), out d))
            {
                d = new List<Delegate>();
                _delegates[typeof(T)] = d;
            }
            if(!d.Contains(listener))
                d.Add(listener);
        }

        public bool RemoveListener<T>(EventDelegate<T> listener) where T : class
        {
            List<Delegate> d;
            if(_delegates.TryGetValue(typeof(T), out d))
            {
                d.Remove(listener);
                return true;
            }
            return false;
        }
        public void Raise<T>(T e) where T : class
        {
            if(e == null)
            {
                UnityEngine.Debug.LogError("Raised event with a null parameter");
            }

            List<Delegate> dlgList = GetDelegateListForEventType(e);

            for(int i = 0; i < dlgList.Count; ++i)
            {
                var callback = dlgList[i] as EventDelegate<T>;

                if(callback != null)
                {
                    try
                    {
                        callback(e);
                    }
                    catch(Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex);
                    }
                }
            }
        }

        public void UnregisterAll()
        {
            _delegates.Clear();
        }

        public void Raise<T>() where T : class, new()
        {
            Raise<T>(new T());
        }

        /// <summary>
        ///     Gets a COPY of the list of delegates registered for a given event type
        /// </summary>
        /// <remarks>
        ///     You need to create a copy of the delegates because TryGetValue returns a reference to the internal list
        ///     of delegates, so if an event listener modifies the delegate list while you are iterating it you'll run
        ///     into problems
        ///     This can happen, for example, if the event listener unregisters itself
        /// </remarks>
        List<Delegate> GetDelegateListForEventType<T>(T e)
        {
            List<Delegate> dlgList;
            if(_delegates.TryGetValue(typeof(T), out dlgList))
            {
                return new List<Delegate>(dlgList);
            }
            else
            {
                return new List<Delegate>();
            }
        }
    }
}