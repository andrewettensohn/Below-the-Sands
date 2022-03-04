using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePressurePlate : MonoBehaviour
{

    public ParticleSystem explosiveEffect;
    public float explosionEffectTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            Player player = collision.GetComponent<Player>();
            player.OnDeltDamage(-1, true);

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
