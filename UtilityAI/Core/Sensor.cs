using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Rendering;

namespace Game.UtilityAI
{
    public class Sensor : MonoBehaviour
    {
        [Header("Memory Section")] [SerializeField] [MemoryEntry(typeof(AlertType))]
        private string alertType;

        [SerializeField] [MemoryEntry(typeof(Vector3))]
        private string lastKnownPlayerLocation;

        [SerializeField] [MemoryEntry(typeof(Vector3))] private string lastKnownPlayerDirection;
        
        [SerializeField] [GameTag] private string playerTag;


        [Header("Vision Properties")] [SerializeField] [Range(0, 360)]
        private float visionRange = 10f;

        [SerializeField] private float visionAngle = 60f;

        [SerializeField] private Color[] alertColors;
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private LayerMask detectionLayers;
        [SerializeField] private LayerMask obstacleLayers;
        [SerializeField] private float meshResolution = 1;

        [SerializeField] private Material coneMat;
        [SerializeField] private float height = 0.5f;

        private List<GameObject> _visibleObjects = new List<GameObject>();
        private float _lastUpdateTime;
        private Collider[] _colliders = new Collider[10];


        private Mesh _mesh;
        MaterialPropertyBlock _propertyBlock;
        private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");
        private Brain _brain;
        private Transform _transform;
        private RenderParams _renderParams;
        private float _viewAngle;
        private float _viewRange;
        private Vector3[] _vertices;
        private int[] _triangles;
        private int _stepCount;
        private float _stepAngleSize;
        private NativeArray<RaycastCommand> _raycastCommands;
        private NativeArray<RaycastHit> _raycastResults;
        private JobHandle _raycastJob;
        

        private void Start()
        {
            _transform = transform;
            _brain = GetComponent<Brain>();
            _mesh = new Mesh();
            _mesh.MarkDynamic();

            _viewAngle = visionAngle;
            _viewRange = visionRange;

            _stepCount = Mathf.RoundToInt(_viewAngle * meshResolution);
            _stepAngleSize = _viewAngle / _stepCount;

            _vertices = new Vector3[_stepCount + 2];
            _triangles = new int[_stepCount * 3];

            _mesh.vertices = _vertices;
            _raycastCommands = new NativeArray<RaycastCommand>(_stepCount + 1, Allocator.Persistent);
            _raycastResults = new NativeArray<RaycastHit>(_stepCount + 1, Allocator.Persistent);

            _propertyBlock = new MaterialPropertyBlock();
            _renderParams = new RenderParams()
            {
                material = coneMat,
                matProps = _propertyBlock,
                shadowCastingMode = ShadowCastingMode.Off,
                renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask
            };
            _brain.SetMemory(alertType, AlertType.Chill);
        }

        private void OnDestroy()
        {
            _raycastResults.Dispose();
            _raycastCommands.Dispose();
            _mesh.Clear();
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastUpdateTime)
            {
                UpdateVisibleObjects(_transform, _viewRange, _viewAngle, height, ref _visibleObjects, ref _colliders,
                    detectionLayers, obstacleLayers);
                _lastUpdateTime = Time.time + updateInterval;
                UpdateMemory();
            }
        }

        private void UpdateMemory()
        {
            var p = GetByTag(playerTag);
            if (p)
            {
                _brain.SetMemory(lastKnownPlayerLocation, p.transform.position);
                _brain.SetMemory(lastKnownPlayerDirection, p.GetComponent<CharacterPhysics>().GetLastDirection());
                _brain.SetMemory(alertType, AlertType.RedAlert);
            }
            else
            {
                if (_brain.GetMemory<Vector3>(lastKnownPlayerLocation) != Vector3.zero)
                {
                    _brain.SetMemory(alertType, AlertType.VerySuspicious);
                }
            }
        }

