using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Camera
{
    
    public class CameraSwitch : MonoBehaviour
    {
        public Transform Camera;

        [FormerlySerializedAs("First Person Position")] public Transform firstPersonPosition;

        [FormerlySerializedAs("Third Person Position")] public Transform thirdPersonPosition;

        public string currentCamera;
        
        private void Start()
        {
            currentCamera = "Third";
            Camera.localPosition = thirdPersonPosition.localPosition;
            Camera.localRotation = thirdPersonPosition.localRotation;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) ToggleCamera();
        }

        private void ToggleCamera()
        {
            ToggleCameraMovement();
            switch (currentCamera)
            {
                case "First":
                    Camera.localPosition = thirdPersonPosition.localPosition;
                    Camera.localRotation = thirdPersonPosition.localRotation;
                    currentCamera = "Third";
                    break;
                default:
                    Camera.localPosition = firstPersonPosition.localPosition;
                    Camera.localRotation = firstPersonPosition.localRotation;
                    currentCamera = "First";
                    break;
            }
        }

        private void ToggleCameraMovement()
        {
            var follower = Camera.gameObject.GetComponent<CameraFollow>();
            follower.enabled = !follower.enabled;
        }
    }

}

