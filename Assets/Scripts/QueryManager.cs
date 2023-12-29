using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    private static bool IsOpened = false;
    
    private SqlConnection Connection;

    public Dish[] GetDishesFromDB()
    {
        List<Dish> list = new List<Dish>();

        var queryOfData = $"SELECT * FROM Dishes order by Category";
        var command = new SqlCommand(queryOfData, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new Dish(){
                ID =          (int)reader[0],
                Name =     (string)reader[1],
                Cost =    (decimal)reader[2],
                Category = (string)reader[3],
                Image =    (byte[])reader[4]
            });
        }
        reader.Close();
        return list.ToArray();
    }

    public Dish[] GetDishesFromDB(string query)
    {
        List<Dish> list = new List<Dish>();
        
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new Dish(){
                ID =          (int)reader[0],
                Name =     (string)reader[1],
                Cost =    (decimal)reader[2],
                Category = (string)reader[3],
                Image =    (byte[])reader[4]
            });
        }
        reader.Close();
        return list.ToArray();
    }


    public string[] GetStringsFromDB(string query)
    {
        List<string> list = new List<string>();
        SqlCommand command = new SqlCommand(query, Connection);
        SqlDataReader reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            list.Add((string)reader[0]);
        }
        reader.Close();

        return list.ToArray();
    }
    
    

    public Dish[] GetDistinctDishesFromDB()
    {
        List<Dish> list = new List<Dish>();
        
        var query = $"Select Top(Select Count(distinct Category) from Dishes) * from Dishes";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Dish(){
                ID =          (int)reader[0],
                Name =     (string)reader[1],
                Cost =    (decimal)reader[2],
                Category = (string)reader[3],
                Image =    (byte[])reader[4]
            });
        }
        reader.Close();
        return list.ToArray();
        
    }

    public Ingredient[] GetIngredientsFromDB()
    {
        var totalCount = $"SELECT COUNT(*) FROM Ingredients";
        SqlCommand command = new SqlCommand(totalCount, Connection);
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        
        Ingredient[] ingredients = new Ingredient[(int)reader[0]];
        reader.Close();
        

        return ingredients;
    }

    public int[] GetOrderIDFromDB()
    {
        var list = new List<int>();
        var query = $"Select OrderID from Orders";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add((int) reader[0]);
        }
        reader.Close();

        return list.ToArray();
    }

    public Dish GetDishByNameFromDB(string name)
    {
        var query = $"Select * from Dishes where Name == N'{name}' ";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        var dish = new Dish()
        {
            ID = (int) reader[0],
            Name = (string) reader[1],
            Cost = (decimal) reader[2],
            Category = (string) reader[3],
            Image = (byte[]) reader[4]
        };
        
        reader.Close();
        
        return dish;
    }
    
    public int GetNotBusyCookIDFromDB()
    {
        Dictionary<int, int> employees = new Dictionary<int, int>();
        var query = $"Select * from Cooks";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            query = $"Select Count(*) from Dishes_Orders where fk_CookID = {reader[0]}";
            var newCommand = new SqlCommand(query, Connection);
            var newReader = newCommand.ExecuteReader();
            
            employees.Add((int) reader[0],(int) newReader[0]);
        }

        return employees.First(e => e.Value == employees.Min(e2 => e2.Value)).Key;
    }

    /// <summary>
    /// Very unsafe
    /// </summary>
    /// <param name="query"></param>
    public void InsertToDB(string query)
    {
        var command = new SqlCommand(query, Connection);
        command.ExecuteNonQuery();
        
    }
    /*private void DishesHandler()
    {
        List<string> list = new List<string>();
        string query = "SELECT DISTINCT Category FROM Dishes";
        SqlCommand command = new SqlCommand(query, Connection);
        SqlDataReader reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            list.Add((string)reader[0]);
        }
        reader.Close();
        
        
        Groups = new GameObject[list.Count];
        
        //var counter = 0;
        for (byte i = 0; i < list.Count; i++)
        {
            query = $"Select * from Dishes where Category = N'{list[i]}' ";

            _dishes = GetDishesFromDB(query);

            
            

            Groups[i].transform.Find("Category").GetComponent<TextMeshProUGUI>().text = list[i];
            
            
            Groups[i].transform.localScale = new Vector3(1, 1, 1);
        }
    }*/
    public SqlConnection SetConnection()
    {
        if (IsOpened)
            return Connection;
        
        
        Connection = new SqlConnection(ConnectionString);
        Connection.Open();

        IsOpened = true;

        return Connection;
    }

    public void Print()
    {
        Debug.Log("Button touched");
    }

}