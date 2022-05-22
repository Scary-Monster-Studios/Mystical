using EasyBuildSystem.Features.Scripts.Core;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System;
using System.Collections.Generic;
using System.Linq;
using Atavism;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EasyBuildSystem.Addons.CircularMenu.Scripts
{
    public class CircularMenu : MonoBehaviour
    {
        #region Fields

        public static CircularMenu Instance;

        [Serializable]
        public class UICustomCategory
        {
            public string Name;
            public GameObject Content;
            public List<CircularButtonData> Buttons = new List<CircularButtonData>();
            public List<CircularButton> InstancedButtons = new List<CircularButton>();
        }

        public enum ControllerType
        {
            KeyboardAndTouch,
            Gamepad
        }

        public ControllerType Controller = ControllerType.KeyboardAndTouch;

        public KeyCode OpenCircularKey = KeyCode.Tab;

        public int DefaultCategoryIndex;

        public List<UICustomCategory> Categories = new List<UICustomCategory>();

        public Image Selection;
        public Image SelectionIcon;
        public Sprite PreviousIcon;
        public Sprite NextIcon;
        public Sprite ReturnIcon;
        public Text SelectionText;
        public Text SelectionDescription;
        public Color ButtonNormalColor;
        public Color ButtonHoverColor;
        public Color ButtonPressedColor;

        public CircularButton CircularButton;
        public float ButtonSpacing = 160f;

        public string GamepadInputOpenName = "Open_Circular";
        public string GamepadInputAxisX = "Mouse X";
        public string GamepadInputAxisY = "Mouse Y";

        public Animator Animator;
        public string ShowStateName;
        public string HideStateName;

        [HideInInspector]
        public GameObject SelectedButton;

        [HideInInspector]
        public UICustomCategory CurrentCategory;

        public bool IsActive = false;

        private readonly List<float> ButtonsRotation = new List<float>();
        private int Elements;
        private float Fill;
        public int DisplayLimit = 10;
        private int page=0;
        #endregion

        #region Methods

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            UpdateList();
            AtavismEventSystem.RegisterEvent("CLAIM_CHANGED", this);

            ChangeCategory(Categories[0].Name);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CLAIM_CHANGED", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
          //  UpdateList();
        }

        private void UpdateList()
        {
         //   Debug.LogError("UpdateList");
               if (BuildManager.Instance != null)
            {
                if (BuildManager.Instance.Pieces != null)
                {
                  //  Debug.LogError("UpdateList | "+Categories[1].Buttons.Count);

                    Categories[1].Buttons.Clear();
                  //  Debug.LogError("UpdateList || "+Categories[1].Buttons.Count);
                  int index = 0;
                  int indexMax = 0;
                  for (int x = 0; x < BuildManager.Instance.Pieces.Count; x++)
                  {

                      if (WorldBuilder.Instance.ActiveClaim != null)
                          if (!WorldBuilder.Instance.GetBuildObjectTemplate(BuildManager.Instance.Pieces[x].BuildObjDefId).onlyAvailableFromItem &&
                              (WorldBuilder.Instance.ActiveClaim.claimType == WorldBuilder.Instance.GetAnyClaimType() ||
                               WorldBuilder.Instance.GetBuildObjectTemplate(BuildManager.Instance.Pieces[x].BuildObjDefId).validClaimTypes.Contains(WorldBuilder.Instance.GetAnyClaimType()) ||
                               WorldBuilder.Instance.GetBuildObjectTemplate(BuildManager.Instance.Pieces[x].BuildObjDefId).validClaimTypes.Contains(WorldBuilder.Instance.ActiveClaim.claimType)
                              ))
                          {
                              if (index < DisplayLimit && page * DisplayLimit < x)
                              {

                                  CircularButtonData cbd = new CircularButtonData()
                                  {
                                      Icon = BuildManager.Instance.Pieces[x].Icon,
                                      Order = index,
                                      Text = BuildManager.Instance.Pieces[x].Name,
                                      Description = BuildManager.Instance.Pieces[x].Description,
                                      Action = new UnityEvent()
                                  };
                                  string n = BuildManager.Instance.Pieces[x].Name;
                                  cbd.Action.AddListener(delegate { ChangePiece(n); });
                                  Categories[1].Buttons.Add(cbd);
                                  index++;
                              }

                              indexMax++;
                          }
                  }

                  if (page > 0)
                    {
                        CircularButtonData cbd_prew = new CircularButtonData()
                        {
                            Icon = PreviousIcon,
                            Order = BuildManager.Instance.Pieces.Count,
                            Text = "Prev",
                            Description = "Previous page",
                            Action = new UnityEvent()
                        };
                        cbd_prew.Action.AddListener(delegate { ChangePagePrev(); });
                        Categories[1].Buttons.Add(cbd_prew);
                    }
                    if (DisplayLimit == index && page * DisplayLimit + index < indexMax)
                    {
                        CircularButtonData cbd_next = new CircularButtonData()
                        {
                            Icon = NextIcon,
                            Order = BuildManager.Instance.Pieces.Count,
                            Text = "Next",
                            Description = "Next Page",
                            Action = new UnityEvent()
                        };
                        cbd_next.Action.AddListener(delegate { ChangePageNext(); });
                        Categories[1].Buttons.Add(cbd_next);
                    }

                    CircularButtonData cbd_return = new CircularButtonData()
                    {
                        Icon = ReturnIcon,
                        Order = BuildManager.Instance.Pieces.Count,
                        Text = "Back",
                        Description = "Return to menu",
                        Action = new UnityEvent()
                    };
                    cbd_return.Action.AddListener(delegate { ChangeCategory("Main"); }); 
                    Categories[1].Buttons.Add(cbd_return);
                 
                }
            }
             //  Debug.LogError("UpdateList ||| "+Categories[1].Buttons.Count+" "+ Categories[1].InstancedButtons.Count);

            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Content != null)
                {
                    foreach (CircularButton go in Categories[i].Content.GetComponentsInChildren<CircularButton>(true).ToList())
                    {
                            GameObject.DestroyImmediate(go.gameObject);
                    }
                    Categories[i].InstancedButtons.Clear();
                    Categories[i].Buttons = Categories[i].Buttons.OrderBy(o => o.Order).ToList();
                  // Debug.LogError("buttons "+i+" "+Categories[i].Buttons.Count+" "+Categories[i].InstancedButtons.Count);
                    for (int x = 0; x < Categories[i].Buttons.Count; x++)
                    {
                        CircularButton Button = Instantiate(CircularButton, Categories[i].Content.transform);
                        Button.Init(Categories[i].Buttons[x].Text, Categories[i].Buttons[x].Description, Categories[i].Buttons[x].Icon, Categories[i].Buttons[x].Action);
                        Categories[i].InstancedButtons.Add(Button);
                    }
                   // Debug.LogError("UpdateList |V "+Categories[1].Buttons.Count+" "+ Categories[1].InstancedButtons.Count);
                    Categories[i].InstancedButtons = Categories[i].Content.GetComponentsInChildren<CircularButton>(true).ToList();
                   // Debug.LogError("UpdateList V "+Categories[1].Buttons.Count+" "+ Categories[1].InstancedButtons.Count);

                }
            }
          //  Debug.LogError("UpdateList V| "+Categories[1].Buttons.Count+" "+ Categories[1].InstancedButtons.Count);

        }
        public void ChangePagePrev()
        {
            page--;
            UpdateList();
            RefreshButtons();
        }
        public void ChangePageNext()
        {
            page++;
            UpdateList();
            RefreshButtons();
        }
        
        
        
        private void Update()
        {
            if (!Application.isPlaying)
                return;
            if (WorldBuilder.Instance.BuildingState == WorldBuildingState.None)
                return;
            if (Input.GetMouseButtonDown(1))
                BuilderBehaviour.Instance.ChangeMode(BuildMode.None);

            if (Controller == ControllerType.KeyboardAndTouch ? Input.GetKeyDown(OpenCircularKey) : Input.GetButtonDown(GamepadInputOpenName))
                Show();
            else if (Controller == ControllerType.KeyboardAndTouch ? Input.GetKeyUp(OpenCircularKey) : Input.GetButtonUp(GamepadInputOpenName))
                Hide();

            if (!IsActive)
                return;

            Selection.fillAmount = Mathf.Lerp(Selection.fillAmount, Fill, .2f);

            Vector3 BoundsScreen = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
            Vector3 RelativeBounds = Input.mousePosition - BoundsScreen;

            float CurrentRotation = ((Controller == ControllerType.KeyboardAndTouch) ? Mathf.Atan2(RelativeBounds.x, RelativeBounds.y)
                : Mathf.Atan2(UnityEngine.Input.GetAxis(GamepadInputAxisX), UnityEngine.Input.GetAxis(GamepadInputAxisY))) * 57.29578f;

            if (CurrentRotation < 0f)
                CurrentRotation += 360f;

            _ = -(CurrentRotation - Selection.fillAmount * 360f / 2f);

            float Average = 9999;

            GameObject Nearest = null;

            for (int i = 0; i < Elements; i++)
            {
                GameObject InstancedButton = CurrentCategory.InstancedButtons[i].gameObject;
                InstancedButton.transform.localScale = Vector3.one;
                float Rotation = Convert.ToSingle(InstancedButton.name);

                if (Mathf.Abs(Rotation - CurrentRotation) < Average)
                {
                    Nearest = InstancedButton;
                    Average = Mathf.Abs(Rotation - CurrentRotation);
                }
            }

            SelectedButton = Nearest;
            float CursorRotation = -(Convert.ToSingle(SelectedButton.name) - Selection.fillAmount * 360f / 2f);
            Selection.transform.localRotation = Quaternion.Slerp(Selection.transform.localRotation, Quaternion.Euler(0, 0, CursorRotation), 15f * Time.deltaTime);

            for (int i = 0; i < Elements; i++)
            {
                CircularButton Button = CurrentCategory.InstancedButtons[i].GetComponent<CircularButton>();

                if (Button.gameObject != SelectedButton)
                    Button.Icon.color = Color.Lerp(Button.Icon.color, ButtonNormalColor, 15f * Time.deltaTime);
                else
                    Button.Icon.color = Color.Lerp(Button.Icon.color, ButtonHoverColor, 15f * Time.deltaTime);
            }

            SelectionIcon.sprite = SelectedButton.GetComponent<CircularButton>().Icon.sprite;
            SelectionText.text = SelectedButton.GetComponent<CircularButton>().Text;
            SelectionDescription.text = SelectedButton.GetComponent<CircularButton>().Description;

            if (Input.GetMouseButtonUp(0))
            {
                if (SelectedButton.GetComponent<CircularButton>().GetComponent<Animator>() != null)
                    SelectedButton.GetComponent<CircularButton>().GetComponent<Animator>().Play("Button Press");

                SelectedButton.GetComponent<CircularButton>().Action.Invoke();
            }
        }

        private void RefreshButtons()
        {
            Elements = CurrentCategory.InstancedButtons.Count;

            if (Elements > 0)
            {
                Fill = 1f / (float)Elements;

                float FillRadius = Fill * 360f;
                float LastRotation = 0;

                for (int i = 0; i < Elements; i++)
                {
                    GameObject Temp = CurrentCategory.InstancedButtons[i].gameObject;

                    float Rotate = LastRotation + FillRadius / 2;
                    LastRotation = Rotate + FillRadius / 2;

                    Temp.transform.localPosition = new Vector2(ButtonSpacing * Mathf.Cos((Rotate - 90) * Mathf.Deg2Rad), -ButtonSpacing * Mathf.Sin((Rotate - 90) * Mathf.Deg2Rad));
                    Temp.transform.localScale = Vector3.one;

                    if (Rotate > 360)
                        Rotate -= 360;

                    Temp.name = Rotate.ToString();

                    ButtonsRotation.Add(Rotate);
                }
            }
        }

        /// <summary>
        /// This method allows to change of category by name.
        /// </summary>
        public void ChangeCategory(string name)
        {
            DefaultCategoryIndex = Categories.ToList().FindIndex(entry => entry.Content.name == name);

            if (DefaultCategoryIndex == -1)
                return;

            CurrentCategory = Categories[DefaultCategoryIndex];

            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Content != null)
                {
                    if (i != DefaultCategoryIndex)
                        Categories[i].Content.SetActive(false);
                    else
                        Categories[i].Content.SetActive(true);
                }
            }

            RefreshButtons();
        }

        /// <summary>
        /// This method allows to upgrade the targeted piece.
        /// </summary>
        public void ChangeAppearance(int appearanceIndex)
        {
            PieceBehaviour TargetPiece = BuilderBehaviour.Instance.GetTargetedPart();

            if (TargetPiece != null)
                TargetPiece.ChangeAppearance(appearanceIndex);
        }

        /// <summary>
        /// This method allows to change mode.
        /// </summary>
        public void ChangeMode(string modeName)
        {
            BuilderBehaviour.Instance.ChangeMode(modeName);
        }

        /// <summary>
        /// This method allows to pass in placement mode with a piece name.
        /// </summary>
        public void ChangePiece(string name)
        {
            CircularMenu.Instance.Hide();

            //Call none to reset the previews.
            BuilderBehaviour.Instance.ChangeMode(BuildMode.None);

            BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.GetPieceByName(name));
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);
        }

        /// <summary>
        /// This method allows to show the circular menu.
        /// </summary>
        protected void Show()
        {
            Animator.CrossFade(ShowStateName, 0.1f);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            IsActive = true;
            UpdateList();
            RefreshButtons();
        }

        /// <summary>
        /// This method allows to close the circular menu.
        /// </summary>
        protected void Hide()
        {
            Animator.CrossFade(HideStateName, 0.1f);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            IsActive = false;
        }

        #endregion
    }
}