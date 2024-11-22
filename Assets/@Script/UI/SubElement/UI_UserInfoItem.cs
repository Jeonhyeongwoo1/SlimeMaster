using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeMaster.View
{
    public class UI_UserInfoItem : MonoBehaviour
    {
        [SerializeField] private List<GoodsButton> _goodButtonList;

        public void Initialize(Action<GoodsType> onClickGoodsType)
        {
            _goodButtonList.ForEach(v =>
            {
                v.Initialize(onClickGoodsType);
            });
        }

        public void UpdateUserGoodsInfo(GoodsType goodsType, string value)
        {
            _goodButtonList.ForEach(v =>
            {
                if (v.GoodsType == goodsType)
                {
                    v.UpdateUI(value);
                }
            });
        }
    }
}