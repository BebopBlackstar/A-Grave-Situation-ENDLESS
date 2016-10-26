using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
[CustomEditor(typeof(MapGenerator))]
public class MapInspector : Editor
{
    MapGenerator mapGen;
    List<Object> selection = new List<Object>();
    public void OnEnable()
    {
        mapGen = (MapGenerator)target;
        SceneView.onSceneGUIDelegate += MapUpdate;
    }
    void MapUpdate(SceneView sceneView)
    {
        Event e = Event.current;
        Ray ray = Camera.current.ScreenPointToRay(new Vector2(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && e.keyCode == KeyCode.LeftAlt && mapGen.creating == true)
        {
            selection.Add(hit.transform.gameObject);
            Selection.objects = selection.ToArray();
        }
        else
        {
            selection = new List<Object>(Selection.objects);
        }

    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(new GUIContent("Map width", "The width of the map"));        
        mapGen.width = EditorGUILayout.IntSlider(mapGen.width, 2, 100);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Map height", "The height of the map"));
        mapGen.height = EditorGUILayout.IntSlider(mapGen.height, 2, 100);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Creating", "Leave ticked if you want alt to select when held down"));
        mapGen.creating = EditorGUILayout.Toggle(mapGen.creating);
        GUILayout.EndHorizontal();

        if (GUILayout.Button(new GUIContent("Generate Map", "Creates the tiles of the map of width and heigh")))
        {
            var children = new List<GameObject>();
            foreach (Transform child in mapGen.transform) children.Add(child.gameObject);
            children.ForEach(child => DestroyImmediate(child));

            float _x = mapGen.transform.position.x;
            float _z = mapGen.transform.position.z;
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Base.prefab", typeof(GameObject));
            for (int x = -mapGen.height / 2; x < mapGen.height / 2; x++)
            {
                for (int z = -mapGen.width / 2; z < mapGen.width / 2; z++)
                {
                    GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                    clone.transform.SetParent(mapGen.transform);
                    clone.transform.position = new Vector3(x + _x + .5f, 0, z + _z + .5f);
                }
            }
        }
        if (GUILayout.Button(new GUIContent("Generate Walls", "Creates walls over all tiles not in a path marked group")))
        {
            var empty = new GameObject("Wall Group");
            empty.tag = "Wall";
            empty.transform.SetParent(mapGen.transform);
            empty.transform.SetAsFirstSibling();
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Wall.prefab", typeof(GameObject));
            foreach (Transform go in mapGen.transform)
            {
                if (go.tag != "Path" && go.tag != "Wall")
                {
                    GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                    clone.transform.SetParent(empty.transform);
                    clone.layer = LayerMask.NameToLayer("Walls");
                    clone.transform.position = new Vector3(go.position.x, go.position.y + clone.transform.localScale.y / 2, go.position.z);

                }
            }
        }
        SceneView.RepaintAll();
    }
}
