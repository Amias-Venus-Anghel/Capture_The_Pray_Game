using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PrayRun : MonoBehaviour
{
    private Hunter[,] tiles;
    public List<Tuple<int, int>> nei;

    
    private int rows, cols;
    private int coord_x = -1, coord_y = -1;

    [SerializeField] private Color exitTileColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private bool showHighlightPath = false;


    public void SetHighlightPath(bool show) {
        showHighlightPath = show;
    }

    // initiate prey data
    public void InitPrey(Hunter[,] gameTiles, int mrows, int mcols) {
        tiles = gameTiles;
        rows = mrows;
        cols = mcols;
        coord_x = -1;
        coord_y = -1;

        SetCoords((rows - 1) /  2, (cols - 1) / 2);
    }

    // sets prey coordonates and move prey
    public void SetCoords(int x, int y) {
        if (coord_x != -1 && coord_y != -1)
            tiles[coord_x, coord_y].markerPlaced = false;
        
        coord_x = x;
        coord_y = y;

        this.gameObject.transform.position = tiles[x, y].gameObject.transform.position;
        tiles[coord_x, coord_y].markerPlaced = true;
    }

    // prey turn
    public void Move() {
        GameObject.Find("GameMaster").GetComponent<GameMaster>().moves++;
        TilesColorReset();

        if (tiles[coord_x, coord_y].isExit) {
            Debug.Log("GAME OVER");
            GameObject.Find("GameMaster").GetComponent<GameMaster>().GameOver();
            return;
        }

        List<(int, int)> shortestPath = ShortestPath((coord_x, coord_y));
        if (shortestPath.Count > 0)
        {
           Debug.Log($"The shortest path to the nearest exit is: {string.Join(" -> ", shortestPath)}");
            shortestPath.RemoveAt(0);
            SetCoords(shortestPath[0].Item1, shortestPath[0].Item2);
           
           if (showHighlightPath) {
            foreach ((int,int) pathTile in shortestPath) {
                tiles[pathTile.Item1, pathTile.Item2].gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
            }
           }
           
        }
        else
        {
            (int, int) next = RandomMove(coord_x, coord_y);
            if (next.Item1 == -1) {
                Debug.Log("GAME WON");
                GameObject.Find("GameMaster").GetComponent<GameMaster>().NewRound();
            }
            else {
                SetCoords(next.Item1, next.Item2);
            }
           
        }

    }

    private void TilesColorReset() {
        for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    tiles[i, j].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    if (tiles[i,j].isExit) {
                        tiles[i, j].gameObject.GetComponent<SpriteRenderer>().color = exitTileColor;
                    }
                }
            }
    }

    private (int, int) RandomMove(int x, int y) {
        List<(int, int)> nei = Neighbors(x, y);
        if (nei.Count > 0) {
            return nei[UnityEngine.Random.RandomRange(0, nei.Count - 1)];
        }
        else {
            return (-1,-1);
        }
    }

    /* functions to calculate exit path */
    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < rows && y >= 0 && y < cols  
                && !tiles[x, y].markerPlaced;
    }

    private int Heuristic((int, int) a, (int, int) b)
    {
        return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);
    }

    private List<(int, int)> Neighbors(int x, int y)
    {
        List<(int, int)> possibleNeighbors;

        if (x % 2 == 0)
        {
            possibleNeighbors = new List<(int, int)>
            {
                (x - 1, y - 1), (x - 1, y), (x, y - 1),
                (x, y + 1), (x + 1, y - 1), (x + 1, y)
            };
        }
        else
        {
            possibleNeighbors = new List<(int, int)>
            {
                (x - 1, y + 1), (x - 1, y), (x, y - 1),
                (x, y + 1), (x + 1, y + 1), (x + 1, y)
            };
        }

        // filter valid neighbors
        return possibleNeighbors.FindAll(neighbor => IsValid(neighbor.Item1, neighbor.Item2));
    }

    private List<(int, int)> ShortestPath((int, int) start)
    {
        var priorityQueue = new List<(int, List<(int, int)>)>();
        var visited = new HashSet<(int, int)>();

        priorityQueue.Add((0, new List<(int, int)>{ start }));

        while (priorityQueue.Count > 0)
        {
            priorityQueue.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            var (cost, path) = priorityQueue[0];
            priorityQueue.RemoveAt(0);

            var current = path[path.Count - 1];

            if (tiles[current.Item1, current.Item2].isExit)
            {
                return path;
            }

            if (visited.Contains(current))
            {
                continue;
            }

            visited.Add(current);

            foreach (var neighbor in Neighbors(current.Item1, current.Item2))
            {
                if (!visited.Contains(neighbor))
                {
                    var newPath = new List<(int, int)>(path) { neighbor };
                    var newCost = cost + 1;
                    priorityQueue.Add((newCost + Heuristic(neighbor, current), newPath));
                }
            }
        }

        return new List<(int, int)>(); // If no path is found
    }

}
