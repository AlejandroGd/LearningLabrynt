using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabryntDisplay : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] List<Cell> cells;
    
    //Clean all cells trail color.
    public void ClearTrail()
    {
        foreach (Cell c in cells)
        {
            c.ClearTrail();
        }
    }

    //Marks a cell as "visited" by darkening the shade of color in it.
    public void MarkTrail(int cell)
    {
        cells[cell].MarkTrail();
    }

    //Move the player to a new cell and mark the trail in that cell.
    public void MovePlayer(int cell)
    {
        player.transform.position = cells[cell].gameObject.transform.position;
        MarkTrail(cell);
    }
}
