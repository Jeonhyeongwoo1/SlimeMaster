using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class MagnetController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
            dropableItemType = DropableItemType.Magnet;
        }
    }
}