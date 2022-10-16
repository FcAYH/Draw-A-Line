using DrawALine.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawALine.Logic
{
    public class LineGenerator : Singleton<LineGenerator>
    {
        public GameObject LinePrefab;
        public LineState State { get; private set; }

        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }

        [Header("插值法参数")]
        public bool AutoSampling = false;

        [Range(2, 100), Tooltip("取样点个数, 仅在采用插值法时起作用")]
        public int SamplingPointCount;


        private Camera _mainCamera;
        private DrawAlgorithm _curAlgorithm => GameSettings.CurrentAlgorithm;

        private Line _line;

        protected override void Awake()
        {
            base.Awake();

            var go = GameObject.Instantiate<GameObject>(LinePrefab);
            go.transform.parent = this.gameObject.transform;

            _line = go.GetComponent<Line>();
            _line.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            _mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            // 点击右键或者esc键，恢复原始状态，重新绘制
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                Reset();
            }

            if (State == LineState.ChooseStart)
            {
                /*
                    1. 左键点击
                    2. 判断点击位置是否在网格内，是3 / 否4
                    3. 记录起点，调用绘制
                    4. 返回1
                */
                if (Input.GetMouseButtonDown(0))
                {
                    var pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 gridPos = new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y));
                    //Debug.Log(gridPos.x + " " + gridPos.y);

                    if (MapGenerator.Instance.IsInMap(gridPos))
                    {
                        StartPoint = pos;
                        State = LineState.ChooseEnd;
                    }
                }
            }
            else if (State == LineState.ChooseEnd)
            {
                /*
                    1. 绘制一条直线跟着鼠标走
                    2. 同时Map端不停绘制当前网格中拟合直线所需的格子
                    3. 如果左键点击，判断点击位置是否在网格内，是4 / 否5
                    4. 记录终点，此时再移动鼠标不会再跟随绘制了
                    5. 无效，返回2
                */
                var pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                if (MapGenerator.Instance.IsInMap(pos))
                {
                    EndPoint = pos;
                    DrawLine(pos);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 gridPos = new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y));

                    if (MapGenerator.Instance.IsInMap(gridPos))
                    {
                        EndPoint = pos;
                        State = LineState.KeepDraw;
                    }
                }
            }
            else if (State == LineState.KeepDraw)
            {
                // 保持绘制
                DrawLine(EndPoint);
            }
        }

        private void Reset()
        {
            State = LineState.ChooseStart;
            _line.gameObject.SetActive(false);

            MapGenerator.Instance.ClearMap();
        }

        private void DrawLine(Vector3 current)
        {
            if (!_line.gameObject.activeInHierarchy)
                _line.gameObject.SetActive(true);

            bool needDrawPoint = _curAlgorithm == DrawAlgorithm.Interpolation;
            _line.Draw(StartPoint, current, needDrawPoint);

            MapGenerator.Instance.DrawMapGrid(_line.PointList);
        }
    }
}