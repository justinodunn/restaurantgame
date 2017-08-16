using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by David Malaky
public class Grid0 : MonoBehaviour {
    public int gridX, gridY;
    public float gridRes;
    #region Variables

    #endregion

    // Methods here
    private void OnDrawGizmosSelected() {
        for (int x = 0; x < gridX; x++) {
            for (int y = 0; y < gridY; y++) {
                Gizmos.DrawWireCube(new Vector3(x, y) * gridRes+transform.position, Vector2.one * gridRes); 
            }
        }
    }
}