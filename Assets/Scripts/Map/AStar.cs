using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AStar : MonoBehaviour
{
    [Header("Map Generation")]
    [SerializeField] private int _mapWidth = 5;
    [SerializeField] private int _mapHeight = 5;
    [Range(35, 65)]
    [SerializeField] private int _obstaclePercentage;
    private Cell[,] _map;
    [SerializeField] private List<Sprite> _sprite;
    private GameObject _base;

    [Header("Pathfinding")]
    [SerializeField] private List<Cell> _openList = new();
    [SerializeField] private List<Cell> _closedList = new();
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _endPos;
    [SerializeField] private LineRenderer _lineRenderer;
    Camera _main;

    private bool _running = false;
    //private Vector2 offset;

    void Start()
    {
        _main = Camera.main;

        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GeneratePath();

        _main.transform.position = _startPos.position + new Vector3(0, 0, -10);
    }

    void GeneratePath()
    {
        if (_running)
            return;

        _running = true;

        Vector3 goal = Camera.main.ScreenToWorldPoint(Input.mousePosition + (Vector3.back * 10));

        goal.x = Mathf.Clamp(_startPos.position.x + (_startPos.position.x - Mathf.Round(goal.x)), 0, (_mapWidth - 1));
        goal.y = Mathf.Clamp(_startPos.position.y + (_startPos.position.y - Mathf.Round(goal.y)), 0, (_mapHeight - 1));
        goal.z = 0;

        _endPos.position = goal;

        if (!ValidateStartPos() || !ValidateEndPos())
        {
            Debug.Log("StartPos: " + ValidateStartPos());
            Debug.Log("EndPos: " + ValidateEndPos());
            _running = false;
            return;
        }


        _openList.Clear();
        _openList = new();

        _closedList.Clear();
        _closedList = new();

        Cell endCell = _map[Mathf.RoundToInt(_endPos.position.x), Mathf.RoundToInt(_endPos.position.y)];
        Cell startCell = _map[Mathf.RoundToInt(_startPos.position.x), Mathf.RoundToInt(_startPos.position.y)];
        startCell.Parent = startCell;
        Debug.Log("StartPos is: " + startCell.Pos);
        Debug.Log("EndPos is " + endCell.Pos);

        startCell.f = 0;
        //Open Start Cell
        OpenCell(startCell);

        bool found = false;

        while (_openList.Count > 0)
        {
            Cell cell = FindOpenCell();
            //Debug.Log("Open Cell is: " + cell.Pos);
            if (PopulateNeighbors(cell, endCell))
            {
                //Debug.Log("Found Path");
                found = true;
                _openList.Clear();
            }


            _openList.Remove(cell);
            _closedList.Add(cell);
        }

        _lineRenderer.positionCount = 0;
        _lineRenderer.SetPositions(new Vector3[0]);

        List<Vector3> pointList = new();
        if (found)
        {
            pointList.Clear();
            pointList = new();

            Vector3 pointPos = (Vector2)endCell.Pos;
            pointList.Add(pointPos);

            while ((Vector2)pointPos != (Vector2)startCell.Pos)
            {
                pointPos = (Vector2)_map[Mathf.RoundToInt(pointPos.x), Mathf.RoundToInt(pointPos.y)].Parent.Pos;
                pointList.Add(pointPos);
            }
            StartCoroutine(StartMoving(pointList));

            for (int i = 0; i < pointList.Count; i++)
            {
                pointList[i] += new Vector3(0, 0, -0.25f);
            }

            _lineRenderer.positionCount = pointList.Count;
            _lineRenderer.SetPositions(pointList.ToArray());

        }
        else
        {
            _running = false;
        }
        ResetCells();
    }

    IEnumerator StartMoving(List<Vector3> path)
    {
        Debug.Log(path.Count);
        for (int i = path.Count - 1; i > -1; i--)
        {
            Debug.Log(path[i]);
            yield return new WaitForSeconds(0.25f);
            _startPos.position = path[i];
        }
        _running = false;
    }

    private void ResetCells()
    {
        for (int i = 0; i < _mapWidth; i++)
        {
            for (int j = 0; j < _mapHeight; j++)
            {
                _map[i, j].Parent = null;
                _map[i, j].f = default;
                _map[i, j].g = default;
                _map[i, j].h = default;
            }
        }
    }

    bool PopulateNeighbors(Cell cell, Cell endCell)
    {
        Debug.Log("Populating neighbors");
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                //if((i == 0 && j == 0) || (i == -1 && j == -1) || (i == -1 && j == 1) || (i == 1 && j == -1) || (i == 1 && j == 1))
                //if ((i != default && j != default) || (i == default && j == default))
                if (Mathf.Abs(i) + Mathf.Abs(j) != 1)
                {
                    //Debug.Log("Skipped Center Cell or Corner");
                    continue;
                }

                int x = cell.Pos.x + i;
                int y = cell.Pos.y + j;

                if (x >= 0 && x < _mapWidth &&
                    y >= 0 && y < _mapHeight)
                {
                    Cell newCell = _map[x, y];

                    if (newCell.Parent != null)
                    {
                        //Debug.Log("Already parented");
                        continue;
                    }

                    if (newCell.IsObstacle)
                    {
                        //Debug.Log("Is Obstacle");
                        continue;
                    }

                    newCell.Parent = cell;
                    if (newCell == endCell)
                    {
                        //Debug.Log("Found Goal");
                        return true;
                    }

                    newCell.g = newCell.Parent.g + Vector2.Distance(cell.Pos, newCell.Pos);
                    newCell.h = Vector2.Distance(endCell.Pos, newCell.Pos);
                    newCell.f = newCell.g + newCell.h;
                    _openList.Add(newCell);
                    //Debug.Log("Open List length is " + _openList.Count);
                }
            }
        }
        return false;
    }

    Cell FindOpenCell()
    {
        //Find least f
        Cell cell = _openList[0];
        for (int i = 1; i < _openList.Count; i++)
        {
            if (cell.f > _openList[i].f)
                cell = _openList[i];
            else if (cell.f == _openList[i].f && cell.h > _openList[i].f)
                cell = _openList[i];
        }

        return cell;
    }


    void CloseCell(Cell cell)
    {
        _closedList.Add(cell);
    }

    void OpenCell(Cell cell)
    {
        _openList.Add(cell);
    }

    bool ValidateStartPos()
    {
        return ((_startPos.position.x > 0 && _startPos.position.x <= _mapWidth &&
        _startPos.position.y > 0 && _startPos.position.y <= _mapHeight) &&
        !_map[Mathf.RoundToInt(_startPos.position.x), Mathf.RoundToInt(_startPos.position.y)].IsObstacle);
    }

    bool ValidateEndPos()
    {
        return ((_endPos.position.x > 0 && _endPos.position.x < _mapWidth &&
        _endPos.position.y > 0 && _endPos.position.y <= _mapHeight)
        &&
        !_map[Mathf.RoundToInt(_endPos.position.x), Mathf.RoundToInt(_endPos.position.y)].IsObstacle);
    }


    void GenerateMap()
    {
        _base = new GameObject("block");
        _base.AddComponent<SpriteRenderer>();
        _base.AddComponent<BoxCollider2D>();

        _map = new Cell[_mapWidth, _mapHeight];

        //offset = new((_mapWidth - 1) * 0.5f, (_mapHeight - 1) * 0.5f);
        for (int i = 0; i < _mapWidth; i++)
        {
            for (int j = 0; j < _mapHeight; j++)
            {
                _map[i, j] = new Cell();
                Cell cell = _map[i, j];

                cell.Pos = new(i, j);
                cell.Sprite = _sprite[0];

                GameObject b = Instantiate(_base, (Vector2)cell.Pos, Quaternion.identity);
                SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
                sr.sprite = cell.Sprite;

                if (IsObstacle())
                {
                    cell.IsObstacle = true;
                    sr.color = Color.black;
                }
            }
        }
        _startPos.position = (Vector2)FindRandomOpenPos();
        Destroy(_base);
    }

    Vector2Int FindRandomOpenPos()
    {
        Vector2Int pos = new(Random.Range(0, _mapWidth), Random.Range(0, _mapHeight));
        if (_map[pos.x, pos.y].IsObstacle == true)
            pos = FindRandomOpenPos();

        return pos;
    }

    private bool IsObstacle()
    {
        return (Random.Range(0, 100) < _obstaclePercentage);
    }


    private class Cell
    {
        int _x = 0;
        int _y = 0;
        public bool IsObstacle = false;
        private Sprite _sprite;
        public Cell Parent;
        public float f = default;
        public float g = default;
        public float h = default;
        public Vector2Int Pos
        {
            get { return new(_x, _y); }
            set
            {
                _x = value.x;
                _y = value.y;
            }
        }

        public Sprite Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }
    }
}
