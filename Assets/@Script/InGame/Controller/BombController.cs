using System;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
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