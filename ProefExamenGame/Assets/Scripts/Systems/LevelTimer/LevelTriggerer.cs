using UnityEngine;

public class LevelTriggerer : MonoBehaviour
{
    [SerializeField] GameObject _uiObject;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void negetigseconde()
    {
        Debug.Log("90");
    }
    public void zestigseconde()
    {
        Debug.Log("60");
    }
    public void dertigeconde()
    {
        Debug.Log("30");
    }
    public void gameOver()
    {
        _uiObject.SetActive(true);
        Debug.Log("0");
    }

}
