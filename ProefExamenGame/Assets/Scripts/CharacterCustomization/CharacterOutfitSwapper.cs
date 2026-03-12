using UnityEngine;

/// <summary>
/// <c>CharacterOutfitSwapper</c> its swaps the characters hats and outfits based on a array of gameobjects
/// </summary>
public class CharacterOutfitSwapper : MonoBehaviour
{
    public GameObject[] outfits;
    public static int SelectedOutfitIndex = 0;

    private void Start()
    {
        RefreshOutfitDisplay();
    }

    /// <summary>
    /// <c>UpOutfitCounter</c> Adds one to the hat counter and cycles through it
    /// </summary>
    public void UpOutfitCounter()
    {
        outfits[SelectedOutfitIndex].SetActive(false);
        SelectedOutfitIndex = (SelectedOutfitIndex + 1) % outfits.Length;
        outfits[SelectedOutfitIndex].SetActive(true);
    }

    /// <summary>
    /// <c>LowerOutfitCounter</c> Removes one to the hat counter an cycles through it
    /// </summary>
    public void LowerOutfitCounter()
    {
        outfits[SelectedOutfitIndex].SetActive(false);
        SelectedOutfitIndex = (SelectedOutfitIndex - 1 + outfits.Length) % outfits.Length;
        outfits[SelectedOutfitIndex].SetActive(true);
    }

    /// <summary>
    /// <c>RefreshOutfitDisplay</c> Ensures only the selected outfit is active, all others are hidden
    /// </summary>
    private void RefreshOutfitDisplay()
    {
        for (int i = 0; i < outfits.Length; i++)
        {
            outfits[i].SetActive(i == SelectedOutfitIndex);
        }
    }
}
