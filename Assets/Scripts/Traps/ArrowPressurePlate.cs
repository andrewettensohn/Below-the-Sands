using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPressurePlate : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float arrowSpawnX;
    public float arrowSpawnY;
    public float arrowXFlightDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            GameObject arrowGameObject = Instantiate(arrowPrefab, new Vector3(arrowSpawnX, arrowSpawnY, 0), Quaternion.identity);

            Arrow arrow = arrowGameObject.GetComponent<Arrow>();
            arrow.Launch(new Vector2(arrowXFlightDirection, 0), 1000);
        }
    }
}
