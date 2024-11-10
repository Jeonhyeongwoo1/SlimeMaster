using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class SkillRange : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _lineSprite;
        [SerializeField] private ParticleSystem _mainCircleParticle;
        [SerializeField] private ParticleSystem _subCircleParticle;

        public void SetLineSizeAndRotation(float distance, Vector3 direction)
        {
            _lineSprite.size = new Vector2(1.3f, distance);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            _lineSprite.transform.rotation = Quaternion.Euler(0, 0, angle);
            ActiveSkill(true);
        }

        private void ActiveSkill(bool isLineEffect)
        {
            _lineSprite.gameObject.SetActive(isLineEffect);
            _mainCircleParticle.gameObject.SetActive(!isLineEffect);
            _subCircleParticle.gameObject.SetActive(!isLineEffect);
        }

        public float SetCircle(float radius)
        {
            _mainCircleParticle.Play();
            var main = _mainCircleParticle.main;
            var subMain = _subCircleParticle.main;
            main.startSize = radius;
            subMain.startSize = radius;

            ActiveSkill(false);
            return main.duration;
        }
    }
}