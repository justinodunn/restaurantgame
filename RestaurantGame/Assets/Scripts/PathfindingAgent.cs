using System.Collections;
using UnityEngine;

public class PathfindingAgent : MonoBehaviour {

    public float movementSpeed;
    Rigidbody2D rb;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }


    public Transform target;
    Vector2[] path;
    int targetIndex;

    public void MoveToTarget(Transform target) {
        Navigation.RequestPath(transform.position, target.position, OnPathFound);
    }
    public void MoveToTarget(Vector2 target) {
        Navigation.RequestPath(transform.position, target, OnPathFound);
    }

    private void OnPathFound(Vector2[] newPath, bool success) {
        StopAllCoroutines();// (FollowPath());
        if (success) {
            path = newPath;
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath() {
        Vector2 currentWaypoint = path[0];
            targetIndex = 0;

        while (true) {
            print("running");
            if (transform.position.x == currentWaypoint.x && transform.position.y == currentWaypoint.y) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    yield break;
                }
                else {
                currentWaypoint = path[targetIndex];
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWaypoint.x, currentWaypoint.y, transform.position.z), movementSpeed * Time.deltaTime);
            yield return null;
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
