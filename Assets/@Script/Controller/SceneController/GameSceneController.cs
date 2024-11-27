using System;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.Controller
{
    public class GameSceneController : MonoBehaviour
    {
        private void Start()
        {
            
        }

        private void OnApplicationQuit()
        {
            Manager.I.SaveGameContinueData();
        }

        private void OnDisable()
        {
            Test_SaveGameContinueData();
            Manager.I.SaveGameContinueData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnDisable();
            }
        }

        public void Test_SaveGameContinueData()
        {
        }
    }
}