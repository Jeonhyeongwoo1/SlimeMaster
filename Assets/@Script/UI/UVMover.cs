using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.View
{
    public class UVMover : MonoBehaviour
    {
        [SerializeField] private RawImage _image;

        private float _speed = 1f;
        
        private void Update()
        {
            Rect rect = _image.uvRect;
            rect.x = Time.deltaTime * _speed;
            rect.y = Time.deltaTime * _speed;
            _image.uvRect = rect;
        }
    }
}