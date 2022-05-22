using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Atavism
{
    [Serializable]
    public struct AOVector
    {
        public float x;
        public float y;
        public float z;
        public AOVector(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }
        public static implicit operator AOVector(Vector3 rValue)
        {
            return new AOVector(rValue.x, rValue.y, rValue.z);
        }
    }

    [XmlRoot("AtavismBuildingColliders")]
    public class AtavismBuildingColliders
    {
        public List<AtavismCollider> colliders = new List<AtavismCollider>();
    }
    
    public class AtavismCollider
    {
        public string type;
        public AOVector position;
        public List<AOVector> halfEdges = new List<AOVector>();
        public float  radius;
        public List<int> triangles = new List<int>();
        public List<float> bounds = new List<float>();

    }
    
    public class ServerBuildingColliders : AtavismFunction
    {
        // Tab selection
        public int selected = 1;

        public Vector2 inspectorScrollPosition = Vector2.zero;
        public float inspectorHeight = 0;
        public static AtavismBuildingColliders atavismColliders = new AtavismBuildingColliders();


        // Use this for initialization
        public ServerBuildingColliders()
        {

        }

        void Awake()
        {
            functionName = "Building Colliders";      

        }

        // Update is called once per frame
        void Update()
        {
        }

        private int SelectTab(Rect pos, int sel)
        {
            pos.y += ImagePack.tabTop;
            pos.x += pos.width - ImagePack.tabMargin * 3;

            bool edit = false;
            bool doc = false;

            switch (sel)
            {
                case 1:
                    edit = true;
                    break;
                case 2:
                    doc = true;
                    break;
            }

            pos.x += ImagePack.tabMargin;
            if (edit)
                pos.y += ImagePack.tabSpace;
            if (ImagePack.DrawTabEdit(pos, edit))
                return 1;
            if (edit)
                pos.y -= ImagePack.tabSpace;
            pos.x += ImagePack.tabMargin;
            if (doc)
                pos.y += ImagePack.tabSpace;
            if (ImagePack.DrawTabDoc(pos, doc))
                return 2;
            if (doc)
                pos.y -= ImagePack.tabSpace;

            return sel;
        }

        /// <summary>
        /// Enables the scroll bar and sets total window height
        /// </summary>
        /// <param name="windowHeight">Window height.</param>
        public void EnableScrollBar(float windowHeight)
        {
            inspectorHeight = windowHeight;
        }
        // Draw the function inspector
        // box: Rect representing the inspector area
        public override void Draw(Rect box)
        {

            // Draw the Control Tabs
            selected = SelectTab(box, selected);
            Rect inspectorScrollWindow = box;
            Rect inspectorWindow = box;
            inspectorWindow.width -= 2;
            inspectorScrollWindow.width += 14;
            inspectorWindow.height = Mathf.Max(box.height, inspectorHeight); Rect pos = box;
            inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
            if (selected == 1)
            {
                // Set the drawing layout
                
                pos.x += ImagePack.innerMargin;
                pos.y += ImagePack.innerMargin;
                pos.width -= ImagePack.innerMargin;
                pos.height = ImagePack.fieldHeight;
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Save Building Colliders"));
                pos.y += ImagePack.fieldHeight ;
                ImagePack.DrawText(pos, Lang.GetTranslate("Get colliders from buildings prefabs and save them in the database."));
                pos.y += ImagePack.fieldHeight*2f;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save colliders")))
                {
                    saveColliders();
                }
                pos.width *= 2;
              

            }
            else if (selected == 2)
            {
                DrawHelp(box);
            }

            GUI.EndScrollView();
            EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
        }

        void saveColliders()
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            string[] querys = new string[]{
                //"SELECT `id`,`gameObject` FROM `build_object_stage` ",
                "SELECT `id`,`prefab` FROM `build_object_stage_progress` "
                //,"SELECT `id`,`prefab` FROM `build_object_stage_damaged` "
            };
            int count2 = 0;
            int skip = 0;
            foreach (var query in querys)
            {
                if (rows != null)
                    rows.Clear();
                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                if ((rows != null) && (rows.Count > 0))
                {
                    int  count = rows.Count;
                    count2 += count;
                    int i = 0;
                    foreach (Dictionary<string, string> data in rows)
                    {
                        atavismColliders.colliders.Clear();
                        int id = int.Parse(data["id"]);

                        String prefabPath = "";
                        if (data.ContainsKey("gameObject"))
                            prefabPath = data["gameObject"];
                        else
                            prefabPath = data["prefab"];
                        EditorUtility.DisplayProgressBar("Geting buildings colliders.", prefabPath, ((float) i / (float) count));
                        i++;
                        GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                        if (prefab == null)
                        {
                            skip++;
                            Debug.LogError(prefabPath + " prefab not found in project for stage id " + id + " !!");
                            continue;
                        }

                        prefab.transform.position = Vector3.zero;
                        prefab.transform.rotation = Quaternion.identity;
                        goAnalysis(prefab);
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(atavismColliders.GetType());
                        var stringwriter = new Utf8StringWriter();
                        x.Serialize(stringwriter, atavismColliders);
                        string trimsah = stringwriter.ToString();
                        
                      //  Debug.LogError("id=" +id+" prefabPath="+prefabPath+" count="+trimsah.Length+" "+trimsah);
                        string queryU = "";
                      //  if(query.Contains("build_object_stage"))
                       //     queryU = "UPDATE build_object_stage SET trimesh=?trimesh WHERE id=?id";
                        if(query.Contains("build_object_stage_progress"))
                            queryU = "UPDATE build_object_stage_progress SET trimesh=?trimesh WHERE id=?id";
                       /* if(query.Contains("build_object_stage_damaged"))
                            queryU = "UPDATE build_object_stage_damaged SET trimesh=?trimesh WHERE id=?id";*/

                        // Setup the register data		
                        List<Register> update = new List<Register>();
                        update.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                        update.Add(new Register("trimesh", "?trimesh", MySqlDbType.Blob, trimsah, Register.TypesOfField.String));
                        DatabasePack.Update(DatabasePack.contentDatabasePrefix, queryU, update);
                    }

                    EditorUtility.ClearProgressBar();
                   
                }
            } 
            Debug.Log("All " + count2 + " prefabs, " + skip + " not found");
        }

        void goAnalysis(GameObject go)
        {
            if (!go.activeSelf)
                return;
            Collider[] colliders = go.GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null && colliders[i].enabled)
                {
                    if (colliders[i].GetType() == typeof(BoxCollider))
                    {
                        BoxCollider bc = (BoxCollider) colliders[i];
                        Vector3 pos = bc.transform.position;
                        Vector3 center = bc.center;
                        Vector3 size = bc.size;
                        Vector3 scale = bc.transform.lossyScale;
                        Vector3 fsize = Vector3.Scale(size, scale);
                        Quaternion q = bc.transform.rotation;
                        Vector3 hevv0 = q * Vector3.up * fsize.y / 2;
                        Vector3 hevv1 = q * Vector3.left * fsize.x / 2;
                        Vector3 hevv2 = q * Vector3.forward * fsize.z / 2;
                        AtavismCollider b = new AtavismCollider();
                        b.type = "Box";
                        b.position = pos + q * center;
                        b.halfEdges.Add(hevv0);
                        b.halfEdges.Add(hevv1);
                        b.halfEdges.Add(hevv2);
                        atavismColliders.colliders.Add(b);
                        //  Debug.LogError("BOX:" + pos + " " + center + " " + fsize + " " + hevv0 + " " + hevv1 + " " + hevv2);
                    }

                    if (colliders[i].GetType() == typeof(SphereCollider))
                    {
                        SphereCollider sc = (SphereCollider) colliders[i];
                        Vector3 pos = sc.transform.position;
                        Vector3 center = sc.center;
                        Vector3 scale = sc.transform.lossyScale;

                        AtavismCollider s = new AtavismCollider();
                        s.type = "Sphere";
                        s.position = pos + center;
                        var scale2 = Math.Max(scale.y, scale.x);
                        scale2 = Math.Max(scale2, scale.z);
                        s.radius = scale2 * sc.radius;
                        atavismColliders.colliders.Add(s);
                        //  Debug.LogError("SPHERE:" + pos + " " + center + " " + scale2 + " " + sc.radius + " " + s.radius );
                    }

                    if (colliders[i].GetType() == typeof(CapsuleCollider))
                    {
                        CapsuleCollider cc = (CapsuleCollider) colliders[i];
                        Vector3 pos = cc.transform.position;
                        Quaternion q = cc.transform.rotation;
                        Vector3 center = cc.center;
                        float radius = cc.radius;
                        float height = cc.height;
                        Vector3 scale = cc.transform.lossyScale;
                        AtavismCollider s = new AtavismCollider();
                        s.type = "CAPSULE";

                        Vector3 vec = q * Vector3.up;
                        //  Debug.LogError("Axi "+cc.direction);
                        switch (cc.direction)
                        {
                            case 0:
                            {
                                //X
                                vec = q * Vector3.left;
                                float _scale = Math.Max(scale.y, scale.z);
                                /* float _scale = scale.y;
                                 if (_scale < scale.z)
                                     _scale = scale.z;*/
                                radius *= Math.Max(scale.y, scale.z);
                                height *= scale.x;
                                break;
                            }
                            case 1:
                            {
                                //Y
                                vec = q * Vector3.up;
                                float _scale = Math.Max(scale.x, scale.z);
                                /*     float _scale = scale.x;
                                     if (_scale < scale.z)
                                         _scale = scale.z;*/
                                radius *= Math.Max(scale.x, scale.z);
                                height *= scale.y;
                                break;
                            }
                            case 2:
                            {
                                //Z
                                vec = q * Vector3.forward;
                                float _scale = Math.Max(scale.y, scale.x);
                                /*float _scale = scale.x;
                                if (_scale < scale.y)
                                    _scale = scale.y;*/
                                radius *= Math.Max(scale.y, scale.x);
                                height *= scale.z;
                                break;
                            }
                        }

                        s.radius = radius;
                        Vector3 pos1 = pos + center + vec * height / 2;
                        Vector3 pos2 = pos + center - vec * height / 2;
                        s.halfEdges.Add(pos1);
                        s.halfEdges.Add(pos2);
                        atavismColliders.colliders.Add(s);
                    }

                    /*  if (colliders[i].GetType() == typeof(MeshCollider))
                      {
                          MeshCollider cc = (MeshCollider) colliders[i];
                          if (cc.sharedMesh != null) {
                             var a =cc.sharedMesh.vertices;
                             var b =cc.sharedMesh.triangles;
                             var c=cc.sharedMesh.bounds;
                             AtavismCollider s = new AtavismCollider();
                             s.type="TRIMESH";
                             foreach (var v in a)
                             {
                                 s.halfEdges.Add(v);
                             }
  
                             foreach (var v in b)
                             {
                                 s.triangles.Add(v);
                             }
                             
                             Bounds _bounds = new Bounds(cc.transform.localToWorldMatrix.MultiplyPoint3x4(a[0]), Vector3.zero);
  
                             for (int j = 1; j < a.Length; j++) {
                                 _bounds.Encapsulate(cc.transform.localToWorldMatrix.MultiplyPoint3x4(a[j]));
                             }
  
                             // Assigned here to avoid changing bounds if vertices would happen to be null
                             c = _bounds;
                             s.bounds.Add(c.center.x + c.extents.x);
                             s.bounds.Add(c.center.y + c.extents.y);
                             s.bounds.Add(c.center.z + c.extents.z);
                             s.bounds.Add(c.center.x - c.extents.x);
                             s.bounds.Add(c.center.y - c.extents.y);
                             s.bounds.Add(c.center.z - c.extents.z);
                              Debug.LogError("bounds "+c);
                             //, localToWorldMatrix);
                             atavismColliders.colliders.Add(s);
                          }
  
                      }*/

                }
            }

            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in go.transform)
            {
                goAnalysis(childT.gameObject);
            }
        }
    }

}