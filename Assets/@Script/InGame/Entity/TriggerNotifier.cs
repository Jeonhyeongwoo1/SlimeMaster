using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class TriggerNotifier : MonoBehaviour
    {
        private Action<Collider2D> _onTriggerEnterAction;
        private Action<Collider2D> _onTriggerExitAction;
        
        public void AddEvent(Action<Collider2D> onTriggerEnterAction, Action<Collider2D> onTriggerExitAction)
        {
            _onTriggerEnterAction = onTriggerEnterAction;
            _onTriggerExitAction = onTriggerExitAction;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _onTriggerEnterAction?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _onTriggerExitAction?.Invoke(other);
        }
    }
}