using System;
using Scripts.PlayerController;
using UnityEngine;

public class ParticleEnabler : MonoBehaviour
{
    public GameObject leftThruster;
    public GameObject rightThruster;
    public GameObject[] upperLeftThrusters;
    public GameObject[] upperRightThrusters;
    private SpaceShipController _spaceShipController;

    private void Awake()
    {
        _spaceShipController = gameObject.GetComponentInParent<SpaceShipController>();
    }

    private void Update()
    {
        HandleKeyboardThrusters();
        HandleMouseThrusters();
    }

    private void HandleMouseThrusters()
    {
        if (_spaceShipController.mouseDistance.y > 0.1)
        {
            leftThruster.SetActive(true);
            rightThruster.SetActive(true);
            foreach (var thruster in upperLeftThrusters) thruster.SetActive(false);
            foreach (var thruster in upperRightThrusters) thruster.SetActive(false);

        }
        else if (_spaceShipController.mouseDistance.y < -0.1)
        {
            leftThruster.SetActive(false);
            rightThruster.SetActive(false);
            foreach (var thruster in upperLeftThrusters) thruster.SetActive(true);
            foreach (var thruster in upperRightThrusters) thruster.SetActive(true);
        }
        else
        {
            foreach (var thruster in upperLeftThrusters) thruster.SetActive(false);
            foreach (var thruster in upperRightThrusters) thruster.SetActive(false);
            
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) return;
            leftThruster.SetActive(false);
            rightThruster.SetActive(false);
            
        }
    }

    private void HandleKeyboardThrusters()
    {
        if (Input.GetKey(KeyCode.D))
        {
            leftThruster.SetActive(true);
            rightThruster.SetActive(false);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rightThruster.SetActive(true);
            leftThruster.SetActive(false);
        }
        else if (!Input.GetKeyDown(KeyCode.D) || !Input.GetKeyDown(KeyCode.A))
        {
            leftThruster.SetActive(false);
            rightThruster.SetActive(false);
        }
    }
}