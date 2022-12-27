using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public float Life;
    public float SpeedMultiOnCollision = 1f;
    public AudioSource aud; //Must automate at some point via hard-code
    Rigidbody2D rb;
    public bool ignoreShape = true;
    Vector2 predictionCollision; 
    Vector2 lastpos;
    private void Start() 
    { 
        Player.OnPlayerRespawn += EarlyEnd; 
        GetComponent<TrailRenderer>().widthMultiplier *= transform.localScale.x; 
        StartCoroutine(Wait()); 
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * Speed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TrailRenderer T = GetComponent<TrailRenderer>();
        if (T) T.emitting = false;
        int c = 0;
        while (c < collision.contactCount)
        {
            Speed *= SpeedMultiOnCollision;
            Vector2 inDirection = rb.velocity.normalized * Speed;
            Vector2 inNormal = collision.contacts[c].normal;
            Vector2 newVelocity = Vector2.Reflect(inDirection, inNormal);
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(newVelocity.y, newVelocity.x) * Mathf.Rad2Deg - 90f);
            rb.velocity = newVelocity;
            rb.MovePosition(collision.GetContact(c).point + (inNormal * collision.GetContact(c).separation));
            c++;
        }
        //rb.MovePosition((Vector2)rb.position + newVelocity * Time.fixedDeltaTime);
        if (T) T.emitting = true;
        aud.Play();
    }
    void EarlyEnd() { StopCoroutine(Wait()); StopCoroutine(End()); StartCoroutine(End()); Player.OnPlayerRespawn -= EarlyEnd; }
    IEnumerator Wait() { yield return new WaitForSeconds(Life); StartCoroutine(End()); }
    IEnumerator End()
    {
        GetComponent<Rigidbody2D>().simulated = false;
        Speed = 0;
        if(GetComponent<TrailRenderer>() is TrailRenderer trail)
        {
            yield return new WaitForSeconds(trail.time);
        }
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        Player.OnPlayerRespawn -= EarlyEnd;
    }
}
