using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CapsuleCollider _playerCollider;
    Rigidbody _myBody;

    void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        A_Start_System.Instance.FindPath(this, _playerCollider);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            A_Start_System.Instance.FindPath(this, _playerCollider);
        }
    }

    public void MovePlayer(float3 position)
    {
        this._myBody.Move(position, Quaternion.identity);
    }
}
