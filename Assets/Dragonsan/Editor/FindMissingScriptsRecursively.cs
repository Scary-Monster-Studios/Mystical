using UnityEngine;
using UnityEditor;

namespace Atavism
{

    public class FindMissingScriptsRecursively : EditorWindow
    {
        static int go_count = 0, components_count = 0, missing_count = 0;

        [MenuItem("Window/Atavism/FindMissingScriptsRecursively")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }
            if (GUILayout.Button("Find Missing Scripts in Scene"))
            {
                FindInScene();
            }

            if (GUILayout.Button("Find Missing references in Scene"))
            {
                FindMissingReferences();
            }

        }

        private static void FindInScene()
        {
            GameObject[] go = GameObject.FindObjectsOfType<GameObject>();
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            AtavismLogger.LogInfoMessage(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }



        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            AtavismLogger.LogInfoMessage(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }

        public static void FindMissingReferences()
        {
            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (var go in objects)
            {
                go_count++;
                var components = go.GetComponents<Component>();

                foreach (var c in components)
                {
                    components_count++;
                    if (c == null)
                    {
                        Debug.LogError("Missing script found on: " + FullObjectPath(go), go);
                    }
                    else
                    {
                        SerializedObject so = new SerializedObject(c);
                        var sp = so.GetIterator();

                        while (sp.NextVisible(true))
                        {
                            if (sp.propertyType != SerializedPropertyType.ObjectReference)
                            {
                                continue;
                            }

                            if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                            {
                                ShowError(FullObjectPath(go), sp.name, go);
                            }
                        }
                    }
                }
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }

        private static void ShowError(string objectName, string propertyName, GameObject go)
        {
            missing_count++;
            Debug.LogError("Missing reference found in: " + objectName + ", Property : " + propertyName, go);
        }

        private static string FullObjectPath(GameObject go)
        {
            return go.transform.parent == null ? go.name : FullObjectPath(go.transform.parent.gameObject) + "/" + go.name;
        }


        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }
            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.name  + " " );
                FindInGO(childT.gameObject);
            }
        }
    }
}