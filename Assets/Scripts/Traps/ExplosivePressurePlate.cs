using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePressurePlate : MonoBehaviour
{

    public ParticleSystem explosiveEffect;
    public float explosionEffectTime;
    public AudioClip explosionAudioClip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            Player player = collision.GetComponent<Player>();
            player.OnDeltDamage(-1, true);

            audioSource.PlayOneShot(explosionAudioClip);
            explosiveEffect.Play();
            StartCoroutine(nameof(HandleBlessedTimer));
        }
    }

    private IEnumerator HandleBlessedTimer()
    {
        yield return new WaitForSeconds(explosionEffectTime);
        explosiveEffect.Stop();
    }
}
