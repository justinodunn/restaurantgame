using System.Collections;
using UnityEngine;
using System;

public class PathfindingAgent : MonoBehaviour {

    Action OnDestinationReached;

    public float movementSpeed;
    Rigidbody2D rb;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }


    public Transform target;
    Vector2[] path;
    int targetIndex;

    public void MoveToTarget(Transform target, Action onDestinationReached) {
        Navigation.RequestPath(transform.position, target.position, OnPathFound);
        OnDestinationReached = onDestinationReached;
    }
    public void MoveToTarget(Vector2 target, Action onDestinationReached) {
        Navigation.RequestPath(transform.position, target, OnPathFound);
        OnDestinationReached = onDestinationReached;
    }

    private void OnPathFound(Vector2[] newPath, bool success) {
        StopAllCoroutines();// (FollowPath());
        if (success) {
            path = newPath;
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath() {
        if (path.Length > 0) {
            Vector2 currentWaypoint = path[0];
            targetIndex = 0;

            while (true) {
                if (transform.position.x == currentWaypoint.x && transform.position.y == currentWaypoint.y) {
                    targetIndex++;
                    if (targetIndex >= path.Length) {
                        if (OnDestinationReached != null)
                            OnDestinationReached();
                        yield break;
                    }
                    else {
                        currentWaypoint = path[targetIndex];
                    }
                }
                rb.MovePosition(Vector3.MoveTowards(transform.position, new Vector3(currentWaypoint.x, currentWaypoint.y, transform.position.z), movementSpeed * Time.deltaTime));
                yield return null;
            }
        }
        else {
            if (OnDestinationReached != null)
                OnDestinationReached();
        }
    }


    // Editor Only
    private void OnDrawGizmos() {
        if (path!= null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one*0.4f);

                if (targetIndex == i) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

}
