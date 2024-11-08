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

        public static bool TryGetComponentInParent<T>(GameObject gameObject, out T t) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out t))
            {
                return true;
            }

            var component = gameObject.GetComponentInParent<T>();
            if (component != null)
            {
                t = component;
                return true;
            }

            return false;
        }

        public static T AddOrGetComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }
        
        public static Vector3 GetPositionInDonut(Transform centerPosition, float minRange, float maxRange)
        {
            int angle = Random.Range(0, 360);
            float distX = Random.Range(minRange, maxRange);
            float distY = Random.Range(minRange, maxRange);
            float posX = Mathf.Cos(angle * Mathf.Deg2Rad) * distX;
            float posY = Mathf.Sign(angle * Mathf.Deg2Rad) * distY;
            Vector3 position = centerPosition.position + new Vector3(posX, posY);
            return position;
        }
    }
}
