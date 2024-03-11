using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;


public class CategoriesManager : MonoBehaviour
{
    
    private Dish[] _categories;
    private GameObject[] dishesObjects;
    
    private QueryManager manager;

    
    //private SqlConnection _connection;
    public GameObject DishContent;
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

            var tempName = _categories[i].Category;
            
            dishesObjects[i].name = $"CategoryItem: {tempName}";
            
            dishesObjects[i].transform.Find("Canvas").Find("Button").GetComponent<Button>()
                .onClick.AddListener(() => DishContent.GetComponent<DishesManager>().ChangePosition(tempName));
        }
    }
    
    public void OnLevelLoad()
    {
        
        manager = SceneCamera.GetComponent<QueryManager>();
        // _connection = 
        manager.SetConnection();
        
        CategoriesHandler();
    }

    void Awake()
    {
        OnLevelLoad();
    }
}
