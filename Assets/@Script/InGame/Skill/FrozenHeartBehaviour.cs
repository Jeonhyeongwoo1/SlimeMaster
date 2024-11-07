using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.InGame.Skill;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
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
            _rotateSequence = DOTween.Sequence();
            Tween rotate = transform.DORotate(new Vector3(0, 0, speed), skillData.Duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            Tween rotate2 = transform.DORotate(new Vector3(0, 0, speed * 1), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
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

        public override void Sleep()
        {
            base.Sleep();
            if (_rotateSequence != null)
            {
                _rotateSequence.Kill();
            }
        }
    }
}