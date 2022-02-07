using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    public Sprite healthy;

    public Sprite unhealthy;

    private Image image;

    public bool isHealthy { get; private set; } = true;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        image.sprite = healthy;
    }

    public void SetUnhealthy()
    {
        image.sprite = unhealthy;
        isHealthy = false;
    }

    public void SetHealthy()
    {
        image.sprite = healthy;
        isHealthy = true;
    }
}
