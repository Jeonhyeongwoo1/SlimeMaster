using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.InGame.View
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private PlayerController _player;
        
        private void OnEnable()
        {
            _player.onHitReceived += OnHitReceived;
        }

        private void OnHitReceived(int currentHp, int maxHp)
        {
            float ratio = (float)currentHp / maxHp;
            _slider.value = ratio;
        }

        private void OnDisable()
        {
            
        }
    }
}