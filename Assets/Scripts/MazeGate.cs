using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MazeGate : MonoBehaviour
{
    public string key;
    AudioSource aud;
    Transform gateObject;
    Vector3 scaleHold;
    SpriteMask signal;
    public int Signal { get; }
    public void Trigger(bool i) { }
    public void Toggle() { }
    private void Start()
    {
        aud = GetComponent<AudioSource>();
        signal = GetComponent<SpriteMask>();
        gateObject = transform.GetChild(0);
        Player.OnPlayerRespawn += Reset;
        MazeKey.OnKeyRecieved += CheckForKey;
        signal.enabled = true;
        scaleHold = gateObject.localScale;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            {
                Collider2D[] col = GetComponentsInChildren<Collider2D>();
                foreach (Collider2D c in col) c.enabled = false;
                aud.Play();
                signal.enabled = false;
                StartCoroutine(AnimOpen());
            }
        }
    }
    IEnumerator AnimOpen()
    {
        float hold = gateObject.localScale.y;
        float stamp = Time.time + 1;
        while (stamp > Time.time)
        {
            float num = Mathf.Sin((Time.time - stamp) * Mathf.PI / 2);
            gateObject.localScale = new Vector2(scaleHold.x, num * hold);
            yield return new WaitForFixedUpdate();
        }
        gateObject.localScale = Vector2.zero;
    }
    private void Reset()
    {
        StopCoroutine(AnimOpen());
        gateObject.localScale = scaleHold;
        Collider2D[] col = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in col) c.enabled = true;
    }
    private void OnDestroy()
    {
        Player.OnPlayerRespawn -= Reset;
        MazeKey.OnKeyRecieved -= CheckForKey;
    }
    void CheckForKey(string checkKey, bool active)
    {
        if (checkKey == key) signal.enabled = !active;
    }
}
/// <summary>
/// Static class for things when C# is a b****
/// </summary>
static class AAGNStaticClass
{
    public static Dictionary<string, Triggerable> Triggerables = new Dictionary<string, Triggerable>();
}

