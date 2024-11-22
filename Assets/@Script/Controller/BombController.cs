using SlimeMaster.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class BombController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);

            dropableItemType = DropableItemType.Bomb;
        }
    }
}