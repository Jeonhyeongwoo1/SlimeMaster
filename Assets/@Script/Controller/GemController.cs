using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class GemController : DropItemController
    {
        public GemType GemType => _gemType;
        private GemType _gemType;

        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
            
            dropableItemType = DropableItemType.Gem;
        }

        public void SetGemInfo(GemType gemType, Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            _gemType = gemType;
        }
        
        public int GetExp()
        {
            if (_gemType == GemType.None)
            {
                Debug.LogError("gemType is none");
                return 0;
            }
            
            return _gemType switch
            {
                GemType.SmallGem => Const.SMALL_EXP_AMOUNT,
                GemType.GreenGem => Const.GREEN_EXP_AMOUNT,
                GemType.BlueGem => Const.BLUE_EXP_AMOUNT,
                GemType.YellowGem => Const.YELLOW_EXP_AMOUNT,
            };
        }
    }
}