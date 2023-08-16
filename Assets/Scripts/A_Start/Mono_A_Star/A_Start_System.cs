using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Extension;

public class A_Start_System : MonoBehaviour
{
    const float DEFAULT_STRAIGHT_VALUE = 10;
    const float DEFAULT_DIAGONAL_VALUE = 14;

    public static A_Start_System Instance;
    public bool _isReachTarget;

    [SerializeField] Transform _endPoint;
    [SerializeField] Transform _endPoint2;
    [SerializeField] List<Collider> _obstaclesColliders;
    private Node _endNode;

    void Awake()
    {
        Instance = this; 
    }

    public void FindPath(PlayerMovement playerMovement, CapsuleCollider playerCollider)
    {
        // Debug.Log(FindCostDistance(playerMovement.transform.position, _endPoint.position));
        // Debug.Log(FindCostDistance(playerMovement.transform.position, _endPoint2.position));

        var endPoint = _endPoint.position;
        var startPoint = playerMovement.transform.position;
        float moveDistance = playerCollider.radius;
        List<Node> closeNodes = new List<Node>(); // Node already check
        List<Node> openNodes = new List<Node>();  // Node haven't check
        List<Node> route = new List<Node>();

        _endNode = new Node(_endPoint.position);

        Node currentNode = new Node(startPoint);
        route.Add(currentNode);
        currentNode.h = FindCostDistance(currentNode, _endNode);
        currentNode.g = 0;

        FindNeighbors(currentNode, openNodes, closeNodes, moveDistance);

        if(openNodes.Count != 0 && _isReachTarget == false)
        {
            currentNode = FindNextNode(route, closeNodes, openNodes, currentNode, moveDistance);
        }
        Debug.Log("Move distance: " + moveDistance);
        Debug.Log("AAAA" + currentNode.position);
        playerMovement.MovePlayer(currentNode.position);
    }

    private Node FindNextNode(List<Node> route, List<Node> closeNodes, List<Node> openNodes, Node currentNode, float moveDistance)
    {
        closeNodes.Add(currentNode);
        Node bestNode = openNodes.GetRandom();
        // Debug.Log(bestNode.f + ": best node");
        foreach(Node node in openNodes)
        {
            // Debug.Log(node.f + ": node");

            if (node.f <= bestNode.f)
            {
                bestNode = node;
            }
        }
        if (bestNode == currentNode)
        {
            bestNode = openNodes.GetRandom();
        }
        if (Vector3.Distance(bestNode.position, _endNode.position) < 0.5f)
        {
            _isReachTarget = true;
            bestNode = _endNode;
        }
        route.Add(bestNode);
        Debug.Log(bestNode.position);
        closeNodes.Add(bestNode);
        openNodes.Remove(bestNode);


        // Find 8 neighbors around the current Node
        FindNeighbors(currentNode, openNodes, closeNodes, moveDistance);
        return bestNode;
    }

    private void FindNeighbors(Node currentNode, List<Node> openNodes, List<Node> closeNodes, float moveDistance)
    {
        (float, float)[] directions = 
        {
            (0, 1), // up
            (0, -1), // down
            (1, 0), // right
            (-1, 0), // left
            (-1, -1), // up, left
            (1, 1), // up, right
            (-1, -1), // down, left
            (1, -1), // up, right
        };
        Node[] neighbors = new Node[8];

        Debug.Log("Current node: " + currentNode.position);
        for(int i = 0; i < directions.Length; i++)
        {
            neighbors[i] = new Node(currentNode.position + new float3(directions[i].Item1 * moveDistance, 0, directions[i].Item2 * moveDistance));
            neighbors[i].g = FindCostDistance(neighbors[i], currentNode);
            neighbors[i].h = FindCostDistance(neighbors[i], _endNode);
            Debug.Log("Neighbors: " + neighbors[i].position);
            if(IsBlock(directions[i], currentNode)) continue;
            if(IsInCloseNodes(neighbors[i], closeNodes)) continue;
            openNodes.Add(neighbors[i]);
        }
    }

    private bool IsBlock((float, float) direction, Node testNode)
    {
        foreach(Collider col in _obstaclesColliders)
        {
            float3 temp = new float3(direction.Item1 * 0.6f, 0, direction.Item2 * 0.6f);
            if (col.bounds.Contains(testNode.position + temp))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInCloseNodes(Node node, List<Node> closeNodes)
    {
        foreach(var nod in closeNodes)
        {
            if(nod.position.Equals(node.position))
            {
                return true;
            }
        }
        return false;
    }

    public float FindCostDistance(Node currentNode, Node targetNode)
    {
        float xValue = math.abs((currentNode.position.x) - (targetNode.position.x));
        float zValue = math.abs((currentNode.position.z) - (targetNode.position.z));
        float remaining = math.abs(xValue - zValue);

        return DEFAULT_DIAGONAL_VALUE * math.min(xValue, zValue) + DEFAULT_STRAIGHT_VALUE * remaining;
    }
    
    public float FindCostDistance(float3 currentPosition, float3 targetPosition)
    {
        float xValue = math.abs((currentPosition.x) - (targetPosition.x));
        float zValue = math.abs((currentPosition.z) - (targetPosition.z));
        float remaining = math.abs(xValue - zValue);

        return DEFAULT_DIAGONAL_VALUE * math.abs(math.min(xValue, zValue)) + DEFAULT_STRAIGHT_VALUE * remaining;
    }
}

public class Node
{
    public float3 position;
    public float g;
    public float h;
    public float f => g + h;

    public Node(float3 position)
    {
        this.position = position;
    }
}
