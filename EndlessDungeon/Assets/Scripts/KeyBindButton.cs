using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyBindButton : MonoBehaviour
{
    private EventSystem eventSystem;
    private Button button;
    public KeyCode key;

    // Start is called before the first frame update
    void Awake()
    {
        eventSystem = EventSystem.current;
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerDownHandler);
        }

        if (Input.GetKeyUp(key))
        {
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerUpHandler);
        }

        //button.onClick.Invoke();
    }

    public void LogPress()
    {
        Debug.Log(key + " pressed");
    }
}
