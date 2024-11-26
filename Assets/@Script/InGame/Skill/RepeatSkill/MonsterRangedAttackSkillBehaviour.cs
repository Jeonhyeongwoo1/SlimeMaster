using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Enum;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class MonsterRangedAttackSkillBehaviour : Projectile
    {
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            transform.localScale = skillData.ScaleMultiplier * Vector3.one;
            gameObject.SetActive(true);
            wantToSleepInTriggerEnter = true;
            _rigidbody.velocity = direction * skillData.ProjSpeed;
            
            GameManager.I.Audio.Play(Sound.Effect, "MonsterProjectile_Start");
            CancelInvoke();
            Invoke(nameof(Release), 10);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tag.Player))
            {
                OnHit?.Invoke(other, this);
                if (wantToSleepInTriggerEnter)
                {
                    Release();
                }
            }
        }
    }
}