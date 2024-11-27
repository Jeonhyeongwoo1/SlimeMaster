using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Entity
{
    public class PlayerAI : MonoBehaviour
    {
        [SerializeField] private AIConfigData _aiConfigData;
        [SerializeField] private float _duration = 0.2f;
        
        private Collider2D[] _hitOverlapCircleCollider2DArray = new Collider2D[10];
        private RaycastHit2D[] _raycastHitArray = new RaycastHit2D[10];
        
        private PlayerMove _playerMove;
        private CancellationTokenSource _aiLogicCts;
        private CancellationTokenSource _aiDirectionLogicCts;
        private AIBehaviourData _selectedAIBehaviourData;
        private Vector3 _direction = Vector3.zero;
        
        public void Activate()
        {
            _playerMove = GetComponent<PlayerMove>();

            if (_aiLogicCts != null)
            {
                _aiLogicCts.Cancel();
            }
            
            Debug.Log("Activate");
            _aiLogicCts = new CancellationTokenSource();
            _aiDirectionLogicCts = new CancellationTokenSource();
            StartAILogicProcess().Forget();
        }

        private async UniTaskVoid StartAILogicProcess()
        {
            List<AIBehaviourData> aiBehaviourDataList = _aiConfigData.AIBehaviourDataList;
            ObjectManager objectManager = Managers.Manager.I.Object;
            while (true)
            {
                float duration = 1;
                int count = objectManager.ActivateMonsterCount;

                _direction = Vector3.zero;
                if (_aiDirectionLogicCts != null)
                {
                    _aiDirectionLogicCts.Cancel();
                    _aiDirectionLogicCts = new CancellationTokenSource();
                }
                
                if (count > 0)
                {
                    _selectedAIBehaviourData =
                        aiBehaviourDataList.Find(v => v.aiBehaviourType == AIBehaviourType.Move);

                    foreach (AIDirectionTypeData aiDirectionTypeData in _selectedAIBehaviourData.aiDirectionTypeDataList)
                    {
                        AIDirectionLogicProcess(aiDirectionTypeData).Forget();
                    }
                    
                    _duration = _selectedAIBehaviourData.duration;;
                }

                await UniTask.WaitForSeconds(_duration, cancellationToken: _aiLogicCts.Token);
            }
        }

        private async UniTaskVoid AIDirectionLogicProcess(AIDirectionTypeData aiDirectionTypeData)
        {
            while (_aiDirectionLogicCts != null && !_aiDirectionLogicCts.IsCancellationRequested)
            {
                switch (aiDirectionTypeData.aiDirectionType)
                {
                    case AIDirectionType.Wander:
                        _direction += GetWanderDirection();
                        break;
                    case AIDirectionType.ToMonster:
                        _direction += GetOverlapCircleDirection(aiDirectionTypeData.radius, aiDirectionTypeData.isOpposite, Layer.BossAndMonster,
                            aiDirectionTypeData.weight);
                        break;
                    case AIDirectionType.ToGem:
                        _direction += GetOverlapCircleDirection(aiDirectionTypeData.radius, aiDirectionTypeData.isOpposite, Layer.Gem,
                            aiDirectionTypeData.weight);
                        break;
                    case AIDirectionType.ToSoul:
                        _direction += GetOverlapCircleDirection(aiDirectionTypeData.radius, aiDirectionTypeData.isOpposite, Layer.Soul,
                            aiDirectionTypeData.weight);
                        break;
                    case AIDirectionType.ToMapBoundary:
                        _direction += GetCircleCastDirection(aiDirectionTypeData.radius, aiDirectionTypeData.isOpposite, Layer.SafeZone,
                            aiDirectionTypeData.weight);
                        break;
                }
                
                try
                {
                    await UniTask.WaitForSeconds(aiDirectionTypeData.duration,
                        cancellationToken: _aiDirectionLogicCts.Token);
                }
                catch (Exception e)
                {
                    
                }
            
                _direction.Normalize();
                _playerMove.SetDirection(_direction);
            }
        }

        private Vector3 GetWanderDirection()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        private Vector3 GetCircleCastDirection(float radius, bool isOpposite, int layer, float weight)
        {
            int count = Physics2D.CircleCastNonAlloc(transform.position, radius, _playerMove.Direction,
                _raycastHitArray, layer);
            Vector3 direction = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = _raycastHitArray[i];
                
                Vector3 dir = (transform.position - hit.transform.position).normalized;
                direction += dir;
            }
            
            return direction * ((isOpposite ? -1 : 1) * weight);
        }
        
        private Vector3 GetOverlapCircleDirection(float radius, bool isOpposite, int layer, float weight)
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _hitOverlapCircleCollider2DArray,
                layer);
            Vector3 direction = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                Collider2D col = _hitOverlapCircleCollider2DArray[i];
                if (col.transform == transform)
                {
                    continue;
                }
                
                Vector3 dir = (col.transform.position - transform.position).normalized;
                direction += dir;
            }

            return direction * ((isOpposite ? -1 : 1) * weight);
        }

        private void OnDrawGizmos()
        {
            if (_aiLogicCts == null)
            {
                return;
            }
            
            foreach (AIDirectionTypeData aiDirectionTypeData in _selectedAIBehaviourData.aiDirectionTypeDataList)
            {
                switch (aiDirectionTypeData.aiDirectionType)
                {
                    case AIDirectionType.Wander:
                        break;
                    case AIDirectionType.ToMonster:
                        Gizmos.color = aiDirectionTypeData.gizmoColor;
                        Gizmos.DrawWireSphere(transform.position, aiDirectionTypeData.radius);
                        break;
                    case AIDirectionType.ToSoul:
                        Gizmos.color = aiDirectionTypeData.gizmoColor;
                        Gizmos.DrawWireSphere(transform.position, aiDirectionTypeData.radius);
                        break;
                    case AIDirectionType.ToGem:
                        Gizmos.color = aiDirectionTypeData.gizmoColor;
                        Gizmos.DrawWireSphere(transform.position, aiDirectionTypeData.radius);
                        break;
                    case AIDirectionType.ToMapBoundary:
                        float radius = aiDirectionTypeData.radius;
                        Gizmos.color = aiDirectionTypeData.gizmoColor;
                        // 1. CircleCast 시작 지점 (원)
                        Gizmos.DrawWireSphere(transform.position, radius);

                        // 2. CircleCast 이동 방향 (사각형으로 범위 표시)
                        Vector2 endPosition = (Vector2)transform.position +((Vector2) _playerMove.Direction * aiDirectionTypeData.radius);

                        // 2-1. 원형 범위의 시작과 끝 위치를 연결하는 사각형
                        Gizmos.DrawLine(transform.position, endPosition);

                        // 3. CircleCast 끝 지점 (원)
                        Gizmos.DrawWireSphere(endPosition, radius);
                        break;
                }
            }
        }
    }
}