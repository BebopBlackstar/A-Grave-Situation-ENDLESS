using UnityEditor;
using UnityEngine;
using System.Collections;
public class Prefab_Locator_Tool : EditorWindow{
    string myString = "Hello World";
    private string msg_ToolUse = "This tool was developed for Team One AIE." + "It's purpose to make it easier to develop levels with prefabs!";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    GameObject Target_Location;
    bool active_Tool = false;

    private bool isNotNull = false;



    [MenuItem("Tools/Prefab_Locator")]

    public static void ShowWindow()
    {
        GetWindow(typeof(Prefab_Locator_Tool));

    }

    void OnGUI ()
    {
        GUIStyle style = new GUIStyle();
        style.richText = true;

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);
///        /*msg_ToolUse = */EditorGUILayout.LabelField(msg_ToolUse/*, EditorApplication.timeSinceStartup.ToString()*/); //test
        GUILayout.Label("<size=14><color=Red>" + msg_ToolUse + "</color></size>",style);
        Target_Location = (GameObject) EditorGUILayout.ObjectField("TARGET- Empty: ", Target_Location, typeof(GameObject));

        if (Target_Location != null)
        { isNotNull = true; }
        else { isNotNull = false; active_Tool = false; }
        
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", isNotNull); //Start group.
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        active_Tool = EditorGUILayout.Toggle("Using Placement: ", active_Tool);
        if(active_Tool == true)
        { AutomatedPrefabAssembly(); }
        EditorGUILayout.EndToggleGroup();                                                   //End group.
    }

    void AutomatedPrefabAssembly()
    {
        ///<foreach will manipulate what list of infomation will be generated. Should smaller scales not fit into a larger scales area by (X and Z), it will reject adding and ignore them.
        /// Should be
        ///vector3 scale_Largest; /*(x 14, y N/A, z 6) area occupied is 14*6 = 84; <<Largest!*/ int current_Largest = 5;  
         

        Debug.Log("Testing Assembly script!");
    }


}
