using UnityEngine;
using Scripts.PlayerController;
namespace Scripts.Camera
{
    
    public class CameraFollow : MonoBehaviour
    {
        public float smoothness;
        public Transform targetObject;//sada ce kamera pratit Player umjesto Spacesparow
        private Vector3 initialOffset;
        private Vector3 cameraPosition;
       
        public enum RelativePosition { InitialPosition, Position1, Position2 }
        public RelativePosition relativePosition;
        public Vector3 position1;
        public Vector3 position2;
      
        private void Start()
        {
            relativePosition = RelativePosition.InitialPosition;
            initialOffset = transform.position - targetObject.position;
        }

        private void Update()
        {
           
                cameraPosition = targetObject.position + CameraOffset(relativePosition);
                transform.position = Vector3.Lerp(transform.position, cameraPosition, smoothness*Time.deltaTime);
                transform.LookAt(targetObject);
                
                
        }

        private Vector3 CameraOffset(RelativePosition relPosition)
        {
            var currentOffset = relPosition switch
            {
                RelativePosition.Position1 => position1,
                RelativePosition.Position2 => position2,
                _ => initialOffset
            };
            return currentOffset;
        }
    }

}