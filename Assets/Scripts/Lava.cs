using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    // Start is called before the first frame update
    new Collider2D collider;
    LineRenderer line; float hold;
    private void Start()
    {
        collider = GetComponent<Collider2D>();
        line = GetComponent<LineRenderer>();
        hold = line.widthMultiplier;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") & !Player.isJumping)
        {
            if(Physics2D.OverlapPointAll(Player.position, LayerMask.GetMask("FloatFloor")).Length > 0) return;
            if (!collider.OverlapPoint(Player.position)) return;
            Player.Kill();
        }
    }
    private void FixedUpdate()
    {
        line.widthMultiplier = (Mathf.Sin(Time.time) + 4f) * 0.25f * hold; 
    }
}
