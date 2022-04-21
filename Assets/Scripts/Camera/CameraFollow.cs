using System;
using UnityEngine;

namespace Scripts.Camera
{
    public class CameraFollow : MonoBehaviour
    {

        public Transform TPP;
        public Transform FPP;
        public Transform POV;
        public Transform spaceship;
        
        private void Update()
        {
            transform.position = TPP.position;
            if (spaceship!=null&&!Input.GetKey(KeyCode.V))
            {
                transform.LookAt(spaceship.transform);
            }
            if (Input.GetKey(KeyCode.V)&&spaceship!=null)
            {
                transform.position = FPP.position;
                transform.LookAt(POV);
            }
        }

       
    }
}