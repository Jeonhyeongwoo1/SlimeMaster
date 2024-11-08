using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class LifecycleTimer : MonoBehaviour
    {
        [SerializeField] private float _timer;
        [SerializeField] private bool _isPooling;
        
        private void Start()
        {
            Invoke(nameof(Destroy), _timer);
        }

        private void Destroy()
        {
            if (_isPooling)
            {
                GameManager.I.Pool.ReleaseObject(transform.name, gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}