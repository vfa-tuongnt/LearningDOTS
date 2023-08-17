using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Extension;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CapsuleCollider _playerCollider;
    [SerializeField] float _speed = 3;
    Rigidbody _myBody;
    List<Node> path = new List<Node>();

    void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        path = A_Start_System.Instance.FindPath(this, _playerCollider);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            path = A_Start_System.Instance.FindPath(this, _playerCollider);
            if(path != null)
            {
                StartCoroutine(MovePlayer(path));
            }
        }
    }

    public void MovePlayer(float3 position)
    {
        this._myBody.Move(position, Quaternion.identity);
    }
    IEnumerator MovePlayer(List<Node> nodes)
    {
        int i = 0;
        while(i < nodes.Count - 1)
        {
            Vector3 dir = (nodes[i].position.ToVector3() - this.transform.position).normalized;
            Debug.Log("Move : " + nodes[i].position.ToVector3());
            _myBody.velocity = dir * _speed;
            if(Vector3.Distance(this.transform.position, nodes[i].position.ToVector3()) < 0.7f)
            {
                i++;
            }
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        if(path == null) return;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i].position, path[i + 1].position, Color.green);
        }
    }
}
