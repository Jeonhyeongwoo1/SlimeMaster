using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Entity;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class WindCutter : RepeatSkill
    {
        private List<IGeneratable> _list = new List<IGeneratable>();
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            _list.ForEach(v=> v.Release());
        }

        protected override async UniTask UseSkill()
        {
            _list.Clear();
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = _skillData.AngleBetweenProj * (i - (_skillData.NumProjectiles - 1) / 2f);
                var direction = Quaternion.AngleAxis(angle, Vector3.forward) * _owner.GetDirection();
                GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                IGeneratable generatable = prefab.GetComponent<IGeneratable>();
                generatable.OnHit = OnHit;
                generatable.Generate(_owner.Position, direction, _skillData, _owner);
                _list.Add(generatable);
            }
            
            try
            {
                await UniTask.WaitForSeconds(_skillData.Duration, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
            }

            float duration = 0.5f;
            _list.ForEach(v=> (v as WindCutterBehaviour).OnReturnToOwner(_owner, duration));
            await UniTask.WaitForSeconds(duration);
        }
    }
}