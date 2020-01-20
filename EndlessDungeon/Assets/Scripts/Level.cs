using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class Level : MonoBehaviour
{
    private static Level instance;
    public static Level Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Level>();
            }
            return instance;
        }
    }

    private Dictionary<int, LevelData> cachedLevelData = new Dictionary<int, LevelData>();

    [SerializeField]
    private LevelGenerationPreset[] levelPresets;

    [SerializeField]
    private int locationViewDistance;

    [SerializeField]
    private LabelManager itemTooltipManager;

    [Header("Gizmos")]
    public bool displayVisibleArea;
    public bool displayTileOutlines;
    public bool displayNodeConnections;

    [System.Serializable]
    public struct NavMeshBuildSettingsSerialized
    {
        public float agentRadius;
        public float agentHeight;
        public float agentSlope;
        public float agentClimb;
        public float minRegionArea;
        public int tileSize;
    }

    [SerializeField]
    private NavMeshBuildSettingsSerialized navMeshBuildSettings;

    [SerializeField]
    private int levelToLoad;

    [SerializeField]
    private PrefabRegistry registry;

    private int currentLevel = -1;
    private List<Room> rooms = new List<Room>();
    private List<Corridor> corridors = new List<Corridor>();
    private List<MapNode> entrances = new List<MapNode>();
    private List<Mob> mobs = new List<Mob>();
    private List<ItemObject> items = new List<ItemObject>();
    private List<ItemObject> itemsShowingLabel = new List<ItemObject>();
    private List<LineOfSightObject> losObjects = new List<LineOfSightObject>();
    private MapNode[,] nodes;
    private List<Location> locations;
    private List<Location> visibleLocations;

    private List<Edge> edges;

    private Vector2[] visibleArea;
    [SerializeField]
    private float visibleRadius;
    private Mesh visibleAreaMesh;
    private Color visibleAreaColor = new Color(1, 0, 1, .3f);
    public float lineOfSightObjectInset;

    public int XSize => nodes.GetLength(0);
    public int ZSize => nodes.GetLength(1);
    public MapNode this[int x, int z] => x < 0 || x >= nodes.GetLength(0) || z < 0 || z >= nodes.GetLength(1) ? null : nodes[x, z];

    public Room StartRoom => rooms[0];
    public Room EndRoom => rooms[1];

    private Location playerLocation;
    public Location PlayerLocation => playerLocation;

    private Vector2Int playerNodePosition;
    public Vector2Int PlayerNodePosition => playerNodePosition;

    public Door GetDoor(MapNode insideNode, MapNode outsideNode, bool determineInside)
    {
        if (insideNode == null || outsideNode == null)
            return null;

        
        if (determineInside)
        {
            if (insideNode.type != MapNode.NodeType.EntranceInside)
            {
                MapNode temp = insideNode;
                insideNode = outsideNode;
                outsideNode = temp;
            }
        }
        

        if (insideNode.type != MapNode.NodeType.EntranceInside)
            return null;

        if (outsideNode.type != MapNode.NodeType.EntranceOutside)
            return null;

        Location location = GetLocation(insideNode);
        if (!(location is Room))
            return null;

        Room room = location as Room;

        return room.GetDoor(new Vector2Int(insideNode.x, insideNode.z), new Vector2Int(outsideNode.x, outsideNode.z));
    }

    public Door GetDoor(Vector2Int insideNode, Vector2Int outsideNode, bool determineInside)
    {
        MapNode node = this[insideNode.x, insideNode.y];
        if (node == null)
            return null;

        if (determineInside)
        {
            if (node.type != MapNode.NodeType.EntranceInside)
            {
                Vector2Int temp = insideNode;
                insideNode = outsideNode;
                outsideNode = temp;
                node = this[insideNode.x, insideNode.y];
                if (node == null)
                    return null;
            }
        }

        if (node.type != MapNode.NodeType.EntranceInside)
            return null;

        Location location = GetLocation(node);
        if (!(location is Room))
            return null;

        Room room = location as Room;

        return room.GetDoor(insideNode, outsideNode);
    }

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        GenerateMap();
    }

    public void Update()
    {
        if (currentLevel >= 0)
            EnableRenderers(true);

        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLevel(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            LeaveDungeon();
            ClearCachedData();
        }
        */
    }

    public Vector3 ScreenPointToRayPlaneIntersection(Vector3 screenPos, float y, Camera camera)
    {
        Vector3 hit = Vector3.zero;
        Ray ray = camera.ScreenPointToRay(screenPos);
        if (new Plane(Vector3.up, new Vector3(0, y, 0)).Raycast(ray, out float distance))
            hit = ray.GetPoint(distance);
        return hit;
    }
    private Vector2Int GetNodePosition(Vector3 worldPos)
    {
        Vector3 scaledWorldPos = new Vector3(worldPos.x / transform.localScale.x, worldPos.y / transform.localScale.y, worldPos.z / transform.localScale.z);
        int nodeX = (int)scaledWorldPos.x;
        int nodeZ = (int)scaledWorldPos.z;
        return new Vector2Int(nodeX, nodeZ);
    }
    private MapNode GetNode(Vector3 worldPos)
    {
        if (nodes == null) return null;

        Vector3 scaledWorldPos = new Vector3(worldPos.x / transform.localScale.x, worldPos.y / transform.localScale.y, worldPos.z / transform.localScale.z);

        int nodeX = (int)scaledWorldPos.x;
        if (nodeX < 0 || nodeX >= nodes.GetLength(0)) return null;
        int nodeZ = (int)scaledWorldPos.z;
        if (nodeZ < 0 || nodeZ >= nodes.GetLength(1)) return null;

        return nodes[nodeX, nodeZ];
    }
    public Location GetLocation(Vector3 worldPos)
    {
        return GetLocation(GetNode(worldPos));
    }
    public Location GetLocation(MapNode node)
    {
        if (node == null) return null;
        return GetLocation(node.type, node.locationIndex);
    }

    public Location GetLocation(MapNode.NodeType nodeType, int locationIndex)
    {
        if (nodeType == MapNode.NodeType.Corridor) return corridors[locationIndex];
        if (nodeType == MapNode.NodeType.Room) return rooms[locationIndex];
        if (nodeType == MapNode.NodeType.EntranceOutside) return corridors[locationIndex];
        if (nodeType == MapNode.NodeType.EntranceInside) return rooms[locationIndex];
        return null;
    }
    public MapNode GetNode(Corridor corridor)
    {
        return nodes[corridor.X, corridor.Z];
    }

    public void ShowAllItemLabels(bool show)
    {
        foreach (ItemObject obj in itemsShowingLabel)
        {
            if (obj == null)
                continue;
            obj.ShowLabelText(false);
        }
        itemsShowingLabel.Clear();

        if (show)
        {
            const float rangeSquared = 20;
            Location playerLocation = PlayerLocation;
            foreach(ItemObject obj in items)
            {
                if (obj == null)
                    continue;

                float dSq = SquareDistance(Player.Instance.GetGroundPosition(), obj.GetGroundPosition());
                if (dSq > rangeSquared)
                    continue;

                

                obj.ShowLabelText(true);
                itemsShowingLabel.Add(obj);

            }
        }
    }

    private void OnDrawGizmos()
    {
        if (displayVisibleArea)
        {
            if (visibleAreaMesh != null)
            {
                Gizmos.color = visibleAreaColor;
                Gizmos.DrawMesh(visibleAreaMesh, new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0), new Vector3(10, 10, 10));
            }
        }

        if (displayTileOutlines)
        {

            if (nodes == null)
                return;

            DrawTile(0, 0, nodes.GetLength(0), nodes.GetLength(1));
            Gizmos.color = Color.blue;

            foreach (MapNode node in nodes)
            {
                if (node == null) continue;
                if (node.type != MapNode.NodeType.Temporary)
                {
                    if (node.type == MapNode.NodeType.Room || node.type == MapNode.NodeType.EntranceInside)
                    {
                        Gizmos.color = Color.yellow;
                        DrawTile(node.x, node.z, 1, 1);
                    }
                    else if (node.type == MapNode.NodeType.Corridor)
                    {
                        Gizmos.color = Color.green;
                        DrawTile(node.x, node.z, 1, 1);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        DrawTile(node.x, node.z, 1, 1);
                    }

                }
            }
        }
        if (displayNodeConnections)
        {
            Vector3 nodePos;
            foreach(MapNode node in nodes)
            {
                if (node == null) continue;
                Gizmos.color = Color.blue;
                nodePos = Vector3.Scale(transform.localScale, new Vector3(node.x + 0.5f, 0, node.z + 0.5f));
                for (int i = 0; i < 8; i++)
                {
                    if (node[i])
                    {
                        Vector2Int neighbourPos = node.GetNeighbourPosition(i);
                        Gizmos.DrawLine(nodePos, Vector3.Scale(transform.localScale, new Vector3(neighbourPos.x + 0.5f, 0, neighbourPos.y + 0.5f)));
                    }
                }
            }
        }



    }
    private void DrawTile(int x, int z, int xSize, int zSize)
    {
        Vector3 c00 = Vector3.Scale(transform.localScale, new Vector3(x, 0, z));
        Vector3 c01 = Vector3.Scale(transform.localScale, new Vector3(x, 0, z + zSize));
        Vector3 c10 = Vector3.Scale(transform.localScale, new Vector3(x + xSize, 0, z));
        Vector3 c11 = Vector3.Scale(transform.localScale, new Vector3(x + xSize, 0, z + zSize));
        
        Gizmos.DrawLine(c00, c01);
        Gizmos.DrawLine(c01, c11);
        Gizmos.DrawLine(c11, c10);
        Gizmos.DrawLine(c10, c00);
    }

    

    #region Player Location Based Visibility
    public void EnableRenderers(bool forceUpdate = false)
    {
       // return;

        Location getLocation = GetLocation(Player.Instance.transform.position);
        Vector2Int getNodePos = GetNodePosition(Player.Instance.transform.position);
        MapNode getNode = nodes[getNodePos.x, getNodePos.y];


        /*
        foreach(Item item in items)
        {
            if (!item) continue;

            if (itemTooltipManager.ShowAll)
            {
                if (item.Dynamic.locationIndex == getNode.locationIndex)
                {
                    itemTooltipManager.AddTip(item);
                    item.OutlineShown = true;
                }
                else
                {
                    item.OutlineShown = false;
                }
            }
            else
            {
                if (item != itemTooltipManager.HoverTipItem)
                    item.OutlineShown = false;
            }
        }
        itemTooltipManager.UpdateTips();
        */

        float scale = 10;
        Vector2 playerPosition = new Vector2(Player.Instance.transform.position.x / scale, Player.Instance.transform.position.z / scale);
        float playerVisibleRadius = visibleRadius / scale;

        //Profiler.BeginSample("Calculate Visible Area");
        visibleArea = LineOfSight.CalculateVisibleArea(edges, true, playerPosition, playerVisibleRadius, 20);
        //Profiler.EndSample();

        //Profiler.BeginSample("Generate Visible Area Mesh");
        if (displayVisibleArea)
            visibleAreaMesh = LineOfSight.GenerateMesh(visibleAreaMesh, playerPosition, visibleArea);
        //Profiler.EndSample();


        if (!forceUpdate && playerNodePosition == getNodePos && playerLocation == getLocation)
        {
            playerLocation = getLocation;
            playerNodePosition = getNodePos;
            return;
        }

        playerLocation = getLocation;
        playerNodePosition = getNodePos;

        int wallsLayer = LayerMask.NameToLayer("Walls");
        int ignoreRayCastLayer = LayerMask.NameToLayer("Ignore Raycast");

        //Profiler.BeginSample("Show Walls");
        #region Show walls
        foreach(Location location in locations)
        {
            foreach (Wall wall in location.Walls)
            {
                Renderer renderer = wall.Renderer;
                
                bool shown =
                (
                    (wall.transform.position.z > Player.Instance.transform.position.z) ||
                    ((int)wall.WorldRotation % 2 == 0 && location is Corridor && playerLocation is Corridor) ||
                    ((int)wall.WorldRotation % 2 == 0 && location is Room && location == playerLocation) ||
                    ((int)wall.WorldRotation % 2 == 0 && location is Room && playerLocation is Corridor && wall.Outside) ||
                    ((int)wall.WorldRotation % 2 == 0 && location is Corridor && playerLocation is Room && (location.X < playerLocation.X || location.X >= playerLocation.MaxX()))
                );

                renderer.gameObject.layer = shown ? wallsLayer : ignoreRayCastLayer;
                renderer.shadowCastingMode = shown ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        #endregion
        #region Show visible objects

        foreach (LineOfSightObject losObject in losObjects)
        {
            Bounds bounds = losObject.Bounds;
            
            bounds.size = new Vector3(Mathf.Max(bounds.size.x + lineOfSightObjectInset * scale, 0), bounds.size.y, Mathf.Max(bounds.size.z + lineOfSightObjectInset * scale, 0));
            Vector2 boundsMin = new Vector2(losObject.Bounds.min.x, losObject.Bounds.min.z) / scale;// new Vector2(losObject.Bounds.min.x, losObject.Bounds.min.z) / scale + Vector2.one * lineOfSightObjectInset;
            Vector2 boundsMax = new Vector2(losObject.Bounds.max.x, losObject.Bounds.max.z) / scale;// new Vector2(losObject.Bounds.max.x, losObject.Bounds.max.z) / scale - Vector2.one * lineOfSightObjectInset;


            if (visibleArea == null)
            {
                losObject.Show();
                losObject.boundsColor = new Color(1, 0, 1, .2f);
            }
            else if (LineOfSight.CircleIntersectsBounds(playerPosition, playerVisibleRadius, boundsMin, boundsMax) && LineOfSight.PolygonIntersectsBounds(visibleArea, boundsMin, boundsMax, out LineOfSight.PolygonBoundsIntersectionType intersectionType))
            {
                if (intersectionType == LineOfSight.PolygonBoundsIntersectionType.BoundsContainsPolygonPoint)
                    losObject.boundsColor = new Color(1, 0, 0, .2f);
                else if (intersectionType == LineOfSight.PolygonBoundsIntersectionType.PolygonContainsBoundsCorner)
                    losObject.boundsColor = new Color(0, 1, 0, .2f);
                else if (intersectionType == LineOfSight.PolygonBoundsIntersectionType.LineSegmentsIntersect)
                    losObject.boundsColor = new Color(0, 0, 1, .2f);

                losObject.Show();
            }
            else
            {
                losObject.Hide();
            }
        }

        foreach (Light light in GetComponentsInChildren<Light>())
        {
            Vector2 position = new Vector2(light.transform.position.x / scale, light.transform.position.z / scale);
            light.enabled = visibleArea == null || (LineOfSight.CircleContainsPoint(playerPosition, playerVisibleRadius, position) && LineOfSight.PolygonContainsPoint(visibleArea, position));
        }
        #endregion

        #endregion
    }

    private List<Location> FloodFilledLocations(int x, int z, int distance)
    {
        List<MapNode> nodes = FloodFill(x, z, distance);
        List<Location> locations = new List<Location>();
        foreach (MapNode node in nodes)
        {
            if (node == null) continue;
            Location location = GetLocation(node);
            if (location != null && !locations.Contains(location))
                locations.Add(location);
        }
        return locations;
    }
    private List<MapNode> FloodFill(int x, int z, int distance)
    {
        List<MapNode> list = new List<MapNode>();
        if (x < 0 || z < 0 || x >= nodes.GetLength(0) || z >= nodes.GetLength(1))
            return list;

        MapNode start = nodes[x, z];
        if (start == null)
            return list;

        list.Add(start);
        return FloodFill(list, distance);
    }
    private List<MapNode> FloodFill(List<MapNode> list, int distance)
    {
        if (distance <= 0)
            return list;
        List<MapNode> toAdd = new List<MapNode>();
        foreach (MapNode node in list)
        {
            for (int i = 0; i < 8; i += 2)
            {
                if (node[i])
                {
                    MapNode neighbourNode = Node(node.GetNeighbourPosition(i));
                    if (toAdd.Contains(neighbourNode)) continue;
                    if (list.Contains(neighbourNode)) continue;
                    toAdd.Add(neighbourNode);
                }
            }
        }
        list.AddRange(toAdd);

        FloodFill(list, distance - 1);
        return list;
    }
    #region Path Finding
    public static readonly float ROOT_2 = Mathf.Sqrt(2f);
    public static float Distance(MapNode n1, MapNode n2)
    {
        return Mathf.Sqrt((n1.x - n2.x) * (n1.x - n2.x) + (n1.z - n2.z) * (n1.z - n2.z));
    }
    private MapPath Find(MapNode start, MapNode end, int maxTries, int maxDistance, float newPathCost, float diagonalness1Cost, float diagonalness2Cost)
    {
        float d = Distance(start, end);

        if (d > maxDistance) return null;

        List<MapNode> visited = new List<MapNode>();
        List<MapNode> open = new List<MapNode>();
        List<MapNode> closed = new List<MapNode>();

        start.pathDistance = 0;
        start.crowFliesDistance = d;

        open.Add(start);

        int tries = 0;

        while (true)
        {
            tries++;
            if (tries > maxTries)
            {
                foreach (MapNode p in visited)
                    p.ClearPathfindindData();
                //Debug.Log("TOO MANY TRIES!");
                return null;
            }

            MapNode currentNode = null;

            if (open.Count == 0)
            {
                foreach (MapNode p in visited)
                    p.ClearPathfindindData();
                return null;
                // throw new RuntimeException("no route");
            }

            foreach (MapNode node in open)
            {
                if (currentNode == null || node.Cost < currentNode.Cost)
                {
                    currentNode = node;
                }
            }

            if (currentNode == end)
            {
                break;
            }

            if (currentNode.pathDistance > maxDistance)
            {
                foreach (MapNode p in visited)
                    p.ClearPathfindindData();

                return null;
            }

            open.Remove(currentNode);
            closed.Add(currentNode);


            for (int i = 0; i < 8; i++)
            {

                if (!currentNode[i]) continue;

                MapNode neighbor = Node(currentNode.GetNeighbourPosition(i));

                if (neighbor == null) continue;
                if (neighbor.type == MapNode.NodeType.Empty) continue;
                float distance = i % 2 == 0 ? 1 : ROOT_2;//currentNode.distances[i];

                float nextG = currentNode.pathDistance + distance;

                if (nextG < neighbor.pathDistance)
                {
                    open.Remove(neighbor);
                    closed.Remove(neighbor);
                }

                if (!open.Contains(neighbor) && !closed.Contains(neighbor))
                {
                    neighbor.pathDistance = nextG;
                    neighbor.crowFliesDistance = Distance(neighbor, end);
                    neighbor.lastDirection = i;
                    neighbor.diagonalness = currentNode.diagonalness + (currentNode.lastDirection != i ? Mathf.Max(diagonalness2Cost, 0) : Mathf.Max(-diagonalness2Cost, 0));
                    neighbor.additionalCost = currentNode.additionalCost + (neighbor.type == MapNode.NodeType.Temporary ? Mathf.Max(newPathCost, 0) : Mathf.Max(-newPathCost, 0));
                    neighbor.xSteps = currentNode.xSteps + Mathf.Abs(neighbor.x - currentNode.x) * diagonalness1Cost;
                    neighbor.zSteps = currentNode.zSteps + Mathf.Abs(neighbor.z - currentNode.z) * diagonalness1Cost;
                    neighbor.parent = currentNode;
                    open.Add(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        List<MapNode> nodes = new List<MapNode>();
        MapNode current = end;
        while (current.parent != null)
        {
            nodes.Add(current);

            //this is backwards.
            current = current.parent;
        }
        nodes.Add(start);
        //so is this.



        MapPath result = new MapPath(nodes, end.pathDistance, end.crowFliesDistance, end.additionalCost, Mathf.Min(end.xSteps, end.zSteps), end.diagonalness);


        foreach (MapNode p in visited)
        {
            p.pathDistance = 0;
            p.crowFliesDistance = 0;
            p.parent = null;
        }

        return result;
    }
    #endregion
    #region Level Generating/Saving/Loading

    [ContextMenu("Generate")]
    private void GenerateMap()
    {
        LoadLevel(levelToLoad);
    }

    [ContextMenu("Clear Cache")]
    public void ClearCachedData()
    {
        cachedLevelData.Clear();
    }

    [ContextMenu("Leave Dungeon")]
    public void LeaveDungeon()
    {
        currentLevel = -1;
        ClearMap();
    }

    [ContextMenu("Move Player to Spawn")]
    public void MoveToSpawn()
    {
        StartRoom.MoveUnitToSpawn(Player.Instance);
    }

    private void FindLocations()
    {
        locations = new List<Location>(GetComponentsInChildren<Location>());
        visibleLocations = new List<Location>(locations);
    }
    public bool LoadLevel(int level)
    {
        if (level == currentLevel)
            return false;

        if (currentLevel >= 0)
        {
            if (cachedLevelData.ContainsKey(currentLevel))
                cachedLevelData[currentLevel] = WriteData();
            else
                cachedLevelData.Add(currentLevel, WriteData());
        }

        if (cachedLevelData.TryGetValue(level, out LevelData data))
        {
            ClearMap();
            ReadData(data);
            GenerateEdges();
            
            currentLevel = level;

            StartRoom.MoveUnitToSpawn(Player.Instance);
            FindLocations();
            EnableRenderers(true);
            return true;
        }
        else
        {
            if (level < 0 || level >= levelPresets.Length)
            {
                Debug.LogError("Failed to load level: Level index out of range.");
                return false;
            }

            LevelGenerationPreset generationPreset = levelPresets[level];

            if (generationPreset == null)
            {
                Debug.LogError("Failed to load level: Generation preset is null.");
                return false;
            }

            ClearMap();

            int tries = 100;
            for (; tries >= 0; tries--)
                if (Generate(generationPreset)) break;

            if (tries == 0)
            {
                ClearMap();
                Debug.LogError("Failed to load level: Couldn't generate a valid map in 100 tries.");
                return false;
            }
            GenerateEdges();

            currentLevel = level;

            StartRoom.MoveUnitToSpawn(Player.Instance);
            FindLocations();
            EnableRenderers(true);
            return true;
        }
    }

    private void GenerateEdges()
    {
        edges = LineOfSight.GenerateEdges(this, false);
    }

    private void ClearMap()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        nodes = null;
        rooms = null;
        corridors = null;
        entrances = null;
        mobs = null;
        items = null;
        locations = null;
        visibleLocations = null;
        losObjects = null;
    }
    private bool Generate(LevelGenerationPreset preset)
    {
        if (preset == null)
        {
            Debug.LogWarning("Cancelling Level Generation: Level Generation Preset was null.");
            return false;
        }

        int xSize = preset.size.x;
        int zSize = preset.size.y;

        nodes = new MapNode[xSize, zSize];
        rooms = new List<Room>();
        corridors = new List<Corridor>();
        entrances = new List<MapNode>();
        items = new List<ItemObject>();
        mobs = new List<Mob>();
        losObjects = new List<LineOfSightObject>();

        #region Add Required Rooms

        T[] ShuffledArrayCopied<T>(T[] array)
        {
            T[] a = new T[array.Length];
            int j = 0;

            for (int i = 0; i < array.Length; i++)
            {
                j = Random.Range(j, i);
                if (j != i)
                    a[i] = a[j];
                a[j] = array[i];
            }

            return a;
        }

        Room AddOneRoomRandomLocation(LevelGenerationPreset.RequiredRoom room)
        {
            if (room == null)
                return null;

            foreach (Vector2Int i in ShuffledArrayCopied(room.locations))
            {
                Room add = AddRoom(room.prefab, i.x, i.y, room.prefab.Rotatable ? (Units.Rotation)Random.Range(0, 4) : Units.Rotation.East, transform, true);
                if (add != null)
                    return add;
            }
            return null;
        }

        if (!AddOneRoomRandomLocation(preset.startRoom))
        {
            Debug.LogWarning("Cancelling Level Generation: Failed to find a location for the start room, or start room was null.");
            return false;
        }

        if (!AddOneRoomRandomLocation(preset.endRoom))
        {
            Debug.LogWarning("Cancelling Level Generation: Failed to find a location for the end room, or end room was null.");
            return false;
        }

        foreach (LevelGenerationPreset.RequiredRoom room in preset.uniqueRooms)
        {
            if (!AddOneRoomRandomLocation(room))
            {
                Debug.LogWarning("Cancelling Level Generation: Failed to find a location for a required room, or the room was null.");
                return false;
            }
        }

        #endregion

        #region Add Random Rooms

        for (int i = 0; i < preset.maxRandomRooms; i++)
        {
            Room prefab = preset.randomRooms[Random.Range(0, preset.randomRooms.Length)];
            Units.Rotation rotation = prefab.Rotatable ? (Units.Rotation)Random.Range(0, 4) : Units.Rotation.East;
            AddRoom(prefab, rotation, transform);
        }

        #endregion

        #region Connect Rooms
        for (int z = 0; z < zSize; z++)
            for (int x = 0; x < xSize; x++)
            {
                if (nodes[x, z] == null)
                    nodes[x, z] = new MapNode()
                    {
                        x = x,
                        z = z,
                        type = MapNode.NodeType.Temporary
                    };

                if (nodes[x, z].type != MapNode.NodeType.Temporary && nodes[x, z].type != MapNode.NodeType.EntranceOutside) continue;

                if (z + 1 < zSize)
                {
                    if (nodes[x, z + 1] == null) nodes[x, z + 1] = new MapNode()
                    {
                        x = x,
                        z = z + 1,
                        type = MapNode.NodeType.Temporary
                    };
                    if (nodes[x, z + 1].type == MapNode.NodeType.Temporary || nodes[x, z + 1].type == MapNode.NodeType.EntranceOutside)
                        SetNeighbour(nodes[x, z], nodes[x, z + 1], 6);
                }

                if (x + 1 < xSize)
                {
                    if (nodes[x + 1, z] == null) nodes[x + 1, z] = new MapNode()
                    {
                        x = x + 1,
                        z = z,
                        type = MapNode.NodeType.Temporary
                    };
                    if (nodes[x + 1, z].type == MapNode.NodeType.Temporary || nodes[x + 1, z].type == MapNode.NodeType.EntranceOutside)
                        SetNeighbour(nodes[x, z], nodes[x + 1, z], 0);
                }
            }

        List<Room> GetConnected(Room room, List<Room> list)
        {
            list.Add(room);
            foreach (Room c in room.connections)
            {
                if (list.Contains(c)) continue;
                GetConnected(c, list);
            }

            return list;
        }


        List<List<Room>> IdentifyIslands()
        {

            List<List<Room>> islandsList = new List<List<Room>>();
            if (rooms.Count <= 1)
            {
                islandsList.Add(new List<Room>(rooms));
                return islandsList;
            }
            List<Room> unincluded = new List<Room>(rooms);

            while (unincluded.Count > 0)
            {
                List<Room> connected = GetConnected(unincluded[0], new List<Room>());
                foreach (Room c in connected)
                    unincluded.Remove(c);
                islandsList.Add(connected);
            }

            return islandsList;
        }


        bool ConnectRooms(Room r1, Room r2, bool desperate = false)
        {
            if (r1 == r2) return false;

            bool skipPair = false;
            MapPath shortestPath = null;

            foreach (MapNode p1 in r1.entranceNodes)
            {
                foreach (MapNode p2 in r2.entranceNodes)
                {
                    if (p1 == p2)
                    {
                        skipPair = true;
                        break;
                    }

                    int maxTries = desperate ? 1000 : preset.corridorMaxTries;
                    int maxDistance = desperate ? Mathf.Max(preset.corridorMaxDistance, Mathf.Max(xSize, zSize)) : preset.corridorMaxDistance;
                    float newPathCost = desperate ? 0 : preset.corridorNewPathCost;
                    float diagonalness1Cost = desperate ? 0 : preset.corridorDiagonalness1Cost;
                    float diagonalness2Cost = desperate ? 0 : preset.corridorDiagonalness2Cost;

                    MapPath pathfind = Find(p1, p2, maxTries, maxDistance, newPathCost, diagonalness1Cost, diagonalness2Cost);
                    if (pathfind == null) continue;

                    if (shortestPath == null || pathfind.Cost < shortestPath.Cost)
                        shortestPath = pathfind;

                }
                if (skipPair) break;
            }

            if (shortestPath != null)
            {
                r1.connections.Add(r2);
                r2.connections.Add(r1);

                foreach (MapNode node in shortestPath.nodes)
                    if (node.type == MapNode.NodeType.Temporary) node.type = MapNode.NodeType.Corridor;

                return true;
            }

            return false;
        }

        foreach (Room r1 in rooms)
            foreach (Room r2 in rooms)
                ConnectRooms(r1, r2);

        List<List<Room>> islands = IdentifyIslands();

        bool ConnectIslands(List<Room> island1, List<Room> island2)
        {
            foreach (Room r1 in island1)
            {
                foreach (Room r2 in island2)
                {
                    if (ConnectRooms(r1, r2, true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        int islandConnectTries = 0;

        void LoopIslands()
        {
            //Debug.Log("Trying to connect rooms " + islandConnectTries);
            islandConnectTries++;
            for (int i1 = 0; i1 < islands.Count; i1++)
            {
                for (int i2 = 0; i2 < islands.Count; i2++)
                {
                    if (i1 == i2) continue;
                    if (ConnectIslands(islands[i1], islands[i2]))
                    {
                        islands[i1].AddRange(islands[i2]);
                        islands.RemoveAt(i2);
                        return;
                    }
                }
            }
        }

        while (islands.Count > 1 && islandConnectTries < 10)
        {
            LoopIslands();
        }

        if (islands.Count > 1)
        {
            Debug.LogWarning("Cancelling Level Generation: Failed to make all rooms accessible.");
            return false;
        }

        for (int z = 0; z < nodes.GetLength(1); z++)
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            if (nodes[x, z] == null) continue;
            if (nodes[x, z].type == MapNode.NodeType.Temporary)
            {
                Disconnect(nodes[x, z]);
                nodes[x, z] = null;
            }
        }

        bool IsCorridor(int x, int z)
        {
            return
                x >= 0 && x < xSize &&
                z >= 0 && z < zSize &&
                nodes[x, z] != null &&
                (nodes[x, z].type == MapNode.NodeType.Corridor || nodes[x, z].type == MapNode.NodeType.EntranceOutside);
        }

        for (int z = 0; z < zSize; z++)
            for (int x = 0; x < xSize; x++)
            {
                if (!IsCorridor(x, z)) continue;

                if (IsCorridor(x + 1, z + 1) && (IsCorridor(x + 1, z) && IsCorridor(x, z + 1)))
                {
                    SetNeighbour(nodes[x, z], nodes[x + 1, z + 1], 7);
                }

                if (IsCorridor(x - 1, z + 1) && (IsCorridor(x - 1, z) && IsCorridor(x, z + 1)))
                {
                    SetNeighbour(nodes[x, z], nodes[x - 1, z + 1], 5);
                }
            }

        #endregion

        #region Add Corridors
        foreach (MapNode node in nodes)
        {
            if (node == null) continue;
            if (node.type == MapNode.NodeType.Corridor || node.type == MapNode.NodeType.EntranceOutside)
            {
                Corridor prefab = null;
                Units.Rotation rotation = Units.Rotation.East;

                int neighboursCount = node.CountCardinalNeighbours();
                //               for (int i = 0; i < 4; i++)
                //                 if (node.neighbours[i * 2] != Null) neighboursCount++;

                #region Floors
                if (neighboursCount == 4)
                {
                    prefab = preset.corridorFloors[Random.Range(0, preset.corridorFloors.Length)];
                    rotation = (Units.Rotation)Random.Range(0, 4);
                }
                #endregion
                #region Deadends
                else if (neighboursCount == 1)
                {
                    if (node[6])
                        rotation = Units.Rotation.North;
                    else if (node[0])
                        rotation = Units.Rotation.East;
                    else if (node[2])
                        rotation = Units.Rotation.South;
                    else
                        rotation = Units.Rotation.West;

                    prefab = preset.corridorDeadends[Random.Range(0, preset.corridorDeadends.Length)];
                }
                #endregion
                #region Passages
                else if (neighboursCount == 2)
                {
                    if (node[2] && node[6])
                    {
                        rotation = (Units.Rotation)(Random.Range(0, 1) * 2);
                        prefab = preset.corridorPassages[Random.Range(0, preset.corridorPassages.Length)];
                    }
                    else if (node[0] && node[4])
                    {
                        rotation = (Units.Rotation)(Random.Range(0, 1) * 2 + 1);
                        prefab = preset.corridorPassages[Random.Range(0, preset.corridorPassages.Length)];
                    }
                    #endregion
                    #region Corners
                    else
                    {
                        if (node[2] && node[4])
                            rotation = Units.Rotation.North;
                        else if (node[4] && node[6])
                            rotation = Units.Rotation.East;
                        else if (node[6] && node[0])
                            rotation = Units.Rotation.South;
                        else
                            rotation = Units.Rotation.West;

                        prefab = preset.corridorCorners[Random.Range(0, preset.corridorCorners.Length)];
                    }
                }
                #endregion
                #region Edges
                else if (neighboursCount == 3)
                {
                    if (!node[6])
                        rotation = Units.Rotation.North;
                    else if (!node[0])
                        rotation = Units.Rotation.East;
                    else if (!node[2])
                        rotation = Units.Rotation.South;
                    else
                        rotation = Units.Rotation.West;

                    prefab = preset.corridorEdges[Random.Range(0, preset.corridorEdges.Length)];
                }
                #endregion

                if (prefab == null)
                {
                    Debug.LogError("Cancelling Level Generation: Corridor prefab was null");
                    continue;
                }

#if UNITY_EDITOR
                Corridor corridor = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as Corridor;
#else
                Corridor corridor = Object.Instantiate(prefab, transform);
#endif
                node.locationIndex = corridors.Count;
                corridor.SetPosition(node.x, node.z, rotation);
                corridors.Add(corridor);

                if (preset.pillarPrefab != null)
                {
                    bool IsNeighbourCorridor(int neighbourIndex)
                    {
                        if (!node[neighbourIndex]) return false;
                        MapNode.NodeType type = Node(node.GetNeighbourPosition(neighbourIndex)).type;
                        return type == MapNode.NodeType.Corridor || type == MapNode.NodeType.EntranceOutside;
                    }


                    for (int i = 0; i < 4; i++)
                    {
                        int open1 = i * 2;
                        int open2 = (i * 2 + 2) % 8;
                        int closed = (i * 2 + 1) % 8;

                        int cornerX = node.x + ((i - 1 + 4) % 4) / 2;
                        int cornerZ = node.z + i / 2;

                        if (IsNeighbourCorridor(open1) &&
                            IsNeighbourCorridor(open2) &&
                            !IsNeighbourCorridor(closed))
                        {
#if UNITY_EDITOR
                            GameObject p = UnityEditor.PrefabUtility.InstantiatePrefab(preset.pillarPrefab, transform) as GameObject;
#else
                            GameObject p = Object.Instantiate(preset.pillarPrefab, transform);
#endif
                            p.transform.localPosition = new Vector3(cornerX, 0, cornerZ);
                            p.transform.localRotation = Quaternion.Euler(0, i * 90, 0);
                            p.transform.SetParent(corridor.WallsContainer.transform, true);
                        }
                    }
                }
            }
        }

        #endregion

        BakeNavMesh();

        MobSpawners(preset);

        ItemSpawners();

        GetLineOfSightObjects();

        return true;
    }

    private void GetLineOfSightObjects()
    {
        losObjects = new List<LineOfSightObject>(GetComponentsInChildren<LineOfSightObject>());
    }

    [ContextMenu("Bake Nav Mesh")]
    private void BakeNavMesh()
    {
        NavMeshBuildSettings buildSettings = new NavMeshBuildSettings
        {
            agentRadius = navMeshBuildSettings.agentRadius,
            agentHeight = navMeshBuildSettings.agentHeight,
            agentSlope = navMeshBuildSettings.agentSlope,
            agentClimb = navMeshBuildSettings.agentClimb,
            minRegionArea = navMeshBuildSettings.minRegionArea,
            tileSize = navMeshBuildSettings.tileSize,
            overrideTileSize = true
        };

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

        foreach (NavMeshObject nmo in GetComponentsInChildren<NavMeshObject>())
            sources.Add(nmo.NavMeshBuildSource());


        Vector3 size = Vector3.Scale(transform.lossyScale, new Vector3(nodes.GetLength(0), 1, nodes.GetLength(1)) * 2);

        NavMeshData data = NavMeshBuilder.BuildNavMeshData(buildSettings, sources, new Bounds(Vector3.zero, size), Vector3.zero, Quaternion.identity);


        NavMesh.RemoveAllNavMeshData();
        NavMesh.AddNavMeshData(data);
    }
    private LevelData WriteData()
    {
        MapNode[,] nodeData = new MapNode[nodes.GetLength(0), nodes.GetLength(1)];
        for (int x = 0; x < nodes.GetLength(0); x++)
            for (int z = 0; z < nodes.GetLength(1); z++)
                nodeData[x, z] = MapNode.Copy(nodes[x, z]);

        LocationData[] roomData = new LocationData[rooms.Count];
        for (int i = 0; i < rooms.Count; i++)
            roomData[i] = rooms[i].WriteData();

        LocationData[] corridorData = new LocationData[corridors.Count];
        for (int i = 0; i < corridors.Count; i++)
            corridorData[i] = corridors[i].WriteData();

        MobData[] mobData = new MobData[mobs.Count];
        for (int i = 0; i < mobs.Count; i++)
            mobData[i] = mobs[i].WriteData();

        ItemData[] itemData = new ItemData[items.Count];
        for (int i = 0; i < items.Count; i++)
            itemData[i] = items[i].WriteData();

        return new LevelData()
        {
            level = currentLevel,
            nodes = nodeData,
            roomData = roomData,
            corridorData = corridorData,
            mobData = mobData,
            itemData = itemData
        };
    }
    private void ReadData(LevelData data)
    {
        LevelGenerationPreset preset = levelPresets[data.level];

        currentLevel = data.level;
        int xSize = data.nodes.GetLength(0);
        int zSize = data.nodes.GetLength(1);
        
        nodes = new MapNode[xSize, zSize];


        for (int x = 0; x < nodes.GetLength(0); x++)
            for (int z = 0; z < nodes.GetLength(1); z++)
                nodes[x, z] = MapNode.Copy(data.nodes[x, z]);

        rooms = new List<Room>();
        corridors = new List<Corridor>();
        mobs = new List<Mob>();
        items = new List<ItemObject>();

        foreach (LocationData roomData in data.roomData)
        {
            Room prefab = registry.Room(roomData.prefabID);
            int x0 = roomData.x;
            int z0 = roomData.z;
            Units.Rotation rotation = roomData.rotation;

            int x1 = x0 + ((int)rotation % 2 == 0 ? prefab.XSize : prefab.ZSize);
            int z1 = z0 + ((int)rotation % 2 == 0 ? prefab.ZSize : prefab.XSize);

#if UNITY_EDITOR
            Room room = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as Room;
#else
        Room room = Object.Instantiate(prefab, transform);
#endif
            room.SetPosition(x0, z0, rotation);
            rooms.Add(room);
        }

        foreach (LocationData corridorData in data.corridorData)
        {
            Corridor prefab = registry.Corridor(corridorData.prefabID);
            MapNode node = Node(new Vector2Int(corridorData.x, corridorData.z));
            Units.Rotation rotation = corridorData.rotation;

#if UNITY_EDITOR
            Corridor corridor = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as Corridor;
#else
            Corridor corridor = Object.Instantiate(prefab, transform);
#endif
            corridor.SetPosition(node.x, node.z, rotation);
            corridors.Add(corridor);

            if (preset.pillarPrefab != null)
            {
                bool IsNeighbourCorridor(int neighbourIndex)
                {
                    if (!node[neighbourIndex]) return false;
                    MapNode.NodeType type = Node(node.GetNeighbourPosition(neighbourIndex)).type;
                    return type == MapNode.NodeType.Corridor || type == MapNode.NodeType.EntranceOutside;
                }


                for (int i = 0; i < 4; i++)
                {
                    int open1 = i * 2;
                    int open2 = (i * 2 + 2) % 8;
                    int closed = (i * 2 + 1) % 8;

                    int cornerX = node.x + ((i - 1 + 4) % 4) / 2;
                    int cornerZ = node.z + i / 2;

                    if (IsNeighbourCorridor(open1) &&
                        IsNeighbourCorridor(open2) &&
                        !IsNeighbourCorridor(closed))
                    {
#if UNITY_EDITOR
                        GameObject p = UnityEditor.PrefabUtility.InstantiatePrefab(preset.pillarPrefab, transform) as GameObject;
#else
                            GameObject p = Object.Instantiate(preset.pillarPrefab, transform);
#endif
                        p.transform.localPosition = new Vector3(cornerX, 0, cornerZ);
                        p.transform.localRotation = Quaternion.Euler(0, i * 90, 0);
                        p.transform.SetParent(corridor.WallsContainer.transform, true);
                    }
                }
            }
        }

        BakeNavMesh();

        foreach (MobData mobData in data.mobData)
        {
            Mob prefab = registry.Mob(mobData.prefabID);
            Mob mob = Instantiate(prefab, transform);
            mob.Teleport(new Vector3(mobData.posX, mobData.posY, mobData.posZ));
            mob.transform.rotation = new Quaternion(mobData.rotX, mobData.rotY, mobData.rotZ, mobData.rotW);
            AddMob(mob);
            AssignObjectLocation(mob.GetComponent<DynamicObject>());
        }

        foreach (ItemData itemData in data.itemData)
        {
            ItemObject prefab = registry.Item(itemData.prefabID);
            ItemObject item = Instantiate(prefab, transform);
            item.transform.position = new Vector3(itemData.posX, itemData.posY, itemData.posZ);
            item.transform.rotation = new Quaternion(itemData.rotX, itemData.rotY, itemData.rotZ, itemData.rotW);
            items.Add(item);
            AssignObjectLocation(item.GetComponent<DynamicObject>());
        }
    }
    private void Disconnect(MapNode node)
    {
        for (int i = 0; i < 8; i++)
            if (node[i])
                Node(node.GetNeighbourPosition(i)).UnsetNeighbour((i + 4) % 8);
        node.neighbours = 0;
    }
    private void SetNeighbour(MapNode node, MapNode neighbour, int direction)
    {
        node.SetNeighbour(direction);
        neighbour.SetNeighbour((direction + 4) % 8);
    }
    public MapNode Node(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= nodes.GetLength(0)) return null;
        if (pos.y < 0 || pos.y >= nodes.GetLength(1)) return null;
        MapNode node = nodes[pos.x, pos.y];
        if (node.type == MapNode.NodeType.Empty) return null;
        return node;
    }
    private Room AddRoom(Room prefab, int x0, int z0, Units.Rotation rotation, Transform parent, bool check = true)
    {

        int x1 = x0 + ((int)rotation % 2 == 0 ? prefab.XSize : prefab.ZSize);
        int z1 = z0 + ((int)rotation % 2 == 0 ? prefab.ZSize : prefab.XSize);

        if (check)
        {
            if (x0 < 0 || x1 >= nodes.GetLength(0)) return null;
            if (z0 < 0 || z1 >= nodes.GetLength(1)) return null;

            for (int z = z0; z < z1; z++)
                for (int x = x0; x < x1; x++)
                    if (nodes[x, z] != null) return null;
        }

#if UNITY_EDITOR
        Room room = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent) as Room;
#else
        Room room = Object.Instantiate(prefab, parent);
#endif

        room.SetPosition(x0, z0, rotation);

        for (int z = z0; z < z1; z++)
        {
            for (int x = x0; x < x1; x++)
            {
                nodes[x, z] = new MapNode()
                {
                    x = x,
                    z = z,
                    type = MapNode.NodeType.Room,
                    locationIndex = rooms.Count
                };
                room.roomNodes.Add(nodes[x, z]);
                if (z > z0)
                {
                    SetNeighbour(nodes[x, z], nodes[x, z - 1], 2);
                }
                if (x > x0)
                {
                    SetNeighbour(nodes[x, z], nodes[x - 1, z], 4);
                }
                if (x > x0 && z > z0)
                {
                    SetNeighbour(nodes[x, z], nodes[x - 1, z - 1], 3);
                }
                if (z > z0 && x < x1 - 1)
                {
                    SetNeighbour(nodes[x, z], nodes[x + 1, z - 1], 1);
                }
            }
        }

        bool[] openEdges = prefab.OpenEdges;

        for (int i = 0; i < room.Circumference; i++)
        {
            if (!openEdges[i]) continue;
            room.GetEntrance(i, out Vector2Int eInside, out Vector2Int eOutside, out int direction);

            int eX = eOutside.x;
            int eZ = eOutside.y;

            if (nodes[eX, eZ] == null) nodes[eX, eZ] = new MapNode()
            {
                x = eX,
                z = eZ,
                type = MapNode.NodeType.EntranceOutside,
                locationIndex = rooms.Count
            };

            room.entranceNodes.Add(nodes[eX, eZ]);

            if (nodes[eInside.x, eInside.y] == null)
            {
                Debug.Log(eInside);
            }

            nodes[eInside.x, eInside.y].type = MapNode.NodeType.EntranceInside;
            MapNode eOutsideNode = nodes[eX, eZ];
            SetNeighbour(nodes[eInside.x, eInside.y], eOutsideNode, direction);

            if (!entrances.Contains(eOutsideNode))
                entrances.Add(eOutsideNode);
        }
        rooms.Add(room);
        return room;
    }
    private Room AddRoom(Room prefab, Units.Rotation rotation, Transform parent)
    {
        if (prefab == null)
        {
            Debug.Log("Room prefab was null");
            return null;
        }

        int xSize = (int)rotation % 2 == 0 ? prefab.XSize : prefab.ZSize;
        int zSize = (int)rotation % 2 == 0 ? prefab.ZSize : prefab.XSize;

        if (FindPlaceFor(xSize, zSize, out Vector2Int position))
        {
            return AddRoom(prefab, position.x, position.y, rotation, parent, false);
        }

        return null;
    }
    private bool FindPlaceFor(int xSize, int zSize, out Vector2Int position)
    {
        position = Vector2Int.zero;

        List<Vector2Int> positions = new List<Vector2Int>();

        for (int x = 1; x < nodes.GetLength(0) - 1 - xSize; x++)
            for (int z = 1; z < nodes.GetLength(1) - 1 - zSize; z++)
                if (PlaceValid(x - 1, z - 1, xSize + 2, zSize + 2)) positions.Add(new Vector2Int(x, z));

        if (positions.Count == 0) return false;

        position = positions[Random.Range(0, positions.Count)];

        return true;
    }
    private bool PlaceValid(int x, int z, int xSize, int zSize)
    {
        for (int x0 = x; x0 < x + xSize; x0++)
            for (int z0 = z; z0 < z + zSize; z0++)
            {
                if (nodes[x0, z0] != null) return false;
            }
        return true;
    }
    private bool InsideMap(int x, int z)
    {
        return x >= 0 && z >= 0 && x < nodes.GetLength(0) && z < nodes.GetLength(1);
    }
    #endregion

    public void RemoveDynamicObject(DynamicObject obj)
    {
        
    }


    public void RemoveMob(Mob mob)
    {
        Location location = GetLocation(GetNode(mob.DynamicObject.transform.position));
        if (location)
            location.DynamicObjects.Remove(mob.DynamicObject);
        mobs.Remove(mob);
        //Destroy(mob.gameObject);
        mob.gameObject.SetActive(false);
    }

    public void RemoveItem(ItemObject item)
    {
        Location location = GetLocation(GetNode(item.DynamicObject.transform.position));
        if (location)
            location.DynamicObjects.Remove(item.DynamicObject);
        items.Remove(item);

        item.gameObject.SetActive(false);
    }

    public void EnableItem(ItemObject item)
    {
        items.Add(item);
        AssignObjectLocation(item.DynamicObject);
    }
    public void AssignObjectLocation(DynamicObject obj)
    {
        //return;

        MapNode node = GetNode(obj.transform.position);
        Location location = GetLocation(node);

        MapNode oldNode = this[obj.nodeX, obj.nodeZ];
        Location oldLocation = GetLocation(oldNode);

        if (oldNode == node &&
            oldLocation == location)
            return;


        if (oldLocation != null)
            oldLocation.DynamicObjects.Remove(obj);

        if (node == null || location == null)
        {
            obj.nodeX = -1;
            obj.nodeZ = -1;
            obj.locationIndex = -1;
            obj.nodeType = MapNode.NodeType.Empty;
            obj.transform.SetParent(null, true);
        }
        else 
        //if (obj.nodeX != node.x || obj.nodeZ != node.z)
        {

            obj.dirty = false;
            obj.nodeX = node.x;
            obj.nodeZ = node.z;
            obj.locationIndex = node.locationIndex;
            obj.nodeType = node.type;
            obj.transform.SetParent(location.ContentsContainer.transform, true);
            location.DynamicObjects.Add(obj);

            /*
            //return;

            LayerMask layerMask = LayerMask.GetMask("Outline", "Shadows");

            if (location.visibility == Location.Visibility.Shown)
            {

                obj.gameObject.layer = LayerMask.NameToLayer("Default");
                if (obj.CompareTag("Interactive")) obj.gameObject.layer = LayerMask.NameToLayer("Interactive");
                if (obj.CompareTag("Mob")) obj.gameObject.layer = LayerMask.NameToLayer("Mobs");
                

                foreach(Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                {
                    int layer = renderer.gameObject.layer;
                    if (layerMask == (layerMask | (1 << layer))) continue;
                    renderer.enabled = true;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            else
            {
                obj.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                {
                    int layer = renderer.gameObject.layer;
                    if (layerMask == (layerMask | (1 << layer))) continue;
                    renderer.enabled = false;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
            */
        }
    }

    private void AddMob(Mob mob)
    {
        mobs.Add(mob);
    }

    private void MobSpawners(LevelGenerationPreset preset)
    {

        foreach (Room room in rooms)
            foreach (MobSpawner spawner in room.GetComponentsInChildren<MobSpawner>())
                foreach (Mob mob in spawner.Spawn())
                    AddMob(mob);

        List<MobSpawner> corridorMobs = new List<MobSpawner>();

        foreach(Corridor corridor in corridors)
            corridorMobs.AddRange(corridor.GetComponentsInChildren<MobSpawner>());

        corridorMobs = corridorMobs.DistanceShortlist(preset.minCorridorMobSpawns, preset.maxCorridorMobSpawns, preset.minCorridorMobDistance);

        foreach (MobSpawner spawner in corridorMobs)
            foreach (Mob mob in spawner.Spawn())
                AddMob(mob);

    }

    private void ItemSpawners()
    {
        foreach (ItemSpawner spawner in GetComponentsInChildren<ItemSpawner>())
        {
            ItemObject item = spawner.Spawn();
            //if (item != null) items.Add(item);
        }
    }

    public static float SquareDistance(Vector3 a, Vector3 b)
    {
        return (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y) + (b.z - a.z) * (b.z - a.z);
    }

    public static bool InsideAngle(float centerX, float centerY, float yaw, float arc, float pointX, float pointY, out float angle, out float angleDelta)
    {
        angle = Mathf.Atan2(pointX - centerX, pointY - centerY);
        angleDelta = Mathf.DeltaAngle(yaw, angle * Mathf.Rad2Deg);
        return Mathf.Abs(angleDelta) < arc / 2;
    }

    public static bool InsideAngle(float centerX, float centerY, float yaw, float arc, float pointX, float pointY)
    {
        if (arc <= 0) return false;
        if (arc >= 360) return true;
        float angle = Mathf.Atan2(pointX - centerX, pointY - centerY);
        float angleDelta = Mathf.DeltaAngle(yaw, angle * Mathf.Rad2Deg);
        return Mathf.Abs(angleDelta) < arc / 2;
    }

    public static bool PointInRadius(Vector3 origin, float radius, bool checkLOS, Vector3 point)
    {
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls");
        if (SquareDistance(origin, point) <= squareDistance)
        {
            if (checkLOS)
            {
                Vector3 direction = point - origin;
                float distance = direction.magnitude;
                if (!Physics.Raycast(origin, direction, distance, layerMask))
                    return true;
            }
            else
                return true;
        }
        return false;
    }
    public List<Unit> UnitsInRadius(Vector3 origin, float radius, bool checkLOS, Unit.Faction casterFaction, Ability.Affects affectsMask)
    {
        List<Unit> list = new List<Unit>();
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls", "Doors");

        foreach (Mob mob in mobs)
        {
            if (!Ability.Filter(casterFaction, mob.GetFaction(), affectsMask))
                continue;

            Vector3 mobCenter = mob.GetCenterPosition();
            if (SquareDistance(origin, mobCenter) <= squareDistance)
            {
                if (checkLOS)
                {
                    Vector3 direction = mobCenter - origin;
                    float distance = direction.magnitude;
                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(mob);
                }
                else
                    list.Add(mob);

            }
        }

        if (Ability.Filter(casterFaction, Player.Instance.GetFaction(), affectsMask))
        {
            Vector3 playerCenter = Player.Instance.GetCenterPosition();
            if (SquareDistance(origin, playerCenter) <= squareDistance)
            {
                if (checkLOS)
                {
                    Vector3 direction = playerCenter - origin;
                    float distance = direction.magnitude;
                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(Player.Instance);
                }
                else
                    list.Add(Player.Instance);

            }
        }

        return list;
    }

    public List<Unit> UnitsInRadius(Vector3 origin, float radius, bool checkLOS, Unit.Faction faction)
    {
        List<Unit> list = new List<Unit>();
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls", "Doors");

        foreach (Mob mob in mobs)
        {
            if (mob.GetFaction() != faction)
                continue;

            Vector3 mobCenter = mob.GetCenterPosition();
            if (SquareDistance(origin, mobCenter) <= squareDistance)
            {
                if (checkLOS)
                {
                    Vector3 direction = mobCenter - origin;
                    float distance = direction.magnitude;
                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(mob);
                }
                else
                    list.Add(mob);

            }
        }

        if (Player.Instance.GetFaction() == faction)
        {
            Vector3 playerCenter = Player.Instance.GetCenterPosition();
            if (SquareDistance(origin, playerCenter) <= squareDistance)
            {
                if (checkLOS)
                {
                    Vector3 direction = playerCenter - origin;
                    float distance = direction.magnitude;
                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(Player.Instance);
                }
                else
                    list.Add(Player.Instance);

            }
        }

        return list;
    }
    public static bool PlayerInRadius(Vector3 origin, float radius, bool checkLOS)
    {
        return PointInRadius(origin, radius, checkLOS, Player.Instance.GetCenterPosition());
    }
    public List<Mob> MobsInRadius(Vector3 origin, float radius, bool checkLOS)
    {
        List<Mob> list = new List<Mob>();
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls", "Doors");

        foreach (Mob mob in mobs)
        {
            Vector3 mobCenter = mob.GetCenterPosition();
            if (SquareDistance(origin, mobCenter) <= squareDistance)
            {
                if (checkLOS)
                {
                    Vector3 direction = mobCenter - origin;
                    float distance = direction.magnitude;
                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(mob);
                }
                else
                    list.Add(mob);

            }
        }

        return list;
    }
    public static bool PointInRadius(Vector3 origin, float radius, float yaw, float arc, bool checkLOS, Vector3 point)
    {
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls");

        if (
            SquareDistance(origin, point) <= squareDistance &&
            InsideAngle(origin.x, origin.z, yaw, arc, point.x, point.z)
        )
        {
            if (checkLOS)
            {
                Vector3 direction = point - origin;
                float distance = direction.magnitude;

                if (!Physics.Raycast(origin, direction, distance, layerMask))
                    return true;
            }
            else
                return true;
        }
        return false;
    }

    public static bool PlayerInRadius(Vector3 origin, float radius, float yaw, float arc, bool checkLOS)
    {
        return PointInRadius(origin, radius, yaw, arc, checkLOS, Player.Instance.GetCenterPosition());
    }
    public List<Mob> MobsInRadius(Vector3 origin, float radius, float yaw, float arc, bool checkLOS)
    {
        List<Mob> list = new List<Mob>();
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls");

        foreach (Mob mob in mobs)
        {
            Vector3 mobCenter = mob.GetCenterPosition();

            if (
                SquareDistance(origin, mobCenter) <= squareDistance &&
                InsideAngle(origin.x, origin.z, yaw, arc, mobCenter.x, mobCenter.z)
            )
            {
                if (checkLOS)
                {
                    Vector3 direction = mobCenter - origin;
                    float distance = direction.magnitude;
                    
                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(mob);
                }
                else
                    list.Add(mob);
            }
        }
        return list;
    }

    public List<Unit> UnitsInRadius(Vector3 origin, float radius, float yaw, float arc, bool checkLOS, Unit.Faction casterFaction, Ability.Affects affectsMask)
    {
        List<Unit> list = new List<Unit>();
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls");

        foreach (Mob mob in mobs)
        {
            if (!Ability.Filter(casterFaction, mob.GetFaction(), affectsMask))
                continue;

            Vector3 mobCenter = mob.GetCenterPosition();

            if (
                SquareDistance(origin, mobCenter) <= squareDistance &&
                InsideAngle(origin.x, origin.z, yaw, arc, mobCenter.x, mobCenter.z)
            )
            {
                if (checkLOS)
                {
                    Vector3 direction = mobCenter - origin;
                    float distance = direction.magnitude;

                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(mob);
                }
                else
                    list.Add(mob);
            }
        }

        if (Ability.Filter(casterFaction, Player.Instance.GetFaction(), affectsMask))
        {
            Vector3 playerCenter = Player.Instance.GetCenterPosition();

            if (
                SquareDistance(origin, playerCenter) <= squareDistance &&
                InsideAngle(origin.x, origin.z, yaw, arc, playerCenter.x, playerCenter.z)
            )
            {
                if (checkLOS)
                {
                    Vector3 direction = playerCenter - origin;
                    float distance = direction.magnitude;

                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(Player.Instance);
                }
                else
                    list.Add(Player.Instance);
            }
        }

        return list;
    }

    public List<Unit> UnitsInRadius(Vector3 origin, float radius, float yaw, float arc, bool checkLOS, Unit.Faction faction)
    {
        List<Unit> list = new List<Unit>();
        float squareDistance = radius * radius;
        int layerMask = LayerMask.GetMask("Walls");

        foreach (Mob mob in mobs)
        {
            if (mob.GetFaction() != faction)
                continue;

            Vector3 mobCenter = mob.GetCenterPosition();

            if (
                SquareDistance(origin, mobCenter) <= squareDistance &&
                InsideAngle(origin.x, origin.z, yaw, arc, mobCenter.x, mobCenter.z)
            )
            {
                if (checkLOS)
                {
                    Vector3 direction = mobCenter - origin;
                    float distance = direction.magnitude;

                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(mob);
                }
                else
                    list.Add(mob);
            }
        }

        if (Player.Instance.GetFaction() == faction)
        {
            Vector3 playerCenter = Player.Instance.GetCenterPosition();

            if (
                SquareDistance(origin, playerCenter) <= squareDistance &&
                InsideAngle(origin.x, origin.z, yaw, arc, playerCenter.x, playerCenter.z)
            )
            {
                if (checkLOS)
                {
                    Vector3 direction = playerCenter - origin;
                    float distance = direction.magnitude;

                    if (!Physics.Raycast(origin, direction, distance, layerMask))
                        list.Add(Player.Instance);
                }
                else
                    list.Add(Player.Instance);
            }
        }

        return list;
    }

    public static bool CheckLOS(Vector3 origin, Vector3 target)
    {
        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        return Physics.Raycast(origin, direction, distance, LayerMask.GetMask("Walls"));
    }

}
