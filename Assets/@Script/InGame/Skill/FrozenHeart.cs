using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class FrozenHeart : RepeatSkill
    {
        private List<IGeneratable> _list = new();
        
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            _list.ForEach(v=> v.Sleep());
        }

        protected override async UniTask UseSkill()
        {
            _list.Clear();
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = (360f / projectileCount) * i * Mathf.Deg2Rad; // 각도 분배
                float x = Mathf.Cos(angle) * _skillData.ProjRange;
                float y = Mathf.Sin(angle) * _skillData.ProjRange;
                Vector3 position = new Vector3(x, y);
                Debug.Log($"{_skillData.PrefabLabel}");
                GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                var shootable = prefab.GetComponent<IGeneratable>();
                shootable.OnHit = OnHit;
                shootable.Level = CurrentLevel;
                shootable.Generate(position, Vector3.zero, _skillData, _owner);
                _list.Add(shootable);
            }

            await UniTask.WaitForSeconds(_skillData.Duration, cancellationToken: _skillLogicCts.Token);
            
            _list.ForEach(v=> v.Sleep());
        }
    }
}