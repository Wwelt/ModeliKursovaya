using System.Collections;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class DishesManager : MonoBehaviour
{
    private Dish[] _dishes;
    private GameObject[] Dishes;
    private GameObject[] Groups;
    private OrderManager _orderManager;
    private QueryManager _manager;

    public GameObject GroupPrefab;
    public GameObject SceneCamera;
    public GameObject DishPrefab;
    
    
    private void DishesHandler()
    {
        _manager.SetConnection();
        
        string query = "SELECT DISTINCT Category FROM Dishes";
        
        string[] categories = _manager.GetCategoriesFromDB(query);
        
        Groups = new GameObject[categories.Length];
        
        //var counter = 0;
        for (byte i = 0; i < Groups.Length; i++)
        {
            Groups[i] = Instantiate(GroupPrefab, gameObject.transform);
            
            Groups[i].name = $"Group: {categories[i]}";
            
            query = $"Select * from Dishes where Category = N'{categories[i]}' ";

            _dishes = _manager.GetDishesFromDB(query);
            
            Groups[i].GetComponent<RectTransform>().sizeDelta = new Vector2(1050 ,330 * (_dishes.Length / 3 + 1) + 40);

            Groups[i].GetComponent<TextMeshProUGUI>().text = categories[i];
            
            Groups[i].transform.localScale = new Vector3(1, 1, 1);

            Dishes = new GameObject[_dishes.Length];

            for (int j = 0; j < Dishes.Length; j++)
            {
                Dishes[j] = Instantiate(DishPrefab, Groups[i].transform);

                Dishes[j].name = $"Dish №{j+1}";

                Dishes[j].transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite =
                    SceneCamera.GetComponent<ImageToSprite>().LoadNewSprite(_dishes[j].Image);

                Dishes[j].transform.Find("Sprite").transform.localScale = new Vector3(37, 37, 1);

                Dishes[j].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = _dishes[j].Name;

                Dishes[j].transform.Find("Payment").Find("Price").GetComponent<TextMeshProUGUI>().text = $"{_dishes[j].Cost:0.00} p.";

                var dishName = _dishes[j].Name;
                Dishes[j].transform.Find("Payment").Find("Button").GetComponent<Button>().
                    onClick.AddListener(() => AddToBasket(dishName));
            }
        }
    }

    public void ChangePosition(string CategoryName)
    {
        switch (CategoryName)
        {
            case "Гарнир":
                transform.position = new Vector3(transform.position.x, 5.9f, transform.position.z);

                break;
            case "Бургеры":
                transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
                break;
            case "Жаренная курица":
                transform.position = new Vector3(transform.position.x, 6.0f, transform.position.z);

                break;
            case "":
                break;
        }
    }
    
    private void AddToBasket(string dishName)
    {
        int dishID = _manager.GetDishByNameFromDB(dishName).ID;

        var query = $"Select COUNT(*) from Dishes_Orders " +
                    $"where fk_OrderID = {_orderManager.OrderID} " +
                    $"and fk_CookID = {_manager.GetCooksFromDB(false)} " +
                    $"and fk_DishID = {dishID}";
        var reader = new SqlCommand(query, _manager.SetConnection()).ExecuteReader();
        reader.Read();
        var isZero = (int)reader[0] == 0;
        reader.Close();
        
        if (isZero)
        {
            query = $"Insert into Dishes_Orders values ({_orderManager.OrderID}, {_manager.GetCooksFromDB(false)}, {dishID}, 1)";
        }
        else
        {
            query = $"Update Dishes_Orders " +
                    $"set Count = Count + 1 " +
                    $"where fk_OrderID = {_orderManager.OrderID} " +
                    $"and fk_CookID = {_manager.GetCooksFromDB(false)} " +
                    $"and fk_DishID = {dishID}";
        }
        
        Debug.Log(query);
            
        _manager.UpdateDB(query);
        
        GameObject.Find("Main Camera").transform.Find("CanvasMain").Find("Buttons")
            .Find("Button-Total-Cost").Find("Text")
            .GetComponent<ShowTotalCost>().UpdateText(_orderManager.OrderID);
    }
    private void OnLevelLoad()
    {
        _orderManager = SceneCamera.GetComponent<OrderManager>();

        _manager = SceneCamera.GetComponent<QueryManager>();

        _manager.SetConnection();
        
        DishesHandler();
    }
    
    void Awake()
    {
        OnLevelLoad();
    }
}
