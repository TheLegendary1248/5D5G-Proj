using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// A cool script to create EZ moving platforms with just a few buttons and alot of functions
/// </summary>
public class WayPath : MonoBehaviour //You should try to decipher and simplfy this to avoid future problems
{
    [LevelEditor(Description = "Return to start location on respawn?")]
    public bool returnOnRespawn;
    static float levelStartTime;
    /// <summary>
    /// Determines if the component should carry out it's function
    /// </summary>
    public bool isActive = true;
    /// <summary>
    /// Used to show gizmo's in the editor
    /// </summary>
    public bool showGizmo = true;
    /// <summary>
    /// Used to show Debug.Log() in the editor
    /// </summary>
    public bool showDebugInfo = false;
    public bool isLoop = false;
    public Color32 gizmoColor = Color.blue;
    public float timeOffset = 0;
    public float deActivationTimeStamp;
    bool isReverse = false; //Used as reference when looping
    float animationStartStamp = 0; //Used for short calculation
    public List<Waypoints> points = new List<Waypoints>(); //Maybe should create an float list as a buffer for position look-up 
    /// <summary>
    /// Gets total time to complete a single cycle
    /// </summary>
    public float TotalAnimTime //Find a way to avoid constant calc
    {
        get
        {
            if (points.Count < 2) return 0;
            float tot = 0;
            for (int l = 0; l < points.Count; l++) tot += points[l].TotalPointTime;
            if (!isLoop)
            {
                tot *= 2;
                tot -= points[points.Count - 1].TotalPointTime + points[points.Count - 1].time + points[0].waitTime;
            }
            return tot;
    }   }
    /// <summary>
    /// Gets total time of points added together
    /// </summary>
    public float TotalTime
    {
        get
        {
            float tot = 0;
            for (int l = 0; l < points.Count; l++) tot += points[l].TotalPointTime;
            return tot;
    }   }
    Waypoints current;
    Waypoints next;
    Coroutine pointpasser;
    int index = -1;
    float timeStamp;
    Rigidbody2D rb; bool rbExists;
    private void Start()
    {
        if (Mathf.Abs(timeOffset) > 0) SetAtTime(timeOffset);
        else pointpasser = StartCoroutine(ChangeWayPoint(0, false));
        animationStartStamp = Time.time - timeOffset;
        rbExists = (rb = GetComponent<Rigidbody2D>()) != null;
        if (points.Count >= 2)
        {
            if(showDebugInfo)Debug.Log($"{TotalAnimTime} / {TotalTime}");
            return;
        }
        else
        {
            if (Application.isEditor)
            {
                Debug.LogError($"Two or more points are required to work properly > {gameObject.name}");
                Debug.Break();
        }   }
    }
    /// <summary>
    /// Use this if you're recycling the component. It basically does all the hard work for you in an easier way(Not yet implemented)
    /// </summary>
    public void ReUse(List<Waypoints> points, float offset, bool isLoop, bool start) { }
    /// <summary>
    /// Used for changing points. I probably recommend not using or tampering with this
    /// </summary>
    /// <param name="offset"> Offset to be applied </param>
    /// <param name="suddenSwitch"> Are we suddenly moving somewhere else </param>
    IEnumerator ChangeWayPoint(float offset, bool suddenSwitch) //Simplify later
    {
        float minimalTimeDiff = 0;
        if (!suddenSwitch) //This part is to account for any time gain/loss from previous interpolation. Do not use if we're suddenly switching current spot in animation
        {                  //It also helps keep CurrentTime on track and sync the component to actual time
            if(showDebugInfo)Debug.LogWarning("Difference use"); 
            minimalTimeDiff = (Time.time - timeStamp) - (isReverse ? current.waitTime + next.time : current.TotalPointTime); //Used to account for minimal time loss/gain
            timeStamp = Time.time;
            timeStamp -= minimalTimeDiff;
        }
        else timeStamp = Time.time;
        index = ++index % (isLoop ? points.Count : ((points.Count - 1) * 2)); //Set index right
        if (index == 0) animationStartStamp = Time.time;
        int cur; int nex; isReverse = false;
        if(isReverse = (!isLoop && index >= points.Count - 1)) //Do check that looping is not enabled and index is larger than should be for reverse
        {
            cur = (points.Count * 2) - 2 - index;
            nex = Mathf.Abs(cur - 1);
        }
        else
        {
            cur = index;
            nex = (index + 1) % points.Count;
        }
        if(showDebugInfo) Debug.Log($"After : {index}  isReverse {isReverse} / Cur {cur} / Nex {nex} \nTime difference : {minimalTimeDiff}   CurTime : {CurrentTime}   AccurTime : {AccurateCurrentTime}");
        current = points[cur];
        next = points[nex];
        timeStamp -= offset;
        if(showDebugInfo)Debug.Log($"Offset : {offset} / Time on point {(isReverse ? current.waitTime + next.time : current.TotalPointTime) - (minimalTimeDiff + offset)}");
        yield return new WaitForSeconds((isReverse ? current.waitTime + next.time : current.TotalPointTime) - (minimalTimeDiff + offset));
        pointpasser = StartCoroutine(ChangeWayPoint(0, false));
    }
    void FixedUpdate()
    {
        if (!isActive) return;
        float timeEval = (Time.time - timeStamp - current.waitTime) / (isReverse ? next.time : current.time);
        bool useLinear = current.useLinear == next.useLinear ? current.useLinear : timeEval < 0.5 ? current.useLinear : next.useLinear; 
        float interpol = useLinear ? 1 - timeEval : (Mathf.Cos(Mathf.Max(timeEval,0) * Mathf.PI) + 1) / 2f;
        if (rbExists && rb.isKinematic) rb.MovePosition(Vector2.Lerp(next, current, interpol));
        else transform.localPosition = Vector2.Lerp(next, current, interpol);
    }
    /// <summary>
    /// Set to a specific time relative the start of animation (excluding timeOffset). Use AccurTime if you wish to add upon current time
    /// </summary>
    public void SetAtTime(float time) 
    {
        if(pointpasser != null) StopCoroutine(pointpasser);                                 //Stop current coroutine
        time = Mathf.Repeat(time ,TotalAnimTime);                                           //Clamp time within total range (allows negatives)
        int iter = 0; float tot = TotalTime - (isLoop ? 0 : points[points.Count - 1].time); //Create iterator and get correct total time of first half
        animationStartStamp = Time.time - time;
        if (isReverse = !isLoop & time >= tot) //If time has been detected to be larger than first half of unlooped cycle, do some magic
            { time += points[points.Count - 1].waitTime - tot; iter = 1; }
        while (true)                //Do the math to figure out which index we'll be on
        {
            float added = isReverse ? points[points.Count - iter].waitTime + points[points.Count - (iter + 1)].time : points[iter].TotalPointTime; //Get time for index
            if (time < added) break; else time -= added; iter++; //If the time remaining is less then the total time for this index break off to get 
        }
        index = isReverse ? points.Count - 3 + iter : iter - 1; //Se the index right
        pointpasser = StartCoroutine(ChangeWayPoint(time, true)); //Start process. Use suddenSwitch to allow coroutine to correctly apply changes
    }
    /// <summary>
    /// Used for testing. Remove later cuz lazy
    /// </summary>
    [System.Obsolete]
    public void Test(string f)
    {
        float num = 0;
        bool worked = float.TryParse(f, out num);
        if (worked)
        {
            SetAtTime(num);
        }
        else Debug.LogWarning("Failed to Parse");
    }
    public void ResetToStart() => SetAtTime(timeOffset);
    /// <summary>
    /// Gets the estimated location of the game object at a certain time relative to start of animation. Not yet implemented
    /// </summary>
    public Vector2 GetLocationAtTime(float time) //Simplify later
    {
        //Reuse algorithm used in SetAtTime//
        time = Mathf.Repeat(time, TotalAnimTime);                      
        int index; bool isReverse;                                    //Avoid class fields with locals
        int iter = 0; float tot = TotalTime - (isLoop ? 0 : points[points.Count - 1].time); //Override index with local
        animationStartStamp = Time.time - time;
        if (isReverse = !isLoop & time >= tot) 
        { time += points[points.Count - 1].waitTime - tot; iter = 1; }
        while (true)
        {
            float added = isReverse ? points[points.Count - iter].waitTime + points[points.Count - (iter + 1)].time : points[iter].TotalPointTime;
            if (time < added) break; else time -= added; iter++;
        }
        index = isReverse ? points.Count - 3 + iter : iter - 1;
        index = ++index % (isLoop ? points.Count : ((points.Count - 1) * 2));
        //Reuse algorithm used in ChangeWayPoint//
        int cur; int nex;                                     
        if (isReverse = !isLoop && index >= points.Count - 1) //Do check that looping is not enabled and index is larger than should be for reverse
        {
            cur = (points.Count * 2) - 2 - index;
            nex = Mathf.Abs(cur - 1);
        }
        else
        {
            cur = index;
            nex = (index + 1) % points.Count;
        }
        Waypoints current = points[cur], next = points[nex];
        //Reuse algorithm used in FixedUpdate
        float timeEval = (time - current.waitTime) / (isReverse ? next.time : current.time); 
        bool useLinear = current.useLinear == next.useLinear ? current.useLinear : timeEval < 0.5 ? current.useLinear : next.useLinear;
        float interpol = useLinear ? 1 - timeEval : (Mathf.Cos(Mathf.Max(timeEval, 0) * Mathf.PI) + 1) / 2f;
        return transform.parent != null ? (Vector2)transform.parent.TransformPoint(Vector2.Lerp(next, current, interpol)) : Vector2.Lerp(next, current, interpol);
    }
    /// <summary>
    /// Like GetLocationAtTime, but takes into consideration other WayPath's up the hierarchy. Not yet implemented, not likely either
    /// </summary>
    public Vector2 GetLocationAtTimeRelativeToHierarchy()
    {
        return new Vector2();
    }
    /// <summary>
    /// This property uses a short calculation and a variable set to Time.time every time the animation loops, very slightly inaccurate
    /// </summary>
    public float CurrentTime => Mathf.Min(Time.time - animationStartStamp, TotalAnimTime);
    /// <summary>
    /// This property returns an accurate representation of the current time in the animation. Much slower, determined by size of waypoint list
    /// </summary>
    public float AccurateCurrentTime //Simplify later
    {
        get
        {
            float time = 0;
            if (isReverse)
            {
                float total = 0;
                foreach (Waypoints pt in points) total += pt.TotalPointTime;
                total -= points[points.Count - 1].TotalPointTime; //DO NOT FUCKING TOUCH
                int i = (index - points.Count) + 2; int iter = 1;
                while(iter < i)
                {
                    total += points[points.Count - iter].waitTime + points[points.Count - (iter + 1)].time;
                    iter++;
                }
                total += Time.time - timeStamp;
                time = total;
            }
            else
            {
                int i = 0; float total = 0;
                while(i < index)
                    total += points[i++].TotalPointTime;
                total += Time.time - timeStamp;
                time = total;
            }
            return time;

        }
    }
    /// <summary>
    /// Use this property to set isLoop safely. Returns if isLoop can be set. Have not tested
    /// </summary>
    public bool SetIsLoop
    {
        get => !isReverse; 
        set => isLoop = isReverse ? isLoop : value;
    }
    #region Miscellaneous
    [RuntimeInitializeOnLoadMethod]
    void Add() => SceneManager.sceneLoaded += SetStartTimeToNow;
    static public void SetStartTimeToNow(Scene s, LoadSceneMode l) { levelStartTime = Time.time; }
    [System.Serializable]
    public struct Waypoints 
    {
        /// <summary>
        /// Point
        /// </summary>
        public Vector2 point;
        /// <summary>
        /// Time to move onto next point
        /// </summary>
        [Min(0.01f)]
        public float time;
        /// <summary>
        /// Time to wait at this point before moving on
        /// </summary>
        [Min(0f)]
        public float waitTime;
        /// <summary>
        /// Used to determine if using linear or smoothstep when interpolating thru point
        /// </summary>
        public bool useLinear;
        public float TotalPointTime { get { return time + waitTime; } }
        public static implicit operator Vector2(Waypoints w) => w.point;
        public Waypoints(Vector2 newPoint, float timeAtPoint, float waitTime, bool useLinear)
        {
            point = newPoint;
            time = timeAtPoint;
            this.waitTime = waitTime;
            this.useLinear = useLinear;
        }
    }
    public Vector2 Relative(Vector2 pt) => transform.parent != null ? (Vector2)transform.parent.TransformPoint(pt) : pt;
    private void OnDrawGizmos()
    {
        float Hue = 0f;
        if (points.Count >= 2 && showGizmo)
        {
            //Draw Lines, with linear lines colored half more hue
            int k = 0;
            Gizmos.color = Color.HSVToRGB((Hue += 0.05f) + (points[points.Count - 1].useLinear ? 0.5f : 0), 1, 1);
            if (isLoop) Gizmos.DrawLine(Relative(points[0].point), Relative(points[points.Count - 1].point));
            while (k < points.Count - 1)
            {
                Gizmos.color = Color.HSVToRGB((Hue += 0.05f) + (points[k].useLinear ? 0.5f : 0f), 1, 1);
                Gizmos.DrawLine(Relative(points[k].point), Relative(points[k + 1].point));
                k++;
            }
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(GetLocationAtTime(timeOffset), transform.lossyScale);
        }
    }
    private void OnValidate()
    {
        if (points.Count == 2 & isLoop == true) Debug.LogWarning($"isLooped will be switched to {isLoop = false} due to being less than 3 points");
    }
    #endregion
}
