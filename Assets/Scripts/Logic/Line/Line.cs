using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawALine.Logic
{
    public class Line : MonoBehaviour
    {
        public GameObject PointPrefab;
        private LineRenderer _line;

        private Vector2 _start;
        private Vector2 _end;

        private List<GameObject> _pointList;
        public List<GameObject> PointList { get { return _pointList; } }

        private int _pointCount
        {
            get
            {
                if (LineGenerator.Instance.AutoSampling)
                {
                    int sx = Mathf.FloorToInt(_start.x);
                    int sy = Mathf.FloorToInt(_start.y);

                    int ex = Mathf.FloorToInt(_end.x);
                    int ey = Mathf.FloorToInt(_end.y);

                    int autoCount = System.Math.Max(System.Math.Abs(ex - sx), System.Math.Abs(ey - sy)) + 1;

                    return System.Math.Max(2, autoCount);
                }
                else
                {
                    return LineGenerator.Instance.SamplingPointCount;
                }
            }
        }

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = 2;

            InitializePointList();
        }

        public void Draw(Vector2 start, Vector2 end, bool needDrawPoint)
        {
            _start = start;
            _end = end;

            _line.SetPositions(new Vector3[] { _start, _end });

            if (needDrawPoint)
            {
                for (int i = 0; i < _pointCount; i++)
                {
                    Vector2 pos = new Vector2();
                    pos.x = _start.x + (_end.x - _start.x) * i / (_pointCount - 1);
                    pos.y = _start.y + (_end.y - _start.y) * i / (_pointCount - 1);
                    var point = _pointList[i];
                    point.transform.position = pos;
                    point.SetActive(true);
                }

                for (int i = _pointCount; i < 100; i++)
                    _pointList[i].SetActive(false);
            }
            else
            {
                for (int i = 0; i < 100; i++)
                    _pointList[i].SetActive(false);
            }
        }

        private void InitializePointList()
        {
            _pointList = new List<GameObject>(100);

            for (int i = 0; i < 100; i++)
            {
                var point = GameObject.Instantiate<GameObject>(PointPrefab);
                point.transform.parent = this.gameObject.transform;
                _pointList.Add(point);
                point.SetActive(false);
            }
        }
    }
}
