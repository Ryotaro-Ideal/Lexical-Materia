using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ItemCreatorTool : EditorWindow
{
    private string saveFolderPath = "Assets/Prefab/Field_Obj/Items/";

    private string itemName = "";
    private string itemID = "";
    private ItemType itemType = ItemType.Material;
    private Sprite icon = null;
    private string description = "これは";
    private string previousItemName = "";
    private int maxStack = 99;
    private int baseAttackPower = 1;

    private List<DestroyMaterial> destroyMaterials = new List<DestroyMaterial>();

    private bool createCraftData = false;

    private GameObject itemBasePrefab = null;
    private Mesh itemMesh = null;
    private Material itemMaterial = null;

    private Vector2 scrollPos;

    [MenuItem("Tools/Inventory/Item Creator Tool")]
    public static void ShowWindow()
    {
        GetWindow<ItemCreatorTool>("Item Creator Tool");
    }

    private void OnEnable()
    {
        itemBasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Field_Obj/Items/ItemBase.prefab");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Item Creator Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("── 保存先 ──", EditorStyles.boldLabel);
        saveFolderPath = EditorGUILayout.TextField("保存フォルダ", saveFolderPath);
        EditorGUILayout.HelpBox(
            "アイテム名フォルダが saveFolderPath/(ItemName)/ に自動作成されます。",
            MessageType.Info);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("── ItemBaseプレハブ ──", EditorStyles.boldLabel);
        itemBasePrefab = (GameObject)EditorGUILayout.ObjectField("ItemBase Prefab", itemBasePrefab, typeof(GameObject), false);
        itemMesh = (Mesh)EditorGUILayout.ObjectField("Item Mesh (任意)", itemMesh, typeof(Mesh), false);
        itemMaterial = (Material)EditorGUILayout.ObjectField("Item Material (任意)", itemMaterial, typeof(Material), false);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("── ItemData パラメータ ──", EditorStyles.boldLabel);

        itemName = EditorGUILayout.TextField("Item Name *", itemName);
        if (itemName != previousItemName)
        {
            description = "これは" + itemName;
            previousItemName = itemName;
        }
        itemID = EditorGUILayout.TextField("ID (例: Mat001)", itemID);
        itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false);
        description = EditorGUILayout.TextField("Description", description);
        maxStack = EditorGUILayout.IntField("Max Stack", maxStack);

        if (itemType == ItemType.Tool)
            baseAttackPower = EditorGUILayout.IntField("Base Attack Power", baseAttackPower);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Destroy Materials (分解素材)", EditorStyles.boldLabel);

        for (int i = 0; i < destroyMaterials.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            destroyMaterials[i].letterData = (LetterData)EditorGUILayout.ObjectField(
                destroyMaterials[i].letterData, typeof(LetterData), false, GUILayout.Width(160));
            destroyMaterials[i].count = EditorGUILayout.IntField(destroyMaterials[i].count, GUILayout.Width(50));
            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                destroyMaterials.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("＋ Destroy Material を追加"))
            destroyMaterials.Add(new DestroyMaterial() { count = 1 });

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("── CraftData ──", EditorStyles.boldLabel);
        createCraftData = EditorGUILayout.Toggle("CraftData も作成する", createCraftData);
        if (createCraftData)
        {
            EditorGUILayout.HelpBox(
                "Required Letters は ItemData の Destroy Materials と同じ内容で自動設定されます。",
                MessageType.Info);
        }

        EditorGUILayout.Space(12);

        GUI.enabled = !string.IsNullOrWhiteSpace(itemName) && itemBasePrefab != null;
        if (GUILayout.Button("▶  アイテムを生成する", GUILayout.Height(36)))
            CreateItem();
        GUI.enabled = true;

        EditorGUILayout.EndScrollView();
    }

    private void CreateItem()
    {
        string baseDir = Path.Combine(saveFolderPath, itemName).Replace("\\", "/");
        string soDir = Path.Combine(baseDir, "SO").Replace("\\", "/");

        EnsureDirectory(baseDir);
        EnsureDirectory(soDir);

        ItemData itemData = CreateItemData(soDir);

        CreateItemPrefab(baseDir, itemData);
        CreateVisualPrefab(baseDir, itemData);

        if (createCraftData)
            CreateCraftData(soDir, itemData);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[ItemCreatorTool] '{itemName}' の生成が完了しました。保存先: {baseDir}");
        EditorUtility.DisplayDialog("完了", $"'{itemName}' の生成が完了しました。\n保存先: {baseDir}", "OK");
    }

    private ItemData CreateItemData(string soDir)
    {
        string assetPath = $"{soDir}/ItemData_{itemName}.asset";

        ItemData existing = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
        if (existing != null)
        {
            Debug.LogWarning($"[ItemCreatorTool] ItemData が既に存在します: {assetPath}");
            return existing;
        }

        ItemData data = ScriptableObject.CreateInstance<ItemData>();
        data.ID = itemID;
        data.itemName = itemName;
        data.itemType = itemType;
        data.icon = icon;
        data.description = description;
        data.maxStack = maxStack;
        data.baseAttackPower = baseAttackPower;
        data.destroyMaterials = new List<DestroyMaterial>(destroyMaterials);

        AssetDatabase.CreateAsset(data, assetPath);
        return data;
    }

    private void CreateItemPrefab(string baseDir, ItemData itemData)
    {
        string prefabPath = $"{baseDir}/{itemName}.prefab";

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.LogWarning($"[ItemCreatorTool] アイテムプレハブが既に存在します: {prefabPath}");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(itemBasePrefab);
        instance.name = itemName;

        ItemManager mgr = instance.GetComponentInChildren<ItemManager>();
        if (mgr != null)
        {
            mgr.itemData = itemData;
            EditorUtility.SetDirty(mgr);
        }
        else
        {
            Debug.LogWarning("[ItemCreatorTool] ItemBase 内に ItemManager が見つかりませんでした。手動で設定してください。");
        }

        if (itemMesh != null)
            ApplyMeshToInstance(instance, itemMesh, itemMaterial, applyCollider: true);

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        DestroyImmediate(instance);
    }

    private void CreateVisualPrefab(string baseDir, ItemData itemData)
    {
        string visualPath = $"{baseDir}/visual_{itemName}.prefab";

        if (AssetDatabase.LoadAssetAtPath<GameObject>(visualPath) != null)
        {
            Debug.LogWarning($"[ItemCreatorTool] ビジュアルプレハブが既に存在します: {visualPath}");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(itemBasePrefab);
        instance.name = $"visual_{itemName}";

        RemoveComponentIfExists<ItemManager>(instance);
        RemoveComponentIfExists<Rigidbody>(instance);
        RemoveComponentsIfExists<MeshCollider>(instance);
        RemoveComponentsIfExists<Collider>(instance);

        if (itemMesh != null)
            ApplyMeshToInstance(instance, itemMesh, itemMaterial, applyCollider: false);

        PrefabUtility.SaveAsPrefabAsset(instance, visualPath);
        DestroyImmediate(instance);

        ItemData data = AssetDatabase.LoadAssetAtPath<ItemData>($"{baseDir}/SO/ItemData_{itemName}.asset");
        if (data != null)
        {
            data.visualPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(visualPath);
            EditorUtility.SetDirty(data);
        }
    }

    private void CreateCraftData(string soDir, ItemData itemData)
    {
        string craftPath = $"{soDir}/CraftData_{itemName}.asset";

        if (AssetDatabase.LoadAssetAtPath<CraftData>(craftPath) != null)
        {
            Debug.LogWarning($"[ItemCreatorTool] CraftData が既に存在します: {craftPath}");
            return;
        }

        CraftData craft = ScriptableObject.CreateInstance<CraftData>();
        craft.itemData = itemData;
        craft.requiredLetters = new List<DestroyMaterial>(destroyMaterials);

        AssetDatabase.CreateAsset(craft, craftPath);
    }

    private static void ApplyMeshToInstance(GameObject root, Mesh mesh, Material material, bool applyCollider)
    {
        MeshFilter filter = root.GetComponentInChildren<MeshFilter>();
        if (filter == null)
            filter = root.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;

        MeshRenderer renderer = root.GetComponentInChildren<MeshRenderer>();
        if (renderer == null)
            renderer = root.AddComponent<MeshRenderer>();

        if (material != null)
            renderer.sharedMaterials = new Material[] { material };

        if (applyCollider)
        {
            MeshCollider col = root.GetComponentInChildren<MeshCollider>();
            if (col == null)
                col = root.AddComponent<MeshCollider>();
            col.sharedMesh = mesh;
        }
    }

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }

    private static void RemoveComponentIfExists<T>(GameObject go) where T : Component
    {
        T[] comps = go.GetComponentsInChildren<T>(true);
        foreach (var c in comps)
            DestroyImmediate(c);
    }

    private static void RemoveComponentsIfExists<T>(GameObject go) where T : Component
    {
        RemoveComponentIfExists<T>(go);
    }
}
