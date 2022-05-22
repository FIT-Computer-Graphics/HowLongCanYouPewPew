using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform TPP;
        public Transform FPP;
        public Transform spaceship;
        [FormerlySerializedAs("firstperson")] public bool isFirstperson;
        private UnityEngine.Camera _camera;

        private void Start()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) isFirstperson = !isFirstperson;

            if (spaceship != null && !isFirstperson)
            {
                transform.position = TPP.position;
                transform.LookAt(spaceship.transform);
                transform.SetParent(null);
            }

            if (isFirstperson && spaceship != null)
            {
                transform.position = FPP.position;
                transform.rotation = FPP.rotation;
                transform.SetParent(FPP);
            }

            // Right click lowers fov
            if (Input.GetMouseButtonDown(1))
                _camera.fieldOfView = 40;
            else if (Input.GetMouseButtonUp(1)) 
                _camera.fieldOfView = 60;

            if (!isFirstperson) return;
            
            switch (_camera.fieldOfView)
            {
                case > 20 when Input.GetAxis("Mouse ScrollWheel") > 0:
                    _camera.fieldOfView -= 1;
                    break;
                case < 130 when Input.GetAxis("Mouse ScrollWheel") < 0:
                    _camera.fieldOfView += 1;
                    break;
            }

        }
    }
}