using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Navigation : MonoBehaviour {
    [SerializeField] private Node[] grid;
    [SerializeField] int gridSizeX;
    [SerializeField] int gridSizeY;

    private void Start() {
        instance = this;
    }

    public float nodeRadius;
    public LayerMask unwalkableMask;

    private Node GetNode(int x, int y) {
        return grid[y * gridSizeY + x];
    }
    private void SetNode(int x, int y, Node node) {
        grid[y * gridSizeY + x] = node;
    }


    public int maxSize {
        get { return gridSizeX * gridSizeY; }
    }

    private void OnDrawGizmos() {

        if (grid.Length != 0) {
            foreach (Node node in grid) {
                Gizmos.color = (node.Walkable) ? Color.white : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector2.one * ((2*nodeRadius) - .1f));
            }
        }
    }


    public void CreateGrid() {

        grid = new Node[gridSizeX * gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * (gridSizeX / 2) - Vector2.up * (gridSizeY / 2);

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector2 worldPoint = Vector2.right * (x * (nodeRadius * 2) + nodeRadius) + Vector2.up * (y * (nodeRadius * 2) + nodeRadius);
                bool walkable = !(Physics2D.BoxCast(worldPoint, Vector2.one * nodeRadius, 0, Vector2.zero, Mathf.Infinity, unwalkableMask));
                SetNode(x, y, new Node(walkable, worldPoint, x, y));
            }
        }
    }

    private Node NodeFromWorldPoint(Vector2 worldPosition) { //??
        //float percentX = (worldPosition.x + gridSizeX / 2) / gridSizeX;
        //float percentY = (worldPosition.y + gridSizeY / 2) / gridSizeY;
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);

        int x =Mathf.RoundToInt( Mathf.Clamp( worldPosition.x*2.5f,0,gridSizeX)) ;
        int y = Mathf.RoundToInt(Mathf.Clamp(worldPosition.y * 2.5f, 0, gridSizeY));
        return GetNode(x, y);
    }

    List<Node> _neighbours;
    private List<Node> GetNeighbours(Node node) {
        _neighbours = new List<Node>();

        for (int x = -1; x < 2; x++) {
            for (int y = -1; y < 2; y++) {
                if (y == x && x == 0) continue;
                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    _neighbours.Add(GetNode(checkX, checkY));
                }
            }
        }
        return _neighbours;
    }


    // pathfinding

    public void StartFindPath(Vector2 start, Vector2 target) {
        StartCoroutine(FindPath(start, target));
    }

    IEnumerator FindPath(Vector2 startPosition, Vector2 endPosition) { // Najít cestu
        Node startNode = NodeFromWorldPoint(startPosition);
        Node endNode = NodeFromWorldPoint(endPosition);

        Vector2[] path = new Vector2[0];
        bool pathFound = false;

        if (endNode.Walkable) {
            Heap<Node> openSet = new Heap<Node>(maxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == endNode) {
                    pathFound = true;
                    break;
                }

                foreach (Node node in GetNeighbours(currentNode)) {
                    if (!node.Walkable || closedSet.Contains(node)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, node);
                    if (newMovementCostToNeighbour < node.gCost || !openSet.Contains(node)) {
                        node.gCost = newMovementCostToNeighbour;
                        node.hCost = GetDistance(node, endNode);
                        node.Parent = currentNode;

                        if (!openSet.Contains(node))
                            openSet.Add(node);
                        else
                            openSet.UpdateItem(node);
                    }
                }
            }
        }
        yield return null;
        if (pathFound) {
            path = RetracePath(startNode, endNode);
        }
        FinishedProcessingPath(path, pathFound);
    }

    int GetDistance(Node startNode, Node endNode) { // Kalkulace hCost
        return (Math.Max(startNode.GridX, endNode.GridX) - Math.Min(startNode.GridX, endNode.GridX)) + (Math.Max(startNode.GridY, endNode.GridY) - Math.Min(startNode.GridY, endNode.GridY));
    }

    Vector2[] RetracePath(Node startNode, Node endNode) { // Zjištění cesty
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        Vector2[] waypoints = SimplifyPath(path.ToArray());
        Array.Reverse(waypoints);
        return waypoints; // funguje jak má
    }

    Vector2[] SimplifyPath(Node[] path) { // převod z Node[] na Vector2[]
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Length; i++) {
            waypoints.Add(path[i].WorldPosition);
        }
        return waypoints.ToArray();
    }
    //


    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    bool isProcessingPath;
    static Navigation instance;

    public static void RequestPath(Vector2 pathStart, Vector2 PathEnd, Action<Vector2[], bool> callback) {
        PathRequest newPathRequest = new PathRequest(pathStart, PathEnd, callback);
        instance.pathRequestQueue.Enqueue(newPathRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext() {
        if (pathRequestQueue.Count > 0) {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;

            StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector2[] path, bool success) {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback) {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }

    [Serializable]
    private class Node {
        public bool Walkable;
        public Vector2 WorldPosition;
        public int GridX, GridY;

        public int gCost;
        public int hCost;
        public Node Parent;

        public int fCost {
            get { return gCost + hCost; }
        }

        int heapIndex;
        public int HeapIndex {
            get {
                return heapIndex;
            }

            set {
                heapIndex = value;
            }
        }

        public Node(bool _walkable, Vector2 _worldPosition, int _gridX, int _gridY) {
            Walkable = _walkable;
            WorldPosition = _worldPosition;
            GridX = _gridX;
            GridY = _gridY;
        }

        public int CompareTo(Node nodeToCompare) {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0) {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }

    private class Heap<T> where T : Node {
        T[] items;
        int currentItemCount;

        public Heap(int maxHeapSize) {
            items = new T[maxHeapSize];
        }

        public void Add(T item) {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }

        public T RemoveFirst() {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        public void UpdateItem(T item) {
            SortUp(item);
        }

        public int Count {
            get { return currentItemCount; }
        }

        public bool Contains(T item) {
            return Equals(items[item.HeapIndex], item);
        }

        void SortUp(T item) {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true) {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0) {
                    Swap(item, parentItem);
                }
                else break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void SortDown(T item) {
            while (true) {
                int childIndexL = item.HeapIndex * 2 + 1;
                int childIndexR = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexL < currentItemCount) {
                    swapIndex = childIndexL;

                    if (childIndexR < currentItemCount) {
                        if (items[childIndexL].CompareTo(items[childIndexR]) < 0) {
                            swapIndex = childIndexR;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0) {
                        Swap(item, items[swapIndex]);
                    }
                    else return;

                }
                else return;
            }
        }

        void Swap(T itemA, T itemB) {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;

            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }
}