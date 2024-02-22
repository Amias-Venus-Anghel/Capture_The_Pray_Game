using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int cols;
    private Hunter[,] tiles;

    [SerializeField] private GameObject tile = null;
    private Vector3 currentPosition;

    [SerializeField] private GameObject prey = null;

    [SerializeField] private Text scoreText = null;
    private int score = 0;
    private int highScore = 0;
    public int moves = 0;

    [SerializeField] private GameObject menu = null;
    [SerializeField] private Text menuScore = null;
    [SerializeField] private Text highScoreText = null;

    void Start() {
        menu.SetActive(true);

        tiles = new Hunter[rows, cols];

        // called by new game button from menu        
        // /* create new map */
        CreateMap();
    }

    private void CreateMap() {
        /* init the tiles referances */

        for (int i = 0; i < rows; i++) {
            currentPosition = tile.transform.position;
            currentPosition.y -= i * 1.26f;
            if (i % 2 != 0) {
                currentPosition.x += 0.735f;
            }

            for (int j = 0; j < cols; j++) {
                currentPosition.x += 1.47f;
                GameObject newTile = Instantiate(tile, currentPosition, tile.transform.rotation);
                newTile.transform.SetParent(tile.transform.parent);

                tiles[i, j] = newTile.GetComponent<Hunter>();
            }
        }
        
        tile.SetActive(false);


        /* mark margins as exits */
        for (int i = 0; i < rows; i++) {
            tiles[i, 0].isExit = true;
            tiles[i, cols - 1].isExit = true;
            tiles[i, 0].gameObject.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f, 1f);
            tiles[i, cols - 1].gameObject.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f, 1f);   
        }

        for (int i = 0; i < cols; i++) {
            tiles[0, i].isExit = true;
            tiles[rows - 1, i].isExit = true;
            tiles[0, i].gameObject.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f, 1f);;
            tiles[rows - 1, i].gameObject.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f, 1f);;
        }

    }

    public void GameOver() {
        menu.SetActive(true);
        menuScore.text = "Last Score: " + score;

        highScore = highScore > score ? highScore : score;
        highScoreText.text = "High Score: " + highScore;
    }

    public void NewGame() {
        GenerateRound();
        score = 0;
        moves = 0;
        UpdateScore(false);
    }

    public void UpdateScore(bool win) {
        if (win) {
            // add to score depinding of nr of moves
            Debug.Log((1.0 / moves));
            score += 100 * (int)(100.0 / moves);
        }
        scoreText.text = "Score: " + score;
    }

    public void NewRound() {
       GenerateRound();
       UpdateScore(true); 
    }

    private void GenerateRound() {
        /* clear existing obstacles */
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                tiles[i, j].ClearObstacle();
            }
        }

        /* generate random obstacles */
        int obstacles = Random.RandomRange(15, 25);

        for (int i = 0; i < obstacles; i++) {
            int r = Random.RandomRange(0, rows - 1);
            int c = Random.RandomRange(0, cols - 1);

            while (!tiles[r, c].PlaceObstacle()) {
                r = Random.RandomRange(0, rows - 1);
                c = Random.RandomRange(0, cols - 1);
            }
        }


        /* reset prey position */
        Hunter startTile = tiles[(rows - 1) /  2, (cols - 1) / 2];
        startTile.ClearObstacle();
        prey.gameObject.GetComponent<PrayRun>().InitPrey(tiles, rows, cols);
    }


}
