using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeOutOfBounds : MonoBehaviour
{
    new Collider2D collider;
    float stamp;
    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }
    private void FixedUpdate()
    {
        if (!collider.bounds.Contains(Player.self.transform.position) & stamp < Time.time) 
        { 
            Player.Kill(); stamp = Time.time + 3f;
        }
    }
}
