using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    private QueryManager _manager;

    private SqlConnection _connection;

    private TransferData transferData;

    public GameObject SceneCamera;

    public void OpenMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void Cancel()
    {
        if (GameObject.Find("TransferData") != null)
        {
            _manager.UpdateDB($"Delete from Dishes_Orders where fk_OrderID = " +
                              $"{GameObject.Find("TransferData").GetComponent<TransferData>().OrderID}");
            _manager.UpdateDB($"Delete from Orders where OrderID = " +
                              $"{GameObject.Find("TransferData").GetComponent<TransferData>().OrderID}");
        }

        ToStart();
    }

    public void ToStart()
    {
        SceneManager.LoadScene("Entry");
    }

    public void OpenBasket()
    {
        SceneManager.LoadScene("Basket");
    }
    

    public void OpenPaymentProcess()
    {
        SceneManager.LoadScene("WaitingForMoney");
    }

    public void OpenFinalScene()
    {
        SceneManager.LoadScene("ThanksForMoney");
    }

    public void OnLevelLoad()
    {

        _manager = SceneCamera.GetComponent<QueryManager>();
        // _connection = 
        _manager.SetConnection();
    }

    void Awake()
    {
        OnLevelLoad();
        if (GameObject.Find("TransferData") == null) return;
        
        transferData = GameObject.Find("TransferData").GetComponent<TransferData>();
        
        if (SceneManager.GetActiveScene().name == "ThanksForMoney" && transferData.OrderID >= 100)
            SceneCamera.transform.Find("Canvas").Find("Capsule").Find("OrderIDText").GetComponent<TextMeshProUGUI>().text =
                transferData.OrderID.ToString();
    }



}