        private void SetVisionColor()
        {
            AlertType alert = _brain.GetMemory<AlertType>(alertType);
            switch (alert)
            {
                case AlertType.Chill:
                    _propertyBlock.SetColor(ColorProperty, alertColors[0]);
                    break;
                case AlertType.Suspicious:
                    _propertyBlock.SetColor(ColorProperty, alertColors[0]);
                    break;
                case AlertType.VerySuspicious:
                    _propertyBlock.SetColor(ColorProperty, alertColors[1]);
                    break;
                case AlertType.RedAlert:
                    _propertyBlock.SetColor(ColorProperty, alertColors[2]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LateUpdate()
        {
            SetVisionColor();
            DrawFieldOfView();
            Graphics.RenderMesh(_renderParams, _mesh, 0, _transform.localToWorldMatrix);
        }

        private void UpdateVisibleObjects(Transform target, float range, float angle, float thickness,
            ref List<GameObject> visibleObjects, ref Collider[] collider, LayerMask detectables, LayerMask obstacles)
        {
            visibleObjects.Clear();
            var pos = target.position;

            var size = Physics.OverlapSphereNonAlloc(pos, range, collider, detectables,
                QueryTriggerInteraction.Collide);

            float cosAngleThreshold = Mathf.Cos(angle * Mathf.Deg2Rad / 2);
            Vector3 targetForward = target.forward;
            Vector3 targetPos = pos + Vector3.up * thickness;

            for (int i = 0; i < size; i++)
            {
                Collider col = collider[i];
                if (col.gameObject == gameObject)
                {
                    continue;
                }

                Vector3 colPos = col.transform.position;
                Vector3 directionToTarget = (colPos - targetPos);

                Vector3 angleDir = directionToTarget;
                angleDir.y = 0;

                if (angleDir.sqrMagnitude < Mathf.Epsilon)
                    continue;

                angleDir.Normalize();
                float cosAngle = Vector3.Dot(angleDir, targetForward);

                if (cosAngle >= cosAngleThreshold)
                {
                    float dist = Vector3.Distance(targetPos, colPos);
                    if (!Physics.Raycast(targetPos, angleDir, dist, obstacles,
                            QueryTriggerInteraction.Ignore))
                    {
                        visibleObjects.Add(collider[i].gameObject);
                    }
                }
            }
        }

        public bool HasThisTag(string gameTag)
        {
            return _visibleObjects.Exists(x => x.CompareTag(gameTag));
        }


        public GameObject GetByTag(string gameTag)
        {
            return _visibleObjects.Find(x => x.CompareTag(gameTag));
        }


        void DrawFieldOfView()
        {
            QueryParameters query = new QueryParameters(obstacleLayers, false, QueryTriggerInteraction.Ignore);
            Vector3 targetPos = _transform.position + Vector3.up * height;
            for (int i = 0; i <= _stepCount; i++)
            {
                float angle = _transform.eulerAngles.y - _viewAngle / 2 + _stepAngleSize * i;
                Vector3 direction = DirFromAngle(angle);
                _raycastCommands[i] = new RaycastCommand(targetPos, direction, query, _viewRange);
            }

            _raycastJob = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastResults, 1);
            
            _raycastJob.Complete();

            _vertices[0] = Vector3.up * height;
            for (int i = 0; i <= _stepCount; i++)
            {
                Vector3 newViewCast = targetPos + _raycastCommands[i].direction * _viewRange;
                if (_raycastResults[i].collider)
                {
                    newViewCast = _raycastResults[i].point;
                }

                _vertices[i + 1] = _transform.InverseTransformPoint(newViewCast);

                if (i < _stepCount)
                {
                    _triangles[i * 3] = 0;
                    _triangles[i * 3 + 1] = i + 1;
                    _triangles[i * 3 + 2] = i + 2;
                }
            }

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.RecalculateBounds();
        }

        private Vector3 DirFromAngle(float angleInDegrees)
        {
            float rad = angleInDegrees * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
        }
    }
}