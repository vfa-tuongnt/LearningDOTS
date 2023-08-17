using System.Collections;
using System.Linq;
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

    private List<Node> closeNodes = new List<Node>(); // Node already check
    private List<Node> openNodes = new List<Node>();  // Node haven't check
    private List<Node> path = new List<Node>();
    private float playerRadius;
    private float moveDistance = 0.3f;

    void Awake()
    {
        Instance = this; 
    }

    public List<Node> FindPath(PlayerMovement playerMovement, CapsuleCollider playerCollider)
    {
        // Debug.Log(FindCostDistance(playerMovement.transform.position, _endPoint.position));
        // Debug.Log(FindCostDistance(playerMovement.transform.position, _endPoint2.position));

        openNodes = new List<Node>();
        closeNodes = new List<Node>();

        var startPoint = playerMovement.transform.position;
        playerRadius = playerCollider.radius + 1;
         

        _endNode = new Node(_endPoint.position);

        Node startNode = new Node(startPoint);
        startNode.ID = (0, 0);
        startNode.h = FindCostDistance(startNode, _endNode);
        startNode.g = 0;

        openNodes.Add(startNode);

        int loopLimit = 1000;
        int loopTime = 0;

        while(openNodes.Count > 0 && loopLimit > 0)
        {
            loopTime++;
            Node currentNode = openNodes[0];
            foreach(Node child in openNodes)
            {
                if(child.f < currentNode.f || child.f == currentNode.f && child.h < currentNode.h)
                {
                    // closer to the end node
                    currentNode = child;
                }
            }

            closeNodes.Add(currentNode);
            openNodes.Remove(currentNode);

            if(Vector3.Distance(currentNode.position,  _endPoint.position) < 2f)
            {
                // got to the end node
                path = new List<Node>();
                int count = 100;
                while(currentNode != startNode && count > 0)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.connectedNode;
                    count--;
                    if(count == 0) Debug.Log("Can't found path");
                }
                Debug.Log("LoopTime: " + loopTime);
                path.Reverse();
                return path;
            }
            
            foreach(var neighbor in FindNeighbors(currentNode, moveDistance))
            {
                if(IsBlock(neighbor)) continue;
                if(IsInCloseNodes(neighbor)) continue;

                bool isOpen = IsInOpenNodes(neighbor);
                float costFromStartToNeighbor = currentNode.g + neighbor.g;

                if(!isOpen || costFromStartToNeighbor < neighbor.g)
                {
                    neighbor.g = costFromStartToNeighbor;
                    neighbor.connectedNode = currentNode;

                    if(!isOpen)
                    {
                        openNodes.Add(neighbor);
                    }
                }                
            }
            loopLimit--;
        }
        Debug.Log("LoopTime: " + loopTime);
        return null;
    }

    private Node[] FindNeighbors(Node currentNode, float distance)
    {
        (int, int)[] directions = 
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

        // Debug.Log("Current node: " + currentNode.position);
        for(int i = 0; i < directions.Length; i++)
        {
            neighbors[i] = new Node(currentNode.position + new float3(directions[i].Item1 * distance, 0, directions[i].Item2 * distance));
            neighbors[i].ID = ((currentNode.ID.Item1 + directions[i].Item1), (currentNode.ID.Item2 + directions[i].Item2));
            neighbors[i].g = FindCostDistance(neighbors[i], currentNode);
            neighbors[i].h = FindCostDistance(neighbors[i], _endNode);
        }
        return neighbors;
    }
    
    private bool IsBlock(Node testNode)
    {
        float3 center = testNode.position;
        foreach(var neighbor in FindNeighbors(testNode, playerRadius))
        {
            // Check if 8 directions is blocked
            foreach(Collider col in _obstaclesColliders)
            {
                if (col.bounds.Contains(neighbor.position))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsInCloseNodes(Node node)
    {
        foreach(var nod in closeNodes)
        {
            if(nod.ID.Equals(node.ID))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInOpenNodes(Node node)
    {
        foreach(var nod in openNodes)
        {
            if(nod.ID.Equals(node.ID))
            {
                return true;
            }
        }
        return false;
    }

    private Node GetNode((int, int) ID)
    {
        return openNodes.First(x => x.ID.Equals(ID));
    }

    public float FindCostDistance(Node currentNode, Node targetNode)
    {
        float xValue = math.abs((currentNode.position.x) - (targetNode.position.x));
        float zValue = math.abs((currentNode.position.z) - (targetNode.position.z));
        float horizontalMove = math.abs(xValue - zValue);

        return DEFAULT_DIAGONAL_VALUE * math.min(xValue, zValue) + DEFAULT_STRAIGHT_VALUE * horizontalMove;
    }
    
    public float FindCostDistance(float3 currentPosition, float3 targetPosition)
    {
        float xValue = math.abs((currentPosition.x) - (targetPosition.x));
        float zValue = math.abs((currentPosition.z) - (targetPosition.z));
        float horizontalMove = math.abs(xValue - zValue);

        return DEFAULT_DIAGONAL_VALUE * math.abs(math.min(xValue, zValue)) + DEFAULT_STRAIGHT_VALUE * horizontalMove;
    }
}

public class Node
{
    public (int, int) ID;
    public Node connectedNode;
    public float3 position;
    public float g;
    public float h;
    public float f => g + h;

    public Node(){}

    public Node(float3 position)
    {
        this.position = position;
    }
}
