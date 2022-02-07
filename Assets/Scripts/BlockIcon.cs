using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockIcon : MonoBehaviour
{
    private Image image;

    private Color originalColor;

    public bool isBlocking { get; private set; } = true;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        originalColor = image.color;
    }

    public void SetBroken()
    {
        image.color = Color.gray;
        isBlocking = false;
    }

    public void SetBlocking()
    {
        image.color = originalColor;
        isBlocking = true;
    }
}
