using Cysharp.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using Script.Server.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.Model;

namespace SlimeMaster.Firebase
{
    public class FirebaseController
    {
        public string UserId => _uid;
        
        private string _uid;
        private readonly string _key = "firebase_uid";

        public async UniTask<bool> TrySignInAnonymously()
        {
            string uid = PlayerPrefs.GetString(_key);
            var response =
                await Managers.Manager.I.Web.SendRequest<DBAuthResponseBase>("/api/auth/login",
                    new DBAuthRequest() { uid = uid });
            if (response.responseCode == ServerErrorCode.Success)
            {
                //내부 저장
                _uid = response.uid;
                PlayerPrefs.SetString(_key, response.uid);
                PlayerPrefs.Save();
                ModelFactory.CreateOrGetModel<UserModel>().Token = response.token;
                return true;
            }

            return false;
        }
    }
}