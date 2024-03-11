using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    private QueryManager _manager;
    
    public int OrderID;
    public GameObject transferPrefab;

    private void CreateOrder()
    {
        if (GameObject.Find("TransferData").GetComponent<TransferData>().OrderID >= 100)
        {
            OrderID = GameObject.Find("TransferData").GetComponent<TransferData>().OrderID;
            return;
        }
        
        var ordersID = _manager.GetOrderIDFromDB();
        
        bool bl;
        do
        {
            OrderID = Random.Range(100, 1000);
            bl = ordersID.Any(t => OrderID == t);

        } while (bl);
        
        _manager.UpdateDB($"Insert into Orders values ({OrderID}, N'Создан', null)");

        GameObject.Find("TransferData").GetComponent<TransferData>().OrderID = OrderID;


    }

    public void OnLevelLoad()
    {
        _manager = gameObject.GetComponent<QueryManager>();

        _manager.SetConnection();
        
        CreateOrder();
    }
    
    void Awake()
    {
        if (GameObject.Find("TransferData") == null)
        {
            var temp = Instantiate(transferPrefab);
            temp.name = "TransferData";
        }
        
        OnLevelLoad();
    }
    


}
