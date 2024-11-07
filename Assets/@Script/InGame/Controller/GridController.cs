using System.Collections.Generic;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class Cell
    {
        public HashSet<DropItemController> _dropItemList = new();
    }
    
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Grid _grid;

        private Dictionary<Vector3Int, Cell> _cellDict = new();
        private List<DropItemController> _cachedDropItemControllerList = new();
        
        public void AddItem(Vector3 position, DropItemController dropItemController)
        {
            Vector3Int pos = _grid.WorldToCell(position);
            if (!_cellDict.TryGetValue(pos, out Cell itemCell))
            {
                itemCell = new();
                itemCell._dropItemList.Add(dropItemController);
                _cellDict.Add(pos, itemCell);
                return;
            }

            itemCell._dropItemList.Add(dropItemController);
            _cellDict[pos] = itemCell;
        }

        public void RemoveItem(DropItemController item)
        {
            foreach (var keyValuePair in _cellDict)
            {
                if (keyValuePair.Value._dropItemList.Contains(item))
                {
                    keyValuePair.Value._dropItemList.Remove(item);
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
                        _cachedDropItemControllerList.AddRange(cell._dropItemList);
                    }
                }
            }

            return _cachedDropItemControllerList;
        }
    }
}
