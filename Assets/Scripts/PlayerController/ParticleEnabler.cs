using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ParticleEnabler : MonoBehaviour
{
    public GameObject leftThruster;
    public GameObject rightThruster;

    private void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            leftThruster.SetActive(true);
            rightThruster.SetActive(false);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            rightThruster.SetActive(true); 
            leftThruster.SetActive(false);
        }
       else if (!Input.GetKeyDown(KeyCode.D)||!Input.GetKeyDown(KeyCode.A))
        {
            leftThruster.SetActive(false);
            rightThruster.SetActive(false);
        }

    }
}
