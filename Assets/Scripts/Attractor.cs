using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour, ICustomDynamic
{
    Vector2 spawn;
    public Vector2 velocity { get; set; }
    public float Speed, SpeedCap, SpeedReduce;
    [LevelEditor(Description = "Should this object follow the target when it sees it, or always follow?")]
    public bool followTargetOnSight;
    [LevelEditor(Description = "Should this object require a trigger to activate")]
    //Remove all three below
    public bool useTrigger;
    public bool triggered { get; set; }
    public void Trigger(bool isTriggered) => triggered = isTriggered;
    private void Start() { spawn = transform.position;  Player.OnPlayerRespawn += ResetAttractor; }
    void ResetAttractor() { transform.position = spawn; velocity = new Vector2(); triggered = false; }
    void OnDestroy() => Player.OnPlayerRespawn -= ResetAttractor;
    private void FixedUpdate()
    {
        if ((useTrigger ? triggered : true) && !followTargetOnSight || Physics2D.LinecastAll(transform.position, Player.self.transform.position, LayerMask.GetMask("Wall")).Length == 0)
        {
            Vector2 move = (Player.self.transform.position - transform.position).normalized;
            velocity += move * Speed * (SpeedReduce - 1f); 
        }
        velocity /= SpeedReduce;
        if (velocity.sqrMagnitude > SpeedCap * SpeedCap) velocity = velocity.normalized * SpeedCap;
        transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
    }
    private void OnCollisionEnter2D(Collision2D collision) //FIX THISSSSSSSSSSSSSSSSSSSSS
    {
        Vector2 inNormal = collision.contacts[0].normal;
        velocity = Vector2.Reflect(velocity, inNormal) * 1f;
        transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Vector2 inNormal = collision.contacts[0].normal;
        //velocity = Vector2.Reflect(velocity, inNormal);
        //transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
        //transform.Translate(collision.normal, Space.World);
    }
    public void ApplyVelocity(bool isAdditive, Vector2 vel)
    {
        if (isAdditive) velocity += vel;
        else velocity = vel;
    }
    
}
