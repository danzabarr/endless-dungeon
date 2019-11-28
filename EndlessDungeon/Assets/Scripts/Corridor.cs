using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Corridor : Location
{
    public override void SetPosition(int x, int z, Units.Rotation rotation)
    {
        this.x = x;
        this.z = z;
        this.rotation = rotation;
        transform.localPosition = new Vector3(x + 0.5f, 0, z + 0.5f);
        transform.localRotation = Quaternion.Euler(0, (int)rotation * 90, 0);
        SetWallRotations();
    }

    public override LocationData.LocationType GetLocationType()
    {
        return LocationData.LocationType.Corridor;
    }
}
