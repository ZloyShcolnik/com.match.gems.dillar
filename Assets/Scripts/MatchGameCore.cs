using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MatchGameCore
{
    private static MatchGameSetup setup;
    private static MonoBehaviour gameObject;

    private static float startX;
    private static float startY;

    private static int[,] Items;

    public static Action<Vector2> OnCellSpawned { get; set; }
    public static Action<Vector2, Vector2, Vector2Int, int> OnItemSpawned { get; set; }
    public static Action<List<Vector2Int>> OnFindMatches { get; set; }
    public static Action<Vector2Int, Vector2Int, Vector2> OnItemMoveDown { get; set; }

    public static void MatchGameInit(MatchGameSetup _setup, MonoBehaviour _gameObject, Action onGameBuildFinish = null)
    {
        setup = _setup;
        gameObject = _gameObject;

        startX = -setup.gridWidth * setup.tileSizeX / 2 + setup.tileSizeX / 2;
        startY = -setup.gridHeight * setup.tileSizeY / 2 + setup.tileSizeY / 2;

        Items = new int[setup.gridWidth, setup.gridHeight];

        BuildGame(onGameBuildFinish);
    }

    private static void BuildGame(Action onGameBuildFinish = null)
    {
        gameObject.StartCoroutine(Build(onGameBuildFinish));
    }

    private static float GetUpperCellY()
    {
        return Math.Abs(startY) + (setup.gridHeight + 1) * setup.tileSizeY / 2;
    }

    private static Vector2 GetNewCellPosition(Vector2Int index)
    {
        return GetPosition(index.x, index.y) + Vector2.up * GetUpperCellY();
    }

    private static IEnumerator Build(Action OnGameBuildFinish)
    {
        if (setup.IsBuildCells)
        {
            BuildCellGrid();
        }

        BuildItemGrid();
        OnGameBuildFinish?.Invoke();
        yield return null;

        while (true)
        {
            var checkedCells = new bool[setup.gridWidth, setup.gridHeight];
            for (int x = 0; x < setup.gridWidth; x++)
            {
                for (int y = 0; y < setup.gridHeight; y++)
                {
                    if (checkedCells[x, y] || Items[x, y] < 1)
                    {
                        continue;
                    }

                    var matches = new List<Vector2Int>();
                    CheckAdjacentBlocks(x, y, matches, checkedCells);
                    if (matches.Count >= setup.minSequence)
                    {
                        while (!ItemsIsStopped())
                        {
                            yield return null;
                        }

                        ClearItemArray(matches);
                        OnFindMatches?.Invoke(matches);
                    }
                }
            }

            for (int x = 0; x < setup.gridWidth; x++)
            {
                for (int y = 0; y < setup.gridHeight; y++)
                {
                    MoveCellsDown(x, y);
                }
            }

            for (int x = 0; x < setup.gridWidth; x++)
            {
                var _count = 0;
                for (int y = 0; y < setup.gridHeight; y++)
                {
                    FillEmptyItems(ref _count, x, y);
                }
            }

            yield return null;
        }
    }

    private static bool ItemsIsStopped()
    {
        return UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IMatchGameItem>().All(item => !item.IsMove);
    }

    private static Vector2 GetPosition(int x, int y)
    {
        return new Vector2(startX + x * setup.tileSizeX, startY + y * setup.tileSizeY);
    }

    private static void ClearItemArray(List<Vector2Int> matches)
    {
        foreach (Vector2Int item in matches)
        {
            Items[item.x, item.y] = 0;
        }
    }

    private static void MoveCellsDown(int x, int y)
    {
        if (Items[x, y] > 0 || y >= setup.gridHeight - 1)
        {
            return;
        }

        MoveCellsDown(x, y + 1);

        var item_index = new Vector2Int(x, y + 1);
        var item_next_index = new Vector2Int(x, y); ;
        var next_position = GetPosition(x, y);
        OnItemMoveDown?.Invoke(item_index, item_next_index, next_position);

        Items[x, y] = Items[x, y + 1];
        Items[x, y + 1] = 0;
    }

    private static void FillEmptyItems(ref int _count, int x, int y)
    {
        if (Items[x, y] > 0 || y > setup.gridHeight - 1)
        {
            _count++;
            return;
        }

        for (int i = 0; i < setup.gridHeight - _count; i++)
        {
            var _index = new Vector2Int(x, y + i);

            var _targetPosition = GetPosition(x, _index.y);
            var v2IntStart = new Vector2Int(_index.x, i);
            var _startPosition = GetNewCellPosition(v2IntStart);

            var _id = Random.Range(1, setup.count + 1);

            OnItemSpawned?.Invoke(_targetPosition, _startPosition, _index, _id);
            Items[x, y + i] = _id;
        }
    }

    private static void CheckAdjacentBlocks(int x, int y, List<Vector2Int> matches, bool[,] checkedCells)
    {
        if (matches.Contains(new Vector2Int(x, y)))
        {
            return;
        }

        matches.Add(new Vector2Int(x, y));
        checkedCells[x, y] = true;

        if (x - 1 >= 0 && Items[x - 1, y] == Items[x, y])
        {
            CheckAdjacentBlocks(x - 1, y, matches, checkedCells);
        }

        if (x + 1 < Items.GetLength(0) && Items[x + 1, y] == Items[x, y])
        {
            CheckAdjacentBlocks(x + 1, y, matches, checkedCells);
        }

        if (y + 1 < Items.GetLength(1) && Items[x, y + 1] == Items[x, y])
        {
            CheckAdjacentBlocks(x, y + 1, matches, checkedCells);
        }

        if (y - 1 >= 0 && Items[x, y - 1] == Items[x, y])
        {
            CheckAdjacentBlocks(x, y - 1, matches, checkedCells);
        }
    }

    private static void BuildCellGrid()
    {
        for (int y = 0; y < setup.gridHeight; y++)
        {
            for (int x = 0; x < setup.gridWidth; x++)
            {
                var _position = GetPosition(x, y);
                OnCellSpawned?.Invoke(_position);
            }
        }
    }

    private static void BuildItemGrid()
    {
        for (int y = 0; y < setup.gridHeight; y++)
        {
            for (int x = 0; x < setup.gridWidth; x++)
            {
                var _index = new Vector2Int(x, y);

                var _targetPosition = GetPosition(x, y);
                var _startPosition = GetNewCellPosition(_index);

                var _id = Random.Range(1, setup.count + 1);

                OnItemSpawned?.Invoke(_targetPosition, _startPosition, _index, _id);
                Items[x, y] = _id;
            }
        }
    }

    [Serializable]
    public struct MatchGameSetup
    {
        public int count;

        public int gridWidth;
        public int gridHeight;

        public float tileSizeX;
        public float tileSizeY;
        public int minSequence;

        public bool IsBuildCells;
    }

    public static IMatchGameItem FindItemByIndex(this List<IMatchGameItem> iitems, Vector2Int index)
    {
        return iitems.FirstOrDefault(item => item.Index == index);
    }

    public static void ChangeWith(this IMatchGameItem iitem1, IMatchGameItem iitem2)
    {
        (iitem2.TargetPosition, iitem1.TargetPosition) = (iitem1.TargetPosition, iitem2.TargetPosition);
        (iitem2.Index, iitem1.Index) = (iitem1.Index, iitem2.Index);

        var temp = Items[iitem1.Index.x, iitem1.Index.y];
        Items[iitem1.Index.x, iitem1.Index.y] = Items[iitem2.Index.x, iitem2.Index.y];
        Items[iitem2.Index.x, iitem2.Index.y] = temp;
    }
}

public interface IMatchGameItem
{
    public int ID { get; set; }
    public Vector2Int Index { get; set; }
    public Vector2 StartPosition { get; set; }
    public Vector2 TargetPosition { get; set; }
    public bool IsMove { get; set; }
    public GameObject GameObject { get; }
}
