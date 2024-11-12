using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SlimeMaster.InGame.Controller;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class Map : MonoBehaviour
    {
        public GridController Grid => _grid;
        
        [SerializeField] private Transform _demarcation;
        
        private GridController _grid;

        private void Awake()
        {
            _grid = GetComponentInChildren<GridController>();
        }

        public void AddItemInGrid(Vector3 position, DropItemController dropItemController)
        {
            _grid.AddItem(position, dropItemController);
        }

        public void DoChangeMap(int lastWaveIndex, int currentIndex)
        {
            _demarcation.DOKill();

            Vector3 originScale = Vector3.one * 20;
            Vector3 scale = originScale * (lastWaveIndex - currentIndex + 1) * 0.1f;
            _demarcation.DOScale(scale, 3);
        }
    }
}