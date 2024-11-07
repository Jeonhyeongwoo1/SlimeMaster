using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace SlimeMaster.Common
{
    public static class Utils
    {
        public static List<T> Shuffle<T>(this List<T> shuffleList)
        {
            int n = shuffleList.Count;
            while (n > 1)
            {
                n--;
                int random = Random.Range(0, n + 1);
                (shuffleList[random], shuffleList[n]) = (shuffleList[n], shuffleList[random]);
            }
            
            return shuffleList;
        }
        
        public static void SafeCancelCancellationTokenSource(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        public static T AddOrGetComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }
    }
}
