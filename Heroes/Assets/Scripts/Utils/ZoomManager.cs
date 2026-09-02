using Packages.CoreUtils.Utils;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(Camera))]
    public class ZoomManager : MonoBehaviour
    {
        [SerializeField] private float _mouseZoomSpeed = 15.0f;
        [SerializeField] private float _touchZoomSpeed = 0.1f;
        [SerializeField] private float _zoomMinBound = 0.1f;
        [SerializeField] private float _zoomMaxBound = 179.9f;
        [SerializeField] private float _dragSpeed = 0.05f;
        [SerializeField] private float _yBorder;
        [SerializeField] private float _xBorder;
        [SerializeField, HideInInspector] private Camera _camera;

        private float _defaultFov;
        private float _previousX;
        private float _previousY;
        private float _targetXBorder;
        private float _targetYBorder;
        
        private void OnValidate()
        {
            _camera = GetComponent<Camera>(); 
        }

        private void OnEnable()
        {
            _defaultFov = _camera.fieldOfView;
            _targetXBorder = _camera.transform.position.x;
            _targetYBorder = _camera.transform.position.y;
        }

        private void Update()
        {
            if (Input.touchSupported)
            {
                if (Input.touchCount == 2)
                {
                    var tZero = Input.GetTouch(0);
                    var tOne = Input.GetTouch(1);
                    var tZeroPrevious = tZero.position - tZero.deltaPosition;
                    var tOnePrevious = tOne.position - tOne.deltaPosition;

                    var oldTouchDistance = Vector2.Distance (tZeroPrevious, tOnePrevious);
                    var currentTouchDistance = Vector2.Distance (tZero.position, tOne.position);

                    var deltaDistance = oldTouchDistance - currentTouchDistance;
                    Zoom(deltaDistance, _touchZoomSpeed);
                }
                else
                {
                    if (!ObjectButton.IsPointerOverUiElement() && Input.GetMouseButton(0))
                        Move();

                    if (Input.GetMouseButtonUp(0))
                    {
                        _previousX = float.MinValue;
                        _previousY = float.MinValue;
                    }
                }
            }
            else
            {
                var scroll = Input.GetAxis("Mouse ScrollWheel");
                Zoom(scroll, _mouseZoomSpeed);
                
                if (!ObjectButton.IsPointerOverUiElement() && Input.GetMouseButton(0))
                    Move();
            }
            
            FixBorders(_camera.transform.position);
        }

        private void Zoom(float deltaMagnitudeDiff, float speed)
        {
            var fieldOfView = _camera.fieldOfView;
            fieldOfView += deltaMagnitudeDiff * speed;
            _camera.fieldOfView = Mathf.Clamp(fieldOfView, _zoomMinBound, _zoomMaxBound);
        }

        private void Move()
        {
            if (_previousX <= float.MinValue)
            {
                _previousX = Input.mousePosition.x;
                _previousY = Input.mousePosition.y;
            }
            
            var addPosition = new Vector3(Input.mousePosition.x - _previousX, Input.mousePosition.y - _previousY) * _dragSpeed;
            var newPosition = _camera.transform.position + addPosition;
            _camera.transform.position = newPosition;
            
            _previousX = Input.mousePosition.x;
            _previousY = Input.mousePosition.y;
        }

        private void FixBorders(Vector3 newPosition)
        {
            var normalizedFov = _defaultFov / 2;
            var fracturedFov = 2 * normalizedFov / _camera.fieldOfView - 1;
            var fracturedPosition = new Vector2(_xBorder * fracturedFov, _yBorder * fracturedFov);
            newPosition.x = Mathf.Min(fracturedPosition.x + _targetXBorder, newPosition.x);
            newPosition.x = Mathf.Max(-fracturedPosition.x + _targetXBorder, newPosition.x);
            newPosition.y = Mathf.Min(fracturedPosition.y + _targetYBorder, newPosition.y);
            newPosition.y = Mathf.Max(-fracturedPosition.y + _targetYBorder, newPosition.y);
            _camera.transform.position = newPosition;

        }
    }
}
