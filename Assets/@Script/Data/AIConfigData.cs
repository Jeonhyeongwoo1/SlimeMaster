using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SlimeMaster.Data
{
    public enum AIDirectionType
    {
        None,
        Wander,
        ToMonster,
        ToSoul,
        ToGem,
        ToMapBoundary,
    }

    public enum AIBehaviourType
    {
        None,
        Move
    }
    
    [Serializable]
    public class AIBehaviourData
    {
        public List<AIDirectionTypeData> aiDirectionTypeDataList;
        public AIBehaviourType aiBehaviourType;
        public float duration;
    }

    [Serializable]
    public class AIDirectionTypeData
    {
        public AIDirectionType aiDirectionType;
        public float weight;
        public float radius;
        public bool isOpposite;
        public float duration; //얼마나 자주 호출되는가
        public Color gizmoColor;
    }
    
    [CreateAssetMenu(fileName = "AIConfigData", menuName = "ScriptableObjects/AIConfigData", order = 1)]
    public class AIConfigData : ScriptableObject
    {
        public List<AIBehaviourData> AIBehaviourDataList => _aiBehaviourDataList;
        
        [SerializeField] private List<AIBehaviourData> _aiBehaviourDataList;
    }
}