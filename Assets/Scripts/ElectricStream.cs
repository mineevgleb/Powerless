using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ElectricStream : MonoBehaviour
    {
        public GameObject PositionFrom;
        public GameObject PositionTo;
        public Color StreamColor = new Color(1, 1, 1, 1);
        public float StreamWidth = 10;
        public Material StreamMaterial;
        public float MaxSegmentLength = 10;
        public float MinAberration = 0.5f;
        public float MaxAberration = 0.8f;
        public float LengthInconsistency = 0.3f;
        public float SustainTime = 0.5f;
        public float FlashDelay = 0.1f;

        private LinkedList<LineRenderer> _visibleLines;
        private LinkedList<LineRenderer> _linesBuffer;
        private LinkedList<float> _linesAlphas;

        void Awake ()
        {   
            _linesBuffer = new LinkedList<LineRenderer>();
            _visibleLines = new LinkedList<LineRenderer>();
            _linesAlphas = new LinkedList<float>();
        }

        void OnEnable ()
        {
            InvokeRepeating("AddNewStream", 0, FlashDelay);
        }

        void OnDisable() {
            CancelInvoke("AddNewStream");
            _linesAlphas.Clear();
            for (var it = _visibleLines.First; it != null; it = it.Next)
            {
                it.Value.SetColors(new Color(0,0,0,0), new Color(0,0,0,0));
                _linesBuffer.AddLast(it.Value);
            }
            _visibleLines.Clear();
        }

        void FixedUpdate()
        {
            float alphaDecrease = (Time.deltaTime / SustainTime) * StreamColor.a;
            var itLine = _visibleLines.First;
            var itAlpha = _linesAlphas.First;
            while (itLine != null && itAlpha!= null)
            {
                var nextItLine = itLine.Next;
                var nextItAlpha = itAlpha.Next;
                itAlpha.Value -= alphaDecrease;
                if (itAlpha.Value <= 0.0f)
                {
                    itLine.Value.SetColors(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0));
                    _linesBuffer.AddFirst(itLine.Value);
                    _visibleLines.Remove(itLine);
                    _linesAlphas.Remove(itAlpha);
                }
                else
                {
                    Color newColor = StreamColor;
                    newColor.a = itAlpha.Value;
                    itLine.Value.SetColors(newColor, newColor);
                }
                itLine = nextItLine;
                itAlpha = nextItAlpha;
            }
        }

        void AddNewStream()
        {
            if (!gameObject.activeInHierarchy) return;
            LineRenderer lineRend;
            if (_linesBuffer.Count == 0)
            {
                GameObject newStream = new GameObject("ElectricStream");
                newStream.transform.position = gameObject.transform.position;
                newStream.transform.parent = gameObject.transform;
                lineRend = newStream.AddComponent<LineRenderer>();
                lineRend.useWorldSpace = false;
                lineRend.material = StreamMaterial;
                newStream.SetActive(true);
            }
            else
            {
                lineRend = _linesBuffer.Last.Value;
                _linesBuffer.RemoveLast();
            }
            var pointPositions = GenerateNew();
            lineRend.SetVertexCount(pointPositions.Count);
            lineRend.SetPositions(pointPositions.ToArray());
            lineRend.SetColors(StreamColor, StreamColor);
            lineRend.SetWidth(StreamWidth, StreamWidth);
            _visibleLines.AddFirst(lineRend);
            _linesAlphas.AddFirst(StreamColor.a);
        }

        LinkedList<Vector3> GenerateNew()
        {
            LinkedList<Vector3> pointsPositions = new LinkedList<Vector3>();
            pointsPositions.AddLast(PositionFrom.transform.localPosition);
            pointsPositions.AddLast(PositionTo.transform.localPosition);
            Subdivide(pointsPositions.First, pointsPositions.Last);
            return pointsPositions;
        }

        void Subdivide(LinkedListNode<Vector3> start, LinkedListNode<Vector3> end)
        {
            float segmentLength = Vector2.Distance(start.Value, end.Value); // we want distance in 2d
            if (segmentLength <= MaxSegmentLength)
                return;

            // Take point close to the middle
            float separationPart = (Random.value - 0.5f) * LengthInconsistency + 0.5f; 
            Vector2 middlePoint = start.Value * (1 - separationPart) + end.Value * separationPart;

            //Calculate displacement
            Vector2 sideVector = Vector3.Cross(
                new Vector3(end.Value.x - start.Value.x, end.Value.y - start.Value.y, 0), 
                Vector3.forward);
            sideVector.Normalize();
            sideVector *= Random.Range(MinAberration, MaxAberration) * (segmentLength / 2);
            if (Random.value > 0.5f)
                sideVector *= -1;
            middlePoint += sideVector;

            //Insert new point and repeat for new segments
            LinkedListNode<Vector3> middle = start.List.AddAfter(start, middlePoint);
            Subdivide(start, middle);
            Subdivide(middle, end);
        }
    }
}
