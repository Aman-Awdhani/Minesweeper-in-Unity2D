using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Transform holder;

    public int rows;
    public int cols;
      
    int bombCount;
    public int totalBombs;
    public Transform panel;

    public GameObject box;
    public GameObject bomb;
    public TextMesh numText;

    public GameObject[,] cells;
    public List<GameObject> bombStore = new List<GameObject>();
    public List<GameObject> textStore = new List<GameObject>();
    public List<GameObject> storeEveryCell;

    [Header("SpawnStore")]
    public Transform bombParent;
    public Transform numberParent;
    public Transform cellParent;

    //for grid
    Vector2 size;
    [Range(0, 1)]
    public float cellGap;
    Vector2 gridScale;
    Vector2 cellScale;
    Vector2 offset;



    void Start()
    {
            

        gridScale.y = (Camera.main.orthographicSize * 2)-7;
        gridScale.x = gridScale.y * Camera.main.aspect;
      
        GameStart();
    }

    void GameStart()
    {
        cols = (rows / 2) + 1;
        cells = new GameObject[rows, cols];
        Grid();
    }

    void Grid()
    {
        cellScale = new Vector2(gridScale.x / cols, gridScale.y / rows);
        offset = -new Vector2((gridScale.x / 2) - cellScale.x / 2, (gridScale.y / 2) - cellScale.y / 2);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {

                Vector2 pos = new Vector2(j * cellScale.x + offset.x, i * cellScale.y + offset.y);
                GameObject c = Instantiate(box, pos, transform.rotation);
                c.transform.localScale = cellScale - cellScale * cellGap;

                cells[i, j] = c;
                cells[i, j].GetComponent<Cell>().Position(i, j);
                storeEveryCell.Add(c);
            }
        }
        StartCoroutine(Spawn());
    }


    private void Update()
    {
        DetecttHit();   
    }
 

    //calls function
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(.1f);
        AsignBomb();
        SpawnBomb();
        NumberSpawn();

        
        //RevealEverything();
    }

    //Asign bomb positions
    void AsignBomb()
    {
        print(totalBombs);
        for (int n = 0; n < totalBombs; n++)
        {
            here:
            float i = Mathf.Floor(Random.Range(0, rows));
            float j = Mathf.Floor(Random.Range(0, cols));

            if(cells[(int)i, (int)j].GetComponent<Cell>().bomb)
            {
                goto here;
            }
            cells[(int)i, (int)j].GetComponent<Cell>().bomb = true;
        }
    }

    //spawn bomb prefab
    void SpawnBomb()
    {
        foreach (GameObject c in cells)
        {
            if (c != null)
            {
                if (c.GetComponent<Cell>().bomb)
                {
                    GameObject b = Instantiate(bomb, c.transform.position, transform.rotation, bombParent);
                    bombStore.Add(b);

                }
            }
        }
    }

    //spawn numbers
    void NumberSpawn()
    {
        foreach (GameObject c in cells)
        {          
            int num = c.GetComponent<Cell>().CheckNeighbours();
            if (num > 0)
            {
                GameObject number = Instantiate(numText.gameObject, c.transform.position, transform.rotation , numberParent);
                number.GetComponent<TextMesh>().text = num.ToString();
                c.GetComponent<Cell>().totalBomb = num;
                textStore.Add(number);
                ;
            }
            else
            {
                c.GetComponent<Cell>().totalBomb = 0;
            }
        }
    }
   
    //cell click open
    void DetecttHit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);           
            foreach (GameObject cell in cells)
            {
                Cell c = cell.GetComponent<Cell>();

                float dis = Vector2.Distance(c.transform.position, mousPos);
                if (dis< 1f)
                {                   
                   
                    if (c.bomb)
                    {
                      StartCoroutine(RevealEverything());
                        return;
                    }
                    else
                    {
                        if (c.totalBomb == 0)
                        {
                            c.OpenNeighbour();
                        }
                        else
                        {
                            c.GetComponent<SpriteRenderer>().sortingOrder = -1;
                            c.GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 255);
                            c.revealed = true;
                            CheckWin();
                        }
                    }
                }
            }   
        }
    }

    public void CheckWin()
    {
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject g in storeEveryCell)
        {
            if (g != null)
            {
                if (g.GetComponent<Cell>().revealed)
                {
                    temp.Add(g);
                }
            }
        }

        foreach(GameObject g in temp)
        {
            if (storeEveryCell.Contains(g))
            {
                storeEveryCell.Remove(g);
            }
        }

        foreach (GameObject g in storeEveryCell)
        {
            if (g != null)
            {
                if (!g.GetComponent<Cell>().bomb)
                    return;
            }
        }
        print("win");
    }



    //reveal every cells
    IEnumerator RevealEverything()
    {
        foreach (GameObject cell1 in cells)
        {
            yield return new WaitForSeconds(.005f);
            cell1.GetComponent<SpriteRenderer>().sortingOrder = -1;
            if (!cell1.GetComponent<Cell>().bomb)
            {
                cell1.GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 255);
            }
        }
        StartCoroutine(Restart());
    }


    //restart the game "Just experimenting"
    public void ResetGame()
    {
        
        foreach (GameObject num in textStore)
        {
            Destroy(num);
        }
        foreach (GameObject bomb in bombStore)
        {
            Destroy(bomb);
        }
        foreach (GameObject cell in cells)
        {
            Destroy(cell);
        }
        textStore.Clear();
        bombStore.Clear();

        GameStart();
    }
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(3);
        ResetGame();
    }

    public void Level(string lvl)
    {
        if (lvl == "easy")
        {
            rows = 12;
            cols = (rows / 2) + 1;
            totalBombs = 8;
        }
        else if (lvl == "medium")
        {
            rows = 20;
            cols = (rows / 2) + 1;
            totalBombs = 15;
        }
        else
        {
            rows = 28;
            cols = (rows / 2) + 1;
            totalBombs = 28;
        }

        ResetGame();
    }

}

