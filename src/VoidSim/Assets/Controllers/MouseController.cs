using UnityEngine;

namespace Assets.Scripts
{
    public class MouseController : MonoBehaviour
    {

        private Vector3 _lastFramePosition;
    
        // Update is called once per frame
        void Update () {
        
            var currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // right mouse + drag to pan camera
            currentFramePosition = HandleMousePan(currentFramePosition);

            // scroll wheel to zoom
            currentFramePosition = HandleMouseZoom(currentFramePosition);

            _lastFramePosition = currentFramePosition;
        }

        private Vector3 HandleMouseZoom(Vector3 currentFramePosition)
        {
            // backward
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - 1, 1);
            }

            // forward
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + 1, 6);
            }

            return currentFramePosition;
        }

        private Vector3 HandleMousePan(Vector3 currentFramePosition)
        {
            if (Input.GetMouseButton(1))
            {
                var diff = _lastFramePosition - currentFramePosition;
                Camera.main.transform.Translate(diff);

                // update the mouse position after the translation
                currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            return currentFramePosition;
        }
    }
}
