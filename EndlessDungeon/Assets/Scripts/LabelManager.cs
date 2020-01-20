using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LabelManager : MonoBehaviour
{
    private static LabelManager instance;

    [SerializeField]
    private ObjectLabel prefab;

    [SerializeField]
    private bool showAll;

    [SerializeField]
    public float tipSeparationRadialStepSize, tipSeparationRotationalStepSize;

    [SerializeField]
    public int tipSeparationMaxSteps;

    private Camera main;
    private Canvas canvas;
    private GraphicRaycaster graphicRaycaster;
    private Dictionary<Interactive, ObjectLabel> labels = new Dictionary<Interactive, ObjectLabel>();

    public void Awake()
    {
        instance = this;
        main = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    public static ObjectLabel Add(Interactive obj)
    {
        if (obj == null)
            return null;

        if (!obj.HasLabelText())
            return null;
        
        if (instance.labels.ContainsKey(obj))
            return instance.labels[obj];
        ObjectLabel label = Instantiate(instance.prefab, instance.transform);
        instance.labels.Add(obj, label);
        instance.Layout(label);
        return label;
    }
   
    public static void Remove(Interactive obj)
    {
        if (obj == null)
            return;

        if (instance.labels.TryGetValue(obj, out ObjectLabel label))
        {
            instance.labels.Remove(obj);
            Destroy(label.gameObject);
        }
    }

    public static void HideAllItemLabels()
    {
        foreach(KeyValuePair<Interactive, ObjectLabel> pair in instance.labels)
        {
            if (pair.Key is ItemObject)
            {

            }
        }
    }

    /*
    public void AddTip(Outline item)
    {
        if (!showAll) return;
        buffer.Add(item);
    }

    public void ClearTips()
    {
        while(tooltips.Count > 0)
        {
            DestroyImmediate(tooltips[0].gameObject);
            tooltips.RemoveAt(0);
        }
        tooltips = new List<ObjectLabel>();
    }

    private ObjectLabel GetTip(Outline item)
    {
        if (labels.TryGetValue(item, out ObjectLabel label))
            return label;

        return null;
    }

    public void UpdateTips()
    {
        showAll = Input.GetKey(KeyCode.LeftAlt);

        Item raycastItem = null;
        float distance = 150;
        int layerMask = LayerMask.GetMask("Interactive", "Walls");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float radius = 0.2f;

        if (Physics.SphereCast(ray, radius, out RaycastHit hitInfo, distance, layerMask))
        {
            //float distanceToPlayer = Vector3.Distance(transform.position, hitInfo.transform.position);
            //if (distanceToPlayer < 5)
            raycastItem = hitInfo.transform.GetComponent<Item>();
        }

        if (!showAll)
        {
            if (mouseRayCastItemTip == null)
                mouseRayCastItemTip = Instantiate(prefab, transform);

            if (raycastItem)
                mouseRayCastItemTip.Focus = raycastItem;

            if (!raycastItem && TipUnderMouse() != mouseRayCastItemTip)
            {
                if (mouseRayCastItemTip != null)
                {
                    DestroyImmediate(mouseRayCastItemTip.gameObject);
                    mouseRayCastItemTip = null;
                }
            }
        }
        else
        {
            if (mouseRayCastItemTip != null)
            {
                DestroyImmediate(mouseRayCastItemTip.gameObject);
                mouseRayCastItemTip = null;
            }
        }

        if (showAll)
        {
            hoverTip = TipUnderMouse();
            if (!hoverTip)
                hoverTip = GetTip(raycastItem);
        }
        else
        {
            hoverTip = mouseRayCastItemTip;
        }

        if (hoverTip)
            hoverTip.Hover = true;


        if (!showAll)
        {
            while(tooltips.Count > 0)
            {
                DestroyImmediate(tooltips[0].gameObject);
                tooltips.RemoveAt(0);
            }
            return;
        }

        List<ObjectLabel> newList = new List<ObjectLabel>();

        foreach(ObjectLabel tip in tooltips)
        {
            int indexOf = buffer.IndexOf(tip.Focus);
            if (indexOf != -1)
            {
                buffer.RemoveAt(indexOf);
                newList.Add(tip);
            }
            else
            {
                DestroyImmediate(tip.gameObject);
            }
        }

        foreach(Item item in buffer)
        {
            if (item == null) continue;
            ObjectLabel tip = Instantiate(prefab, transform);
            tip.Focus = item;
            newList.Add(tip);
        }

        buffer = new List<Item>();
        tooltips = newList;
    }

    private ObjectLabel TipUnderMouse()
    {
        PointerEventData ped = new PointerEventData(null);
        //Set required parameters, in this case, mouse position
        ped.position = Input.mousePosition;
        //Create list to receive all results
        List<RaycastResult> results = new List<RaycastResult>();
        //Raycast it
        graphicRaycaster.Raycast(ped, results);

        foreach (RaycastResult result in results)
        {
            ObjectLabel tip = result.gameObject.GetComponent<ObjectLabel>();
            if (tip) return tip;
        }
        return null;
    }
    


    public void LateUpdate()
    {
        if (!showAll)
        {
            Layout(mouseRayCastItemTip);
        }
        else
        {
            RectTransform[] rects = new RectTransform[tooltips.Count];

            for (int i = 0; i < tooltips.Count; i++)
            {
                ObjectLabel tip = tooltips[i];
                RectTransform rectTransform = tip.transform as RectTransform;

                rects[i] = rectTransform;

                Layout(tip);

                
            }

            SeparateRectangles(rects, tipSeparationRadialStepSize, tipSeparationRotationalStepSize, tipSeparationMaxSteps);
        }

        
    }
    */

    public void LateUpdate()
    {
        List<RectTransform> rectsToSeparate = new List<RectTransform>();
        List<ObjectLabel> labelsToZSort = new List<ObjectLabel>();
        foreach (ObjectLabel label in labels.Values)
        {
            if (!label.isActiveAndEnabled)
                continue;
            if (label.SeparateRectangles)
                rectsToSeparate.Add(label.transform as RectTransform);
            if (label.ZSort)
                labelsToZSort.Add(label);
            Layout(label);
        }

        SeparateRectangles(rectsToSeparate, tipSeparationRadialStepSize, tipSeparationRotationalStepSize, tipSeparationMaxSteps);
        labelsToZSort.Sort(ZSort);

        foreach (ObjectLabel label in labelsToZSort)
            label.transform.SetAsLastSibling();
    }

    public static int ZSort(ObjectLabel o1, ObjectLabel o2)
    {
        if (o1.worldTransform.position.z < o2.worldTransform.position.z) return 1;
        if (o1.worldTransform.position.z > o2.worldTransform.position.z) return -1;
        return 0;
    }

    private void Layout(ObjectLabel tip)
    {
        if (tip == null)
            return;

        Vector3 world = tip.worldOffset;
        if (tip.worldTransform) world += tip.worldTransform.position;

        Vector2 screen = main.WorldToScreenPoint(world);

        screen.x -= Screen.width / 2;
        screen.y -= Screen.height / 2;

        screen /= canvas.scaleFactor;

        screen += tip.screenOffset;

        (tip.transform as RectTransform).anchoredPosition = screen;
    }

    public static Rect GetWorldSpaceRect(RectTransform rt)
    {
        var r = rt.rect;
        r.center = rt.TransformPoint(r.center);
        r.size = rt.TransformVector(r.size);
        return r;
    }

    public static bool PositionValid(RectTransform current, List<RectTransform> rects)
    {
        if (current == null) return true;
        if (rects.Count <= 1) return true;

        foreach (RectTransform rect in rects)
        {
            if (rect == null) continue;
            if (rect == current) continue;
            if (GetWorldSpaceRect(current).Overlaps(GetWorldSpaceRect(rect), true))
            {
                return false;
            }
        }
        return true;
    }

    public static void SeparateRectangles(List<RectTransform> rects, float spiralRadialStepSize, float spiralRotationalStepSize, int maxSteps)
    {
        if (rects.Count <= 1) return;

        foreach (RectTransform rect in rects)
        {
            int steps = 0;
            float currentRadius = 0;
            float currentRingCircumference = 0;
            float currentRingAdvance = 0;
            Vector2 origin = rect.localPosition;
            
            while (steps < maxSteps)
            {
                steps++;

                if (PositionValid(rect, rects)) break;

                currentRingAdvance += spiralRotationalStepSize;
                while (currentRingAdvance >= currentRingCircumference)
                {
                    currentRingAdvance -= currentRingCircumference;
                    currentRadius += spiralRadialStepSize;
                    currentRingCircumference = 2 * Mathf.PI * currentRadius;
                }

                float posX = origin.x + currentRadius * Mathf.Cos(currentRingAdvance * Mathf.Deg2Rad);
                float posY = origin.y + currentRadius * Mathf.Sin(currentRingAdvance * Mathf.Deg2Rad);

                rect.localPosition = new Vector2(posX, posY);
            }
        }
    }
}
