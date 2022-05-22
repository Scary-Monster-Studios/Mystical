using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace Atavism
{

    public class UGUIArenaScore : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Transform[] TeamsGrids;
        [SerializeField] TextMeshProUGUI Score;
        [SerializeField] TextMeshProUGUI[] teamName;
        List<ArenaTeamEntity>[] _teams = { null, null };
        bool showing = false;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("ARENA_SCORE_SETUP", this);
            AtavismEventSystem.RegisterEvent("ARENA_SCORE_UPDATE", this);
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
            NetworkAPI.RegisterExtensionMessageHandler("arena_end", HandleArenaEndMessage);
            ArenaSetup();
            Hide();

        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ARENA_SCORE_SETUP", this);
            AtavismEventSystem.UnregisterEvent("ARENA_SCORE_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
            NetworkAPI.RemoveExtensionMessageHandler("arena_end", HandleArenaEndMessage);
        }

        private void HandleArenaEndMessage(Dictionary<string, object> props)
        {
            Show();
            UpdateDisplay();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2) && !AtavismSettings.UIHasFocus() && Arena.Instance.InArena)
            {
                if (showing)
                    Hide();
                else
                    Show();
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ARENA_SCORE_UPDATE" || eData.eventType == "UPDATE_LANGUAGE")
            {
                UpdateDisplay();
            }
            else if (eData.eventType == "ARENA_SCORE_SETUP")
            {
                ArenaSetup();
            }
        }




        void ArenaSetup()
        {
            List<ArenaTeamEntry> teams = Arena.Instance.ArenaTeamEntries;
            if (teams != null)
            {
                if (teams.Count > 0)
                {
                    for (int i = 0; i < teams.Count; i++)
                    {
                        foreach (Transform child in TeamsGrids[i].transform)
                        {
                            GameObject.Destroy(child.gameObject);
                        }

                        if (_teams[i] == null)
                            _teams[i] = new List<ArenaTeamEntity>();
                        for (int j = 0; j < teams[i].players.Count; j++)
                        {

                            GameObject go = Instantiate(prefab, TeamsGrids[i]);
                            ArenaTeamEntity ply = go.GetComponent<ArenaTeamEntity>();
                            ply.PlyName.text = teams[i].players[j].playerName;
                            ply.PlyScore.text = teams[i].players[j].score.ToString();
                            ply.PlyKills.text = teams[i].players[j].kills.ToString();
                            ply.PlyDeaths.text = teams[i].players[j].deaths.ToString();
                            ply.PlyDamageTaken.text = teams[i].players[j].damageTaken.ToString();
                            ply.PlyDamageDealt.text = teams[i].players[j].damageDealt.ToString();
                            _teams[i].Add(ply);
                        }
                    }
                }
                else
                {
                    //  Debug.LogError("ArenaSetup teams = 0");
                }
            }
            else
            {
                //   Debug.LogError("ArenaSetup teams null");
            }
        }
        void UpdateDisplay()
        {
            List<ArenaTeamEntry> teams = Arena.Instance.ArenaTeamEntries;
            if (Score != null)
                Score.text = "";
            if (teams != null)
            {
                if (teams.Count > 0)
                {
                    for (int i = 0; i < teams.Count; i++)
                    {
                        if (teamName[i] != null)
                            teamName[i].text = teams[i].teamName;
                        if (Score != null)
                        {
                            Score.text = Score.text + teams[i].score.ToString();
                            if (teams.Count - 1 > i)
                                Score.text = Score.text + " : ";
                        }

                        if (_teams[i] == null)
                            _teams[i] = new List<ArenaTeamEntity>();
                        for (int j = 0; j < teams[i].players.Count; j++)
                        {
                            if (_teams[i].Count - 1 < j)
                            {
                                GameObject go = Instantiate(prefab, TeamsGrids[i]);
                                ArenaTeamEntity ply = go.GetComponent<ArenaTeamEntity>();
                                ply.PlyName.text = teams[i].players[j].playerName;
                                ply.PlyScore.text = teams[i].players[j].score.ToString();
                                ply.PlyKills.text = teams[i].players[j].kills.ToString();
                                ply.PlyDeaths.text = teams[i].players[j].deaths.ToString();
                                ply.PlyDamageTaken.text = teams[i].players[j].damageTaken.ToString();
                                ply.PlyDamageDealt.text = teams[i].players[j].damageDealt.ToString();
                                _teams[i].Add(ply);
                            }
                            else
                            {
                                ArenaTeamEntity ply = _teams[i][j];
                                ply.PlyName.text = teams[i].players[j].playerName;
                                ply.PlyScore.text = teams[i].players[j].score.ToString();
                                ply.PlyKills.text = teams[i].players[j].kills.ToString();
                                ply.PlyDeaths.text = teams[i].players[j].deaths.ToString();
                                ply.PlyDamageTaken.text = teams[i].players[j].damageTaken.ToString();
                                ply.PlyDamageDealt.text = teams[i].players[j].damageDealt.ToString();
                                _teams[i][j] = ply;
                            }
                        }

                    }
                }
                else
                {
                    Debug.LogError("UpdateDisplay teams = 0");
                }
            }
            else
            {
                Debug.LogError("UpdateDisplay teams null");
            }
        }

        public void Show()
        {

            showing = true;
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            AtavismUIUtility.BringToFront(gameObject);
         //   transform.position = new Vector3((Screen.width / 2) - GetComponent<RectTransform>().sizeDelta.x / 2 * GetComponent<RectTransform>().localScale.x, (Screen.height / 2) - GetComponent<RectTransform>().sizeDelta.y / 2 * GetComponent<RectTransform>().localScale.y, 0);

        }
        public void Hide()
        {
            showing = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}