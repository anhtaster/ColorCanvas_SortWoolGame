using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GogoGaga.OptimizedRopesAndCables
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class Rope : MonoBehaviour
    {
        public event Action OnPointsChanged;

        [Header("Rope Transforms")]
        [SerializeField] private Transform startPoint;
        public Transform StartPoint => startPoint;

        [SerializeField] private Transform midPoint;
        public Transform MidPoint => midPoint;

        [SerializeField] private Transform endPoint;
        public Transform EndPoint => endPoint;

        [Header("Rope Settings")]
        [Range(2, 100)] public int linePoints = 10;
        public float stiffness = 350f;
        public float damping = 15f;
        public float ropeLength = 15f;
        public float ropeWidth = 0.1f;

        [Header("Bezier Weight")]
        [Range(1, 15)] public float midPointWeight = 1f;
        private const float StartPointWeight = 1f;
        private const float EndPointWeight = 1f;

        [Header("Midpoint Position")]
        [Range(0.25f, 0.75f)] public float midPointPosition = 0.5f;

        // --- NEW FLAG ---
        [Header("External Control (Color Override Fix)")]
        public bool externalColorControl = false;

        // internal vars
        private Vector3 currentValue;
        private Vector3 currentVelocity;
        private Vector3 targetValue;
        public Vector3 otherPhysicsFactors { get; set; }

        private LineRenderer lineRenderer;
        private bool isFirstFrame = true;

        private Vector3 prevStartPointPosition;
        private Vector3 prevEndPointPosition;
        private float prevMidPointPosition;
        private float prevMidPointWeight;
        private float prevLineQuality;
        private float prevRopeWidth;
        private float prevStiffness;
        private float prevDampness;
        private float prevRopeLength;

        public bool IsPrefab => gameObject.scene.rootCount == 0;

        private void Start()
        {
            InitializeLineRenderer();

            if (AreEndPointsValid())
            {
                currentValue = GetMidPoint();
                targetValue = currentValue;
                currentVelocity = Vector3.zero;
                SetSplinePoint();
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                InitializeLineRenderer();
                if (AreEndPointsValid())
                {
                    RecalculateRope();
                    SimulatePhysics();
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }
            }
        }

        private void InitializeLineRenderer()
        {
            if (!lineRenderer)
                lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth;

            // --- CRITICAL FIX ---
            if (externalColorControl)
            {
                // prevents Unity from auto-resetting gradient after width changes
                Gradient g = lineRenderer.colorGradient;
                lineRenderer.colorGradient = g;
            }
        }

        private void Update()
        {
            if (IsPrefab) return;

            if (AreEndPointsValid())
            {
                SetSplinePoint();

                if (!Application.isPlaying && (IsPointsMoved() || IsRopeSettingsChanged()))
                {
                    SimulatePhysics();
                    NotifyPointsChanged();
                }

                prevStartPointPosition = startPoint.position;
                prevEndPointPosition = endPoint.position;
                prevMidPointPosition = midPointPosition;
                prevMidPointWeight = midPointWeight;
                prevLineQuality = linePoints;
                prevRopeWidth = ropeWidth;
                prevStiffness = stiffness;
                prevDampness = damping;
                prevRopeLength = ropeLength;
            }
        }

        private bool AreEndPointsValid()
        {
            return startPoint != null && endPoint != null;
        }

        private void SetSplinePoint()
        {
            if (lineRenderer.positionCount != linePoints + 1)
                lineRenderer.positionCount = linePoints + 1;

            // --- backup gradient BEFORE spline updates ---
            Gradient backupGradient = lineRenderer.colorGradient;

            Vector3 mid = GetMidPoint();
            targetValue = mid;
            mid = currentValue;

            if (midPoint != null)
            {
                midPoint.position = GetRationalBezierPoint(startPoint.position, mid, endPoint.position, midPointPosition,
                    StartPointWeight, midPointWeight, EndPointWeight);
            }

            for (int i = 0; i < linePoints; i++)
            {
                Vector3 p = GetRationalBezierPoint(startPoint.position, mid, endPoint.position,
                    i / (float)linePoints, StartPointWeight, midPointWeight, EndPointWeight);

                lineRenderer.SetPosition(i, p);
            }

            lineRenderer.SetPosition(linePoints, endPoint.position);

            // --- restore gradient (fixes default color override) ---
            if (externalColorControl)
                lineRenderer.colorGradient = backupGradient;
        }

        private float CalculateYFactorAdjustment(float weight)
        {
            float k = Mathf.Lerp(0.493f, 0.323f, Mathf.InverseLerp(1, 15, weight));
            float w = 1f + k * Mathf.Log(weight);
            return w;
        }

        private Vector3 GetMidPoint()
        {
            Vector3 a = startPoint.position;
            Vector3 b = endPoint.position;

            Vector3 mid = Vector3.Lerp(a, b, midPointPosition);
            float yFactor = (ropeLength - Mathf.Min(Vector3.Distance(a, b), ropeLength))
                            / CalculateYFactorAdjustment(midPointWeight);

            mid.y -= yFactor;
            return mid;
        }

        private Vector3 GetRationalBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t, float w0, float w1, float w2)
        {
            Vector3 wp0 = w0 * p0;
            Vector3 wp1 = w1 * p1;
            Vector3 wp2 = w2 * p2;

            float denominator = w0 * Mathf.Pow(1 - t, 2) +
                               2 * w1 * (1 - t) * t +
                               w2 * Mathf.Pow(t, 2);

            Vector3 point = (wp0 * Mathf.Pow(1 - t, 2) +
                             wp1 * 2 * (1 - t) * t +
                             wp2 * Mathf.Pow(t, 2)) / denominator;

            return point;
        }

        public Vector3 GetPointAt(float t)
        {
            return GetRationalBezierPoint(startPoint.position, currentValue, endPoint.position, t,
                                          StartPointWeight, midPointWeight, EndPointWeight);
        }

        private void FixedUpdate()
        {
            if (IsPrefab) return;

            if (AreEndPointsValid())
            {
                if (!isFirstFrame)
                    SimulatePhysics();

                isFirstFrame = false;
            }
        }

        private void SimulatePhysics()
        {
            float dampingFactor = Mathf.Max(0, 1 - damping * Time.fixedDeltaTime);

            Vector3 acceleration = (targetValue - currentValue) * stiffness * Time.fixedDeltaTime;

            currentVelocity = currentVelocity * dampingFactor +
                              acceleration +
                              otherPhysicsFactors;

            currentValue += currentVelocity * Time.fixedDeltaTime;
        }

        public void RecalculateRope()
        {
            if (!AreEndPointsValid())
            {
                lineRenderer.positionCount = 0;
                return;
            }

            currentValue = GetMidPoint();
            targetValue = currentValue;
            currentVelocity = Vector3.zero;
            SetSplinePoint();
        }

        public void SetStartPoint(Transform newStartPoint, bool instantAssign = false)
        {
            startPoint = newStartPoint;
            prevStartPointPosition = startPoint == null ? Vector3.zero : startPoint.position;

            if (instantAssign || newStartPoint == null)
                RecalculateRope();

            NotifyPointsChanged();
        }

        public void SetEndPoint(Transform newEndPoint, bool instantAssign = false)
        {
            endPoint = newEndPoint;
            prevEndPointPosition = endPoint == null ? Vector3.zero : endPoint.position;

            if (instantAssign || newEndPoint == null)
                RecalculateRope();

            NotifyPointsChanged();
        }

        public void SetMidPoint(Transform newMidPoint, bool instantAssign = false)
        {
            midPoint = newMidPoint;
            prevMidPointPosition = midPoint == null ? 0.5f : midPointPosition;

            if (instantAssign || newMidPoint == null)
                RecalculateRope();

            NotifyPointsChanged();
        }

        private void NotifyPointsChanged()
        {
            OnPointsChanged?.Invoke();
        }

        private bool IsPointsMoved()
        {
            return startPoint.position != prevStartPointPosition ||
                   endPoint.position != prevEndPointPosition;
        }

        private bool IsRopeSettingsChanged()
        {
            return !Mathf.Approximately(linePoints, prevLineQuality)
                   || !Mathf.Approximately(ropeWidth, prevRopeWidth)
                   || !Mathf.Approximately(stiffness, prevStiffness)
                   || !Mathf.Approximately(damping, prevDampness)
                   || !Mathf.Approximately(ropeLength, prevRopeLength)
                   || !Mathf.Approximately(midPointPosition, prevMidPointPosition)
                   || !Mathf.Approximately(midPointWeight, prevMidPointWeight);
        }
    }
}
