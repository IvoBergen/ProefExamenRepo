using UnityEngine;

public class StartGamePlayerSpawner : MonoBehaviour
{
    public GameObject[] outfits;

    private void Awake()
    {
        SpawnPlayerModel();
    }

    private void SpawnPlayerModel()
    {
        if (outfits == null || outfits.Length == 0)
        {
            Debug.LogWarning("StartGamePlayerSpawner: No outfits assigned!");
            return;
        }

        // Activate only the outfit the player selected in the menu
        for (int i = 0; i < outfits.Length; i++)
        {
            outfits[i].SetActive(i == CharacterOutfitSwapper.SelectedOutfitIndex);
        }
    }
}
