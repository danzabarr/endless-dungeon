using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRaycaster : MonoBehaviour
{
    private static UIRaycaster instance;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    private List<RaycastResult> resultAppendList;
    private int resultCount;

    private void Awake()
    {
        instance = this;
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        resultAppendList = new List<RaycastResult>();
    }

    void Update()
    {
        resultAppendList.Clear();

        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        raycaster.Raycast(eventData, resultAppendList);
        resultCount = resultAppendList.Count;
        if (OverUI)
            Debug.Log("yo");
    }

    public static bool OverUI => instance.eventSystem.IsPointerOverGameObject() || instance.eventSystem.currentSelectedGameObject;//instance.resultCount > 0;
    public static GameObject UIElement => instance.resultCount <= 0 ? null : instance.resultAppendList[0].gameObject;
}
