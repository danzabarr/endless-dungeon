using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSnap : MonoBehaviour
{
    public Vector3 positionInterval, positionOffset;
    public Vector3 scaleInterval, scaleOffset;
    public Vector3 rotationInterval, rotationOffset;
    struct TransformData
    {
        public Vector3 position, scale;
        public Quaternion rotation;
        public TransformData(Transform transform)
        {
            position = transform.localPosition;
            scale = transform.localScale;
            rotation = transform.localRotation;
        }

        public void ApplyTo(Transform transform)
        {
            transform.localPosition = position;
            transform.localScale = scale;
            transform.localRotation = rotation;
        }
    }

    private Dictionary<Transform, TransformData> undo;

    [ContextMenu("Undo")]
    public void Undo()
    {
        if (undo == null)
        {
            Debug.LogWarning("No Undo data exists.");
            return;
        }
        foreach (KeyValuePair<Transform, TransformData> entry in undo)
            entry.Value.ApplyTo(entry.Key);

        undo = null;
    }

    [ContextMenu("Snap Positions")]
    public void SnapPositions()
    {
        undo = new Dictionary<Transform, TransformData>();
        foreach(Transform child in transform)
        {
            Vector3 position = child.transform.localPosition;
            undo.Add(child, new TransformData(child));
            if (positionInterval.x != 0)
                position.x = Mathf.Round(position.x / positionInterval.x) * positionInterval.x + positionOffset.x;
            if (positionInterval.y != 0)
                position.y = Mathf.Round(position.y / positionInterval.y) * positionInterval.y + positionOffset.y;
            if (positionInterval.z != 0)
                position.z = Mathf.Round(position.z / positionInterval.z) * positionInterval.z + positionOffset.z;

            child.transform.localPosition = position;
        }
    }


    [ContextMenu("Snap Rotations")]
    public void SnapRotations()
    {
        float ModRotation(float rotation)
        {
            rotation %= 360;
            rotation += 360;
            rotation %= 360;
            return rotation;
        }

        undo = new Dictionary<Transform, TransformData>();
        foreach (Transform child in transform)
        {
            Vector3 rotation = child.transform.localRotation.eulerAngles;
            undo.Add(child, new TransformData(child));
            if (rotationInterval.x != 0)
                rotation.x = Mathf.Round(rotation.x / rotationInterval.x) * rotationInterval.x + rotationOffset.x;
            if (rotationInterval.y != 0)
                rotation.y = Mathf.Round(rotation.y / rotationInterval.y) * rotationInterval.y + rotationOffset.y;
            if (rotationInterval.z != 0)
                rotation.z = Mathf.Round(rotation.z / rotationInterval.z) * rotationInterval.z + rotationOffset.z;

            child.transform.localRotation = Quaternion.Euler(rotation);
        }
    }

    [ContextMenu("Snap Scales")]
    public void SnapScales()
    {
        undo = new Dictionary<Transform, TransformData>();
        foreach (Transform child in transform)
        {
            Vector3 scale = child.transform.localScale;
            undo.Add(child, new TransformData(child));
            if (scaleInterval.x != 0)
                scale.x = Mathf.Round(scale.x / scaleInterval.x) * scaleInterval.x + scaleOffset.x;
            if (scaleInterval.y != 0)
                scale.y = Mathf.Round(scale.y / scaleInterval.y) * scaleInterval.y + scaleOffset.y;
            if (scaleInterval.z != 0)
                scale.z = Mathf.Round(scale.z / scaleInterval.z) * scaleInterval.z + scaleOffset.z;

            child.transform.localScale = scale;
        }
    }
}
