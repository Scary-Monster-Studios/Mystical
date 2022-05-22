using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Group;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Storage.Data;
using EasyBuildSystem.Features.Scripts.Core.Base.Storage.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Storage
{
    [AddComponentMenu("Easy Build System/Features/Build Storage")]
    public class BuildStorage : MonoBehaviour
    {
        #region Fields

        public static BuildStorage Instance;

        public StorageType StorageType;

        public bool AutoDefineInDataPath;

        public bool AutoSave = false;

        public float AutoSaveInterval = 60f;

        public bool LoadAndWaitEndFrame;

        public bool SavePrefabs = true;

        public bool LoadPrefabs = true;

        public string StorageOutputFile;

        [HideInInspector]
        public bool LoadedFile = false;

        private float TimerAutoSave;

        private List<PieceBehaviour> PrefabsLoaded = new List<PieceBehaviour>();

        private bool FileIsCorrupted;

        #endregion Fields

        #region Methods

        /// <summary>
        /// (Editor) This method allows to load a storage file in Editor scene.
        /// </summary>
        public void LoadInEditor(string path)
        {
            int PrefabLoaded = 0;

            PrefabsLoaded = new List<PieceBehaviour>();

            BuildManager Manager = FindObjectOfType<BuildManager>();

            if (Manager == null)
            {
                Debug.LogError("<b>Easy Build System</b> : The BuildManager is not in the scene, please add it to load a file.");

                return;
            }

            FileStream Stream = File.Open(path, FileMode.Open);

            PieceData Serializer = null;

            try
            {
                using (StreamReader Reader = new StreamReader(Stream))
                {
                    Serializer = JsonUtility.FromJson<PieceData>(Reader.ReadToEnd());
                }
            }
            catch
            {
                Stream.Close();

                Debug.LogError("<b>Easy Build System</b> : Please check that the file extension to load is correct.");

                return;
            }

            if (Serializer == null || Serializer.Pieces == null)
            {
                Debug.Log("<b>Easy Build System</b> : The file is empty or the data are corrupted.");

                return;
            }

            GroupBehaviour Group = new GameObject("(Editor) " + path).AddComponent<GroupBehaviour>();

            for (int i = 0; i < Serializer.Pieces.Count; i++)
            {
                if (Serializer.Pieces[i] != null)
                {
                    PieceBehaviour Prefab = Manager.GetPieceById(Serializer.Pieces[i].Id);

                    if (Prefab != null)
                    {
                        PieceBehaviour PlacedPrefab = Manager.PlacePrefab(Prefab,
                            PieceData.ParseToVector3(Serializer.Pieces[i].Position),
                            PieceData.ParseToVector3(Serializer.Pieces[i].Rotation),
                            PieceData.ParseToVector3(Serializer.Pieces[i].Scale),
                            Group, null);

                        PlacedPrefab.name = Prefab.Name;
                        PlacedPrefab.transform.position = PieceData.ParseToVector3(Serializer.Pieces[i].Position);
                        PlacedPrefab.transform.eulerAngles = PieceData.ParseToVector3(Serializer.Pieces[i].Rotation);
                        PlacedPrefab.transform.localScale = PieceData.ParseToVector3(Serializer.Pieces[i].Scale);

                        PrefabsLoaded.Add(PlacedPrefab);

                        PrefabLoaded++;
                    }
                    else
                    {
                        Debug.Log("<b>Easy Build System</b> : The Prefab (" + Serializer.Pieces[i].Id + ") does not exists in the Build Manager.");
                    }
                }
            }

            Stream.Close();

#if UNITY_EDITOR
            Selection.activeGameObject = Group.gameObject;

            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }
#endif

            Debug.Log("<b>Easy Build System</b> : Data file loaded " + PrefabLoaded + " Prefab(s) loaded in " + Time.realtimeSinceStartup.ToString("#.##") + " ms in the Editor scene.");

            PrefabsLoaded.Clear();
        }

        /// <summary>
        /// This method allows to load the storage file.
        /// </summary>
        public void LoadStorageFile()
        {
            StartCoroutine(LoadDataFile());
        }

        /// <summary>
        /// This method allows to save the storage file.
        /// </summary>
        public void SaveStorageFile()
        {
            StartCoroutine(SaveDataFile());
        }

        /// <summary>
        /// This method allows to delete the storage file.
        /// </summary>
        public void DeleteStorageFile()
        {
            StartCoroutine(DeleteDataFile());
        }

        /// <summary>
        /// This method allows to check if the storage file.
        /// </summary>
        public bool ExistsStorageFile()
        {
            return File.Exists(StorageOutputFile);
        }

        private void Awake()
        {
            Instance = this;

            if (AutoDefineInDataPath)
            {
                StorageOutputFile = Application.dataPath + "/data.dat";
            }
        }

        private void Start()
        {
            if (LoadPrefabs)
            {
                StartCoroutine(LoadDataFile());
            }

            if (AutoSave && SavePrefabs)
            {
                TimerAutoSave = AutoSaveInterval;
            }
        }

        private void Update()
        {
            if (AutoSave && SavePrefabs)
            {
                if (TimerAutoSave <= 0)
                {
                    Debug.Log("<b>Easy Build System</b> : Saving of " + FindObjectsOfType<PieceBehaviour>().Length + " Part(s) ...");

                    SaveStorageFile();

                    Debug.Log("<b>Easy Build System</b> : Saved with successfuly !");

                    TimerAutoSave = AutoSaveInterval;
                }
                else
                {
                    TimerAutoSave -= Time.deltaTime;
                }
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (StorageType == StorageType.Android)
            {
                if (!SavePrefabs)
                {
                    return;
                }

                SaveStorageFile();
            }
        }

        private void OnApplicationQuit()
        {
            if (!SavePrefabs)
            {
                return;
            }

            SaveStorageFile();
        }

        private IEnumerator LoadDataFile()
        {
            if (StorageType == StorageType.Desktop)
            {
                if (StorageOutputFile == string.Empty || Directory.Exists(StorageOutputFile))
                {
                    Debug.LogError("<b>Easy Build System</b> : Please define output path.");

                    yield break;
                }
            }

            int PrefabLoaded = 0;

            PrefabsLoaded = new List<PieceBehaviour>();

            if (ExistsStorageFile() || StorageType == StorageType.Android)
            {
                Debug.Log("<b>Easy Build System</b> : Loading data file ...");

                FileStream Stream = null;

                if (StorageType == StorageType.Desktop)
                {
                    Stream = File.Open(StorageOutputFile, FileMode.Open);
                }

                PieceData Serializer = null;

                try
                {
                    if (StorageType == StorageType.Desktop)
                    {
                        using (StreamReader Reader = new StreamReader(Stream))
                        {
                            Serializer = JsonUtility.FromJson<PieceData>(Reader.ReadToEnd());
                        }
                    }
                    else
                    {
                        Serializer = JsonUtility.FromJson<PieceData>(PlayerPrefs.GetString("EBS_Storage"));
                    }
                }
                catch (Exception ex)
                {
                    Stream.Close();

                    FileIsCorrupted = true;

                    Debug.LogError("<b>Easy Build System</b> : " + ex);

                    BuildEvent.Instance.OnStorageLoadingResult.Invoke(null);
                    yield break;
                }

                if (Serializer == null)
                {
                    BuildEvent.Instance.OnStorageLoadingResult.Invoke(null);
                    yield break;
                }

                GroupBehaviour Group = new GameObject("(Runtime) " + StorageOutputFile).AddComponent<GroupBehaviour>();

                for (int i = 0; i < Serializer.Pieces.Count; i++)
                {
                    if (Serializer.Pieces[i] != null)
                    {
                        PieceBehaviour Prefab = BuildManager.Instance.GetPieceById(Serializer.Pieces[i].Id);

                        if (Prefab != null)
                        {
                            PieceBehaviour PlacedPrefab = BuildManager.Instance.PlacePrefab(Prefab,
                                PieceData.ParseToVector3(Serializer.Pieces[i].Position),
                                PieceData.ParseToVector3(Serializer.Pieces[i].Rotation),
                                PieceData.ParseToVector3(Serializer.Pieces[i].Scale),
                                Group, null);

                            PlacedPrefab.name = Serializer.Pieces[i].Name;
                            PlacedPrefab.transform.position = PieceData.ParseToVector3(Serializer.Pieces[i].Position);
                            PlacedPrefab.transform.eulerAngles = PieceData.ParseToVector3(Serializer.Pieces[i].Rotation);
                            PlacedPrefab.transform.localScale = PieceData.ParseToVector3(Serializer.Pieces[i].Scale);
                            PlacedPrefab.ChangeAppearance(Serializer.Pieces[i].AppearanceIndex);
                            PlacedPrefab.ExtraProperties = Serializer.Pieces[i].Properties;

                            PrefabsLoaded.Add(PlacedPrefab);

                            PrefabLoaded++;

                            if (LoadAndWaitEndFrame)
                            {
                                yield return new WaitForEndOfFrame();
                            }
                        }
                        else
                        {
                            Debug.Log("<b>Easy Build System</b> : The prefab (" + Serializer.Pieces[i].Id + ") does not exists in the list of Build Manager.");
                        }
                    }
                }

                if (Stream != null)
                {
                    Stream.Close();
                }

                if (!LoadAndWaitEndFrame)
                {
                    Debug.Log("<b>Easy Build System</b> : Data file loaded " + PrefabLoaded + " prefab(s) loaded in " + Time.realtimeSinceStartup.ToString("#.##") + " ms.");
                }
                else
                {
                    Debug.Log("<b>Easy Build System</b> : Data file loaded " + PrefabLoaded + " prefab(s).");
                }

                LoadedFile = true;

                BuildEvent.Instance.OnStorageLoadingResult.Invoke(PrefabsLoaded.ToArray());

                yield break;
            }
            else
            {
                BuildEvent.Instance.OnStorageLoadingResult.Invoke(null);
            }

            yield break;
        }

        private IEnumerator SaveDataFile()
        {
            if (FileIsCorrupted)
            {
                Debug.LogWarning("<b>Easy Build System</b> : The file is corrupted, the Prefabs could not be saved.");

                yield break;
            }

            if (StorageOutputFile == string.Empty || Directory.Exists(StorageOutputFile))
            {
                Debug.LogError("<b>Easy Build System</b> : Please define out file path.");

                yield break;
            }

            int SavedCount = 0;

            if (ExistsStorageFile())
            {
                File.Delete(StorageOutputFile);
            }
            else
            {
                BuildEvent.Instance.OnStorageSavingResult.Invoke(null);
            }

            if (BuildManager.Instance.CachedParts.Count > 0)
            {
                Debug.Log("<b>Easy Build System</b> : Saving data file ...");

                FileStream Stream = null;

                if (StorageType == StorageType.Desktop)
                {
                    Stream = File.Create(StorageOutputFile);
                }

                PieceData Data = new PieceData();

                PieceBehaviour[] PartsAtSave = BuildManager.Instance.CachedParts.ToArray();

                for (int i = 0; i < PartsAtSave.Length; i++)
                {
                    if (PartsAtSave[i] != null)
                    {
                        if (PartsAtSave[i].CurrentState == StateType.Placed || PartsAtSave[i].CurrentState == StateType.Remove)
                        {
                            PieceData.SerializedPiece DataTemp = new PieceData.SerializedPiece
                            {
                                Id = PartsAtSave[i].Id,
                                Name = PartsAtSave[i].name,
                                Position = PieceData.ParseToSerializedVector3(PartsAtSave[i].transform.position),
                                Rotation = PieceData.ParseToSerializedVector3(PartsAtSave[i].transform.eulerAngles),
                                Scale = PieceData.ParseToSerializedVector3(PartsAtSave[i].transform.localScale),
                                AppearanceIndex = PartsAtSave[i].AppearanceIndex,
                                Properties = PartsAtSave[i].ExtraProperties
                            };

                            Data.Pieces.Add(DataTemp);

                            SavedCount++;
                        }
                    }
                }

                if (StorageType == StorageType.Desktop)
                {
                    using (StreamWriter Writer = new StreamWriter(Stream))
                    {
                        Writer.Write(JsonUtility.ToJson(Data));
                    }

                    Stream.Close();
                }
                else
                {
                    PlayerPrefs.SetString("EBS_Storage", JsonUtility.ToJson(Data));

                    PlayerPrefs.Save();
                }

                Debug.Log("<b>Easy Build System</b> : Data file saved " + SavedCount + " Prefab(s).");

                if (BuildEvent.Instance != null)
                {
                    BuildEvent.Instance.OnStorageSavingResult.Invoke(PrefabsLoaded.ToArray());
                }

                yield break;
            }
        }

        private IEnumerator DeleteDataFile()
        {
            if (StorageOutputFile == string.Empty || Directory.Exists(StorageOutputFile))
            {
                Debug.LogError("<b>Easy Build System</b> : Please define out file path.");

                yield break;
            }

            if (File.Exists(StorageOutputFile) == true)
            {
                for (int i = 0; i < PrefabsLoaded.Count; i++)
                {
                    Destroy(PrefabsLoaded[i].gameObject);
                }

                File.Delete(StorageOutputFile);

                Debug.Log("<b>Easy Build System</b> : The storage file has been removed.");
            }
            else
            {
                if (BuildEvent.Instance != null)
                {
                    BuildEvent.Instance.OnStorageSavingResult.Invoke(null);
                }
            }
        }

        #endregion Methods
    }
}