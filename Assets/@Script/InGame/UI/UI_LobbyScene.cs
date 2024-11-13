using System;
using SlimeMaster.InGame.Interface;
using SlimeMaster.View;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.InGame.UI
{
    public class UI_LobbyScene : BaseUI, IView
    {
        [SerializeField] private UI_UserInfoItem _userInfoItem;

        public Action<GoodsType> onGoodsClickAction; 
        
        public override void Initialize()
        {
            base.Initialize();
            
            _userInfoItem.Initialize(onGoodsClickAction);
        }

        public void UpdateUserGoodsInfo(GoodsType goodsType, string value)
        {
            _userInfoItem.UpdateUserGoodsInfo(goodsType, value);
        }
        
    }
}