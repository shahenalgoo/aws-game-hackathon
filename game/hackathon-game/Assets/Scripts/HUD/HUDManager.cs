using System;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }
    public static Action<int> lootUpdater;
    public TextMeshProUGUI lootText;

    public void Awake()
    {
        SingletonCheck();

        lootUpdater += UpdateLootText;
    }

    void SingletonCheck()
    {
        // If there is an instance, and it's not this one, delete this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance
        Instance = this;
    }
    public void UpdateLootText(int amount)
    {
        lootText.text = "Loot Collected: " + amount.ToString("N0");
    }


}
