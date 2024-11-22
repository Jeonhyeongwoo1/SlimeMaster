using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Popup;
using UnityEngine;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_CheckOutPopup : BasePopup
    {
        public Transform CheckOutBoardObject => _checkOutBoardObject;
     
        [SerializeField] private Transform _checkOutBoardObject;
        
        
        
    }
}