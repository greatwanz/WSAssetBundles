# WSAssetBundles
Generate Asset Bundles for WS Simulator

* Use the Card Sets Editor Window under Window->Card Sets Editor to enter set name and set codes of cards.   
* Place any card jsons under WSAssetBundles/Assets/Editor/Cards/[SetCode]/[Series]/[CardNumber].json. (Folder should have been generated if you used the card creator)  
* When finished, generate the AssetBundle with Assets->Build Asset Bundles.  
* Generated Asset Bundles can be found under WSAssetBundles/Assets/AssetBundles

Files are named:  
* cards  
* cardsets  
* manifest.json

TODO: Use Newtonsoft Json.NET instead. SimpleJSON doesn't do pretty print :(
