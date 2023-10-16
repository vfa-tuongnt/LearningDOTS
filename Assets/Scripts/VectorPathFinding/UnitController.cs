using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    [SerializeField] private GameObject unit;
    [SerializeField] private GridController gridController;
    public int numberUnit;
    List<GameObject> unitsInGame = new();
    public float moveSpeed = 5;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnUnit();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeleteUnit();
        }
    }

    private void FixedUpdate()
    {
        if(gridController.curFlowField == null)
        { return; }
        foreach(GameObject unit in unitsInGame)
        {
            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(unit.transform.position);
            Vector3 moveDirection = new(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
            Rigidbody rb = unit.GetComponent<Rigidbody>();
            rb.velocity = moveDirection * moveSpeed;
        }
    }

    private void SpawnUnit()
    {
        Vector2Int gridSize = gridController.gridSize;
        float cellRadius = gridController.cellRadius;
        Vector2 maxSpawnPos = new(gridSize.x * cellRadius * 2 + cellRadius, gridSize.y * 2 * cellRadius + cellRadius);
        int colMask = LayerMask.GetMask("Wall", "Mud");
        Vector3 newPos;
        for (int i = 0; i < numberUnit; i++)
        {
            GameObject newUnit = Instantiate(unit);
            newPos = new Vector3(Random.Range(0, maxSpawnPos.x), 0.55f, Random.Range(0, maxSpawnPos.y));
            newUnit.transform.position = newPos;
            unitsInGame.Add(newUnit);
        }
    }

    private void DeleteUnit()
    {
        foreach(GameObject unit in unitsInGame)
        {
            Destroy(unit);
        }
        unitsInGame.Clear();
    }
}
