using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform enemy;
    public int health;
    public float speed;
    public Transform player;
    public bool isMoving;
    private Vector3 newPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {

    }

    private void MoveRandomly()
    {
        // move towards a random point around the enemy
        // wait until the enemy is at the new position
        // then move again
        if (isMoving)
        {
            enemy.position = Vector3.MoveTowards(enemy.position, newPosition, speed * Time.deltaTime);
            enemy.LookAt(newPosition);
            if (enemy.position == newPosition)
            {
                isMoving = false;
            }
        }
        else
        {
            float angle = Random.Range(0, 45);
            float distance = Random.Range(5, 50);
            newPosition = enemy.position + Quaternion.AngleAxis(angle, Vector3.up) * enemy.forward + Quaternion.AngleAxis(angle, Vector3.right) * enemy.forward * distance;

            isMoving = true;
        }
        
    }
    

    private void Move()
    {
        FlyTowardPlayer();
        LookAtPlayer();
    }
    
    private void FlyTowardPlayer(){
        enemy.position = Vector3.MoveTowards(enemy.position, player.position, speed * Time.deltaTime);
    }

    private void LookAtPlayer()
    {
        Vector3 dir = player.position - enemy.position;
        dir.y = 0;
        enemy.rotation = Quaternion.LookRotation(dir);
        //enemy.LookAt(player);
    }
    

    

    }

