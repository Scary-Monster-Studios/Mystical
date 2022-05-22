using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Atavism
{
    public class UGUIMobCreator : MonoBehaviour
    {
        static UGUIMobCreator instance;
        [SerializeField] GameObject spawnMarkerTemplate;
        [SerializeField] GameObject patrolMarkerTemplate;
        [SerializeField] GameObject spawnRoamTemplate;
        [SerializeField] GameObject spawnAggroTemplate;

        [SerializeField] TextMeshProUGUI templateName;
        [SerializeField] TextMeshProUGUI templateName2;
        [SerializeField] TextMeshProUGUI templateName3;
        [SerializeField] TextMeshProUGUI templateName4;
        [SerializeField] TextMeshProUGUI templateName5;
        [SerializeField] TMP_InputField despawnTimeInput;
        [SerializeField] TMP_InputField respawnTimeInput;
        [SerializeField] TMP_InputField respawnTimeMaxInput;
        [SerializeField] TMP_InputField startTimeInput;
        [SerializeField] TMP_InputField endTimeInput;
        [SerializeField] TextMeshProUGUI alternateTemplateName;
        [SerializeField] TextMeshProUGUI alternateTemplateName2;
        [SerializeField] TextMeshProUGUI alternateTemplateName3;
        [SerializeField] TextMeshProUGUI alternateTemplateName4;
        [SerializeField] TextMeshProUGUI alternateTemplateName5;
        [SerializeField] TMP_InputField roamRatiusInput;
        [SerializeField] TextMeshProUGUI selectedPatrolPathName;
        [SerializeField] TMP_InputField pickupItemInput;
        [SerializeField] Toggle offersBank;
        [SerializeField] TextMeshProUGUI merchantTableValue;
        [SerializeField] TMP_InputField positionXInput;
        [SerializeField] TMP_InputField positionYInput;
        [SerializeField] TMP_InputField positionZInput;
        MobSpawn spawnInCreation;
        [SerializeField] UGUIMobCreatorEntry rowPrefab;
        [SerializeField] UGUIMobCreatorPathEntry rowPathPrefab;
        [SerializeField] GameObject spawnPanel;
        [SerializeField] GameObject mobListPanel;
        [SerializeField] GameObject startQuestPanel;
        [SerializeField] GameObject endQuestPanel;
        [SerializeField] GameObject dialogPanel;
        [SerializeField] GameObject merchantPanel;
        [SerializeField] GameObject selectSpawnPanel;
        [SerializeField] GameObject patrolPathPanel;
        [SerializeField] GameObject patrolPathCreatePanel;
        [SerializeField] GameObject editPositionPanel;

        [SerializeField] Transform mobTemplateGrid;
        [SerializeField] Transform startQuestAvailableGrid;
        [SerializeField] Transform startQuestSelectedGrid;
        [SerializeField] Transform endQuestAvailableGrid;
        [SerializeField] Transform endQuestSelectedGrid;
        [SerializeField] Transform dialogAvailableGrid;
        [SerializeField] Transform dialogSelectedGrid;
        [SerializeField] Transform merchantGrid;
        [SerializeField] Transform patrolPathGrid;
        [SerializeField] Transform patrolPathCreateGrid;
        [SerializeField] Button deleteButton;
        [SerializeField] Button saveButton;
        [SerializeField] Button spawnButton;
        [SerializeField] Button positionButton;
        [SerializeField] GameObject panel;
        [SerializeField] TMP_InputField lingerTimeInput;
        [SerializeField] Toggle travelReverse;
        [SerializeField] TMP_InputField PathNameInput;
        [SerializeField] TMP_InputField mobSearchInput;
        [SerializeField] TextMeshProUGUI markersButton;
        [SerializeField] TextMeshProUGUI roamButton;
        [SerializeField] TextMeshProUGUI aggroButton;

        List<MobTemplate> mobTemplates = new List<MobTemplate>();
        List<QuestTemplate> questTemplates = new List<QuestTemplate>();
        List<DialogueTemplate> dialogueTemplates = new List<DialogueTemplate>();
        List<MerchantTableTemplate> merchantTables = new List<MerchantTableTemplate>();
        Dictionary<int, PatrolPath> patrolPaths = new Dictionary<int, PatrolPath>();
        bool hasAccess = false;
        bool accessChecked = false;
        bool mobselectState = false;
        Dictionary<int, MobSpawn> mobSpawns = new Dictionary<int, MobSpawn>();
        string lingerTime = "0";
        bool showingMarker = false;
        bool showingRoam = false;
        bool showingAggro = false;


        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;
            NetworkAPI.RegisterExtensionMessageHandler("world_developer_response", WorldDeveloperHandler);
            NetworkAPI.RegisterExtensionMessageHandler("mobTemplates", HandleMobTemplateUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("questTemplates", HandleQuestTemplateUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("dialogueTemplates", HandleDialogueTemplateUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("merchantTables", HandleMerchantTableUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("patrolPoints", HandlePatrolPathUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("add_visible_spawn_marker", HandleSpawnList);
            NetworkAPI.RegisterExtensionMessageHandler("spawn_data", HandleSpawnData);
            NetworkAPI.RegisterExtensionMessageHandler("spawn_marker_added", HandleSpawnAdded);
            NetworkAPI.RegisterExtensionMessageHandler("spawn_marker_deleted", HandleSpawnDeleted);

            // Verify we have access
            
            spawnPanel.SetActive(false);
            mobListPanel.SetActive(false);
            startQuestPanel.SetActive(false);
            endQuestPanel.SetActive(false);
            dialogPanel.SetActive(false);
            merchantPanel.SetActive(false);
            selectSpawnPanel.SetActive(false);
            patrolPathPanel.SetActive(false);
            panel.SetActive(false);
            patrolPathCreatePanel.SetActive(false);
            editPositionPanel.SetActive(false);
          //  CheckAccess();
        }

        private void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("world_developer_response", WorldDeveloperHandler);
            NetworkAPI.RemoveExtensionMessageHandler("mobTemplates", HandleMobTemplateUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("questTemplates", HandleQuestTemplateUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("dialogueTemplates", HandleDialogueTemplateUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("merchantTables", HandleMerchantTableUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("patrolPoints", HandlePatrolPathUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("add_visible_spawn_marker", HandleSpawnList);
            NetworkAPI.RemoveExtensionMessageHandler("spawn_data", HandleSpawnData);
            NetworkAPI.RemoveExtensionMessageHandler("spawn_marker_added", HandleSpawnAdded);
            NetworkAPI.RemoveExtensionMessageHandler("spawn_marker_deleted", HandleSpawnDeleted);

        }


        // Update is called once per frame
        void Update()
        {
            if (mobselectState && Input.GetMouseButtonDown(0))
            {
                // Do raycast with layer mask of the spawn makers layer
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                LayerMask layerMask = 1 << spawnMarkerTemplate.layer;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.transform.gameObject.GetComponent<SpawnMarker>() != null)
                    {
                        SpawnSelected(hit.transform.gameObject.GetComponent<SpawnMarker>().MarkerID);
                    }
                }
            }
        }
        public void SetPlayerPosition()
        {
            spawnInCreation.marker.transform.position = ClientAPI.GetPlayerObject().Position;
            spawnInCreation.marker.transform.rotation = ClientAPI.GetPlayerObject().Orientation;
            spawnInCreation.position = ClientAPI.GetPlayerObject().Position;
            spawnInCreation.orientation = ClientAPI.GetPlayerObject().Orientation;
            ShowEditPosition();
        }
        public void ShowEditPosition()
        {
            editPositionPanel.SetActive(true);
            Vector3 position = spawnInCreation.marker.transform.position;
            positionXInput.text = position.x.ToString("0.00");
            //   position.x = float.Parse(posX);
            positionYInput.text = position.y.ToString("0.00");
            //  position.y = float.Parse(posY);
            positionZInput.text = position.z.ToString("0.00");
            // position.z = float.Parse(posZ);
        }

        public void HideEditPosition()
        {
            Vector3 position = new Vector3(float.Parse(positionXInput.text), float.Parse(positionYInput.text), float.Parse(positionZInput.text));

            spawnInCreation.marker.transform.position = position;
            spawnInCreation.position = position;
            editPositionPanel.SetActive(false);

        }



        public void ToggleBuildingModeEnabled()
        {
            if (!accessChecked)
            {
                CheckAccess();
            }
            if (panel.activeSelf == false && hasAccess)
            {
                panel.SetActive(true);
                spawnPanel.SetActive(false);
                mobListPanel.SetActive(false);
                startQuestPanel.SetActive(false);
                endQuestPanel.SetActive(false);
                dialogPanel.SetActive(false);
                merchantPanel.SetActive(false);
                selectSpawnPanel.SetActive(false);
                patrolPathPanel.SetActive(false);
                patrolPathCreatePanel.SetActive(false);
                editPositionPanel.SetActive(false);
                GetMobTemplates();
                AtavismUIUtility.BringToFront(this.gameObject);
            }
            else
            {
                panel.SetActive(false);
                ClearSpawns();
                ClearPatrolPath();
            }

        }

        void CheckAccess()
        {
            if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetPlayerObject().PropertyExists("adminLevel"))
            {
                int adminLevel = (int)ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "adminLevel");
                if (adminLevel == 5)
                {
                    hasAccess = true;
                }
                else
                {
                    int currentWorld = (int)ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "world");
                    if (currentWorld > 0)
                    {
                        Dictionary<string, object> props = new Dictionary<string, object>();
                        props.Add("senderOid", ClientAPI.GetPlayerOid());
                        props.Add("world", currentWorld);
                        NetworkAPI.SendExtensionMessage(0, false, "ao.REQUEST_DEVELOPER_ACCESS", props);
                    }
                }
                accessChecked = true;
            }
        }


        public void ShowSelectSpawnPanel()
        {
            selectSpawnPanel.SetActive(true);
            mobselectState = true;
        }

        public void HideSelectSpawnPanel()
        {
            selectSpawnPanel.SetActive(false);
            mobselectState = false;
        }

        public void ShowSpawnMobPanel()
        {
            //   spawnPanel.SetActive(true);
            //  GetMobTemplates();
            spawnInCreation = new MobSpawn();
            spawnPanel.SetActive(true);
            deleteButton.gameObject.SetActive(false);
            spawnButton.gameObject.SetActive(true);
            saveButton.gameObject.SetActive(false);
            positionButton.gameObject.SetActive(false);
            ShowSpawnMob();
        }




        void ShowSpawnMob()
        {
            spawnPanel.SetActive(true);
            templateName.text = spawnInCreation.GetMobTemplateName(1);
            templateName2.text = spawnInCreation.GetMobTemplateName(2);
            templateName3.text = spawnInCreation.GetMobTemplateName(3);
            templateName4.text = spawnInCreation.GetMobTemplateName(4);
            templateName5.text = spawnInCreation.GetMobTemplateName(5);
            despawnTimeInput.text = spawnInCreation.despawnTime.ToString();
            respawnTimeInput.text = spawnInCreation.respawnTime.ToString();
            respawnTimeMaxInput.text = spawnInCreation.respawnTimeMax.ToString();
            startTimeInput.text = spawnInCreation.spawnActiveStartHour;
            endTimeInput.text = spawnInCreation.spawnActiveEndHour;
            alternateTemplateName.text = spawnInCreation.GetAlternateMobTemplateName(1);
            alternateTemplateName2.text = spawnInCreation.GetAlternateMobTemplateName(2);
            alternateTemplateName3.text = spawnInCreation.GetAlternateMobTemplateName(3);
            alternateTemplateName4.text = spawnInCreation.GetAlternateMobTemplateName(4);
            alternateTemplateName5.text = spawnInCreation.GetAlternateMobTemplateName(5);
            roamRatiusInput.text = spawnInCreation.roamRadius.ToString("N1");
            //  pickupItemInput.text = spawnInCreation.pickupItemID.ToString();
            offersBank.isOn = spawnInCreation.otherActions.Contains("Bank");
            merchantTableValue.text = spawnInCreation.merchantTable.ToString();
            string pname = "";
            foreach (PatrolPath tmpl in patrolPaths.Values)
            {
                if (tmpl.pathID.Equals(spawnInCreation.patrolPath))
                    pname = tmpl.name;
            }
            if (selectedPatrolPathName != null)
                selectedPatrolPathName.text = spawnInCreation.patrolPath + " " + pname;
            /*     if (spawnInCreation.mobTemplate != null && mobCreationState == MobCreationState.EditSpawn) {
                    if (GUILayout.Button("Edit Position")) {
                   //     propertySelectState = MobPropertySelectState.SpawnPositioning;
                    }

               }*/
        }

        public void SelectTemplate(int id)
        {
            if (alternateTemplate)
            {
                foreach (MobTemplate mt in mobTemplates)
                {
                    if (mt.ID.Equals(id))
                    {
                        switch (templSelectId)
                        {
                            case 1:
                                spawnInCreation.alternateMobTemplate = mt;
                                spawnInCreation.alternateMobTemplateID = mt.ID;
                                break;
                            case 2:
                                spawnInCreation.alternateMobTemplate2 = mt;
                                spawnInCreation.alternateMobTemplateID2 = mt.ID;
                                break;
                            case 3:
                                spawnInCreation.alternateMobTemplate3 = mt;
                                spawnInCreation.alternateMobTemplateID3 = mt.ID;
                                break;
                            case 4:
                                spawnInCreation.alternateMobTemplate4 = mt;
                                spawnInCreation.alternateMobTemplateID4 = mt.ID;
                                break;
                            case 5:
                                spawnInCreation.alternateMobTemplate5 = mt;
                                spawnInCreation.alternateMobTemplateID5 = mt.ID;
                                break;
                        }
                        break;
                    }
                }
            }
            else
            {
                foreach (MobTemplate mt in mobTemplates)
                {
                    if (mt.ID.Equals(id))
                    {
                        switch (templSelectId)
                        {
                            case 1:
                                spawnInCreation.mobTemplate = mt;
                                spawnInCreation.mobTemplateID = mt.ID;
                                break;
                            case 2:
                                spawnInCreation.mobTemplate2 = mt;
                                spawnInCreation.mobTemplateID2 = mt.ID;
                                break;
                            case 3:
                                spawnInCreation.mobTemplate3 = mt;
                                spawnInCreation.mobTemplateID3 = mt.ID;
                                break;
                            case 4:
                                spawnInCreation.mobTemplate4 = mt;
                                spawnInCreation.mobTemplateID4 = mt.ID;
                                break;
                            case 5:
                                spawnInCreation.mobTemplate5 = mt;
                                spawnInCreation.mobTemplateID5 = mt.ID;
                                break;
                        }
                    }
                }
            }
            mobListPanel.SetActive(false);
            ShowSpawnMob();
        }
        public void ClearTemplate()
        {
            if (alternateTemplate)
            {
                switch (templSelectId)
                {
                    case 1:
                        spawnInCreation.alternateMobTemplate = null;
                        spawnInCreation.alternateMobTemplateID = -1;
                        break;
                    case 2:
                        spawnInCreation.alternateMobTemplate2 = null;
                        spawnInCreation.alternateMobTemplateID2 = -1;
                        break;
                    case 3:
                        spawnInCreation.alternateMobTemplate3 = null;
                        spawnInCreation.alternateMobTemplateID3 = -1;
                        break;
                    case 4:
                        spawnInCreation.alternateMobTemplate4 = null;
                        spawnInCreation.alternateMobTemplateID4 = -1;
                        break;
                    case 5:
                        spawnInCreation.alternateMobTemplate5 = null;
                        spawnInCreation.alternateMobTemplateID5 = -1;
                        break;
                }
            }
            else
            {
                switch (templSelectId)
                {
                    case 1:
                        spawnInCreation.mobTemplate = null;
                        spawnInCreation.mobTemplateID = -1;
                        break;
                    case 2:
                        spawnInCreation.mobTemplate2 = null;
                        spawnInCreation.mobTemplateID2 = -1;
                        break;
                    case 3:
                        spawnInCreation.mobTemplate3 = null;
                        spawnInCreation.mobTemplateID3 = -1;
                        break;
                    case 4:
                        spawnInCreation.mobTemplate4 = null;
                        spawnInCreation.mobTemplateID4 = -1;
                        break;
                    case 5:
                        spawnInCreation.mobTemplate5 = null;
                        spawnInCreation.mobTemplateID5 = -1;
                        break;
                }
            }
            mobListPanel.SetActive(false);
            ShowSpawnMob();
        }

        bool alternateTemplate = false;
        int templSelectId = 1;
        //  List<UGUIMobCreatorEntry> mobList = new List<UGUIMobCreatorEntry>();

        public void HideMobTemplate()
        {
            mobListPanel.SetActive(false);
        }
        public void ShowMobTemplates(int template)
        {
            this.alternateTemplate = false;
            this.templSelectId = template;
            mobListPanel.SetActive(true);
            foreach (Transform child in mobTemplateGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            //   mobList.Clear();
            int pos = 0;
            foreach (MobTemplate tmpl in mobTemplates)
            {
                if (tmpl.name.Contains(mobSearchInput.text))
                {
                    UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, mobTemplateGrid, false);
                    go.SetEntryDetails(tmpl.name, tmpl.ID, true, false, false, false, false, false, pos, this);
                    pos++;
                }
            }
        }

        public void ShowMobTemplatesAlter(int template)
        {
            this.alternateTemplate = true;
            this.templSelectId = template;
            mobListPanel.SetActive(true);
            foreach (Transform child in mobTemplateGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            //   mobList.Clear();
            int pos = 0;
            foreach (MobTemplate tmpl in mobTemplates)
            {
                if (tmpl.name.Contains(mobSearchInput.text))
                {
                    UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, mobTemplateGrid, false);
                    go.SetEntryDetails(tmpl.name, tmpl.ID, true, false, false, false, false, false, pos, this);
                    pos++;
                }
            }
        }

        public void ShowMobTemplatesSearch()
        {
            //    this.alternateTemplate = alternateTemplate;
            mobListPanel.SetActive(true);
            foreach (Transform child in mobTemplateGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            //   mobList.Clear();
            int pos = 0;
            foreach (MobTemplate tmpl in mobTemplates)
            {
                if (tmpl.name.Contains(mobSearchInput.text))
                {
                    UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, mobTemplateGrid, false);
                    go.SetEntryDetails(tmpl.name, tmpl.ID, true, false, false, false, false, false, pos, this);
                    pos++;
                }
            }
        }






        public void StartQuestClicked(int id)
        {
            foreach (QuestTemplate tmpl in questTemplates)
            {
                if (tmpl.questID.Equals(id))
                {
                    if (!spawnInCreation.startsQuests.Contains(tmpl.questID))
                    {
                        spawnInCreation.startsQuests.Add(tmpl.questID);
                    }
                    else
                    {
                        spawnInCreation.startsQuests.Remove(tmpl.questID);
                    }
                    break;
                }
            }
            ShowStartQuest();
        }

        public void EndQuestClicked(int id)
        {
            foreach (QuestTemplate tmpl in questTemplates)
            {
                if (tmpl.questID.Equals(id))
                {
                    if (!spawnInCreation.endsQuests.Contains(tmpl.questID))
                    {
                        spawnInCreation.endsQuests.Add(tmpl.questID);
                    }
                    else
                    {
                        spawnInCreation.endsQuests.Remove(tmpl.questID);
                    }
                    break;
                }
            }
            ShowEndQuest();
        }

        public void ShowStartQuest()
        {
            startQuestPanel.SetActive(true);

            foreach (Transform child in startQuestSelectedGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in startQuestAvailableGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            // Draw Display names
            int pos = 0;
            foreach (int questID in spawnInCreation.startsQuests)
            {
                QuestTemplate tmpl = GetQuestTemplate(questID);
                UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, startQuestSelectedGrid, false);
                go.SetEntryDetails(tmpl.title, tmpl.questID, false, true, false, false, false, false, pos, this);

                pos++;
            }
            // Draw Display names
            pos = 0;
            foreach (QuestTemplate tmpl in questTemplates)
            {
                if (!spawnInCreation.startsQuests.Contains(tmpl.questID))
                {
                    UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, startQuestAvailableGrid, false);
                    go.SetEntryDetails(tmpl.title, tmpl.questID, false, true, false, false, false, false, pos, this);
                    pos++;
                }
            }

        }

        public void ShowEndQuest()
        {
            endQuestPanel.SetActive(true);
            foreach (Transform child in endQuestSelectedGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in endQuestAvailableGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            // Draw Display names
            int pos = 0;
            foreach (int questID in spawnInCreation.endsQuests)
            {
                QuestTemplate tmpl = GetQuestTemplate(questID);
                UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, endQuestSelectedGrid, false);
                go.SetEntryDetails(tmpl.title, tmpl.questID, false, false, true, false, false, false, pos, this);

                pos++;
            }
            // Draw Display names
            pos = 0;
            foreach (QuestTemplate tmpl in questTemplates)
            {
                if (!spawnInCreation.endsQuests.Contains(tmpl.questID))
                {
                    UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, endQuestAvailableGrid, false);
                    go.SetEntryDetails(tmpl.title, tmpl.questID, false, false, true, false, false, false, pos, this);
                    pos++;
                }
            }

        }

        public void HideStartQuest()
        {
            startQuestPanel.SetActive(false);
        }

        public void HideEndQuest()
        {
            endQuestPanel.SetActive(false);
        }

        public void HideMerchantTable()
        {
            merchantPanel.SetActive(false);
        }

        public void MerchandTableClicked(int id)
        {
            spawnInCreation.merchantTable = id;
            //  if (id != -1)
            merchantPanel.SetActive(false);
            ShowSpawnMob();
        }

        public void ShowMerchandTables()
        {
            merchantPanel.SetActive(true);


            foreach (Transform child in merchantGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (MerchantTableTemplate tmpl in merchantTables)
            {
                UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, merchantGrid, false);
                go.SetEntryDetails(tmpl.title, tmpl.tableID, false, false, false, true, false, false, 0, this);
            }
        }
        public void HideDialogues()
        {
            dialogPanel.SetActive(false);

        }
        public void DialoguesClicked(int id)
        {
            foreach (DialogueTemplate tmpl in dialogueTemplates)
            {
                if (tmpl.dialogueID.Equals(id))
                {
                    if (!spawnInCreation.startsDialogues.Contains(tmpl.dialogueID))
                    {
                        spawnInCreation.startsDialogues.Add(tmpl.dialogueID);
                    }
                    else
                    {
                        spawnInCreation.startsDialogues.Remove(tmpl.dialogueID);
                    }
                    break;
                }
            }
            ShowDialogues();
        }

        public void ShowDialogues()
        {
            dialogPanel.SetActive(true);
            foreach (Transform child in dialogSelectedGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in dialogAvailableGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            // Draw Display names
            int pos = 0;
            foreach (int questID in spawnInCreation.startsDialogues)
            {
                DialogueTemplate tmpl = GetDialogueTemplate(questID);
                UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, dialogSelectedGrid, false);
                go.SetEntryDetails(tmpl.title, tmpl.dialogueID, false, false, false, false, true, false, pos, this);

                pos++;
            }
            // Draw Display names
            pos = 0;
            foreach (DialogueTemplate tmpl in dialogueTemplates)
            {
                if (!spawnInCreation.startsDialogues.Contains(tmpl.dialogueID))
                {
                    UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, dialogAvailableGrid, false);
                    go.SetEntryDetails(tmpl.title, tmpl.dialogueID, false, false, false, false, true, false, pos, this);
                    pos++;
                }
            }

        }
        public void HidePatrolPath()
        {
            patrolPathPanel.SetActive(false);

        }


        public void HidePatrolPathCreate()
        {
            //  ClearPatrolPath();
            // spawnInCreation.patrolPoints.Clear();
            patrolPathCreatePanel.SetActive(false);
            if (selectedPatrolPathName != null)
                selectedPatrolPathName.text = "New " + PathNameInput.text;
            spawnInCreation.patrolPath = -1;

        }
        public void SetLinger()
        {

        }

        public void PatrolPathAddPointClicked()
        {
            PatrolPoint pp = new PatrolPoint();
            pp.marker = Instantiate<GameObject>(patrolMarkerTemplate);
            pp.marker.transform.position = ClientAPI.GetPlayerObject().Position;
            pp.marker.transform.rotation = ClientAPI.GetPlayerObject().Orientation;
            pp.lingerTime = int.Parse(lingerTimeInput.text);
            spawnInCreation.patrolPoints.Add(pp);
            ShowPatrolPathCreate();
        }

        public void PatrolPathDeletePointClicked(PatrolPoint pp)
        {
            spawnInCreation.patrolPoints.Remove(pp);
            DestroyImmediate(pp.marker);
            ShowPatrolPathCreate();
        }
        /// <summary>
        /// Show Window of Create of the new patrol path
        /// </summary>
        public void ShowPatrolPathCreate()
        {
            foreach (Transform child in patrolPathCreateGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            // Draw Display names
            foreach (PatrolPoint point in spawnInCreation.patrolPoints)
            {
                UGUIMobCreatorPathEntry go = (UGUIMobCreatorPathEntry)Instantiate(rowPathPrefab, patrolPathCreateGrid, false);
                go.SetEntryDetails(point.marker.transform.position.x + "," + point.marker.transform.position.y + "," + point.marker.transform.position.z, point, this);
            }
            lingerTimeInput.text = lingerTime;
            travelReverse.isOn = spawnInCreation.travelReverse;
            patrolPathCreatePanel.SetActive(true);
            patrolPathPanel.SetActive(false);
        }
        /// <summary>
        /// set travel path to be  Reverse
        /// </summary>
        public void SetTravelReverse()
        {
            spawnInCreation.travelReverse = travelReverse.isOn;
        }

        public void PatrolPathClicked(int id)
        {
            spawnInCreation.patrolPoints.Clear();
            spawnInCreation.patrolPath = id;
            string pname = "";
            foreach (PatrolPath tmpl in patrolPaths.Values)
            {
                if (tmpl.pathID.Equals(id))
                    pname = tmpl.name;
            }
            if (selectedPatrolPathName != null)
                selectedPatrolPathName.text = id + " " + pname;
            patrolPathPanel.SetActive(false);
        }

        /// <summary>
        /// Show window with list of partrol path
        /// </summary>
        public void ShowPatrolPath()
        {

            patrolPathPanel.SetActive(true);
            foreach (Transform child in patrolPathGrid)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (PatrolPath tmpl in patrolPaths.Values)
            {
                UGUIMobCreatorEntry go = (UGUIMobCreatorEntry)Instantiate(rowPrefab, patrolPathGrid, false);
                go.SetEntryDetails(tmpl.name, tmpl.pathID, false, false, false, false, false, true, 0, this);
            }
        }

        /// <summary>
        /// Set Bank option to spawner
        /// </summary>

        public void SetBank()
        {
            bool _offersBank = spawnInCreation.otherActions.Contains("Bank");
            bool _nowOffersBank = offersBank.isOn;
            if (_nowOffersBank)
            {
                if (!_offersBank)
                    spawnInCreation.otherActions.Add("Bank");
            }
            else
            {
                spawnInCreation.otherActions.Remove("Bank");
            }

        }
        /// <summary>
        /// Prepare Dictionary of spawn properties to sent 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        Dictionary<string, object> SetMobSpawnMessageProps(Vector3 position, Quaternion orientation)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("playerOid", ClientAPI.GetPlayerOid());
            props.Add("markerID", spawnInCreation.ID);
            props.Add("loc", position);
            props.Add("orient", orientation);
            props.Add("mobTemplate", spawnInCreation.mobTemplateID);
            props.Add("mobTemplate2", spawnInCreation.mobTemplateID2);
            props.Add("mobTemplate3", spawnInCreation.mobTemplateID3);
            props.Add("mobTemplate4", spawnInCreation.mobTemplateID4);
            props.Add("mobTemplate5", spawnInCreation.mobTemplateID5);
            props.Add("respawnTime", spawnInCreation.respawnTime);
            props.Add("respawnTimeMax", spawnInCreation.respawnTimeMax);
            props.Add("despawnTime", spawnInCreation.despawnTime);
            props.Add("numSpawns", 1);
            props.Add("spawnRadius", 0);
            props.Add("spawnActiveStartHour", int.Parse(spawnInCreation.spawnActiveStartHour));
            props.Add("spawnActiveEndHour", int.Parse(spawnInCreation.spawnActiveEndHour));
            props.Add("alternateMobTemplate", spawnInCreation.alternateMobTemplateID);
            props.Add("alternateMobTemplate2", spawnInCreation.alternateMobTemplateID2);
            props.Add("alternateMobTemplate3", spawnInCreation.alternateMobTemplateID3);
            props.Add("alternateMobTemplate4", spawnInCreation.alternateMobTemplateID4);
            props.Add("alternateMobTemplate5", spawnInCreation.alternateMobTemplateID5);
            props.Add("roamRadius", (int)spawnInCreation.roamRadius);
            props.Add("merchantTable", spawnInCreation.merchantTable);
            props.Add("patrolPath", spawnInCreation.patrolPath);
            props.Add("patrolPointsCount", spawnInCreation.patrolPoints.Count);
            props.Add("patrolPointName", PathNameInput.text);
            for (int i = 0; i < spawnInCreation.patrolPoints.Count; i++)
            {
                props.Add("patrolPoint" + i + "x", spawnInCreation.patrolPoints[i].marker.transform.position.x);
                props.Add("patrolPoint" + i + "y", spawnInCreation.patrolPoints[i].marker.transform.position.y);
                props.Add("patrolPoint" + i + "z", spawnInCreation.patrolPoints[i].marker.transform.position.z);
                props.Add("patrolPoint" + i + "linger", spawnInCreation.patrolPoints[i].lingerTime);
            }
            props.Add("patrolPointsTravelReverse", spawnInCreation.travelReverse);
            props.Add("pickupItem", spawnInCreation.pickupItemID);
            props.Add("isChest", spawnInCreation.isChest);
            props.Add("domeID", -1);
            props.Add("startsQuestsCount", spawnInCreation.startsQuests.Count);
            props.Add("endsQuestsCount", spawnInCreation.endsQuests.Count);
            for (int i = 0; i < spawnInCreation.startsQuests.Count; i++)
            {
                props.Add("startsQuest" + i + "ID", spawnInCreation.startsQuests[i]);
            }
            for (int i = 0; i < spawnInCreation.endsQuests.Count; i++)
            {
                props.Add("endsQuest" + i + "ID", spawnInCreation.endsQuests[i]);
            }
            props.Add("startsDialoguesCount", spawnInCreation.startsDialogues.Count);
            for (int i = 0; i < spawnInCreation.startsDialogues.Count; i++)
            {
                props.Add("startsDialogue" + i + "ID", spawnInCreation.startsDialogues[i]);
            }
            props.Add("otherActionsCount", spawnInCreation.otherActions.Count);
            for (int i = 0; i < spawnInCreation.otherActions.Count; i++)
            {
                props.Add("otherAction" + i, spawnInCreation.otherActions[i]);
            }
            return props;
        }

        /// <summary>
        /// Send Spawn new mob to the server
        /// </summary>
        public void SpawnMobHere()
        {
            spawnInCreation.despawnTime = int.Parse(despawnTimeInput.text);
            spawnInCreation.respawnTime = int.Parse(respawnTimeInput.text);
            spawnInCreation.respawnTimeMax = int.Parse(respawnTimeMaxInput.text);
            spawnInCreation.spawnActiveStartHour = startTimeInput.text;
            spawnInCreation.spawnActiveEndHour = endTimeInput.text;
            spawnInCreation.roamRadius = float.Parse(roamRatiusInput.text);

            Vector3 position = ClientAPI.GetPlayerObject().Position;
            position.y = Mathf.Ceil(position.y*100f)/100f;
            Dictionary<string, object> props = SetMobSpawnMessageProps(position, ClientAPI.GetPlayerObject().Orientation);
            NetworkAPI.SendExtensionMessage(0, false, "mob.CREATE_MOB_SPAWN", props);
            ClientAPI.Write("Sending create mob spawn");
            ClearPatrolPath();
            Dictionary<string, object> sProps = new Dictionary<string, object>();
            sProps.Add("senderOid", ClientAPI.GetPlayerOid());
            sProps.Add("type", "patrol");
            NetworkAPI.SendExtensionMessage(0, false, "mob.GET_TEMPLATES", sProps);
        }
        /// <summary>
        /// Send Delete Spawn Marker to the server
        /// </summary>
        public void DeleteSpawn()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("senderOid", ClientAPI.GetPlayerOid());
            props.Add("markerID", spawnInCreation.ID);
            NetworkAPI.SendExtensionMessage(0, false, "ao.DELETE_SPAWN_MARKER", props);
            spawnInCreation.DeleteMarker();
            spawnInCreation = null;
            spawnPanel.SetActive(false);

        }

        /// <summary>
        /// Cancel Spawn
        /// </summary>
        public void CancelSpawn()
        {
            if (spawnInCreation.ID < 0)
            {
                spawnInCreation.DeleteMarker();
            }
            spawnInCreation = null;
            spawnPanel.SetActive(false);
            ClearPatrolPath();
        }

        /// <summary>
        /// Apply Changes in inputs of the spawn
        /// </summary>
        public void ApplyChangespawn()
        {
            spawnInCreation.despawnTime = int.Parse(despawnTimeInput.text);
            spawnInCreation.respawnTime = int.Parse(respawnTimeInput.text);
            spawnInCreation.respawnTimeMax = int.Parse(respawnTimeMaxInput.text);
            spawnInCreation.spawnActiveStartHour = startTimeInput.text;
            spawnInCreation.spawnActiveEndHour = endTimeInput.text;
            spawnInCreation.roamRadius = float.Parse(roamRatiusInput.text);

        }
        /// <summary>
        /// Send Update Sprawn to server
        /// </summary>
        public void UpdateSpawn()
        {
            spawnInCreation.despawnTime = int.Parse(despawnTimeInput.text);
            spawnInCreation.respawnTime = int.Parse(respawnTimeInput.text);
            spawnInCreation.respawnTimeMax = int.Parse(respawnTimeMaxInput.text);
            spawnInCreation.spawnActiveStartHour = startTimeInput.text;
            spawnInCreation.spawnActiveEndHour = endTimeInput.text;
            spawnInCreation.roamRadius = float.Parse(roamRatiusInput.text);

            Dictionary<string, object> props = SetMobSpawnMessageProps(spawnInCreation.position, spawnInCreation.orientation);
            NetworkAPI.SendExtensionMessage(0, false, "ao.EDIT_SPAWN_MARKER", props);
            ClearPatrolPath();
            spawnPanel.SetActive(false);
        }

        /// <summary>
        /// Select Spawner
        /// </summary>
        /// <param name="spawnID"></param>
        public void SpawnSelected(int spawnID)
        {
            if (!mobselectState)
                return;
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("senderOid", ClientAPI.GetPlayerOid());
            props.Add("markerID", spawnID);
            NetworkAPI.SendExtensionMessage(0, false, "ao.REQUEST_SPAWN_DATA", props);
        }
        /// <summary>
        /// Geting Quest Template from the list of templates
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        QuestTemplate GetQuestTemplate(int questID)
        {
            foreach (QuestTemplate tmpl in questTemplates)
            {
                if (tmpl.questID == questID)
                    return tmpl;
            }
            return null;
        }
        /// <summary>
        /// Geting dialogue from the list od dialoges
        /// </summary>
        /// <param name="dialogueID"></param>
        /// <returns></returns>
        DialogueTemplate GetDialogueTemplate(int dialogueID)
        {
            foreach (DialogueTemplate tmpl in dialogueTemplates)
            {
                if (tmpl.dialogueID == dialogueID)
                    return tmpl;
            }
            return null;
        }

        /// <summary>
        /// Delete all path markers and clear paths
        /// </summary>
        void ClearPatrolPath()
        {
            if (spawnInCreation != null)
            {
                foreach (PatrolPoint pp in spawnInCreation.patrolPoints)
                {
                    DestroyImmediate(pp.marker);
                }
                spawnInCreation.patrolPoints.Clear();
            }
            if (PathNameInput != null)
                PathNameInput.text = "";
            if (selectedPatrolPathName != null)
                selectedPatrolPathName.text = "";
        }

        /// <summary>
        /// Geting Mob Template from the list of templates
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MobTemplate GetMobTemplateByID(int id)
        {
            MobTemplate template = null;
            foreach (MobTemplate tmpl in mobTemplates)
            {
                if (tmpl.ID == id)
                    return tmpl;
            }
            return template;
        }

        void ClearSpawns()
        {
            if (spawnInCreation != null)
                spawnInCreation.DeleteMarker();
            foreach (MobSpawn spawn in mobSpawns.Values)
            {
                spawn.DeleteMarker();
            }
            mobSpawns.Clear();
        }

        public void ToggleSpawns()
        {
            if (showingMarker)
            {
                foreach (MobSpawn spawn in mobSpawns.Values)
                {
                    showingMarker = false;
                    spawn.HideMarker();
                }
                if (markersButton != null)
                    markersButton.color = Color.red;
            }
            else
            {
                foreach (MobSpawn spawn in mobSpawns.Values)
                {
                    spawn.ShowMarker();
                    showingMarker = true;
                }
                if (markersButton != null)
                    markersButton.color = Color.green;
            }
        }

        public void ToggleAggro()
        {
            if (showingAggro)
            {
                foreach (MobSpawn spawn in mobSpawns.Values)
                {
                    showingAggro = false;
                    spawn.HideAggro();
                }
                if (aggroButton != null)
                    aggroButton.color = Color.red;
            }
            else
            {
                if (!showingMarker)
                    ToggleSpawns();
                foreach (MobSpawn spawn in mobSpawns.Values)
                {
                    spawn.ShowAggro();
                    showingAggro = true;
                }
                if (aggroButton != null)
                    aggroButton.color = Color.green;
            }
        }

        public void ToggleRoam()
        {
            if (showingRoam)
            {
                foreach (MobSpawn spawn in mobSpawns.Values)
                {
                    spawn.HideRoam();
                    showingRoam = false;
                }
                if (roamButton != null)
                    roamButton.color = Color.red;
            }
            else
            {
                if (!showingMarker)
                    ToggleSpawns();
                foreach (MobSpawn spawn in mobSpawns.Values)
                {
                    spawn.ShowRoam();
                    showingRoam = true;
                }
                if (roamButton != null)
                    roamButton.color = Color.green;
            }

        }



        public void GetMobTemplates()
        {
            // get mob templates
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("senderOid", ClientAPI.GetPlayerOid());
            props.Add("type", "mob,quests,dialogues,merchantTables,patrol");
            NetworkAPI.SendExtensionMessage(0, false, "mob.GET_TEMPLATES", props);

            // Get spawn markers
            props = new Dictionary<string, object>();
            props.Add("senderOid", ClientAPI.GetPlayerOid());
            NetworkAPI.SendExtensionMessage(0, false, "ao.VIEW_MARKERS", props);
        }
        #region Message Handlers

        public void WorldDeveloperHandler(Dictionary<string, object> props)
        {
            bool isAdmin = (bool)props["isAdmin"];
            bool isDeveloper = (bool)props["isDeveloper"];
            if (isAdmin || isDeveloper)
            {
                hasAccess = true;
            }
            else
            {
                hasAccess = false;
            }
        }

        public void HandleMobTemplateUpdate(Dictionary<string, object> props)
        {
            mobTemplates.Clear();
            int numTemplates = (int)props["numTemplates"];
            for (int i = 0; i < numTemplates; i++)
            {
                MobTemplate template = new MobTemplate();
                template.name = (string)props["mob_" + i + "Name"];
                template.ID = (int)props["mob_" + i + "ID"];
                template.subtitle = (string)props["mob_" + i + "SubTitle"];
                template.species = (string)props["mob_" + i + "Species"];
                template.subspecies = (string)props["mob_" + i + "Subspecies"];
                template.level = (int)props["mob_" + i + "Level"];
                template.attackable = (bool)props["mob_" + i + "Attackable"];
                template.faction = (int)props["mob_" + i + "Faction"];
                template.mobType = (int)props["mob_" + i + "MobType"];
                //template.gender = (string)props["mob_" + i + "Gender"];
                template.scale = (float)props["mob_" + i + "Scale"];
                List<string> displays = new List<string>();
                int numDisplays = (int)props["mob_" + i + "NumDisplays"];
                for (int j = 0; j < numDisplays; j++)
                {
                    string display = (string)props["mob_" + i + "Display" + j];
                    displays.Add(display);
                }
                template.displays = displays;
                /*for itemID in props["mob_%dEquipment" % i]:
                    template.equippedItems.append(GetItemTemplateByID(itemID))
                numLootTables = int(props["mob_%dNumLootTables" % i])
                for j in range(0, numLootTables):
                    lootTableID = int(props["mob_%dLootTable%d" % (i, j)])
                    lootTableChance = int(props["mob_%dLootTable%dChance" % (i, j)])
                    lootTable = LootTableDropEntry()
                    lootTable.itemID = lootTableID
                    lootTable.dropChance = lootTableChance
                    template.lootTables.append(lootTable)
                    //ClientAPI.Write("Loot table %s for mob %s has chance: %s" % (lootTableID, template.name, lootTableChance))*/
                mobTemplates.Add(template);
            }
            ClientAPI.Write("Number of mob templates added: " + mobTemplates.Count);
            //mobCreationState = MobCreationState.SelectTemplate;
            //selectedTemplate = -1;
        }

        public void HandleQuestTemplateUpdate(Dictionary<string, object> props)
        {
            questTemplates.Clear();
            int numTemplates = (int)props["numTemplates"];
            for (int i = 0; i < numTemplates; i++)
            {
                QuestTemplate template = new QuestTemplate();
                template.title = (string)props["quest_" + i + "Title"];
                template.questID = (int)props["quest_" + i + "Id"];
                questTemplates.Add(template);
            }
            ClientAPI.Write("Number of quest templates added: " + questTemplates.Count);
        }

        public void HandleDialogueTemplateUpdate(Dictionary<string, object> props)
        {
            dialogueTemplates.Clear();
            int numTemplates = (int)props["numTemplates"];
            for (int i = 0; i < numTemplates; i++)
            {
                DialogueTemplate template = new DialogueTemplate();
                template.title = (string)props["dialogue_" + i + "Title"];
                template.dialogueID = (int)props["dialogue_" + i + "Id"];
                dialogueTemplates.Add(template);
            }
            ClientAPI.Write("Number of dialogue templates added: " + dialogueTemplates.Count);
        }

        public void HandleMerchantTableUpdate(Dictionary<string, object> props)
        {
            merchantTables.Clear();
            int numTemplates = (int)props["numTemplates"];
            for (int i = 0; i < numTemplates; i++)
            {
                MerchantTableTemplate template = new MerchantTableTemplate();
                template.title = (string)props["merchant_" + i + "Title"];
                template.tableID = (int)props["merchant_" + i + "Id"];
                merchantTables.Add(template);
            }
            ClientAPI.Write("Number of merchant tables added: " + merchantTables.Count);
        }

        public void HandlePatrolPathUpdate(Dictionary<string, object> props)
        {
            patrolPaths.Clear();
            int numTemplates = (int)props["numPatrols"];
            for (int i = 0; i < numTemplates; i++)
            {
                PatrolPath template = new PatrolPath();
                template.name = (string)props["patrol_" + i + "Title"];
                template.pathID = (int)props["patrol_" + i + "Id"];
                patrolPaths[template.pathID] = template;
            }
            ClientAPI.Write("Number of patrol paths added: " + patrolPaths.Count);
        }

        private void HandleSpawnDeleted(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("Got spawn marker delete");
            int id = (int)props["spawnID"];
            if (mobSpawns.ContainsKey(id))
            {
                mobSpawns[id].DeleteMarker();
                mobSpawns.Remove(id);
            }
            AtavismLogger.LogDebugMessage("Removed spawn: " + id);
        }

        public void HandleSpawnList(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("Got spawn list");
            ClearSpawns();
            int numMarkers = (int)props["numMarkers"];
            for (int i = 0; i < numMarkers; i++)
            {
                MobSpawn spawn = new MobSpawn();
                spawn.ID = (int)props["markerID_" + i];
                spawn.position = (Vector3)props["markerLoc_" + i];
                spawn.orientation = (Quaternion)props["markerOrient_" + i];
                spawn.roamRadius = (int)props["markerRoamRadius_" + i];
                spawn.aggroRadius = (int)props["markerAggroRadius_" + i];
                spawn.CreateMarkerObject(spawnMarkerTemplate);
                spawn.CreateRoamObject(spawnRoamTemplate);
                spawn.CreateAggroObject(spawnAggroTemplate);
                if (!showingMarker)
                    spawn.HideMarker();
                if (!showingRoam)
                    spawn.HideRoam();
                if (!showingAggro)
                    spawn.HideAggro();
                mobSpawns.Add(spawn.ID, spawn);
                AtavismLogger.LogDebugMessage("Added spawn: " + spawn.ID);
            }

            if (markersButton != null)
                if (showingMarker)
                {
                    markersButton.color = Color.green;
                }
                else
                {
                    markersButton.color = Color.red;
                }
            if (roamButton != null)
                if (showingRoam)
                {
                    roamButton.color = Color.green;
                }
                else
                {
                    roamButton.color = Color.red;
                }
            if (aggroButton != null)
                if (showingAggro)
                {
                    aggroButton.color = Color.green;
                }
                else
                {
                    aggroButton.color = Color.red;
                }

            if (!panel.activeSelf)
            {
                ClearSpawns();
                ClearPatrolPath();
            }
        }

        public void HandleSpawnAdded(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("Got spawn added");
            MobSpawn spawn = new MobSpawn();
            spawn.ID = (int)props["markerID"];
            spawn.position = (Vector3)props["markerLoc"];
            spawn.orientation = (Quaternion)props["markerOrient"];
            spawn.roamRadius = (int)props["markerRoamRadius"];
            spawn.aggroRadius = (int)props["markerAggroRadius"];
            spawn.CreateMarkerObject(spawnMarkerTemplate);
            spawn.CreateRoamObject(spawnRoamTemplate);
            spawn.CreateAggroObject(spawnAggroTemplate);
            if (!showingMarker)
                spawn.HideMarker();
            if (!showingRoam)
                spawn.HideRoam();
            if (!showingAggro)
                spawn.HideAggro();
            if (mobSpawns.ContainsKey(spawn.ID))
            {
                mobSpawns[spawn.ID].DeleteMarker();
                mobSpawns.Remove(spawn.ID);
            }


            mobSpawns.Add(spawn.ID, spawn);
            AtavismLogger.LogDebugMessage("Added spawn: " + spawn.ID);
        }


        public void HandleSpawnData(Dictionary<string, object> props)
        {
            int spawnID = (int)props["spawnID"];
            spawnInCreation = mobSpawns[spawnID];
            //spawnInCreation.ID = 
            spawnInCreation.numSpawns = (int)props["numSpawns"];
            spawnInCreation.despawnTime = (int)props["despawnTime"];
            spawnInCreation.respawnTime = (int)props["respawnTime"];
            spawnInCreation.respawnTimeMax = (int)props["respawnTimeMax"];
            spawnInCreation.spawnRadius = (int)props["spawnRadius"];
            spawnInCreation.mobTemplateID = (int)props["mobTemplate"];
            spawnInCreation.mobTemplateID2 = (int)props["mobTemplate2"];
            spawnInCreation.mobTemplateID3 = (int)props["mobTemplate3"];
            spawnInCreation.mobTemplateID4 = (int)props["mobTemplate4"];
            spawnInCreation.mobTemplateID5 = (int)props["mobTemplate5"];
            spawnInCreation.mobTemplate = GetMobTemplateByID(spawnInCreation.mobTemplateID);
            spawnInCreation.mobTemplate2 = GetMobTemplateByID(spawnInCreation.mobTemplateID2);
            spawnInCreation.mobTemplate3 = GetMobTemplateByID(spawnInCreation.mobTemplateID3);
            spawnInCreation.mobTemplate4 = GetMobTemplateByID(spawnInCreation.mobTemplateID4);
            spawnInCreation.mobTemplate5 = GetMobTemplateByID(spawnInCreation.mobTemplateID5);
            spawnInCreation.roamRadius = (int)props["roamRadius"];
            spawnInCreation.patrolPath = (int)props["patrolPath"];
            spawnInCreation.spawnActiveStartHour = "" + (int)props["spawnActiveStartHour"];
            spawnInCreation.spawnActiveEndHour = "" + (int)props["spawnActiveEndHour"];
            spawnInCreation.alternateMobTemplateID = (int)props["alternateMobTemplate"];
            spawnInCreation.alternateMobTemplateID2 = (int)props["alternateMobTemplate2"];
            spawnInCreation.alternateMobTemplateID3 = (int)props["alternateMobTemplate3"];
            spawnInCreation.alternateMobTemplateID4 = (int)props["alternateMobTemplate4"];
            spawnInCreation.alternateMobTemplateID5 = (int)props["alternateMobTemplate5"];
            spawnInCreation.alternateMobTemplate = GetMobTemplateByID(spawnInCreation.alternateMobTemplateID);
            spawnInCreation.alternateMobTemplate2 = GetMobTemplateByID(spawnInCreation.alternateMobTemplateID2);
            spawnInCreation.alternateMobTemplate3 = GetMobTemplateByID(spawnInCreation.alternateMobTemplateID3);
            spawnInCreation.alternateMobTemplate4 = GetMobTemplateByID(spawnInCreation.alternateMobTemplateID4);
            spawnInCreation.alternateMobTemplate5 = GetMobTemplateByID(spawnInCreation.alternateMobTemplateID5);
            //spawnInCreation.hasCombat = (bool)props["hasCombat"];
            //spawnInCreation.startsQuests = (List<object>)props["startsQuests"];
            spawnInCreation.merchantTable = (int)props["merchantTable"];
            List<object> questList = (List<object>)props["startsQuests"];
            foreach (object quest in questList)
            {
                spawnInCreation.startsQuests.Add((int)quest);
            }
            //spawnInCreation.endsQuests = (List<object>)props["endsQuests"];
            questList = (List<object>)props["endsQuests"];
            foreach (object quest in questList)
            {
                spawnInCreation.endsQuests.Add((int)quest);
            }
            List<object> dialogueList = (List<object>)props["startsDialogues"];
            foreach (object dialogue in dialogueList)
            {
                spawnInCreation.startsDialogues.Add((int)dialogue);
            }
            spawnInCreation.pickupItemID = (int)props["pickupItem"];
            spawnInCreation.isChest = (bool)props["isChest"];
            List<object> otherActions = (List<object>)props["otherActions"];
            foreach (object action in otherActions)
            {
                spawnInCreation.otherActions.Add((string)action);
            }
            //  spawnInCreation.otherActions
            // mobCreationState = MobCreationState.EditSpawn;
            HideSelectSpawnPanel();
            deleteButton.gameObject.SetActive(true);
            spawnButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(true);
            positionButton.gameObject.SetActive(true);

            ShowSpawnMob();
        }

        #endregion Message Handlers

        public static UGUIMobCreator Instance
        {
            get
            {
                return instance;
            }
        }
    }
}