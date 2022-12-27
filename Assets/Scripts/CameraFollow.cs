using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Coroutine cor; float smoothed = 0;
    void Start() => Player.OnPlayerDeath += StartShake; void OnDestroy() => Player.OnPlayerDeath -= StartShake;
    private void FixedUpdate()
    {
        if(!Player.self)return;
        transform.position = (Vector3)Vector2.Lerp(transform.position, Player.self.transform.position, 0.07f) + -new Vector3(0,0, 4 + 
            (smoothed = Mathf.Lerp(smoothed, Player.self.velocity.magnitude / Player.self.Speed, 0.1f)) * 2f);
    }
    public void StartShake() { if (cor != null) StopCoroutine(cor); cor = StartCoroutine("Shake"); }//make this more user-friendly
    IEnumerator Shake()
    {
        int i = 1; Vector3 dif = Random.insideUnitCircle * 0.2f; transform.position += dif;
        while(i < 20)
        {
            yield return new WaitForFixedUpdate();
            transform.position += -dif;
            transform.position += dif = Random.insideUnitCircle * 0.2f * (1f - (i / 20f));
            i++;
        }
    }
}
