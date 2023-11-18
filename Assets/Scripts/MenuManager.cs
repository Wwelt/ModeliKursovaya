using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public void OpenMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    public void Cancel()
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
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(20);
        Cancel();
    }
}
