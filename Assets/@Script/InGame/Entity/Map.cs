using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SlimeMaster.Controller;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class Map : MonoBehaviour
    {
        public GridController Grid => _grid;
        
        [SerializeField] private Transform _demarcation;
        
        private GridController _grid;
        private List<ObstacleController> _obstacleList;
        
        private void Awake()
        {
            _demarcation = GameObject.Find("Demarcation").transform;
            _grid = GetComponentInChildren<GridController>();
            _obstacleList = GetComponentsInChildren<ObstacleController>().ToList();
            
            foreach (var obstacleController in _obstacleList)
            {
                GameManager game = Managers.Manager.I.Game;
                game.AddCreatureInGrid(game.WorldToCell(obstacleController.Position), obstacleController);
            }
        }

        public void AddItemInGrid(Vector3 position, DropItemController dropItemController)
        {
            _grid.AddItem(position, dropItemController);
        }

        public void DoChangeMap(int lastWaveIndex, int currentIndex)
        {
            _demarcation.DOKill();

            Vector3 originScale = Vector3.one * 20;
            Vector3 scale = originScale * ((lastWaveIndex - currentIndex + 1) * 0.1f);
            _demarcation.DOScale(scale, 3);
        }
    }
}