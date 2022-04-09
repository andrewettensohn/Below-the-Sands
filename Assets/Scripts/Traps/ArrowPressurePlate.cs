using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPressurePlate : MonoBehaviour
{
    public GameObject arrowPrefab;
    public AudioClip arrowFiredClip;
    public float arrowSpawnX;
    public float arrowSpawnY;
    public float arrowXFlightDirection;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            GameObject arrowGameObject = Instantiate(arrowPrefab, new Vector3(arrowSpawnX, arrowSpawnY, 0), Quaternion.identity);
            audioSource.PlayOneShot(arrowFiredClip);
            Arrow arrow = arrowGameObject.GetComponent<Arrow>();
            arrow.Launch(new Vector2(arrowXFlightDirection, 0), 1200);
        }
    }
}
