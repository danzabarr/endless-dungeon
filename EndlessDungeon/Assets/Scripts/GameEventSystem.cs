using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public class EventArgs
    {
        public readonly object source, target;
        public readonly Location location;
        public readonly Vector3 position;

        public EventArgs(object source, object target, Location location, Vector3 position)
        {
            this.source = source;
            this.target = target;
            this.location = location;
            this.position = position;
        }

        public EventArgs(Unit unit)
        {
            source = unit;
            target = unit;
            position = unit.transform.position;
            location = Level.Instance.GetLocation(unit.transform.position);
        }
    }

    public delegate void Handler(EventArgs e);

    public static void UnitDeaths(EventArgs args)
    {
        Debug.Log(args.source + " died.");
    }
}
