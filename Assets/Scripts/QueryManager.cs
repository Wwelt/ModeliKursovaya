using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UnityEngine;

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


    public string[] GetCategoriesFromDB(string query)
    {
        SetConnection();

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
        SetConnection();
        
        List<Dish> list = new List<Dish>();
        
        var query = "Select Top(Select Count(distinct Category) from Dishes) * from Dishes";
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
    
    public int[] GetOrderIDFromDB()
    {
        SetConnection();

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

    public Dish GetDishByNameFromDB(string dishName)
    {
        SetConnection();

        var query = $"Select * from Dishes where Name = N'{dishName}' ";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();
        reader.Read();
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
        List<int> list = new List<int>();
        Dictionary<int, int> employees = new Dictionary<int, int>();
        var query = $"Select CookID from Cooks";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add((int) reader[0]);
        }
        reader.Close();

        foreach (var element in list)
        {
            query = $"Select Count from Dishes_Orders where fk_CookID = {element}";
            var newCommand = new SqlCommand(query, Connection);
            var newReader = newCommand.ExecuteReader();
            var summary = 0;
            while (newReader.Read())
            {
                summary += (int)newReader[0];
            }
             
            newReader.Close();
            
            
            employees.Add(element, summary);
        }
        
        return employees.First(e => e.Value == employees.Min(e2 => e2.Value)).Key;
    }

    public int GetNotBusyCashierIDFromDB() /////////remake
    {
        Dictionary<int, int> employees = new Dictionary<int, int>();
        var query = $"Select * from Cashiers";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            query = $"Select Count(*) from Orders where fk_CashierID = {reader[0]}";
            var newCommand = new SqlCommand(query, Connection);
            var newReader = newCommand.ExecuteReader();
            
            employees.Add((int) reader[0],(int) newReader[0]);
        }

        return employees.First(e => e.Value == employees.Min(e2 => e2.Value)).Key;
    }
    
    
    
    /// <summary>
    /// Very unsafe. 
    /// </summary>
    /// <summary>
    /// Use full query to make it work properly
    /// </summary>
    /// <param name="query">query string</param>
    public void UpdateDB(string query)
    {
        SetConnection();

        var command = new SqlCommand(query, Connection);
        
        command.ExecuteNonQuery();
        
    }

    public SqlConnection SetConnection()
    {
        if (IsOpened)
            return Connection;
        
        
        Connection = new SqlConnection(ConnectionString);
        Connection.Open();

        IsOpened = true;

        return Connection;
    }
    
    IEnumerator Routine()
    {
        yield return new WaitForSeconds(200);
    }
}