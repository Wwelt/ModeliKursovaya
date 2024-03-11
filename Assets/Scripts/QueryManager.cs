using System.Collections.Generic;
using System.Data;
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

public class SeveralDishes
{
    public Dish Dish;
    public int Count;
}
#endregion 

public class QueryManager : MonoBehaviour
{
    private static string ConnectionString = "Data Source=.;Initial Catalog=EFC;Integrated Security=True";
    
    public SqlConnection Connection;
    
    public QueryManager instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    public List<Ingredient> GetIngredientsFromDB()
    {
        List<Ingredient> list = new List<Ingredient>();
        
        var query = $"Select i.IngredientID, i.Name, di.IngredientCount, i.Image " +
                    $"from Ingredients i full join Dishes_Ingredients di " +
                    $"on i.IngredientID = di.fk_IngredientID";

        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new Ingredient()
            {
                ID = (int) reader[0],
                Name = (string) reader[1],
                Count = (int) reader[2],
                Image = (byte[]) reader[3]
            });
        }
        reader.Close();
        return list;
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

    public bool IfExistsIngredientFk(int DishID, int IngredientID)
    {
        var query = $"Select * from Dishes_Ingredients where fk_DishID = {DishID} and fk_IngredientID = {IngredientID}";

        SqlCommand command = new SqlCommand(query, Connection);

        var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            if (reader[0] != null)
            {
                reader.Close();
                
                return true;
            }
        }
        reader.Close();

        return false;
    }
    public SeveralDishes[] GetSeveralDishesFromDB(int orderID)
    {
        var query = $"SELECT d.DishID, d.Name, d.Cost, d.Category, d.Image, do.Count " +
                    $"FROM Dishes_Orders do full JOIN Dishes d " +
                    $"ON do.fk_DishID = d.DishID " +
                    $"where do.fk_OrderID = {orderID}";
        
        List<SeveralDishes> list = new List<SeveralDishes>();
        
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var temp = new Dish()
            {
                ID = (int)reader[0],
                Name = (string)reader[1],
                Cost = (decimal)reader[2],
                Category = (string)reader[3],
                Image = (byte[])reader[4]
            };
            
            list.Add(
                new SeveralDishes(){
                    Dish = temp,
                    Count = (int)reader[5] 
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

    public int GetCooksFromDB(bool isBusy)
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
        
        return isBusy ? 
            employees.First(e => 
                e.Value == employees.Max(e2 => e2.Value)).Key 
            : 
            employees.First(e => 
                e.Value == employees.Min(e2 => e2.Value)).Key;
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

    public decimal GetTotalCostFromDB(int OrderID)
    {
        SetConnection();

        var query = $"SELECT SUM(d.Cost * do.Count) AS TotalCost " +
                    $"FROM Dishes_Orders do " +
                    $"JOIN Dishes d ON do.fk_DishID = d.DishID " +
                    $"WHERE do.fk_OrderID = {OrderID} ";
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();
        
        reader.Read();
        var summary = (decimal)reader[0];
        reader.Close();
        
        return summary;
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

    public int[] GetCountsFromBasket(string query)
    {
        SetConnection();

        List<int> counts = new List<int>();
        var command = new SqlCommand(query, Connection);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            counts.Add((int)reader[0]);
        }
        reader.Close();
        
        return counts.ToArray();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public SqlConnection SetConnection()
    {
        
        Connection = new SqlConnection(ConnectionString);
        Connection.Open();

        
        return Connection;
    }
    
}