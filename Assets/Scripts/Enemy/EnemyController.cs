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
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(enemy.gameObject);
        }
    }


}

