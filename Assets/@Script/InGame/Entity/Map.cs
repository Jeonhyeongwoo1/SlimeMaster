using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.Controller;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class Map : MonoBehaviour
    {
        public GridController Grid => _grid;
        
        private GridController _grid;

        private void Awake()
        {
            _grid = GetComponentInChildren<GridController>();
        }

        public void AddItemInGrid(Vector3 position, DropItemController dropItemController)
        {
            _grid.AddItem(position, dropItemController);
        }
        
    }
}