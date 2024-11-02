using System;
using System.Collections.Generic;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Data
{
    // [Serializable]
    // public struct PoolData
    // {
    //     public PoolType poolType;
    //     public GameObject prefab;
    //     public int poolCount;
    // }
    //
    // [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectPoolData", order = 1)]
    // public class ObjectPoolData : ScriptableObject
    // {
    //     public int PoolCount => _poolDataList.Count;
    //     
    //     [SerializeField] private List<PoolData> _poolDataList;
    //
    //     public GameObject GetPrefab(PoolType poolType)
    //     {
    //         foreach (PoolData poolData in _poolDataList)
    //         {
    //             if (poolData.poolType == poolType)
    //             {
    //                 return poolData.prefab;
    //             }
    //         }
    //
    //         Debug.LogError($"Failed get pool prefab {poolType}");
    //         return null;
    //     }
    //
    //     public PoolData GetPoolData(int index)
    //     {
    //         if (index >= PoolCount)
    //         {
    //             Debug.LogError($"index : {index} pool count is over");
    //             return default;
    //         }
    //         
    //         try
    //         {
    //             PoolData poolData = _poolDataList[index];
    //             return poolData;
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.LogError($"Failed get prefab {e}");
    //             return default;
    //         }
    //     }
    // }
}