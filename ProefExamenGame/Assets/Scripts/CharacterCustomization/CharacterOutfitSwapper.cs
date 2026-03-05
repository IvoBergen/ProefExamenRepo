using UnityEngine;

/// <summary>
/// <c>CharacterOutfitSwapper</c> its swaps the characters hats and outfits based on a array of gameobjects
/// </summary>
public class CharacterOutfitSwapper : MonoBehaviour
{
    public GameObject[] hats;
    public GameObject[] clothes;

    private int _hatCount = 0;
    private int _clothesCount = 0;

    public void UpHatCounter()
    {
        hats[_hatCount].SetActive(false);
        _hatCount = (_hatCount + 1) % hats.Length;
        hats[_hatCount].SetActive(true);
    }

    public void LowerHatCounter()
    {
        hats[_hatCount].SetActive(false);
        _hatCount = (_hatCount - 1 + hats.Length) % hats.Length;
        hats[_hatCount].SetActive(true);
    }

    public void UpClothesCounter()
    {
        clothes[_clothesCount].SetActive(false);
        _clothesCount = (_clothesCount + 1) % clothes.Length;
        clothes[_clothesCount].SetActive(true);
    }

    public void LowerClothesCounter()
    {
        clothes[_clothesCount].SetActive(false);
        _clothesCount = (_clothesCount - 1 + clothes.Length) % clothes.Length;
        clothes[_clothesCount].SetActive(true);
    }
}
