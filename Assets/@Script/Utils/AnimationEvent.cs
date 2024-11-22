using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlimeMaster.Util
{
    public class AnimationEvent : MonoBehaviour
    {
        public UnityEvent _animationEvent;

        public void OnAnimationEvent()
        {
            _animationEvent.Invoke();
        }
    }
}