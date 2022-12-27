using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MazeKey : MonoBehaviour
{
    public string keyName;
    AudioSource aud;
    public bool resetOnDeath = false; LineRenderer resetIndicator;
    static void Reset(Scene scene, LoadSceneMode mode) { }//=> keys.Clear();
    private void Start()
    {
        aud = GetComponent<AudioSource>();
        //if (!hasSubbed) SceneManager.sceneLoaded += Reset;
        resetIndicator = GetComponent<LineRenderer>();
        //if(!keys.ContainsKey(keyName)) keys.Add(keyName, false);
        if (resetOnDeath)
        {
            Player.OnPlayerRespawn += ResetKey;
        }
        resetIndicator.enabled = resetOnDeath;
    }
    private void FixedUpdate()
    {
        float reusedX, reusedY; // Some effeciency stuff
        transform.eulerAngles = new Vector3(0, 0, (reusedX = Mathf.Sin(Time.time)) * 25); //'wiggle' key Animation
        if (resetOnDeath)
        {
            Vector3 point = new Vector3(reusedX * 0.4f, (reusedY = Mathf.Cos(Time.time)) * 0.4f, 0); //Animate Indicator
            resetIndicator.SetPositions(new Vector3[4]
                { point,
                new Vector3(point.y, -point.x, 0),
                -point,
                new Vector3(-point.y, point.x, 0) }
                );
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            aud.Play();
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            //keys[keyName] = true;
            OnKeyRecieved(keyName, true);
        }
    }
    void ResetKey() //Reset key on death (if enabled)
    {
        GetComponent<Collider2D>().enabled = GetComponent<SpriteRenderer>().enabled = true;
        //keys[keyName] = false;
        OnKeyRecieved(keyName, false);
    }
    private void OnDestroy()
    {
        if (resetOnDeath) Player.OnPlayerRespawn -= ResetKey;
    }
    public delegate void Empty(string keyName, bool active);
    public static event Empty OnKeyRecieved;

}
