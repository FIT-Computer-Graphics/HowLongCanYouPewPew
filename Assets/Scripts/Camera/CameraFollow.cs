using UnityEngine;

namespace Scripts.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform TPP;
        public Transform FPP;
        public Transform spaceship;
        public bool firstperson;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F)) firstperson = !firstperson;

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
        }
    }
}