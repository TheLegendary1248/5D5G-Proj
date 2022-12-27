using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatFloor : MonoBehaviour
{
    //This might have been entirely cosmectic lol


    // Start is called before the first frame update
    /*
    new Collider2D collider;
    public static Collider2D[] allInst;
    static bool isSafe = false; static bool alreadyUnchecked = false;
    private void Start()
    {
        collider = GetComponent<Collider2D>(); Player.OnPlayerRespawn += Check;
        
    }
    private void OnDestroy() => Player.OnPlayerRespawn -= Check;
    void Check()
    {
        if (!Player.isFloor && collider.OverlapPoint(Player.self.transform.position)) Player.isFloor = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Player.isFloor = collider.OverlapPoint(Player.self.transform.position);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Player.isFloor = false;
    }
    */
}
