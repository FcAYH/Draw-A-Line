using DrawALine.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawALine.Logic
{
    public class MapGenerator : Singleton<MapGenerator>
    {
        // 地图长宽比要为 30:19
        // 1. 60 * 38
        // 2. 30 * 19

        // 暂时将地图强制为 60 * 38
        public int Row { get; private set; } = 60;
        public int Column { get; private set; } = 38;

        public Vector2 LeftBottomPoint { get; private set; }
        public Vector2 RightUpperPoint { get; private set; }

        public GameObject GridPrefab;

        private DrawAlgorithm _curAlgorithm => GameSettings.CurrentAlgorithm;
        private Vector2 _curStart => LineGenerator.Instance.StartPoint;
        private Vector2 _curEnd => LineGenerator.Instance.EndPoint;

        private SpriteRenderer[,] _grids;

        protected override void Awake()
        {
            base.Awake();
            LeftBottomPoint = new Vector2(-26, -19);
            RightUpperPoint = new Vector2(34, 19);
            _grids = new SpriteRenderer[60, 38];
            // TODO: 这里我写死了_grids的大小为60*38，但是我后面bfs的时候判断是不是出地图了用的是Row和Column
        }

        // Start is called before the first frame update
        void Start()
        {
            GenerateNewMap(Row, Column);
        }

        /// <summary>
        /// 清空地图，将地图恢复到初始状态
        /// </summary>
        public void ClearMap()
        {
            foreach (var grid in _grids)
                grid.color = Color.white;
        }

        private void GenerateNewMap(int row, int column)
        {
            // 为了方便后续添加修改地图大小的功能
            Row = row;
            Column = column;

            for (float i = LeftBottomPoint.x; i < RightUpperPoint.x; i += 1.0f)
            {
                for (float j = LeftBottomPoint.y; j < RightUpperPoint.y; j += 1.0f)
                {
                    var grid = GameObject.Instantiate<GameObject>(GridPrefab);
                    Vector2 pos = new Vector2(i, j);
                    grid.transform.SetParent(this.transform);
                    grid.transform.position = pos;

                    int x = (int)(Mathf.Floor(i) - LeftBottomPoint.x);
                    int y = (int)(Mathf.Floor(j) - LeftBottomPoint.y);
                    // Debug.Log(x + " " + y);
                    _grids[x, y] = grid.GetComponent<SpriteRenderer>();
                }
            }
        }

        /// <summary>
        /// 判断一个世界坐标是否在地图中
        /// </summary>
        /// <param name="pos">世界坐标</param>
        /// <returns></returns>
        public bool IsInMap(Vector2 pos)
        {
            bool isInMap = pos.x >= LeftBottomPoint.x
                    && pos.x < RightUpperPoint.x
                    && pos.y >= LeftBottomPoint.y
                    && pos.y < RightUpperPoint.y;
            // Debug.Log(isInMap);
            return isInMap;
        }

        // 将世界坐标转换为网格坐标，即_grids数组下标
        private (int, int) WorldPointToGrid(Vector2 pos)
        {
            int x = (int)(Mathf.Floor(pos.x) - LeftBottomPoint.x);
            int y = (int)(Mathf.Floor(pos.y) - LeftBottomPoint.y);
            return (x, y);
        }

        // 将世界坐标转换为网格坐标，即_grids数组下标
        private (int, int) WorldPointToGrid(int posX, int posY)
        {
            int x = (int)(Mathf.Floor(posX) - LeftBottomPoint.x);
            int y = (int)(Mathf.Floor(posY) - LeftBottomPoint.y);
            return (x, y);
        }

        // 将给定网格坐标的网格染成灰色
        private void ColorAGrid(int x, int y)
        {
            // Debug.Log(x + " " + y);
            _grids[x, y].color = Color.gray;
        }

        // 给定网格坐标，判断直线是否经过该网格（默认仅经过顶点不算）
        private bool IsLineThroughGrid(int nextGridX, int nextGridY, bool careVertex = false)
        {
            // Ax + by + c = 0;
            // A = y2 - y1, B = x1 - x2, C = x2y1 - x1y2
            float A = _curEnd.y - _curStart.y;
            float B = _curStart.x - _curEnd.x;
            float C = _curEnd.x * _curStart.y - _curStart.x * _curEnd.y;

            int[,] vertexes = new int[,]
            {
                {nextGridX, nextGridY},
                {nextGridX + 1, nextGridY},
                {nextGridX, nextGridY + 1},
                {nextGridX + 1, nextGridY + 1}
            };

            float[] values = new float[4];
            for (int i = 0; i < 4; i++)
            {
                values[i] = A * vertexes[i, 0] + B * vertexes[i, 1] + C;
            }

            bool through;
            if (careVertex)
            {
                through = !((values[0] > 0 && values[1] > 0 && values[2] > 0 && values[3] > 0)
                            || (values[0] < 0 && values[1] < 0 && values[2] < 0 && values[3] < 0));
            }
            else
            {
                through = !((values[0] >= 0 && values[1] >= 0 && values[2] >= 0 && values[3] >= 0)
                            || (values[0] <= 0 && values[1] <= 0 && values[2] <= 0 && values[3] <= 0));
            }

            return through;
        }

        /// <summary>
        /// 根据选定算法计算Map上哪些格子用来拟合line
        /// </summary>
        /// <param name="pointList">仅使用插值法时有用，传入插值点坐标</param>
        public void DrawMapGrid(List<GameObject> pointList = null)
        {
            ClearMap();

            if (_curAlgorithm == DrawAlgorithm.Interpolation)
            {
                /*
                    1. 根据采样点，将采样点所在的格子涂色
                */
                foreach (var point in pointList)
                {
                    if (!point.gameObject.activeInHierarchy) break;
                    (int x, int y) = WorldPointToGrid(point.transform.position);
                    ColorAGrid(x, y);
                }
            }

            else if (_curAlgorithm == DrawAlgorithm.Scanning)
            {
                /*
                    1. 先根据线段起点终点，求得“方向向量”, 包括
                    ( 0,  1), ( 1,  1), ( 1,  0), ( 1, -1), 
                    ( 0, -1), (-1, -1), (-1,  0), (-1,  1) 共八种可能性
                    2. 从起点向终点，类似bfs思路，如果当前格子坐标为(x0, y0)，方向向量为(x, y)
                    则需要探索(x0 + x, y0), (x0 + x, y0 + y), (x0, y0 + y) 三个格子，并
                    将其中被线段经过的格子涂色，然后加入队列中，直到到达终点。
                */
                Vector2 dirVec = _curEnd - _curStart;
                if (dirVec.sqrMagnitude < 1) return;

                Vector2Int dir = new Vector2Int();
                dir.x = dirVec.x > 0 ? 1 : dirVec.x < 0 ? -1 : 0;
                dir.y = dirVec.y > 0 ? 1 : dirVec.y < 0 ? -1 : 0;

                // Debug.Log("dir: " + dir.x + " " + dir.y);

                Queue<(int, int)> gridQ = new Queue<(int, int)>();

                // 为起点染色
                int startX = Mathf.FloorToInt(_curStart.x);
                int startY = Mathf.FloorToInt(_curStart.y);
                (int startGridX, int startGridY) = WorldPointToGrid(startX, startY);
                ColorAGrid(startGridX, startGridY);
                gridQ.Enqueue((startX, startY));

                (int, int) endGrid = (Mathf.FloorToInt(_curEnd.x), Mathf.FloorToInt(_curEnd.y));

                int layer = 0;
                while (true)
                {
                    layer++;
                    if (layer > 200) break; // 总感觉写个while(true)会有死循环卡死程序的风险所以加了个魔法数

                    (int x, int y) = gridQ.Dequeue();

                    // 当扫描到终点后就可以退出循环了
                    if (x == endGrid.Item1 && y == endGrid.Item2)
                    {
                        (int gridX, int gridY) = WorldPointToGrid(x, y);
                        ColorAGrid(gridX, gridY);
                        break;
                    }

                    int[,] worldPoints = new int[,]
                    {
                        {x + dir.x, y},
                        {x + dir.x, y + dir.y},
                        {x, y + dir.y}
                    };

                    for (int i = 0; i < 3; i++)
                    {
                        int worldPointX = worldPoints[i, 0], worldPointY = worldPoints[i, 1];
                        if (IsLineThroughGrid(worldPointX, worldPointY))
                        {
                            (int gridX, int gridY) = WorldPointToGrid(worldPointX, worldPointY);

                            if (gridX < 0 || gridX >= Row || gridY < 0 || gridY >= Column)
                                continue;

                            if (_grids[gridX, gridY].color != Color.gray)
                            {
                                ColorAGrid(gridX, gridY);
                                gridQ.Enqueue((worldPointX, worldPointY));
                            }
                        }
                    }
                }
            }
        }
    }
}