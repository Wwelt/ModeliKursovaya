using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CategoriesManager : MonoBehaviour
{
    
    private Dish[] _categories;
    private GameObject[] dishesObjects;
    
    private QueryManager manager;

    
    //private SqlConnection _connection;
    
    public GameObject Itemprefab;
    public GameObject SceneCamera;

    private void CategoriesHandler()
    {
        _categories = manager.GetDistinctDishesFromDB();
        dishesObjects = new GameObject[_categories.Length];
        
        for (short i = 0; i < dishesObjects.Length; i++)
        {
            dishesObjects[i] = Instantiate(Itemprefab, gameObject.transform);
            
            dishesObjects[i].transform.localScale = new Vector3(37.0f,37.0f,1.0f);
            
            dishesObjects[i].transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = 
                Itemprefab.GetComponent<ImageToSprite>().LoadNewSprite(_categories[i].Image);

            dishesObjects[i].transform.Find("Canvas").GetComponent<Canvas>().worldCamera = 
                transform.parent.parent.parent.parent.GetComponent<Camera>();

            dishesObjects[i].transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>().text = _categories[i].Category;

            dishesObjects[i].name = $"CategoryItem: {_categories[i].Category}";

        }
    }
    void Start()
    {
        manager = SceneCamera.GetComponent<QueryManager>();
        // _connection = 
        manager.SetConnection();
        
        StartCoroutine(Routine());
    }

    private void OnEnable()
    {
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator Routine()
    {
        yield return new WaitForSecondsRealtime(1);
        
        CategoriesHandler();

    }

}
