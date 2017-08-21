using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Made by David Malaky
public class Player : MonoBehaviour {
    public static Player current;
    [SerializeField] LayerMask controlMask;
    [SerializeField] PathfindingAgent pathfindingAgent;
    public GameObject Canvas;
    public string PlayerStatus = "Moving";
    public Role role;
    private static FSM fsm = FSM.Game;

    public List<InventoryItem> inventory;

    // Methods here
    private void Update() {
        switch (fsm) {
            case FSM.Game:
                if (Input.GetMouseButtonDown(1)) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, controlMask)) {
                        if (hit.transform.gameObject.tag == "Interactable") {
                            Interactable target;
                            Vector2 destination = Vector2.zero;
                            target = hit.transform.gameObject.GetComponent<Interactable>();
                            switch (role) {
                                case Role.Waiter:
                                    destination = target.WaiterSpot;
                                    break;
                                case Role.Chef:
                                    destination = target.ChefSpot;
                                    break;
                                case Role.Manager:
                                    destination = target.ManagerSpot;
                                    break;
                            }
                            if (destination == Vector2.zero) return;
                            pathfindingAgent.MoveToTarget(destination, null);

                        }
                        else pathfindingAgent.MoveToTarget(hit.point, null);
                    }
                }
                if (Input.GetMouseButtonDown(0)) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, controlMask)) {
                        if (hit.transform.gameObject.tag == "Interactable") {
                            Interactable target;
                            Vector2 destination = Vector2.zero;
                            target = hit.transform.gameObject.GetComponent<Interactable>();
                            print(hit.transform.gameObject.name);
                            Action onReachedAction = null;
                            switch (role) {
                                case Role.Waiter:
                                    destination = target.WaiterSpot;
                                    onReachedAction = target.Interact;
                                    break;
                                case Role.Chef:
                                    destination = target.ChefSpot;
                                    onReachedAction = target.Interact;
                                    break;
                                case Role.Manager:
                                    destination = target.ManagerSpot;
                                    onReachedAction = target.Interact;
                                    break;
                            }
                            if (destination == Vector2.zero) return;
                            pathfindingAgent.MoveToTarget(destination, onReachedAction);

                        }
                        else pathfindingAgent.MoveToTarget(hit.point, null);
                    }
                }
                break;
        }
    }

    public enum Role {
        Waiter,
        Chef,
        Manager
    }

    private enum FSM {
        UI,
        Game
    }
}
