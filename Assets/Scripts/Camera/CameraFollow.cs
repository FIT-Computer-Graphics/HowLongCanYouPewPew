using System;
using UnityEngine;

namespace Scripts.Camera
{
    public class CameraFollow : MonoBehaviour
    {

        public Transform TPP;
        public Transform FPP;
        public Transform spaceship;
        
        private void Update()
        {
            
            if (spaceship!=null&&!Input.GetKey(KeyCode.V))
            {
                transform.position = TPP.position;
                transform.LookAt(spaceship.transform);
                transform.SetParent(null);
            }
            if (Input.GetKey(KeyCode.V)&&spaceship!=null)
            {
                transform.position = FPP.position;
                transform.SetParent(FPP);
                transform.rotation = FPP.rotation;
            }
        }

       
    }
}