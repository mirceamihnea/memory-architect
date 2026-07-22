using UnityEngine;
using UnityEditor;

public class ItemDataCreator
{
    [MenuItem("Tools/Create Effigy Item Data")]
    public static void CreateEffigy()
    {
        ItemData asset = ScriptableObject.CreateInstance<ItemData>();

        asset.itemID = "effigy_01";
        asset.itemName = "Effigy";
        asset.description = "A creepy burlap effigy.";

        // Incercam sa gasim prefab-ul automat
        string prefabPath = "Assets/Prefabs/Simple_Burlap_Pin_Effigy_FBX.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            asset.heldPrefab = prefab;
        }

        string path = "Assets/Script/Inventory/Effigy_Item.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log("Effigy Item Data creat cu succes la: " + path);
    }

    [MenuItem("Tools/Create Busta Item Data")]
    public static void CreateBusta()
    {
        ItemData asset = ScriptableObject.CreateInstance<ItemData>();

        asset.itemID = "busta_01";
        asset.itemName = "Busta";
        asset.description = "A bust of Bohuslav Martinu.";

        // Incercam sa gasim prefab-ul automat
        string prefabPath = "Assets/Prefabs/busta_bohuslav_martinu.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            asset.heldPrefab = prefab;
        }

        string path = "Assets/Script/Inventory/Busta_Item.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log("Busta Item Data creat cu succes la: " + path);
    }

    [MenuItem("Tools/Create Lupa Item Data")]
    public static void CreateLupa()
    {
        ItemData asset = ScriptableObject.CreateInstance<ItemData>();

        asset.itemID = "lupa_01";
        asset.itemName = "Lupa";
        asset.description = "A magnifying glass.";

        // Incercam sa gasim prefab-ul automat (bazat pe screenshot-ul tau unde era 'fdj')
        string prefabPath = "Assets/Prefabs/fdj.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            asset.heldPrefab = prefab;
        }

        string path = "Assets/Script/Inventory/Lupa_Item.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log("Lupa Item Data creat cu succes la: " + path);
    }

    [MenuItem("Tools/Create Flower Item Data")]
    public static void CreateFlower()
    {
        ItemData asset = ScriptableObject.CreateInstance<ItemData>();

        asset.itemID = "flower_01";
        asset.itemName = "Floare"; // Matching your 'floare' entry in PuzzleManager
        asset.description = "A beautiful flower.";

        // Incercam sa gasim prefab-ul automat
        string prefabPath = "Assets/Prefabs/floare.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            asset.heldPrefab = prefab;
        }

        string path = "Assets/Script/Inventory/Flower_Item.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log("Flower Item Data creat cu succes la: " + path);
    }

    [MenuItem("Tools/Create Mask Item Data")]
    public static void CreateMask()
    {
        ItemData asset = ScriptableObject.CreateInstance<ItemData>();

        asset.itemID = "mask_01";
        asset.itemName = "masca"; // Matching your 'masca' entry in PuzzleManager
        asset.description = "A mysterious mask.";

        // Incercam sa gasim prefab-ul automat
        string prefabPath = "Assets/Prefabs/masca.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            asset.heldPrefab = prefab;
        }

        string path = "Assets/Script/Inventory/Mask_Item.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log("Mask Item Data creat cu succes la: " + path);
    }

    [MenuItem("Tools/Create Book Item Data")]
    public static void CreateBook()
    {
        ItemData asset = ScriptableObject.CreateInstance<ItemData>();

        asset.itemID = "book_01";
        asset.itemName = "Carte"; // Matching your 'Carte' entry in PuzzleManager
        asset.description = "An old dusty book.";

        // Incercam sa gasim prefab-ul automat
        string prefabPath = "Assets/Prefabs/book.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            asset.heldPrefab = prefab;
        }

        string path = "Assets/Script/Inventory/Book_Item.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log("Book Item Data creat cu succes la: " + path);
    }
}
