using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Classes of tables

#region Tables

public class Dish
{
    public int ID;
    public string Name;
    public decimal Cost;
    public string Category;
    public byte[] Image;
}

public class Ingredient
{
    public int ID;
    public string Name;
    public int Count;
    public byte[] Image;
}

#endregion 

public class QueryManager : MonoBehaviour
{
    private static string ConnectionString = "Data Source=.;Initial Catalog=EFC;Integrated Security=True";
    private SqlConnection Connection;
    private Dish[] _dishes;
    private GameObject[] dishesObjects;


    public GameObject prefab;
    public Camera InnerCamera;

    private Dish[] GetDishesFromDB()
    {
        var queryOfCount = $"SELECT COUNT(*) FROM Dishes";
        SqlCommand command = new SqlCommand(queryOfCount,Connection);
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        
        Dish[] dishesTemporary = new Dish[(int)reader[0]];
        reader.Close();
        
        //------------------------------------------------------------
        var queryOfData = $"SELECT * FROM Dishes";
        
        command = new SqlCommand(queryOfData,Connection);
        int counter = 0;
        reader = command.ExecuteReader();
        while (reader.Read())
        {
            dishesTemporary[counter] = new Dish()
            {
                ID =          (int)reader[0],
                Name =     (string)reader[1],
                Cost =    (decimal)reader[2],
                Category = (string)reader[3],
                Image =    (byte[])reader[4]
            };
            counter++;
        }
        reader.Close();
        return dishesTemporary;
    }

    private void DishesHandler()
    {
        _dishes = GetDishesFromDB();
        dishesObjects = new GameObject[_dishes.Length];
        
        for (int i = 0; i < dishesObjects.Length; i++)
        {
            dishesObjects[i] = Instantiate(prefab);

            dishesObjects[i].transform.SetParent(gameObject.transform);
            
            dishesObjects[i].transform.localScale = new Vector3(37.0f,37.0f,1.0f);

            //  objects[i].GetComponentInChildren<Transform>().transform.position += new Vector3(-2.5f, -2.0f,0.0f);

            dishesObjects[i].transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = 
                prefab.GetComponent<ImageToSprite>().LoadNewSprite(_dishes[i].Image);

            dishesObjects[i].transform.Find("Canvas").GetComponent<Canvas>().worldCamera = 
                transform.parent.parent.parent.parent.GetComponent<Camera>();

            dishesObjects[i].transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>().text = _dishes[i].Category;


        }
    }
    void Start()
    {
        Connection = new SqlConnection(ConnectionString);
        Connection.Open();
        DishesHandler();


    }
}
