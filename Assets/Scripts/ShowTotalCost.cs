using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowTotalCost : MonoBehaviour
{
    public GameObject Camera;

    private QueryManager _manager;
    private TextMeshProUGUI _temp;

    void Awake()
    {
        _temp = GetComponent<TextMeshProUGUI>();
        
        OnLevelLoad();

        if (SceneManager.GetActiveScene().name == "Basket")
        {
            SetThePrice();
        }
    }

    private void OnLevelLoad()
    {
        _manager = Camera.GetComponent<QueryManager>();

        _manager.SetConnection();
    }
    
    public void UpdateText(int orderID)
    {
        if (SceneManager.GetActiveScene().name == "Basket")
        {
            var cost = _manager.GetTotalCostFromDB(orderID);
            
            _temp.text = $"{Math.Round(cost, 1)}р";

            GameObject.Find("TransferData").GetComponent<TransferData>().totalCost = cost;
        }
        else
        {
            _temp.text = $"Итого: {Math.Round(_manager.GetTotalCostFromDB(orderID), 1)}р";
        }
        
    }

    private void SetThePrice()
    {
        var id = GameObject.Find("TransferData").GetComponent<TransferData>().OrderID;
        
        UpdateText(id);
    }
}
