using UnityEngine;

#if WORLDAPI_PRESENT
using WAPI;

    /// <summary>
    /// Manage Dynamic Nature from WorldAPI - add a snow manager in the scene and drop this script onto it
    /// </summary>
    [RequireComponent(typeof(SnowManager))]
    [ExecuteInEditMode]
    public class DynamicNatureWorldAPIIntegration : MonoBehaviour, IWorldApiChangeHandler
    {
        public bool m_updateSnow = true;
        public bool m_updateSeasons = true;
        private SnowManager m_weatherManager;

    /// <summary>
    /// Set up connection to WAPI to Manage Dynamic Nature
    /// </summary>
    void Start () {
            m_weatherManager = GetComponent<SnowManager>();
            ConnectToWorldAPI();
            if (m_updateSnow)
            {
                m_weatherManager.snowValue = WorldManager.Instance.SnowPowerTerrain;
            }
            if (m_updateSeasons)
            {
                m_weatherManager.seasonValue = WorldManager.Instance.Season;
            }
	    }
	
        /// <summary>
        /// Connect to world api and start listening to updates
        /// </summary>
        void ConnectToWorldAPI()
        {
            WorldManager.Instance.AddListener(this);
        }

        /// <summary>
        /// Disconnect from world api
        /// </summary>
        void DisconnectFromWorldAPI()
        {
            WorldManager.Instance.RemoveListener(this);
        }

    /// <summary>
    /// On destroy object disconnetc from world API
    /// </summary>
    private void OnDestroy()
    {
        DisconnectFromWorldAPI();
    }

    /// <summary>
    /// Handle updates from world api
    /// </summary>
    /// <param name="changeArgs">Change arguements</param>
    public void OnWorldChanged(WorldChangeArgs changeArgs)
        {
            if (m_weatherManager == null)
            {
                m_weatherManager = GetComponent<SnowManager>();
            }
            if (m_updateSnow && changeArgs.HasChanged(WorldConstants.WorldChangeEvents.SnowChanged))
            {
                m_weatherManager.snowValue = WorldManager.Instance.SnowPowerTerrain*2f;
            }
          
            if (m_updateSeasons && changeArgs.HasChanged(WorldConstants.WorldChangeEvents.SeasonChanged))
            {
                m_weatherManager.seasonValue = WorldManager.Instance.Season;
            }
        }
    }

#endif