using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, ICustomDynamic
{
    public static Vector2 spawn;
    public static Player self;
    public static Vector2 position;
    public GameObject DeathFX;
    float sizeLerp = 0;
    float height = 1f; public static bool isJumping = false; bool jumpKillFrame = false;
    public static bool isFloor = false; public static bool isDead = false;
    public float Speed;
    public float SpeedCap;
    public float SpeedReduce;
    public float JumpSpeedReduce;
    public float JumpSpeed;
    public float JumpTime;
    public AudioSource windSrc;
    public AudioSource jumpSrc;
    public AudioClip deathAud;
    public Vector2 velocity { get; set; }
    void Start()
    {
        self = this;
        spawn = transform.position;
        OnPlayerDeath += Death;
    }
    private void FixedUpdate()
    {
        if (isDead) return;
        //Velocity Calculation
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        velocity += move * (isJumping ? (JumpSpeed * JumpSpeedReduce) : (Speed * SpeedReduce));
        velocity /= (isJumping ? JumpSpeedReduce : SpeedReduce) + 1f;
        if (velocity.sqrMagnitude > SpeedCap * SpeedCap) velocity = velocity.normalized * SpeedCap;
        transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
        
        //Point in velocity normal calculation
        Vector2 dif = Vector2.Lerp(transform.up, move, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg - 90f);
        //Size calculation from jump height
        sizeLerp = Mathf.Lerp(sizeLerp, move.magnitude, 0.17f);
        transform.localScale = (Vector3)Vector2.Lerp(Vector2.one * 0.18f, new Vector2(0.13f, 0.3f), sizeLerp) * height + new Vector3(0,0,1);
        //Misc
        windSrc.volume = velocity.sqrMagnitude / Mathf.Pow(Speed * 1.5f, 2);
        position = transform.position;
        if (Input.GetKey(KeyCode.Space) & !isJumping & !jumpKillFrame) StartCoroutine(JumpAnim());
    }
    //Implement pause later
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseScreen.Pause();//Change
        }
    } */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnPlayerDeath();
    }
    void Death() => StartCoroutine(DeathAnim()); 
    public static void Kill() => OnPlayerDeath();
    IEnumerator DeathAnim()
    {
        GetComponent<Rigidbody2D>().simulated = false; GetComponent<SpriteRenderer>().enabled = false; isDead = true; velocity = new Vector2(0, 0);
        windSrc.volume = 0; AudioSource.PlayClipAtPoint(deathAud, transform.position, 0.6f);
        Instantiate(DeathFX, transform.position, transform.rotation);
        yield return new WaitForSeconds(1f);
        transform.position = spawn;
        GetComponent<Rigidbody2D>().simulated = true; 
        GetComponent<SpriteRenderer>().enabled = true;  
        isDead = false; 
        OnPlayerRespawn();
    }
    IEnumerator JumpAnim()
    {
        float stamp = Time.time; isJumping = true; jumpSrc.Play(); jumpSrc.pitch = Random.Range(0.8f, 1.2f);
        while(stamp + 0.6f > Time.time)
        {
            yield return new WaitForFixedUpdate();
            height = Mathf.Sin((Time.time - stamp) / 0.6f * Mathf.PI) * 0.65f + 1;
        }
        height = 1; isJumping = false; jumpKillFrame = true;
        yield return new WaitForFixedUpdate(); jumpKillFrame = false;
    }
    private void OnDestroy()
    {
        OnPlayerDeath -= Death;
        height = 1; isJumping = false;
    }
    public void ApplyVelocity(bool isAdditive, Vector2 vel)
    {
        if (isAdditive) velocity += vel;
        else velocity = vel;
    }
    public delegate void Empty();
    public static event Empty OnPlayerDeath = delegate { };
    public static event Empty OnPlayerRespawn = delegate { };
}
interface ICustomDynamic
{
    Vector2 velocity { get; set; }
    void ApplyVelocity(bool isAdditive, Vector2 vel);
}
