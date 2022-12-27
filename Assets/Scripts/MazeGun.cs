using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MazeGun : MonoBehaviour
{
    float startAngle;
    public float range;
    bool useLinearRotation = false;
    bool alwaysfire = false;
    public byte directionRange = 255;
    /// <summary>
    /// The directional range of the gun, equal to directionRange(the actual internal value) + 1 because a 0 is useless
    /// </summary>
    public ushort DirectionRangeGetter { get => (ushort)(directionRange + 1); }
    ushort ammo;//also not yet
    /// <summary>
    /// Multiply a byte by this to convert it's ratio to 256 to a 360 angle(if that makes sense)
    /// </summary>
    public const float byteToAngle = 1.40625f;

    public GameObject projectile; public float projectileSizeMulti, projectileSpeedMulti, projectileLifetimeMulti = 1f;
    public Vector2 startingNormal;
    LineRenderer sight;
    Coroutine shootCo; Coroutine sightCo; bool anim = false;
    public float Rate = 0.5f;
    private void Start()
    {
        startAngle = transform.localEulerAngles.z;
        sight = GetComponent<LineRenderer>();
        sight.SetPosition(0, transform.position);
        sight.SetPosition(1, transform.position);
        startingNormal = transform.up;
        Player.OnPlayerRespawn += Stop;
    }
    /// <summary>
    /// Call this function before rotating the gun if you want the starting normal to rotate with any rotation you may apply. 
    /// Calling will allow the gun script to record the starting rotation, then apply the difference when After
    /// </summary>
    public void BeforeRotation() { }
    /// <summary>
    /// Call this function after rotating the gun and if you have called BeforeRotation
    /// </summary>
    public void AfterRotation() { }
    void FixedUpdate()
    {
        Vector2 dif = Player.position - (Vector2)transform.position;
        if (
            (range <= 0 || (Player.position - (Vector2)transform.position).sqrMagnitude < (range * range)) //Check within range
            && (directionRange == 255 ? true : Mathf.Acos( Vector2.Dot(startingNormal, dif) / (startingNormal.magnitude * dif.magnitude )) * Mathf.Rad2Deg < DirectionRangeGetter * byteToAngle / 2f) //Check within look range
            && (Physics2D.LinecastAll(transform.position, Player.position, LayerMask.GetMask("Wall")).Length == 0)) //Check if within sight
        {
            
            float lerp = Mathf.LerpAngle(Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg - 90f, transform.eulerAngles.z, 0.9f);
            transform.eulerAngles = new Vector3(0, 0, lerp);
            sight.SetPosition(0, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, range > 0f ? range : Mathf.Infinity, LayerMask.GetMask("Wall", "Player"));
            sight.SetPosition(1, hit ? (Vector3)hit.point : transform.up * range + transform.position);
            sight.startColor = new Color(1, 1, 1, 0.25f); //Figure out how to simplfy
            sight.endColor = new Color(1, 1, 1, 0.25f);
            anim = true;
            if (shootCo == null) shootCo = StartCoroutine(Shoot());
            if (sightCo != null) StopCoroutine(sightCo);
        }
        else if (anim) { anim = false; sightCo = StartCoroutine(AnimSight()); }
    }
    private void OnDrawGizmos()
    {
        if (range <= 0) return;
        Gizmos.color = new Color(0, 1, 1, 0.1f);
        Gizmos.DrawWireSphere(transform.position, range);
    }
    private void OnDestroy() => Player.OnPlayerRespawn -= Stop;
    void Stop()
    {
        if (shootCo != null) {StopCoroutine(shootCo); shootCo = null; }
        transform.localEulerAngles = new Vector3(0, 0, startAngle);
    }
    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(Rate);
        GameObject inst = Instantiate(projectile, transform.position, transform.rotation);
        inst.transform.localScale = inst.transform.localScale * projectileSizeMulti;
        inst.GetComponent<Bullet>().Speed *= projectileSpeedMulti;
        shootCo = null; 
    }
    IEnumerator AnimSight()
    {
        float stamp = Time.time + 0.5f;
        while(Time.time < stamp)
        {
            yield return new WaitForFixedUpdate();
            sight.startColor = new Color(1, 1, 1, 1f * stamp - Time.time);
            sight.endColor = new Color(1, 1, 1, 1f * stamp - Time.time);
        }
        sight.endColor = Color.clear;
        sightCo = null;
    }
}
[CustomEditor(typeof(MazeGun))]
public class GunHelper : Editor
{
    public void OnSceneGUI()
    {
        var t = target as MazeGun;
        Handles.color = new Color(0,1,1,0.05f);
        //Rotation of up vector
        float angle = t.DirectionRangeGetter * MazeGun.byteToAngle / 2f * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        Vector2 v = Application.isPlaying ? t.startingNormal : (Vector2)t.transform.up;
        Vector2 rotatedNormal = new Vector2((v.x * cos) + (v.y * sin), (v.x * -sin) + (v.y * cos));
        Handles.DrawSolidArc(t.transform.position, Vector3.forward, rotatedNormal, t.DirectionRangeGetter * MazeGun.byteToAngle, t.range < 0 ? 50 : t.range);
    }
}

