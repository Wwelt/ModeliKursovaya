using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class BasketManager : MonoBehaviour
{
    private int orderID;
    private QueryManager _manager;
    private SeveralDishes[] _dishes;
    private List<GameObject> _ingredients;
    private List<GameObject> UIElements;
    
    public GameObject Camera;
    public GameObject BasketElementPrefab;
    public GameObject TotalCost;
    public GameObject Content;
    public GameObject IngredientPrefab;
    void Awake()
    {
        OnLevelLoad();
        
        Camera.transform.Find("IngredientCanvas").gameObject.SetActive(false);
        
        orderID = GameObject.Find("TransferData").GetComponent<TransferData>().OrderID;
        
        BasketHandler();
    }
    
    private void OnLevelLoad()
    {
        _manager = Camera.GetComponent<QueryManager>();

        _manager.SetConnection();

        UIElements = new List<GameObject>();
        
        _ingredients = new List<GameObject>();
    }

    private void BasketHandler()
    {
        _dishes = _manager.GetSeveralDishesFromDB(orderID);

        for (int i = 0; i < _dishes.Length; i++)
        {

            foreach (var element in UIElements)
            {
                if (element.name.Split(':')[0] != _dishes[i].Dish.ID.ToString()) continue;
                
                var newText = element.transform.Find("ChangeCount").Find("Count").Find("Text").GetComponent<TextMeshProUGUI>().text;

                newText = $"{int.Parse(newText) + _dishes[i].Count}";

                element.transform.Find("ChangeCount").Find("Count").Find("Text").GetComponent<TextMeshProUGUI>().text =
                    newText;
                
                var name = element.name.Split(':');
                name[2] = (int.Parse(name[2]) + _dishes[i].Count).ToString();

                element.name = $"{name[0]}:{name[1]}:{name[2]}:{name[3]}";

                element.transform.Find("Cost").Find("Text").GetComponent<TextMeshProUGUI>().text =
                    $"Цена: {Math.Round(_dishes[i].Dish.Cost * int.Parse(name[2]), 1)}р";
                
                goto EndOfLoop;
            }
            
            UIElements.Add(Instantiate(BasketElementPrefab, gameObject.transform));

            UIElements[i].name = $"{_dishes[i].Dish.ID}:{_dishes[i].Dish.Name}:{_dishes[i].Count}:{_dishes[i].Dish.Cost}";

            UIElements[i].transform.Find("DishName").GetComponent<TextMeshProUGUI>().text = _dishes[i].Dish.Name;

            UIElements[i].transform.Find("ChangeCount").Find("Count").Find("Text").GetComponent<TextMeshProUGUI>()
                .text = _dishes[i].Count.ToString();

            var i1 = i;
            UIElements[i].transform.Find("ChangeCount").Find("Decrease").GetComponent<Button>()
                .onClick.AddListener(() => ChangeUICount(_dishes[i1].Dish.ID, i1, -1));
            
            UIElements[i].transform.Find("ChangeCount").Find("Increase").GetComponent<Button>()
                .onClick.AddListener(() => ChangeUICount(_dishes[i1].Dish.ID, i1, 1));

            UIElements[i].transform.Find("Cost").Find("Text").GetComponent<TextMeshProUGUI>().text =
                $"Цена: {Math.Round(_dishes[i].Dish.Cost * _dishes[i].Count,1)}р";
            
            UIElements[i].transform.Find("DishSprite").GetComponent<SpriteRenderer>().sprite =
                Camera.GetComponent<ImageToSprite>().LoadNewSprite(_dishes[i].Dish.Image);
            
            UIElements[i].transform.Find("DishSprite").transform.localScale = new Vector3(42, 42, 1);
            
            UIElements[i].transform.Find("Delete").GetComponent<Button>()
                .onClick.AddListener(() => {Destroy(UIElements[i1]);});
            
            UIElements[i].transform.Find("Edit").GetComponent<Button>()
                .onClick.AddListener(() => Edit(_dishes[i1], i1));
            
            EndOfLoop: ;
        }
    }

    private void Edit(SeveralDishes dish, int index)
    {
        List<Ingredient> ingredients = new List<Ingredient>();

        if (dish.Dish.Category == "Бургеры")
        {
            ingredients = _manager.GetIngredientsFromDB();
            
        }
        
        var canvas = Camera.transform.Find("IngredientCanvas").Find("UI");
        
        canvas.parent.gameObject.SetActive(true);
        
        canvas.Find("Image").GetComponent<SpriteRenderer>().sprite = 
            Camera.GetComponent<ImageToSprite>().LoadNewSprite(dish.Dish.Image);
        
        canvas.Find("Image").transform.localScale = new Vector3(42, 42, 1);


        canvas.Find("DishName").GetComponent<TextMeshProUGUI>().text = dish.Dish.Name;
        
        canvas.Find("Confirm").GetComponent<Button>()
            .onClick.AddListener(() => ConfirmEdit(ingredients, dish.Dish, index));
        
        canvas.Find("Discard").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                ClearContent();
                
                canvas.parent.gameObject.SetActive(false);
            });
        
        
        LinkIngredients(ingredients);
    }

    private void LinkIngredients(List<Ingredient> ingredients)
    {
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (i < _ingredients.Count)
            {
                _ingredients[i].gameObject.SetActive(true);
            }
            else
            {
                _ingredients.Add(Instantiate(IngredientPrefab , Content.transform)); 
            }

            _ingredients[i].transform.Find("IngredientName").GetComponent<TextMeshProUGUI>().text = ingredients[i].Name;

            _ingredients[i].transform.Find("Image").GetComponent<SpriteRenderer>().sprite = 
                Camera.GetComponent<ImageToSprite>().LoadNewSprite(ingredients[i].Image);
            
            _ingredients[i].transform.Find("Image").localScale = new Vector3(42, 42, 1);
            
            _ingredients[i].transform.Find("buttons").Find("SingleCost").GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"+20р";

            _ingredients[i].transform.Find("buttons").Find("ChangeCount").Find("Count").GetChild(0)
                .GetComponent<TextMeshProUGUI>()
                .text = ingredients[i].Count.ToString();

            var i1 = i;
            _ingredients[i].transform.Find("buttons").Find("ChangeCount").Find("Increase").GetComponent<Button>()
                .onClick.AddListener(() => ChangeCount(ingredients, i1, 1));
            
            _ingredients[i].transform.Find("buttons").Find("ChangeCount").Find("Decrease").GetComponent<Button>()
                .onClick.AddListener(() => ChangeCount(ingredients, i1, -1));
        }
    }

    private void ConfirmEdit(List<Ingredient> list, Dish dish, int index)
    {

        if (list.Count == 0)
        {
            CloseEdit();

            return;
        }

        int summary = 0;
        
        
        foreach (var ingredient in list)
        {
            var query = "";
            if (_manager.IfExistsIngredientFk(dish.ID, ingredient.ID))
            {
                query =
                    $"Update Dishes_Ingredients set IngredientCount = {ingredient.Count} where fk_DishID = {dish.ID} and fk_IngredientID = {ingredient.ID}";
            }
            else
            {
                query = $"Insert into Dishes_Ingredients values ({dish.ID}, {ingredient.ID}, {ingredient.Count}, null)";    
            }
            
        
            _manager.UpdateDB(query);

            summary += ingredient.Count * 20;

            var cost = UIElements[index].transform.Find("Cost").Find("Text").GetComponent<TextMeshProUGUI>().text;

            cost = $"Цена: {decimal.Parse(cost.Split(':')[1].Split('р')[0]) + ingredient.Count * 20}";

            UIElements[index].transform.Find("Cost").Find("Text").GetComponent<TextMeshProUGUI>().text = cost;
        }

        var text =
            _ingredients[index].transform.Find("buttons").Find("ChangeCount").Find("Count").Find("Text")
                .GetComponent<TextMeshProUGUI>().text;

        text = $"{int.Parse(text) + summary}";

        _ingredients[index].transform.Find("buttons").Find("ChangeCount").Find("Count").Find("Text")
            .GetComponent<TextMeshProUGUI>().text = text;
        
        CloseEdit();
        
    }

    private void ClearContent()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Content.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    private void CloseEdit()
    {
        Camera.transform.Find("IngredientCanvas").gameObject.SetActive(false);
        
        ClearContent();
    }
    
    private void ChangeUICount(int dishID, int index, int sign)
    {
        var temp = UIElements[index].transform.Find("ChangeCount").Find("Count").Find("Text")
            .GetComponent<TextMeshProUGUI>().text;

        string query;
        
        
        if (int.Parse(temp) + sign > 0)
        {
            query = $"update Dishes_Orders " +
                    $"SET Count = Count + {sign} " +
                    $"where fk_DishID = {dishID} and fk_CookID = {_manager.GetCooksFromDB(true)}";
            
            temp = $"{int.Parse(temp) + sign}";
        }
        else
        {
            query = $"delete Dishes_Orders where fk_DishID = {dishID}";
                
            _manager.UpdateDB(query);
            
            Debug.Log(query);
            
            Destroy(UIElements[index]);
            
            return;
        }
        Debug.Log(query);
        
        _manager.UpdateDB(query);

        UIElements[index].transform.Find("ChangeCount").Find("Count").Find("Text")
            .GetComponent<TextMeshProUGUI>().text = temp;

        var name = UIElements[index].name.Split(':');
        name[2] = (int.Parse(name[2]) + sign).ToString();

        UIElements[index].name = $"{name[0]}:{name[1]}:{name[2]}:{name[3]}";
        
        UIElements[index].transform.Find("Cost").Find("Text").GetComponent<TextMeshProUGUI>().text =
            $"Цена: {Math.Round(decimal.Parse(name[3]) * int.Parse(name[2]), 1)}р";
        
        TotalCost.GetComponent<ShowTotalCost>().UpdateText(orderID);
    }
    
    private void ChangeCount(List<Ingredient> list, int index, int sign)
    {
        var temp = _ingredients[index].transform.Find("buttons").Find("ChangeCount").Find("Count").Find("Text")
            .GetComponent<TextMeshProUGUI>().text;


        var count = int.Parse(temp) + sign; 
        if (count > 0)
        {
            if (count > 10)
            {
                return;
            }
            
            temp = $"{count}";

            list[index].Count += sign;
            
            


            /*if (_manager.IfExistsIngredient(dishID, ingredient.ID ))
            {
                query = $"update Dishes_Ingredients " +
                        $"SET IngredientCount = IngredientCount + {sign} " +
                        $"where fk_DishID = {dishID} and fk_IngredientID = {_manager.GetCooksFromDB(true)}";
            }
            else
            {
                query = "Insert into Dishes_Ingredients " +
                        $"values ({dishID}, {ingredient.ID}, {ingredient.Count}, null)";
            }*/
        }
        else
        {
            return;
        }

        _ingredients[index].transform.Find("buttons").Find("TotalCost").GetChild(0).GetComponent<TextMeshProUGUI>()
                .text =
            $"Итого: {count * 20}р";

        _ingredients[index].transform.Find("buttons").Find("ChangeCount").Find("Count").Find("Text")
            .GetComponent<TextMeshProUGUI>().text = temp;
        
    }
}
