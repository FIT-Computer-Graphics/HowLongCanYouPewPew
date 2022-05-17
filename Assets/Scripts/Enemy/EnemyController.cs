using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public int health;
    public int scoreValue = 1;
    public ScoreKeeper score;
    public ParticleSystem explosionEffect;
    public AudioClip[] explosionSounds;
    public Image enemyMarker;
    private Camera mainCam;

    private void Awake()
    {
        score = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        var canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        // enemy marker as child of canvas
        enemyMarker = Instantiate(enemyMarker, canvas);
        enemyMarker.transform.SetParent(canvas);
    }

    private void Update()
    {
        // Display an enemyMarker for each enemy.
        CalculateMarker();
    }

    private void CalculateMarker()
    {
        var screenPos = mainCam.WorldToScreenPoint(gameObject.transform.position);

        if (screenPos.z <= 0f) screenPos *= -1f;

        enemyMarker.transform.localScale =
            screenPos.x < 0 || screenPos.y < 0 || screenPos.x > Screen.width || screenPos.y > Screen.height
                ? new Vector3(0.6f, 0.6f, 0.6f)
                : new Vector3(0.3f, 0.3f, 0.3f);
        var image = enemyMarker.gameObject.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b,
            screenPos.x < 0 || screenPos.y < 0 || screenPos.x > Screen.width || screenPos.y > Screen.height
                ? 0.8f
                : 0.1f);

        switch (screenPos.x)
        {
            // if off screen put to edge closest
            case < 0:
                screenPos.x = 0 + Screen.width * 0.09f;
                break;
            default:
            {
                if (screenPos.x > Screen.width) screenPos.x = Screen.width - Screen.width * 0.09f;

                break;
            }
        }

        switch (screenPos.y)
        {
            case < 0:
                screenPos.y = 0 + Screen.height * 0.09f;
                break;
            default:
            {
                if (screenPos.y > Screen.height) screenPos.y = Screen.height - Screen.height * 0.09f;

                break;
            }
        }

        enemyMarker.transform.position = screenPos;
        enemyMarker.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health > 0) return;

        score.score += scoreValue;
        score.enemiesKilled += 1;
        PlayAudio();
        PlayExplosionEffect();
        Destroy(enemyMarker.gameObject);
        Destroy(gameObject);
    }

    private void PlayExplosionEffect()
    {
        var explosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
        explosion.Emit(1);
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