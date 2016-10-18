using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class LevelBuild_Tool : EditorWindow {


    GameObject[] Objects; //THIS HERE will determin what prefabs are in the created folder. 

    ///<new: Text A113 https://docs.unity3d.com/ScriptReference/EditorGUILayout.BeginScrollView.html
    Vector2 scrollPos;
    string t = "This is a string inside a Scroll view!";

    [MenuItem("Examples/Modify internal Quaternion cs")]
    static void Init()
    {
        LevelBuild_Tool window = (LevelBuild_Tool)EditorWindow.GetWindow(typeof(LevelBuild_Tool), true, "My Empty Window");
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        scrollPos =
        EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.Label(t);
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Add More Text", GUILayout.Width(100), GUILayout.Height(100)))
            t += " \nAnd this is more text!";
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Clear"))
            t = "";
    }
    //  A113
    

    
    private string folderName = "LB_Tool Prefabs";
    public bool FirstStart = true;                      //Let's it only create one folder.
	// Use this for initialization
	void Start () {
        Debug.LogWarning("Have you checked you put Prefabs in: Assets/" + folderName);

    }
	
    void firstRun()
    {
        if(!Directory.Exists("Assets/" + folderName))
        { Debug.Log("Is null.");
            Debug.Log("Is NOT null.");
            string guid = AssetDatabase.CreateFolder("Assets", folderName);
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
        } 

    }
	// Update is called once per frame
	void Update () {
        if(FirstStart == true)
        { FirstStart = false; firstRun(); }
	
	} 
}
