using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFlameStrike : MonoBehaviour
{
    public GameObject flameStrikePrefab;
    public float flameStrikeCooldown;
    private bool canFlameStrike = true;
    private Enemy enemy;

    // Start is called before the first frame update
    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(enemy.isPlayerDetected)
        {
            DoFlameStrike();
        }
    }

    private void DoFlameStrike()
    {
        if(!canFlameStrike) return;

        canFlameStrike = false;

        GameObject flameStrikeGameObject = Instantiate(flameStrikePrefab, new Vector2(enemy.target.position.x, enemy.target.position.y + 1.5f), Quaternion.identity);
        VerticalTrap trap = flameStrikeGameObject.GetComponent<VerticalTrap>();
        trap.StartLifeTimeTimer(1f);

        StartCoroutine(HandleFlameStrikeCooldown());
    }

    protected virtual IEnumerator HandleFlameStrikeCooldown()
    {
        yield return new WaitForSeconds(flameStrikeCooldown);

        canFlameStrike = true;
    }
}
