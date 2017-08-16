using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by David Malaky
public class Player : MonoBehaviour {
    [SerializeField] LayerMask controlMask;
    [SerializeField] PathfindingAgent pathfindingAgent;
    #region Variables

    #endregion

    // Methods here
    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity ,controlMask)) {
                pathfindingAgent.MoveToTarget(hit.point);
            print(hit.point);
            }
        }

    }
}