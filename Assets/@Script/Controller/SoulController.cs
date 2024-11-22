using SlimeMaster.Data;
using SlimeMaster.Enum;
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