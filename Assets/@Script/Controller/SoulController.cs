using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class SoulController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
            dropableItemType = DropableItemType.Soul;
        }

        public int GetSoulAmount()
        {
            return Const.STAGE_SOULCOUNT;
        }
    }
}