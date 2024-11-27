using System.Collections.Generic;
using SlimeMaster.Enum;
using UnityEngine;

namespace SlimeMaster.Managers
{
    public delegate void OnEvent(object value);
    public class EventManager
    {
        private Dictionary<GameEventType, OnEvent> _eventDict = new();

        public void Raise(GameEventType gameEventType, object value = null)
        {
            if (!_eventDict.ContainsKey(gameEventType))
            {
                Debug.LogWarning($"There isn't subscriber {gameEventType}");
                return;
            }
            
            _eventDict[gameEventType]?.Invoke(value);
        }
        
        public void AddEvent(GameEventType gameEventType, OnEvent onEvent)
        {
            OnEvent @event = null;
            if (!_eventDict.TryGetValue(gameEventType, out @event))
            {
                @event = onEvent;
                _eventDict[gameEventType] = @event;
            }
            else
            {
                @event += onEvent;
                _eventDict[gameEventType] = @event;
            }
        }

        public void RemoveEvent(GameEventType gameEventType, OnEvent onEvent)
        {
            OnEvent @event = null;
            if (!_eventDict.TryGetValue(gameEventType, out @event))
            {
                return;
            }

            @event -= onEvent;
            _eventDict[gameEventType] = @event;
        }

        public void ClearEventDict()
        {
            _eventDict.Clear();
        }
    }
}