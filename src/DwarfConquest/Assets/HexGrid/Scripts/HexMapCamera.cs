using Assets.Utility.Attributes;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexMapCamera : MonoBehaviour
    {
        private Transform _swivel;
        private Transform _stick;

        private float _zoom = 1f;

        public float StickMinZoom;
        public float StickMaxZoom;

        public float SwivelMinZoom;
        public float SwivelMaxZoom;

        public float MoveSpeedMinZoom;
        public float MoveSpeedMaxZoom;

        public float RotationSpeed;
        private float _rotationAngle;

        [RequireReference]
        public HexGrid Grid;

        void Awake()
        {
            _swivel = transform.GetChild(0);
            _stick = _swivel.GetChild(0);
        }

        void Update()
        {
            var zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(zoomDelta) > float.Epsilon)
            {
                AdjustZoom(zoomDelta);
            }

            var rotationDelta = Input.GetAxis("Rotation");
            if (Mathf.Abs(rotationDelta) > float.Epsilon)
            {
                AdjustRotation(rotationDelta);
            }

            var xDelta = Input.GetAxis("Horizontal");
            var zDelta = Input.GetAxis("Vertical");
            if (Mathf.Abs(xDelta) > float.Epsilon ||
                Mathf.Abs(zDelta) > float.Epsilon)
            {
                AdjustPosition(xDelta, zDelta);
            }
        }

        private void AdjustRotation(float rotationDelta)
        {
            _rotationAngle += rotationDelta * RotationSpeed * Time.deltaTime;
            if (_rotationAngle < 0f)
            {
                _rotationAngle += 360;
            }
            else if (_rotationAngle >= 360f)
            {
                _rotationAngle -= 360f;
            }

            transform.localRotation = Quaternion.Euler(0f, _rotationAngle, 0f);
        }

        private void AdjustPosition(float xDelta, float zDelta)
        {
            var direction = transform.localRotation 
                * new Vector3(xDelta, 0f, zDelta).normalized;
            var damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            var moveSpeed = Mathf.Lerp(MoveSpeedMinZoom, MoveSpeedMaxZoom, _zoom);
            var distance = moveSpeed * damping * Time.deltaTime;

            var position = transform.localPosition;
            position +=  direction * distance;
            transform.localPosition = ClampPosition(position);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            var xMax = Grid.ChunkCountX * HexMetrics.ChunkSizeX
                       * (2f * HexMetrics.InnerRadius);
            position.x = Mathf.Clamp(position.x, 0f, xMax);

            var zMax = Grid.ChunkCountZ * HexMetrics.ChunkSizeZ
                       * (1.5f * HexMetrics.OuterRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);

            return position;
        }

        private void AdjustZoom(float zoomDelta)
        {
            _zoom = Mathf.Clamp01(_zoom + zoomDelta);

            var distance = Mathf.Lerp(StickMinZoom, StickMaxZoom, _zoom);
            _stick.localPosition = new Vector3(0f, 0f, distance);

            var angle = Mathf.Lerp(SwivelMinZoom, SwivelMaxZoom, _zoom);
            _swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}
