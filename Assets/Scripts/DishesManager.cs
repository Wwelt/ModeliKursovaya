using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

using TMPro;
using UnityEngine;

public class DishesManager : MonoBehaviour
{
    private Dish[] _dishes;
    private GameObject[] Dishes;
    private GameObject[] Groups;
    
    private QueryManager _manager;

    
    //private SqlConnection _connection;
    
    public GameObject GroupPrefab;
    public GameObject SceneCamera;
    public GameObject DishPrefab;
    
    
    private void DishesHandler()
    {
        string query = "SELECT DISTINCT Category FROM Dishes";
        
        string[] categories = _manager.GetStringsFromDB(query);
        
        
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
            }
        }
    }
    void Start()
    {
        _manager = SceneCamera.GetComponent<QueryManager>();

        _manager.SetConnection();
        
        DishesHandler();

    }

}
