using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace SlimeMaster.InGame.Manager
{
    public class ResourcesManager
    {
        private readonly Dictionary<string, Object> _resourceDict = new Dictionary<string, Object>();
        private readonly string _defineSprite = ".sprite";

        //UI - 스프라이트 위주
        public T Load<T>(string key) where T : Object
        {
            Object resource = null;
            if (_resourceDict.TryGetValue(key, out resource))
            {
                return (T)resource;
            }

            if (typeof(T) == typeof(Sprite))
            {
                if (!key.Contains(_defineSprite))
                {
                    key += _defineSprite;
                }
                
                if (_resourceDict.TryGetValue(key, out resource))
                {
                    return (T)resource;
                }
            }

            Debug.LogError($"Failed get resource {key}");
            return null;
        }

        //게임오브젝트를 위한 리소스 로드
        public GameObject Instantiate(string key, bool isPooling = true)
        {
            GameObject prefab = Load<GameObject>(key);

            if (isPooling)
            {
                var obj = GameManager.I.Pool.GetObject(prefab.name, prefab);
                obj.name = prefab.name;
                return obj;
            }

            var newGameObject = Object.Instantiate(prefab);
            return newGameObject;
        }

        public async UniTask LoadResourceAsync<T>(string key, Action<float> callback) where T : Object
        {
            var tcs = new UniTaskCompletionSource();
            AsyncOperationHandle<IList<IResourceLocation>> op = Addressables.LoadResourceLocationsAsync(key, typeof(T));
            int resultCount = 0;
            int currentLoadedCount = 0;
            op.Completed += (o) =>
            {
                IList<IResourceLocation> result = o.Result;
                resultCount = result.Count;
                foreach (IResourceLocation location in result)
                {
                    string key = location.PrimaryKey;
                    if (key.Contains(_defineSprite))
                    {
                        LoadAssetAsync<Sprite>(key, ()=>
                        {
                            currentLoadedCount++;
                            callback.Invoke((float)currentLoadedCount / resultCount);
                        });
                    }
                    else
                    {
                        LoadAssetAsync<T>(key, ()=>
                        {
                            currentLoadedCount++;
                            callback.Invoke((float)currentLoadedCount / resultCount);
                        });
                    }
                }

                if (op.IsDone)
                {
                    tcs.TrySetResult();
                }
            };

            await tcs.Task;
            await UniTask.WaitUntil(() => resultCount == currentLoadedCount);
            Debug.Log($"Done {resultCount} / {currentLoadedCount}");
        }

        private async UniTask LoadAssetAsync<T>(string key, Action callback) where T : Object
        {
            string newKey = null;
            if (key.Contains(_defineSprite))
            {
                // Debug.Log("oldKey :" + key);
                // key = $"{key}[{key.Replace(_defineSprite, "")}]";
                // Debug.Log("newKey :" + key);
            }

            AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(key);
            op.Completed += (handle) =>
            {
                var result = handle.Result;

                //캐쉬를 확인
                if (_resourceDict.TryGetValue(key, out var value))
                {
                    callback?.Invoke();
                    return;
                }

                _resourceDict.Add(key, result);
                callback?.Invoke();
            };
        }
    }
}

















