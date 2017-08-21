using UnityEngine;
using System;
using System.Collections.Generic;

public class Interactable : MonoBehaviour {
    public Vector3 WaiterSpot;
    public Vector3 ChefSpot;
    public Vector3 ManagerSpot;

    private void Awake() {
        gameObject.tag = "Interactable";
    }

    public virtual void Interact() {
        print("INTERACTED");
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(WaiterSpot + Vector3.forward * -10, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ChefSpot + Vector3.forward * -10, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ManagerSpot + Vector3.forward * -10, 0.5f);
    }
}

public class GameManager : MonoBehaviour {
    public static GameManager current;

    public List<GDOrder> PlacedOrders;

    private void Awake() {
        current = this;
    }

}

public class Storage : Interactable {
    public GDIngredient IngredientStored;

    public override void Interact() {
        base.Interact();
    }

    public bool DepositIngredients(GDIngredient toDeposit) {
        if (IngredientStored.name == toDeposit.name) {
            if (--toDeposit.count <= 0) {
                Player.current.inventory.Remove(toDeposit);
            }
            IngredientStored.count++;
            return true;
        }
        else return false;
    }

    public bool WithdrawIngredients() {
        for (int i = 0; i < 9; i++) {
            if (i >= Player.current.inventory.Count) {
                Player.current.inventory.Add(new GDIngredient() { name = IngredientStored.name, count = 1 });
                IngredientStored.count--;
                return true;
            }
            if (Player.current.inventory[i].Name == IngredientStored.name) {
                (Player.current.inventory[i] as GDIngredient).count++;
                IngredientStored.count--;
                return true;
            }

                    
        }
        return false;
    }
}

public class AssemblyTable : Interactable {
    public Recipe[] recipes;
    public GDIngredient[] placedIngredients = new GDIngredient[9];
    public void Assembly() {
        if (Player.current.inventory.Count > 7) {
            return;
        }
        
        for (int i = 0; i < recipes.Length; i++) {
            if (placedIngredients == recipes[i].Ingredients) {
                foreach (var item in placedIngredients) {
                    if (--placedIngredients[i].count <= 0) {
                        //Player.current.
                    }
                }
            }
        }
    }

    public struct Recipe {
        public GDIngredient[] Ingredients;
        public GDFood Result;
    }
}

public class OrderGenerator {
    
}


public class GDOrder {
    public int orderNumber;
    public GDFood[] orderContent;
    public float orderSatisfaction;
    public int orderValue;


}

public class GDFood : InventoryItem {
    public string name;
    public string Name {
        get {
            return name;
        }
    }
    public bool Available = true;

    public enum Treatments {
        Cut,
        Fried,
        Cooked
    }

    public enum FoodType {
        Main,
        Side,
        Drink
    }
}

/*
 - Meats (pork, beef, chicken, fish, sausage)
- Grains (bread, corn, egg, rice)
- Vegetables (lettuce, broccoli, tomato, potato, ) 
- Dairy (milk, cheese, butter)
- Extras (pepper, salt, oil, ketchup,  sugar, bbq sauce, mustard, lemon) 
*/
 
public class GDIngredient : InventoryItem {
    public string name;
    public int count;

    public string Name {
        get {
            return name;
        }
    }
}

public interface InventoryItem {
    string Name { get; }

}