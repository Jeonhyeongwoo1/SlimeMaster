using System;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Skill;
using SlimeMaster.Manager;
using UnityEngine;

public interface IGeneratable
{
    Action<Collider2D, Projectile> OnHit { get; set; }
    void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner);
    int Level { get; set; }
    Projectile ProjectileMono { get; }
    void Release();
    void OnChangedSkillData(SkillData skillData);
    bool IsRelease { get; }
}

namespace SlimeMaster.InGame.Entity
{
    public class EnergyBoltBehaviour : Projectile
    {
        public int BounceCount { get; private set; }
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            UpdateVelocity(direction, skillData.ProjSpeed);
            GameManager.I.Audio.Play(Sound.Effect, "EnergyBolt_Start");
            Invoke(nameof(Release), 3);
        }

        public void Bounce(Vector3 direction, float speed)
        {
            BounceCount++;
            UpdateVelocity(direction, speed);
        }

        private void UpdateVelocity(Vector3 direction, float speed)
        {
            _rigidbody.velocity = direction * speed;
        }

        private void OnDisable()
        {
            BounceCount = 0;
        }
    }
}