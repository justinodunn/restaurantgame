using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Made by David Malaky
public class GridHolder : MonoBehaviour {

    #region Variables
    List<InventoryItem> UnusedItems;
    List<InventoryItem> UsedItems= new List<InventoryItem>();
    #endregion

    // Methods here

    private void Start() {
        Camera.main.gameObject.SetActive(false);
        //UnusedItems = Player.current.inventory;
        UnusedItems = new List<InventoryItem>() { new GDFood(),new GDFood(),new GDFood(),new GDFood(), new GDFood(), new GDFood(), new GDFood() };
        CreateNewBox();
    }

    int currentItem;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (--currentItem < 0) currentItem = UnusedItems.Count - 1;
            currentRend.color = Color.blue / 100 * currentItem;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (++currentItem >= UnusedItems.Count) currentItem = 0;
            currentRend.color = Color.white / 10 * currentItem + new Color(0,0,0,1);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            UsedItems.Add(UnusedItems[currentItem]);
            UnusedItems.RemoveAt(currentItem);
            CreateNewBox();
            if (UnusedItems.Count == 0) {
                FinalizeFood();
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
            FinalizeFood();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UnusedItems.AddRange(UsedItems);
            Camera.main.gameObject.SetActive(true);
            SceneManager.UnloadSceneAsync(gameObject.scene);
            
        }
    }

    public GameObject boxprefab;
    private SpriteRenderer currentRend;
    public float VertOffset;

    public void CreateNewBox() {
        GameObject box = Instantiate(boxprefab);
        box.transform.position = Vector2.up * UsedItems.Count * VertOffset;
        currentRend = box.GetComponent<SpriteRenderer>();
        transform.position = box.transform.position;
        currentItem = 0;
    }

    public void FinalizeFood() {
        enabled = false;

        UnusedItems.Add(new GDFood() { name = "new" });
        Camera.main.gameObject.SetActive(true);
        SceneManager.UnloadSceneAsync(gameObject.scene);

    }

}