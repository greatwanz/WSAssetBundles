using UnityEditor;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles/";
        Directory.CreateDirectory(assetBundleDirectory);
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        AssetBundleManifest assetMf = BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);

        //CRC
        uint cardsCRC;
        uint cardsetsCRC;
        BuildPipeline.GetCRCForAssetBundle(assetBundleDirectory + "cards", out cardsCRC);
        BuildPipeline.GetCRCForAssetBundle(assetBundleDirectory + "cardsets", out cardsetsCRC);
        Debug.LogFormat("cards CRC: {0}", cardsCRC);
        Debug.LogFormat("cardsets CRC: {0}", cardsetsCRC);

        //Hash128
        Hash128 cardsHash128 = assetMf.GetAssetBundleHash("cards");
        Hash128 cardsetsHash128 = assetMf.GetAssetBundleHash("cardsets");
        Debug.LogFormat("cards Hash128: {0}", cardsHash128);
        Debug.LogFormat("cardsets Hash128: {0}", cardsetsHash128);

        //Save to manifest
        JSONNode N = new JSONObject();
        N["cards_hash128"] = cardsHash128.ToString();
        N["cardsets_hash128"] = cardsetsHash128.ToString();
        N["cards_crc"] = cardsCRC;
        N["cardsets_crc"] = cardsetsCRC;

        File.WriteAllText(assetBundleDirectory + "manifest.json", JsonHelper.FormatJson(N.ToString()));

        File.Delete(assetBundleDirectory + "CardText");
        File.Delete(assetBundleDirectory + "CardText.manifest");
        File.Delete(assetBundleDirectory + "CardText.manifest.meta");
        File.Delete(assetBundleDirectory + "CardText.meta");
        CardsCount();
    }


    static void CardsCount()
    {
        int count = 0;
        foreach (string s in Directory.GetFiles(Application.dataPath + "/Editor/Cards","*.json", SearchOption.AllDirectories))
        {
            count++;
        }
        Debug.LogFormat("Cards Count: {0}", count);
    }
}