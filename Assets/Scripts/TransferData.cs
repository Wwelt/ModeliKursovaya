using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferData : MonoBehaviour
{
    public int OrderID;
    public decimal totalCost = 0;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "ThanksForMoney")
        {
            Destroy(gameObject);
        }
    }
    
}
