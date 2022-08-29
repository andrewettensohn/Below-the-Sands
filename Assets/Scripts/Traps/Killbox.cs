using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject == null) return;
        Destroy(collider.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.gameObject == null) return;
        Destroy(collider.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject == null) return;

        Destroy(collider.gameObject);
    }
}
