using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Data;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class IcicleArrowBehaviour : Projectile
    {
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            gameObject.SetActive(true);
            wantToSleepInTriggerEnter = true;

            UpdateVelocity(direction, skillData.ProjSpeed);
            Invoke(nameof(Sleep), 3);
        }

        private void UpdateVelocity(Vector3 direction, float speed)
        {
            _rigidbody.velocity = direction * speed;
        }
    }
}