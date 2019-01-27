using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using SimpleJSON;
using System.IO;

/// <summary>
/// Editable Key-Value pair class
/// </summary>
public class KeyVal<Key, Val>
{
    public Key key { get; set; }

    public Val value { get; set; }

    public KeyVal()
    {
    }

    public KeyVal(Key key, Val val)
    {
        this.key = key;
        this.value = val;
    }

}

/// <summary>
/// Card sets editor window. Allows for defining and editing of card sets information
/// </summary>
public class CardSetsEditor : EditorWindow
{
    readonly string cardsetPath = "/Editor/CardSets/cardsets.json";

    Action helpbox;
    Vector2 scrollPos;
    List<KeyVal<string,string>> kvpList;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Card Sets Editor")]
    public static void ShowWindow()
    {
        Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        EditorWindow.GetWindow<CardSetsEditor>("Card Sets Editor", true, inspectorType);
    }

    void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// Init list of sets
    /// </summary>
    void Init()
    {
        kvpList = new  List<KeyVal<string,string>>();
        string cardsets = File.ReadAllText(Application.dataPath + cardsetPath); 
        JSONNode node = JSON.Parse(cardsets);
        GUILayout.ExpandWidth(false);
        foreach (KeyValuePair<string,JSONNode> kvp in node)
        {
            string codes = string.Empty;
            for (int i = 0; i < kvp.Value.Count; ++i)
            {
                codes += kvp.Value[i] + ",";
            }
            codes = codes.TrimEnd(',');
            kvpList.Add(new KeyVal<string,string>(kvp.Key, codes));
        }
    }

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 80;
        EditorGUILayout.BeginVertical(null);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (helpbox != null)
            helpbox();
        
        EditorGUILayout.BeginHorizontal(null);
        GUILayout.Label("Card Sets Editor", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        Button("Add", Add, GUILayout.Width(50));
        Button("Save", Save, GUILayout.Width(50));
        Button("Revert", Revert, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        DisplaySets();
        if (EditorGUI.EndChangeCheck())
            helpbox = null;
        EditorGUILayout.Space();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Button that executes action on click
    /// </summary>
    /// <param name="text">Text to display on button</param>
    /// <param name="onClick">Action to execute on click</param>
    /// <param name="options">GUILayoutOptions</param>
    void Button(string text, Action onClick, params GUILayoutOption[] options)
    {
        GUI.SetNextControlName(text);

        if (GUILayout.Button(text, options))
        { 
            onClick.Invoke();
            GUI.FocusControl(text);
        }
    }

    /// <summary>
    /// Adds a new card set
    /// </summary>
    void Add()
    {
        kvpList.Add(new KeyVal<string,string>("", ""));
        helpbox = null;
    }

    /// <summary>
    /// Saves current card sets
    /// </summary>
    void Save()
    {
        JSONNode node = new JSONObject();
        foreach (KeyVal<string,string> kvp in kvpList)
        {
            node[kvp.key] = new JSONArray();
            string[] setcodes = (kvp.value + ",").Split(',');
            for (int i = 0; i < setcodes.Length - 1; ++i)
            {
                node[kvp.key][i] = setcodes[i];
            }
        }
        StreamWriter writer = new StreamWriter(Application.dataPath + cardsetPath, false);
        writer.WriteLine(JsonHelper.FormatJson(node.ToString()));
        writer.Close();
        helpbox = () => EditorGUILayout.HelpBox("Card Sets Saved", MessageType.Info);
    }

    /// <summary>
    /// Reverts to saved cardsets, undoing all changes
    /// </summary>
    void Revert()
    {
        Init();
        scrollPos = new Vector2(0, 1);
        helpbox = null;
    }

    /// <summary>
    /// Displays sets
    /// </summary>
    void DisplaySets()
    {
        for (int i = kvpList.Count - 1; i > -1; --i)
        {
            EditorGUILayout.BeginHorizontal(null);
            kvpList[i].key = EditorGUILayout.TextField("Set Name", kvpList[i].key);

            kvpList[i].value = EditorGUILayout.TextField("Set Codes", kvpList[i].value, GUILayout.MaxWidth(250));
            if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                kvpList.Remove(kvpList[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
