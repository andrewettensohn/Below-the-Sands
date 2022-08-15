using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicRift : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private new CircleCollider2D collider;
    public float lifetime;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (transform.position.magnitude > 45.0f)
        {
            rigidbody2d.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == GameManager.instance.playerCharacterName)
        {
            Player player = collision.GetComponent<Player>();

            if (player == null) return;

            player.OnDeltDamage(-1);
            rigidbody2d.velocity = Vector2.zero;
        }
    }

    public void StartLifeTimeTimer()
    {
       StartCoroutine(HandleLifetimeTimer()); 
    }

    protected virtual IEnumerator HandleLifetimeTimer()
    {
        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
    }
}
