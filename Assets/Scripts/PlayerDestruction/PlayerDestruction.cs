
using Scripts.PlayerController;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDestruction : MonoBehaviour
{
    [SerializeField] private GameObject Asteroid;

    [FormerlySerializedAs("Player")] [SerializeField]
    private GameObject player;

    [FormerlySerializedAs("Effect")] [SerializeField]
    private GameObject effect;

    private float timeRemaining = 3f;

    public GameObject ScoreBoard;
    public GameObject spaceShip;
    private void Update()
    {
        if (spaceShip)
        {
            return;
        }
        player.GetComponent<SpaceShipController>().enabled=false;
        if (timeRemaining > 0) timeRemaining -= Time.deltaTime;
        else
        {
            ScoreBoard.SetActive(true);
        }

           
    }

    private void OnCollisionEnter(Collision collision)
    {
           
       // if (collision.gameObject != Asteroid) return;
        Destroy(spaceShip);
        var explosion = Instantiate(effect, player.transform.position, Quaternion.identity);
        Destroy(explosion, 1.5f);
        Cursor.visible = true;
    
    }
}
