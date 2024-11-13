using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class PotionController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);

            dropableItemType = DropableItemType.Potion;
        }
    }
}