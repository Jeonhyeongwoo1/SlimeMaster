using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.View
{
    public enum GoodsType
    {
        Stamina,
        Dia,
        Gold
    }
    
    public class GoodsButton : MonoBehaviour
    {
        public GoodsType GoodsType => _goodsType;
        
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _goodsCountText;
        [SerializeField] private GoodsType _goodsType;

        private Action<GoodsType> _onClickGoodsAction;

        private void Awake()
        {
            _button.onClick.AddListener(()=> _onClickGoodsAction?.Invoke(_goodsType));
        }

        public void Initialize(Action<GoodsType> onClickGoodsAction)
        {
            _onClickGoodsAction = onClickGoodsAction;
        }
        
        public void UpdateUI(string goodsCount)
        {
            _goodsCountText.text = goodsCount;
        }
    }
}