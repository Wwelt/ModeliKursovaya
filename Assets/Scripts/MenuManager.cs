using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    private QueryManager _manager;

    public GameObject SceneCamera;

    public void OpenMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void Cancel()
    {
        _manager.UpdateDB($"Delete from Dishes_Orders where fk_OrderID = {SceneCamera.GetComponent<OrderManager>().OrderID}");
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
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(20);
        Cancel();
    }

    public void Start()
    {
        _manager = SceneCamera.GetComponent<QueryManager>();

        _manager.SetConnection();
        
        
    }
}
