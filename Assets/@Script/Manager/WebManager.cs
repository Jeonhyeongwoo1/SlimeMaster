using SlimeMaster.Data.Clicker.ConfigData;

using System;
using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SlimeMaster.Common;
using SlimeMaster.Factory;
using SlimeMaster.Model;
using UnityEngine;
using UnityEngine.Networking;

namespace Clicker.Manager
{
    public class WebManager
    {
        public string BaseUrl { get; set; }

        private WebSettings _webSettings;

        public void Initialize()
        {
            _webSettings = SlimeMaster.Managers.Manager.I.Resource.Load<WebSettings>(nameof(WebSettings));
            IPAddress ipv4 = Utils.GetIpv4Address(_webSettings.ip);
            if (ipv4 == null)
            {
                return;
            }

            BaseUrl = $"http://{ipv4}:{_webSettings.port}";
        }

        public async UniTask<T> SendRequest<T>(string url, object obj, string method = "POST")
        {
            return await SendRequestAsync<T>(url, obj, method);
        }

        private async UniTask<T> SendRequestAsync<T>(string url, object obj, string method)
        {
            if (string.IsNullOrEmpty(url))
            {
                Initialize();
            }

            string sendUrl = $"{BaseUrl}{url}";
            using (var uwr = new UnityWebRequest(sendUrl, method))
            {
                byte[] jsonBytes = null;
                if (obj != null)
                {
                    string json = JsonConvert.SerializeObject(obj);
                    jsonBytes = Encoding.UTF8.GetBytes(json);
                }

                string jwtToken = ModelFactory.CreateOrGetModel<UserModel>().Token;
                uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");
                uwr.SetRequestHeader("Authorization", $"Bearer {jwtToken}"); //Header에 반드시 포함되어야함. 검증!!
                Debug.Log($"SendUrl {sendUrl}");
                try
                {
                    await uwr.SendWebRequest().ToUniTask();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(SendRequestAsync)} / error : {e.Message}");
                    return default;
                }

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    string text = uwr.downloadHandler.text;
                    return JsonConvert.DeserializeObject<T>(text);
                }
            }

            Debug.LogError($"{nameof(SendRequestAsync)} / failed get res {url}");
            return default;
        }

    }
}    
    
