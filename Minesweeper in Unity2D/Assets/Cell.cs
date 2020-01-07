using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    public bool bomb;
    public bool revealed;
    public int x, y;
    public int totalBomb;
    Manager m;

    //store each cells position
    public void Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }


    //check neighbour for bomb
    public int CheckNeighbours()
    {
        m = FindObjectOfType<Manager>();

        if (this.bomb)
        {
            return -1;
        }

        int total = 0;
        for (int xOff = 1; xOff >= -1; xOff--)
        {
            for (int yOff = 1; yOff >= -1; yOff--)
            {
                if (x + xOff >= 0 && x + xOff < m.rows && y + yOff < m.cols && y + yOff >= 0 )
                {
                    if(m.cells[x + xOff , y + yOff].GetComponent<Cell>().bomb)
                    {
                        total++;
                    }
                }
            }
        }
        return total;
    }


    //open neighbours on click
    public void OpenNeighbour()
    {
        if (this.bomb)
        {
            return;
        }
        
        for (int xOff = 1; xOff >= -1; xOff--)
        {
            for (int yOff = 1; yOff >= -1; yOff--)
            {
                if (x + xOff >= 0 && x + xOff < m.rows && y + yOff < m.cols && y + yOff >= 0)
                {
                    GameObject c = m.cells[x + xOff, y + yOff];
                    StartCoroutine(Open(c));                   
                }
            }
        }
    }

    IEnumerator Open(GameObject c)
    {
        yield return new WaitForSeconds(0f);
        if (!c.GetComponent<Cell>().bomb && !c.GetComponent<Cell>().revealed && totalBomb == 0)
        {  
            c.GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 255);
            c.GetComponent<SpriteRenderer>().sortingOrder = -1;
            c.GetComponent<Cell>().revealed = true;
            c.GetComponent<Cell>().OpenNeighbour();
        }
        else
        {
            m.CheckWin();
        }
    }

}

