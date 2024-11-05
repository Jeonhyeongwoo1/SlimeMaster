using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class GemBehaviour : DropItemController
    {
        public GemType GemType => _gemType;
        
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private GemType _gemType;

        public void Initialize(Sprite sprite, GemType gemType, Vector3 spawnPosition)
        {
            Refresh(sprite, gemType);
            transform.position = spawnPosition;
            gameObject.SetActive(true);
        }
        
        public void Refresh(Sprite sprite, GemType gemType)
        {
            _gemType = gemType;
            _spriteRenderer.sprite = sprite;
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