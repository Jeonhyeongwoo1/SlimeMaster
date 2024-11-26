using DG.Tweening;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Skill;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class FrozenHeartBehaviour : Projectile
    {
        [SerializeField] private GameObject[] _spinEffectObjectArray;
        [SerializeField] private GameObject[] _spinEffectFinalObjectArray;

        private CreatureController _owner;
        private Sequence _rotateSequence;
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            _owner = owner;
            transform.position = owner.Position;
            gameObject.SetActive(true);
            ActivateSpinObject(spawnPosition);
            
            float speed = skillData.RoatateSpeed * skillData.Duration;
            DoRotate(speed, skillData.Duration);
            GameManager.I.Audio.Play(Sound.Effect, "FrozenHeart_Start");
        }

        private void DoRotate(float speed, float duration)
        {
            if (_rotateSequence != null)
            {
                _rotateSequence.Kill();
            }
            
            _rotateSequence = DOTween.Sequence();
            Tween rotate = transform.DORotate(new Vector3(0, 0, speed), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);
            Tween rotate2 = transform.DORotate(new Vector3(0, 0, speed * 1), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);
            _rotateSequence.Append(rotate).Append(rotate2);            
        }

        private void ActivateSpinObject(Vector3 spawnPosition)
        {
            if (Level == Const.MAX_SKILL_Level)
            {
                foreach (GameObject go in _spinEffectObjectArray)
                {
                    go.SetActive(false);
                }

                int index = 0;
                foreach (GameObject go in _spinEffectFinalObjectArray)
                {
                    go.transform.localPosition = spawnPosition;
                    go.SetActive(index == Level - 1);
                    index++;
                }
            }
            else
            {
                foreach (GameObject go in _spinEffectFinalObjectArray)
                {
                    go.SetActive(false);
                }
                
                int index = 0;
                foreach (GameObject go in _spinEffectObjectArray)
                {
                    go.transform.localPosition = spawnPosition;
                    go.SetActive(index == Level);
                    index++;
                }
            }
        }
        
        private void Update()
        {
            var position = _owner.transform.position;
            transform.position = position;
        }

        public override void Release()
        {
            base.Release();
            if (_rotateSequence != null)
            {
                _rotateSequence.Kill();
            }
        }

        public override void OnChangedSkillData(SkillData skillData)
        {
            float speed = skillData.RoatateSpeed * skillData.Duration;
            DoRotate(speed, skillData.Duration);
        }
    }
}