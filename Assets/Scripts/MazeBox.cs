using System.Collections;
using UnityEngine; 

public class MazeBox : MonoBehaviour
{
    Vector3 size; SpriteRenderer sprite; new Collider2D collider; Coroutine coroutine;
    private void Start()
    {
        size = transform.localScale;
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        Player.OnPlayerRespawn += Reset;
    }
    private void Reset()
    {
        if(coroutine != null)
            StopCoroutine(coroutine);
        collider.enabled = true;
        sprite.enabled = true;
        sprite.color = Color.white;
        transform.localScale = size;
    }
    private void OnDestroy()
    {
        Player.OnPlayerRespawn -= Reset;
    }
    IEnumerator DeathAnim()
    {
        float stamp = Time.time + 0.2f; collider.enabled = false;
        while (stamp > Time.time)
        {
            transform.localScale = Mathf.Lerp(1,1.5f,1 + 5 * (Time.time - stamp)) * size;
            sprite.color = new Color(1, 1, 1, (stamp - Time.time) * 5f);
            yield return new WaitForFixedUpdate();
        }
        
        sprite.enabled = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) return;
        coroutine = StartCoroutine(DeathAnim());
    }
}
