
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    private QueryManager _manager;
    
    public int OrderID;

    private void CreateOrder()
    {
        _manager.SetConnection();
        
        if (PlayerPrefs.HasKey("OrderID"))
        {
            OrderID = PlayerPrefs.GetInt("OrderID");
            return;
        }
        
        var ordersID = _manager.GetOrderIDFromDB();
        while (true)
        {
            OrderID = Random.Range(100, 1000);
            var bl = ordersID.Any(t => OrderID == t);

            if (!bl) break;
        }
        
        _manager.UpdateDB($"Insert into Orders values ({OrderID}, N'Создан', null)");
        
        PlayerPrefs.SetInt("orderID", OrderID);

        
    }
    void Start()
    {
        _manager = gameObject.GetComponent<QueryManager>();

        _manager.SetConnection();
        
        StartCoroutine(Routine());

        

    }
    
    IEnumerator Routine()
    {
        yield return new WaitForSecondsRealtime(1);
        
        CreateOrder();

    }

}
