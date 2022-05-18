using UnityEngine;
using UnityEngine.UIElements;

namespace Scripts.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform TPP;
        public Transform FPP;
        public Transform spaceship;
        public bool firstperson;
        public bool isZoomedIn;
        private UnityEngine.Camera camera;
        
        private void Start()
        {
            camera = GetComponent<UnityEngine.Camera>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) firstperson = !firstperson;

            if (spaceship != null && !firstperson)
            {
                transform.position = TPP.position;
                transform.LookAt(spaceship.transform);
                transform.SetParent(null);
            }

            if (firstperson && spaceship != null)
            {
                transform.position = FPP.position;
                transform.SetParent(FPP);
                transform.rotation = FPP.rotation;
            }
            
            // Right click lowers fov
            if (Input.GetMouseButtonDown(1))
            {
                camera.fieldOfView = 40;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                camera.fieldOfView = 60;
            }
            
        }
        
    }
}