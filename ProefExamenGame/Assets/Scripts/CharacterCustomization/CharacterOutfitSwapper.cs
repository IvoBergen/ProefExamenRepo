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

    public void UpHatCounter()
    {
        outfits[SelectedOutfitIndex].SetActive(false);
        SelectedOutfitIndex = (SelectedOutfitIndex + 1) % outfits.Length;
        outfits[SelectedOutfitIndex].SetActive(true);
    }

    public void LowerHatCounter()
    {
        outfits[SelectedOutfitIndex].SetActive(false);
        SelectedOutfitIndex = (SelectedOutfitIndex - 1 + outfits.Length) % outfits.Length;
        outfits[SelectedOutfitIndex].SetActive(true);
    }

    // Ensures only the selected outfit is active, all others are hidden
    private void RefreshOutfitDisplay()
    {
        for (int i = 0; i < outfits.Length; i++)
        {
            outfits[i].SetActive(i == SelectedOutfitIndex);
        }
    }
}
