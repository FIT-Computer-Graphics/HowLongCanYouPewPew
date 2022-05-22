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

    public AudioClip[] explosionSounds;

    [FormerlySerializedAs("ScoreBoard")] public GameObject spaceboard;
    public GameObject spaceShip;
    private bool isDead;

    private float timeRemaining = 3f;

    private void Update()
    {
        if (spaceShip) return;

        player.GetComponent<SpaceShipController>().enabled = false;
        if (timeRemaining > 0) timeRemaining -= Time.deltaTime;
        else spaceboard.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        var explosion = Instantiate(effect, player.transform.position, Quaternion.identity);
        PlayAudio();
        Destroy(spaceShip);
        Destroy(explosion, 1.5f);
        Cursor.visible = true;
    }

    private void PlayAudio()
    {
        var audioSource =
            Instantiate(new GameObject().AddComponent<AudioSource>(), transform.position, Quaternion.identity);
        audioSource.spatialBlend = 1;
        audioSource.volume = 1;
        audioSource.spread = 360;
        audioSource.PlayOneShot(explosionSounds[Random.Range(0, explosionSounds.Length)]);
        Destroy(audioSource.gameObject, 5f);
    }
}