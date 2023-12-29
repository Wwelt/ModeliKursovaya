using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    private QueryManager _manager;
    
    public int OrderID;

    private void CreateOrder()
    {
        var ordersID = _manager.GetOrderIDFromDB();
        while (true)
        {
            OrderID = Random.Range(0, 1000);
            var bl = ordersID.Any(t => OrderID == t);

            if (!bl) break;
        }
        
        
    }
    void Start()
    {
        _manager = gameObject.GetComponent<QueryManager>();

        _manager.SetConnection();
        
        CreateOrder();

    }

}
