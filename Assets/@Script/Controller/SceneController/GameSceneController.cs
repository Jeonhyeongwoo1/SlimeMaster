using System;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.Controller
{
    public class GameSceneController : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            Manager.I.SaveGameContinueData();
        }

        private void OnDisable()
        {
            Manager.I.SaveGameContinueData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnDisable();
            }
        }
    }
}