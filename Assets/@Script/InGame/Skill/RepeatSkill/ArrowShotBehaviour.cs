using DG.Tweening;
using SlimeMaster.Data;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class ArrowShotBehaviour : Projectile
    {
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            transform.localScale = Vector3.zero;
            transform.DOKill();
            transform.DOScale(Vector3.one * skillData.ScaleMultiplier, 0.2f);
            gameObject.SetActive(true);
            _rigidbody.velocity = direction * skillData.ProjSpeed;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            Invoke(nameof(Release), 4);
        }

        public override void OnChangedSkillData(SkillData skillData)
        {
            transform.localScale = Vector3.one * skillData.ScaleMultiplier;
        }
    }
}