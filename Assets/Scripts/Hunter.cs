using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] GameObject lionMarkerPrefab;
    [SerializeField] GameObject rockMarkerPrefab;

    private GameObject obstacle = null;

    public bool markerPlaced = false;
    public bool isExit = false;
  
    void OnMouseDown() {
        if (!markerPlaced && obstacle == null) {
            markerPlaced = true;
            obstacle = Instantiate(lionMarkerPrefab, this.transform.position, Quaternion.identity);

            GameObject.Find("Prey").GetComponent<PrayRun>().Move();
        }
    }

    public bool PlaceObstacle() {
        if (!markerPlaced) {
            markerPlaced = true;
            obstacle = Instantiate(rockMarkerPrefab, this.transform.position, Quaternion.identity);
            return true;
        }
        return false;
    }

    // debug 
    // void Update() {
    //     if (!markerPlaced && obstacle != null) {
    //         this.GetComponent<SpriteRenderer>().color = Color.red;
    //         markerPlaced = true;
    //     }
    // }

    public void ClearObstacle() {
        if (obstacle != null) {
            GameObject.Destroy(obstacle);
            obstacle = null;
        }
        markerPlaced = false;
    }

}
