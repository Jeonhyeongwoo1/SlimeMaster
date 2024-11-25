using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Popup;
using UnityEngine;
using UnityEngine.Serialization;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_AchievementPopup : BasePopup
    {
        public Transform AchievementScrollObject => _achievementScrollObject;
        
        [SerializeField] private Transform _achievementScrollObject;
    }
}