using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    static GameObject currentTab;
    public GameObject linkedTab;
    bool isOver;
    float interval;
    Image image; float alpha = 0; Vector3 origin;
    public float moveVar = 10; public float offset = 5; public float locOffset = 0.05f;
    public void OnPointerEnter(PointerEventData dat)
    {
        isOver = true;
        if (currentTab) currentTab.SetActive(false);
        currentTab = linkedTab;
        linkedTab.SetActive(true);
    }
    public void OnPointerExit(PointerEventData dat) => isOver = false;
    private void Update()
    {
        transform.localScale = Vector2.Lerp(new Vector2(0.7f, 0.7f), Vector2.one, interval = Mathf.Clamp01(interval += 8f * (isOver ? Time.deltaTime : -Time.deltaTime)));
        Color hold = image.color; hold.a = alpha * interval; image.color = hold; Vector3 copy = origin; float f = 0;
        if (!isOver && Camera.main.WorldToScreenPoint(transform.position).y > Input.mousePosition.y) f = locOffset;
        else if(!isOver) f = -locOffset;
        copy.y += (Input.mousePosition.y / Screen.height * moveVar) + offset + f;
        transform.position = Vector3.Lerp(transform.position,origin + (Vector3.up * copy.y), Time.deltaTime * 5);
        
    }
    private void FixedUpdate()
    {
        //Debug.Log($"{Camera.main.WorldToScreenPoint(transform.position + (Vector3.up * size)).y} / {Input.mousePosition.y} / {Camera.main.WorldToScreenPoint(transform.position + (Vector3.down * size)).y}");
    }
    private void Start()
    {
        image = GetComponent<Image>();
        alpha = image.color.a;
        Color hold = image.color; hold.a = 0; image.color = hold;
        origin = transform.position;
    }
}
