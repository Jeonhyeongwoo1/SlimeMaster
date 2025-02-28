using System;
using System.Collections.Generic;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class Cell
    {
        public readonly HashSet<DropItemController> DropItemList = new();
    }
    
    public class GridController : MonoBehaviour
    {
        public Grid Grid => _grid;
        
        [SerializeField] private Grid _grid;

        private Dictionary<Vector3Int, Cell> _cellDict = new();
        private List<DropItemController> _cachedDropItemControllerList = new();

        private void Awake()
        {
            _grid.GetComponent<Grid>();
        }

        public void AddItem(Vector3 position, DropItemController dropItemController)
        {
            Vector3Int pos = _grid.WorldToCell(position);
            if (!_cellDict.TryGetValue(pos, out Cell itemCell))
            {
                itemCell = new();
                itemCell.DropItemList.Add(dropItemController);
                _cellDict.Add(pos, itemCell);
                return;
            }

            itemCell.DropItemList.Add(dropItemController);
            _cellDict[pos] = itemCell;
        }

        public void RemoveAllItem(DropableItemType dropableItemType)
        {
            foreach (var (key, value) in _cellDict)
            {
                value.DropItemList.RemoveWhere(v => v.DropableItemType == dropableItemType);
            }
        }

        public void RemoveItem(DropItemController item)
        {
            foreach (var keyValuePair in _cellDict)
            {
                if (keyValuePair.Value.DropItemList.Contains(item))
                {
                    keyValuePair.Value.DropItemList.Remove(item);
                }
            }
        }

        public List<DropItemController> GetDropItem(Vector3 position, float range = 3)
        {
            _cachedDropItemControllerList.Clear();
            Vector3Int left = _grid.WorldToCell(position + new Vector3(-range, 0));
            Vector3Int right = _grid.WorldToCell(position + new Vector3(range, 0));
            Vector3Int top = _grid.WorldToCell(position + new Vector3(0, range));
            Vector3Int bottom = _grid.WorldToCell(position + new Vector3(0, -range));

            for (int x = left.x; x <= right.x; x++)
            {
                for (int y = bottom.y; y <= top.y; y++)
                {
                    Vector3Int value = new Vector3Int(x, y, 0);
                    if (!_cellDict.ContainsKey(value))
                    {
                        continue;
                    }
                    if (_cellDict.TryGetValue(value, out var cell))
                    {
                        _cachedDropItemControllerList.AddRange(cell.DropItemList);
                    }
                }
            }

            return _cachedDropItemControllerList;
        }
    }
}
