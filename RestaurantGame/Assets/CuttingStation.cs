using UnityEngine.SceneManagement;

public class CuttingStation : Interactable {
    public override void Interact() {
        SceneManager.LoadScene("Minigame", LoadSceneMode.Additive);
        base.Interact();
    }
}
