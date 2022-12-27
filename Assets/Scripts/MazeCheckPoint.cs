using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCheckPoint : MonoBehaviour
{
    GameObject fill; bool active = false; ParticleSystem PSys;
    private void Start()
    {
        fill = transform.GetChild(0).gameObject;
        PSys = GetComponent<ParticleSystem>();
        ResetPoints += Reset;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") & !active)
        {
            ResetPoints();
            active = true;
            Player.spawn = transform.position;
            PSys.Play();
            StartCoroutine(AnimSprite());
        } 
    }
    IEnumerator AnimSprite()
    {
        float stamp = Time.time + 1f;
        while (stamp > Time.time) 
        {
            fill.transform.localPosition = Vector2.Lerp(Vector2.zero,Vector2.down, stamp - Time.time);
            yield return new WaitForFixedUpdate();
        }
        fill.transform.localPosition = Vector2.zero;
    }
    private void OnDestroy()
    {
        ResetPoints -= Reset;
    }
    private void Reset() { StopCoroutine(AnimSprite()); fill.transform.localPosition = Vector2.down; active = false; }
    delegate void Empty();
    static event Empty ResetPoints;
}
