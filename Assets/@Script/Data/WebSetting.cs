namespace SlimeMaster.Data
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    namespace Clicker.ConfigData
    {
        [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WebSettings", order = 1)]
        public class WebSettings : ScriptableObject
        {
            public string ip;
            public string port;
        }
    }
}