using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BoostPad : MonoBehaviour
{
    [LevelEditor]
    public float regenTime = -1;
    [LevelEditor]
    public float boostPower = 1;
    [LevelEditor]
    public BoostType type;
    [LevelEditor(Description = "Should the boost pad add velocity, or set it?")]
    public bool isAdditive;
    [LevelEditor(Description = "Number of uses the boost pad has before disabling")]
    public byte uses;
    [LevelEditor]
    public float cooldown;
    public enum BoostType
    {
        NonDirectVelocity, NonDirectRotation, Direct
    }
    public Sprite NonDirectVelocity;
    public Sprite NonDirectRotation;
    public Sprite Direct;
    public GameObject fx;
    AudioSource source;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<ICustomDynamic>() is ICustomDynamic dyna )
        {
            Vector2 dir = Vector2.zero;
            switch (type)
            {
                case BoostType.NonDirectVelocity:
                    dyna.ApplyVelocity(isAdditive, dir = dyna.velocity.normalized * boostPower);
                    break;
                case BoostType.NonDirectRotation:
                    dyna.ApplyVelocity(isAdditive, dir = collision.gameObject.transform.up * boostPower);
                    break;
                case BoostType.Direct:
                    dyna.ApplyVelocity(isAdditive, dir = transform.right * boostPower); 
                    break;
            }
            Instantiate(fx, transform.position, Quaternion.LookRotation(-transform.forward, dir.normalized));
            source.Play();
            source.pitch = Random.Range(0.75f, 1.25f);
        }
    }
    private void Start() => source = GetComponent<AudioSource>();

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, boostPower / 2, LayerMask.GetMask("Wall"));
        Gizmos.DrawRay(transform.position, transform.right * (hit ? hit.distance : boostPower / 2));
    }

    public void OnValidate()
    {
        if (GetComponent<SpriteRenderer>() is SpriteRenderer s)
        {
            switch (type)
            {
                case BoostType.NonDirectVelocity:
                    s.sprite = NonDirectVelocity;
                    break;
                case BoostType.NonDirectRotation:
                    s.sprite = NonDirectRotation;
                    break;
                case BoostType.Direct:
                    s.sprite = Direct;
                    break;
            }
        }
    }
}
