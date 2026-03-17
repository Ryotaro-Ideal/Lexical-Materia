using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CraftInvManager : MonoBehaviour
{
    public static CraftInvManager Instance { get; private set; }
    [SerializeField] private CraftDatabase craftDatabase;
    [SerializeField] private CraftSlotManager[] craftSlots;

    void Awake()
    {
        Instance = this;
        craftSlots = gameObject.GetComponentsInChildren<CraftSlotManager>(true)
                     .OrderBy(s => s.transform.GetSiblingIndex()).ToArray();
    }

    void Start()
    {
        RefreshCraftSlots();
    }

    public void RefreshCraftSlots()
    {
        if (craftDatabase == null) return;

        List<CraftData> recipes = craftDatabase.GetSortedRecipes();
        for (int i = 0; i < craftSlots.Length; i++)
        {
            if (i < recipes.Count)
            {
                craftSlots[i].SetCraftData(recipes[i]);
            }
            else
            {
                craftSlots[i].ClearSlot();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}