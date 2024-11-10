using SlimeMaster.InGame.Data;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class SoulController : DropItemController
    {
        
        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
        }

        public int GetSoulAmount()
        {
            return Const.STAGE_SOULCOUNT;
        }
    }
}