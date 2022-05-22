using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Group;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket.Data;
using EasyBuildSystem.Features.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Atavism;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Condition;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Builder
{
    [RequireComponent(typeof(Camera), typeof(BuilderDesktopInput))]
    [DefaultExecutionOrder(999)]
    [AddComponentMenu("Easy Build System/Features/Builders Behaviour/Builder Behaviour")]
    public class BuilderBehaviour : MonoBehaviour
    {
        #region Fields

        public static BuilderBehaviour Instance;

        public float ActionDistance = 6f;
        public float SnapThreshold = 5f;
        public float OutOfRangeDistance = 0f;

        public float OverlapAngles = 35f;
        public bool LockRotation;
        public DetectionType RayDetection = DetectionType.OverlapSphere;
        public RayType CameraType;
        public Vector3 RaycastOffset = new Vector3(0, 0, 1);
        public Transform RaycastOriginTransform;
        public Transform RaycastAnchorTransform;

        public MovementType PreviewMovementType;
        public bool PreviewMovementOnlyAllowed;
        public float PreviewGridSize = 1.0f;
        public float PreviewGridOffset;
        public float PreviewSmoothTime = 5.0f;

        public bool UsePlacementMode = true;
        public bool ResetModeAfterPlacement = false;
        public bool UseDestructionMode = true;
        public bool ResetModeAfterDestruction = false;
        public bool UseEditionMode = true;
        public bool ResetModeAfterEdition = false;

        public AudioSource Source;
        public AudioClip[] PlacementClips;
        public AudioClip[] DestructionClips;
        public AudioClip[] EditionClips;

        public virtual Ray GetRay
        {
            get
            {
                if (CameraType == RayType.TopDown)
                {
              //      Debug.LogError("GetRey: TopDown mousePosition"+UnityEngine.Input.mousePosition+" RaycastOffset="+RaycastOffset);
                    return BuilderCamera.ScreenPointToRay(UnityEngine.Input.mousePosition + RaycastOffset);
                }
                else if (CameraType == RayType.FirstPerson)
                {
                    return new Ray(BuilderCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f) + RaycastOffset), BuilderCamera.transform.forward);
                }
                else if (CameraType == RayType.ThirdPerson)
                {
                    if (RaycastOriginTransform != null && BuilderCamera != null)
                    {
                        return new Ray(RaycastOriginTransform.position + RaycastOriginTransform.TransformDirection(RaycastOffset), BuilderCamera.transform.forward);
                    }
                }

                return new Ray();
            }
        }

        private Transform _Caster;
        public virtual Transform GetCaster
        {
            get
            {
                if (_Caster == null)
                {
                  /*  _Caster = CameraType != RayType.TopDown ? transform.parent != null ? transform.parent
                    : transform : RaycastAnchorTransform != null ? RaycastAnchorTransform : transform;*/
                    _Caster = ClientAPI.GetPlayerObject().GameObject.transform;
                }

                return _Caster;
            }
        }

        public BuildMode CurrentMode { get; set; }
        public BuildMode LastMode { get; set; }

        public PieceBehaviour SelectedPrefab { get; set; }

        public PieceBehaviour CurrentPreview { get; set; }
        public PieceBehaviour CurrentEditionPreview { get; set; }
        public PieceBehaviour CurrentRemovePreview { get; set; }

        public Vector3 CurrentRotationOffset { get; set; }

        public Vector3 InitialScale { get; set; }

        public bool AllowPlacement { get; set; }
        public bool AllowDestruction { get; set; }
        public bool AllowEdition { get; set; }

        public bool HasSocket { get; set; }

        public SocketBehaviour CurrentSocket { get; set; }
        public SocketBehaviour LastSocket { get; set; }

        private Camera BuilderCamera;

        private RaycastHit TopDownHit;
        private Vector3 LastAllowedPosition;

        private readonly RaycastHit[] Hits = new RaycastHit[PhysicExtension.MAX_ALLOC_COUNT];

        #endregion Fields

        #region Methods

        public virtual void Awake()
        {
            Instance = this;
        }

        public virtual void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            BuilderCamera = GetComponent<Camera>();

            if (BuilderCamera == null)
            {
                Debug.LogWarning("<b>Easy Build System</b> : The Builder Behaviour require a camera!");
            }
        }

        public virtual void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            UpdateModes();
        }

        #region Placement

        /// <summary>
        /// This method allows to update the placement preview.
        /// </summary>
        public void UpdatePreview()
        {
            HasSocket = false;

            if (CameraType == RayType.TopDown)
            {
                Physics.Raycast(GetRay, out TopDownHit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
            }
          //  Debug.LogWarning("!!!!!!!!!!!! UpdatePreview RayDetection="+RayDetection+" TopDownHit="+TopDownHit.point);

            if (RayDetection == DetectionType.Vector)
            {
                PieceBehaviour[] NeighboursParts =
                    BuildManager.Instance.GetAllNearestPiece(BuilderCamera.transform.TransformPoint(Vector3.forward * ActionDistance), ActionDistance);
            //    Debug.LogWarning("!!!!!!!!!!!! UpdatePreview DetectionType.Vector NeighboursParts count" + NeighboursParts.Length);

                PieceBehaviour[] ApplicableParts = new PieceBehaviour[NeighboursParts.Length];

                for (int i = 0; i < NeighboursParts.Length; i++)
                {
                    if (NeighboursParts[i].Sockets == null)
                    {
                        continue;
                    }

                    foreach (SocketBehaviour Socket in NeighboursParts[i].Sockets)
                    {
                        if (NeighboursParts[i].gameObject.activeInHierarchy && !Socket.IsDisabled && Socket.AllowPiece(CurrentPreview))
                        {
                            ApplicableParts[i] = NeighboursParts[i];
                            break;
                        }
                    }
                }

                if (ApplicableParts.Length > 0)
                {
                    UpdateMultipleSocket(ApplicableParts);
                }
                else
                {
                    UpdateFreeMovement();
                }
            }
            else if (RayDetection == DetectionType.Raycast)
            {
                SocketBehaviour Socket = null;

                int ColliderCount = Physics.RaycastNonAlloc(GetRay, Hits, ActionDistance, LayerMask.GetMask(Constants.LAYER_SOCKET));
               // Debug.LogWarning("!!!!!!!!!!!! UpdatePreview DetectionType.Raycast ColliderCount=" + ColliderCount);

                for (int i = 0; i < ColliderCount; i++)
                {
                    if (Hits[i].collider.GetComponent<SocketBehaviour>() != null)
                    {
                        Socket = Hits[i].collider.GetComponent<SocketBehaviour>();
                    }
                }

                if (Socket != null)
                {
                    UpdateSingleSocket(Socket);
                }
                else
                {
                    UpdateFreeMovement();
                }
            }
            else if (RayDetection == DetectionType.OverlapSphere)
            {
                PieceBehaviour[] NeighboursParts =
                    PhysicExtension.GetNeighborsTypeBySphere<PieceBehaviour>(GetCaster.position, ActionDistance, LayerMask.GetMask(Constants.LAYER_SOCKET));
               // Debug.LogWarning("!!!!!!!!!!!! UpdatePreview DetectionType.OverlapSphere NeighboursParts count " + NeighboursParts.Length);
                PieceBehaviour[] ApplicableParts = new PieceBehaviour[NeighboursParts.Length];

                for (int i = 0; i < NeighboursParts.Length; i++)
                {
                   // Debug.LogWarning("!!!!!!!!!!!! UpdatePreview Sockets=" + NeighboursParts[i].Sockets);
                    if (NeighboursParts[i].Sockets == null)
                    {
                        continue;
                    }

                    foreach (SocketBehaviour Socket in NeighboursParts[i].Sockets)
                    {
                      //  Debug.LogWarning("!!!!!!!!!!!! UpdatePreview IsDisabled=" + Socket.IsDisabled +" AllowPart="+Socket.AllowPiece(CurrentPreview));

                      if (NeighboursParts[i].gameObject.activeInHierarchy && !Socket.IsDisabled && Socket.AllowPiece(CurrentPreview))
                        {
                            ApplicableParts[i] = NeighboursParts[i];
                            break;
                        }
                    }
                }

              //  Debug.LogWarning("!!!!!!!!!!!! UpdatePreview ApplicableParts=" + ApplicableParts.Length);
                if (ApplicableParts.Length > 0)
                {
                    UpdateMultipleSocket(ApplicableParts);
                }
                else
                {
                    UpdateFreeMovement();
                }
            }

           // Debug.LogError("UpdatePreview CheckPlacementConditions="+CurrentPreview.CheckPlacementConditions()+" CheckInternalConditions="+CheckInternalConditions());
            AllowPlacement = CurrentPreview.CheckPlacementConditions() && CheckInternalConditions();
           // Debug.LogError("UpdatePreview AllowPlacement=" + AllowPlacement);
            CurrentPreview.gameObject.ChangeAllMaterialsColorInChildren(CurrentPreview.Renderers.ToArray(),
                AllowPlacement ? CurrentPreview.PreviewAllowedColor : CurrentPreview.PreviewDeniedColor, SelectedPrefab.PreviewColorLerpTime, SelectedPrefab.PreviewUseColorLerpTime);
        }

        /// <summary>
        /// This method allows to check the preview condition (build surface, require socket, build distance).
        /// </summary>
        public bool CheckInternalConditions()
        {
           // Debug.LogError("CheckInternalConditions="+CurrentPreview.RequireSocket+" HasSocket="+HasSocket+" OutOfRangeDistance="+OutOfRangeDistance+" dist="+Vector3.Distance(GetCaster.position, CurrentPreview.transform.position)+" ActionDistance="+ActionDistance);
            if (CurrentPreview.RequireSocket && !HasSocket)
            {
                return false;
            }

            if (OutOfRangeDistance != 0)
            {
                return Vector3.Distance(GetCaster.position, CurrentPreview.transform.position) < ActionDistance;
            }

            return true;
        }

        /// <summary>
        /// This method allows to rotate the current preview.
        /// </summary>
        public void RotatePreview(Vector3 rotateAxis)
        {
            if (CurrentPreview == null)
            {
                return;
            }

            CurrentRotationOffset += rotateAxis;
        }

        /// <summary>
        /// This method allows to move the preview in free movement.
        /// </summary>
        public void UpdateFreeMovement()
        {
            if (CurrentPreview == null)
            {
                return;
            }

            float Distance = OutOfRangeDistance == 0 ? ActionDistance : OutOfRangeDistance;

            if (Physics.Raycast(GetRay, out RaycastHit Hit, Distance, BuildManager.Instance.FreeLayers))
            {
                if (PreviewMovementType == MovementType.Normal)
                {
                    if (PreviewMovementOnlyAllowed)
                    {
                        CurrentPreview.transform.position = Hit.point + CurrentPreview.PreviewOffset;

                        if (CurrentPreview.CheckPlacementConditions())
                        {
                            LastAllowedPosition = Hit.point + CurrentPreview.PreviewOffset;
                        }
                        else
                        {
                            CurrentPreview.transform.position = LastAllowedPosition;
                        }
                    }
                    else
                    {
                        CurrentPreview.transform.position = Hit.point + CurrentPreview.PreviewOffset;
                    }
                }
                else if (PreviewMovementType == MovementType.Grid)
                {
                    CurrentPreview.transform.position = MathExtension.PositionToGridPosition(PreviewGridSize, PreviewGridOffset, Hit.point + CurrentPreview.PreviewOffset);
                }
                else if (PreviewMovementType == MovementType.Smooth)
                {
                    CurrentPreview.transform.position = Vector3.Lerp(CurrentPreview.transform.position, Hit.point + CurrentPreview.PreviewOffset, PreviewSmoothTime * Time.deltaTime);
                }

                if (!CurrentPreview.RotateAccordingSlope)
                {
                    if (LockRotation)
                    {
                        CurrentPreview.transform.rotation = GetCaster.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentRotationOffset);
                    }
                    else
                    {
                        CurrentPreview.transform.rotation = Quaternion.Euler(CurrentRotationOffset);
                    }
                }
                else
                {
                    if (Hit.collider is TerrainCollider)
                    {
                        float SampleHeight = Hit.collider.GetComponent<UnityEngine.Terrain>().SampleHeight(Hit.point);

                        if (Hit.point.y - .1f < SampleHeight)
                        {
                            CurrentPreview.transform.rotation = Quaternion.FromToRotation(GetCaster.up, Hit.normal) * Quaternion.Euler(CurrentRotationOffset) *
                                                                GetCaster.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentRotationOffset);
                        }
                        else
                        {
                            CurrentPreview.transform.rotation = GetCaster.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentRotationOffset);
                        }
                    }
                    else
                    {
                        CurrentPreview.transform.rotation = GetCaster.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentRotationOffset);
                    }
                }

                return;
            }

            if (LockRotation)
            {
                CurrentPreview.transform.rotation = GetCaster.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentRotationOffset);
            }
            else
            {
                CurrentPreview.transform.rotation = Quaternion.Euler(CurrentRotationOffset);
            }

            Transform StartTransform = (CurrentPreview.GroundUpperHeight == 0) ? GetCaster : BuilderCamera.transform;

            Vector3 LookDistance = StartTransform.position + StartTransform.forward * Distance;

            if (CurrentPreview.UseGroundUpper)
            {
                LookDistance.y = Mathf.Clamp(LookDistance.y, GetCaster.position.y - CurrentPreview.GroundUpperHeight,
                    GetCaster.position.y + CurrentPreview.GroundUpperHeight);
            }
            else
            {
                if (!CurrentPreview.FreePlacement)
                {
                    if (Physics.Raycast(CurrentPreview.transform.position + new Vector3(0, 0.3f, 0),
                        Vector3.down, out RaycastHit HitLook, Mathf.Infinity, BuildManager.Instance.FreeLayers, QueryTriggerInteraction.Ignore))
                    {
                        LookDistance.y = HitLook.point.y;
                    }
                }
                else
                {
                    LookDistance.y = Mathf.Clamp(LookDistance.y, GetCaster.position.y,
                        GetCaster.position.y);
                }
            }

            if (PreviewMovementType == MovementType.Normal)
            {
                CurrentPreview.transform.position = LookDistance;
            }
            else if (PreviewMovementType == MovementType.Grid)
            {
                CurrentPreview.transform.position = MathExtension.PositionToGridPosition(PreviewGridSize, PreviewGridOffset, LookDistance + CurrentPreview.PreviewOffset);
            }
            else if (PreviewMovementType == MovementType.Smooth)
            {
                CurrentPreview.transform.position = Vector3.Lerp(CurrentPreview.transform.position, LookDistance, PreviewSmoothTime * Time.deltaTime);
            }

            CurrentSocket = null;

            LastSocket = null;

            HasSocket = false;
        }

        /// <summary>
        /// This method allows to move the preview only on available socket(s).
        /// </summary>
        public void UpdateMultipleSocket(PieceBehaviour[] pieces)
        {
           // Debug.LogError("UpdateMultipleSocket");

            if (CurrentPreview == null || pieces == null)
            {
                return;
            }

            float ClosestAngle = Mathf.Infinity;
            
            CurrentSocket = null;

            foreach (PieceBehaviour Piece in pieces)
            {
                if (Piece == null || Piece.Sockets.Length == 0)
                {
                   // Debug.LogError("Piece is null");
                    continue;
                }
             //   Debug.LogError("UpdateMultipleSocket Socket count "+Piece.Sockets.Length);

                for (int x = 0; x < Piece.Sockets.Length; x++)
                {
                    
                    SocketBehaviour Socket = Piece.Sockets[x];
                    if (Socket != null)
                    {
                      //  Debug.LogError("UpdateMultipleSocket Socket activeSelf="+Socket.gameObject.activeSelf+" IsDisabled="+Socket.IsDisabled);
                        if (Socket.gameObject.activeSelf && !Socket.IsDisabled)
                        {
                            if (!Socket.CheckOccupancy(SelectedPrefab) && Socket.AllowPiece(CurrentPreview) && !Piece.IgnoreSocket)
                            {
                                /*Debug.LogError("UpdateMultipleSocket Socket CameraType="+CameraType+" "+
                                               (Socket.transform.position - (CameraType != RayType.TopDown ? GetCaster.position : TopDownHit.point)).sqrMagnitude+" < "+Mathf.Pow(CameraType != RayType.TopDown ? ActionDistance : SnapThreshold, 2)+
                                    " Soc loc= "+Socket.transform.position+" TopDown loc="+TopDownHit.point+"  GetCaster.position="+ GetCaster.position, Socket.gameObject);*/

                                if ((Socket.transform.position - (CameraType != RayType.TopDown ? GetCaster.position : TopDownHit.point)).sqrMagnitude <
                                    Mathf.Pow(CameraType != RayType.TopDown ? ActionDistance : SnapThreshold, 2))
                                {
                                    float Angle = Vector3.Angle(GetRay.direction, Socket.transform.position - GetRay.origin);
                                  //  Debug.LogError("UpdateMultipleSocket Socket Angle=" + Angle);
                                    if (Angle < ClosestAngle && Angle < OverlapAngles)
                                    {
                                        ClosestAngle = Angle;

                                        if (CameraType != RayType.TopDown && CurrentSocket == null)
                                        {
                                            CurrentSocket = Socket;
                                        }
                                        else
                                        {
                                            CurrentSocket = Socket;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        //    Debug.LogError("UpdateMultipleSocket CurrentSocket=" + CurrentSocket);
            if (CurrentSocket != null)
            {
                Offset OffsetPiece = CurrentSocket.GetOffset(CurrentPreview);

                if (CurrentSocket.CheckOccupancy(CurrentPreview))
                {
                    return;
                }

                if (OffsetPiece != null)
                {
                    CurrentPreview.transform.position = CurrentSocket.transform.position + CurrentSocket.transform.TransformVector(OffsetPiece.Position);

                    CurrentPreview.transform.rotation = CurrentSocket.transform.rotation *
                        (CurrentPreview.RotateOnSockets ? Quaternion.Euler(OffsetPiece.Rotation + CurrentRotationOffset) : Quaternion.Euler(OffsetPiece.Rotation));

                    if (OffsetPiece.Scale != Vector3.one)
                    {
                        CurrentPreview.transform.localScale = OffsetPiece.Scale;
                    }
                    else
                    {
                        CurrentPreview.transform.localScale = CurrentSocket.transform.parent.localScale;
                    }

                    LastSocket = CurrentSocket;

                    HasSocket = true;

                    return;
                }
            }
            else
                CurrentPreview.transform.localScale = InitialScale;

            UpdateFreeMovement();
        }

        /// <summary>
        /// This method allows to move the preview only on available socket.
        /// </summary>
        public void UpdateSingleSocket(SocketBehaviour socket)
        {
            if (CurrentPreview == null || socket == null)
            {
                return;
            }

            CurrentSocket = null;

            if (socket != null)
            {
                if (socket.gameObject.activeSelf && !socket.IsDisabled)
                {
                    if (socket.AllowPiece(CurrentPreview) && !CurrentPreview.IgnoreSocket)
                    {
                        CurrentSocket = socket;
                    }
                }
            }

            if (CurrentSocket != null)
            {
                Offset Offset = CurrentSocket.GetOffset(CurrentPreview);

                if (CurrentSocket.CheckOccupancy(CurrentPreview))
                {
                    return;
                }

                if (Offset != null)
                {
                    CurrentPreview.transform.position = CurrentSocket.transform.position + CurrentSocket.transform.TransformVector(Offset.Position);

                    CurrentPreview.transform.rotation = CurrentSocket.transform.rotation *
                        (CurrentPreview.RotateOnSockets ? Quaternion.Euler(Offset.Rotation + CurrentRotationOffset) : Quaternion.Euler(Offset.Rotation));

                    if (Offset.Scale != Vector3.one)
                    {
                        CurrentPreview.transform.localScale = Offset.Scale;
                    }

                    LastSocket = CurrentSocket;

                    HasSocket = true;

                    return;
                }
            }

            UpdateFreeMovement();
        }

        /// <summary>
        /// This method allows to place the current preview.
        /// </summary>
        public virtual void PlacePrefab(GroupBehaviour group = null)
        {
    //        Debug.LogError("PlacePrefab AllowPlacement="+AllowPlacement+" CurrentPreview="+CurrentPreview+" CurrentEditionPreview="+CurrentEditionPreview);
            if (!AllowPlacement)
            {
                return;
            }

            if (CurrentPreview == null)
            {
                return;
            }

            if (CurrentEditionPreview != null)
            {
              //  Debug.LogError("Destroy");
             // Destroy(CurrentEditionPreview.gameObject);
            }

           /*             BuildManager.Instance.PlacePrefab(SelectedPrefab,
                CurrentPreview.transform.position,
                CurrentPreview.transform.eulerAngles,
                CurrentPreview.transform.localScale,
                group,
                CurrentSocket);*/
      //     Debug.LogError("PlacePrefab CurrentPreview.CurrentState="+CurrentPreview.CurrentState+" LastMode="+LastMode);
      //     Debug.LogError("PlacePrefab WorldBuilder.Instance.BuildingState="+WorldBuilder.Instance.BuildingState+" CurrentMode="+CurrentMode);
            if(WorldBuilder.Instance.BuildingState!= WorldBuildingState.None)
            if (LastMode == BuildMode.Edition)
            {
             
              WorldBuilder.Instance.SendEditObjectPosition(WorldBuilder.Instance.SelectedObject, CurrentPreview.transform,-1);
              UGUIWorldBuilder.Instance.SaveObjectChanges();

            } else if (CurrentMode == BuildMode.Placement)
            {
                int parentId = -1;
                string parents="";
                List<int> ids = new List<int>();
               
                foreach (var con in CurrentPreview.Conditions)
                {
                    if(con.GetType() == typeof(InternalPhysicsCondition))
                    {
                        InternalPhysicsCondition ipc = (InternalPhysicsCondition) con;
                       // bool[] Results = new bool[Detections.Length];
                      // Debug.LogError("PlacePrefab Detections=" + ipc.Detections.Length);
                        for (int i = 0; i < ipc.Detections.Length; i++)
                        {
                           // Debug.LogError("PlacePrefab Detection=" + ipc.Detections[i]);
                            if (ipc.Detections[i] != null)
                            {
                                PieceBehaviour[] Pieces = PhysicExtension.GetNeighborsTypeByBox<PieceBehaviour>(ipc.transform.TransformPoint(ipc.Detections[i].DetectionBounds.center),
                                    ipc.Detections[i].DetectionBounds.extents, ipc.transform.rotation, ipc.Detections[i].RequireLayer);

                               // Debug.LogError("PlacePrefab Pieces=" + Pieces.Length);
                                for (int p = 0; p < Pieces.Length; p++)
                                {
                                    PieceBehaviour CollapsePiece = Pieces[p].GetComponent<PieceBehaviour>();
                                   // Debug.LogError("PlacePrefab CollapsePiece=" + CollapsePiece);
                                    if (CollapsePiece != null)
                                    {
                                        if (CollapsePiece != CurrentPreview)
                                        {
                                            if (CollapsePiece.CurrentState != StateType.Queue && ipc.Detections[i].CheckType(CollapsePiece.Category))
                                            {
                                              //  Debug.LogError("PlacePrefab || CollapsePiece=" + CollapsePiece);

                                               // Results[i] = true;
                                               ClaimObject co = CollapsePiece.GetComponent<ClaimObject>();
                                               if (co != null)
                                               {
                                                   if (parentId > 0)
                                                   {
                                                       if(!ids.Contains(parentId))
                                                           ids.Add(parentId);
                                                       ids.Add(co.ID);
                                                   }
                                                   else
                                                   {
                                                       parentId = co.ID;
                                                   }

                                                 //  Debug.LogError("PlacePrefab parent=" + co.ID);
                                               }

                                            }
                                        }
                                    }
                                }

                                Collider[] Colliders = PhysicExtension.GetNeighborsTypeByBox<Collider>(transform.TransformPoint(ipc.Detections[i].DetectionBounds.center),
                                    ipc.Detections[i].DetectionBounds.extents, transform.rotation, ipc.Detections[i].RequireLayer);

                                for (int x = 0; x < Colliders.Length; x++)
                                {
                                    if (ipc.Detections[i].RequireSupport)
                                    {
                                      //  Debug.LogError("PlacePrefab Colliders=" + Colliders[x]);
                                        if (BuildManager.Instance.IsSupport(Colliders[x]))
                                        {
                                           // Results[i] = true;
                                           ClaimObject co = Colliders[x].GetComponent<ClaimObject>();
                                           if (co != null)
                                           {
                                               if (parentId > 0)
                                               {
                                                   if(!ids.Contains(parentId))
                                                       ids.Add(parentId);
                                                   ids.Add(co.ID);
                                               }
                                               else
                                               {
                                                   parentId = co.ID;
                                               }
                                             //  Debug.LogError("PlacePrefab Col parent=" + co.ID);
                                           }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (ids.Count > 0)
                    parentId = -1;
                parents = string.Join(";", ids.ConvertAll(i => i.ToString()).ToArray());
              //  Debug.LogError("PlacePrefab parentId="+parentId+" parents="+parents +" "+CurrentPreview.BuildObjDefId);
                
                Atavism.WorldBuilder.Instance.SendPlaceClaimObject(CurrentPreview.BuildObjDefId, null, CurrentPreview.transform, parentId,parents);
            }

            if (Source != null)
            {
                if (PlacementClips.Length != 0)
                {
                    Source.PlayOneShot(PlacementClips[UnityEngine.Random.Range(0, PlacementClips.Length)]);
                }
            }

            CurrentRotationOffset = Vector3.zero;
            CurrentSocket = null;
            LastSocket = null;
            AllowPlacement = false;
            HasSocket = false;

            if (LastMode == BuildMode.Edition && ResetModeAfterEdition)
            {
                ChangeMode(BuildMode.None);
            }

            if (CurrentMode == BuildMode.Placement && ResetModeAfterPlacement)
            {
                ChangeMode(BuildMode.None);
            }

            if (CurrentPreview != null)
            {
                Destroy(CurrentPreview.gameObject);
            }
        }

        /// <summary>
        /// This method allows to create a preview.
        /// </summary>
        public virtual PieceBehaviour CreatePreview(GameObject prefab)
        {
            if (prefab == null)
            {
                return null;
            }
            PreviewMovementType = MovementType.Grid;

            CurrentPreview = Instantiate(prefab).GetComponent<PieceBehaviour>();
            CurrentPreview.transform.eulerAngles = Vector3.zero;
            CurrentRotationOffset = Vector3.zero;
            if(!CurrentPreview.gameObject.activeSelf)
                CurrentPreview.gameObject.SetActive(true);
            InitialScale = CurrentPreview.transform.localScale;

            if (Physics.Raycast(GetRay, out RaycastHit Hit, Mathf.Infinity, BuildManager.Instance.FreeLayers, QueryTriggerInteraction.Ignore))
            {
                CurrentPreview.transform.position = Hit.point;
            }

            CurrentPreview.ChangeState(StateType.Preview);

            SelectedPrefab = prefab.GetComponent<PieceBehaviour>();

            BuildEvent.Instance.OnPieceInstantiated.Invoke(CurrentPreview, null);

            CurrentSocket = null;

            LastSocket = null;

            AllowPlacement = false;

            HasSocket = false;

            return CurrentPreview;
        }

        /// <summary>
        /// This method allows to clear the current preview.
        /// </summary>
        public virtual void ClearPreview()
        {
            if (CurrentPreview == null)
            {
                return;
            }
            PreviewMovementType = MovementType.Normal;

            BuildEvent.Instance.OnPieceDestroyed.Invoke(CurrentPreview);

            Destroy(CurrentPreview.gameObject);

            AllowPlacement = false;

            CurrentPreview = null;
        }

        /// <summary>
        /// This method allows to get the piece that the camera is currently looking at.
        /// </summary>
        public PieceBehaviour GetTargetedPart()
        {
            if (Physics.SphereCast(CameraType == RayType.FirstPerson ? new Ray(BuilderCamera.transform.position, BuilderCamera.transform.forward) : GetRay,
                .1f, out RaycastHit Hit, ActionDistance, Physics.AllLayers))
            {
                PieceBehaviour Part = Hit.collider.GetComponentInParent<PieceBehaviour>();

                if (Part != null)
                    return Part;
            }

            return null;
        }

        #endregion Placement

        #region Destruction

        /// <summary>
        /// This method allows to update the destruction preview.
        /// </summary>
        public void UpdateRemovePreview()
        {
            float Distance = OutOfRangeDistance == 0 ? ActionDistance : OutOfRangeDistance;

            if (CurrentRemovePreview != null)
            {
                CurrentRemovePreview.ChangeState(StateType.Remove);
                foreach (var socket in CurrentRemovePreview.Sockets)
                {
                  //socket.ParentPiece.
                  
                }

               
              //  Debug.LogError("UpdateRemovePreview");
                AllowPlacement = false;
            }

            if (Physics.Raycast(GetRay, out RaycastHit Hit, Distance, BuildManager.Instance.FreeLayers))
            {
                PieceBehaviour Part = Hit.collider.GetComponentInParent<PieceBehaviour>();

                if (Part != null)
                {
                    if (CurrentRemovePreview != null)
                    {
                        if (CurrentRemovePreview.GetInstanceID() != Part.GetInstanceID())
                        {
                            ClearRemovePreview();

                            CurrentRemovePreview = Part;
                        }
                    }
                    else
                    {
                        CurrentRemovePreview = Part;
                    }
                }
                else
                {
                    ClearRemovePreview();
                }
            }
            else
            {
                ClearRemovePreview();
            }
        }

        /// <summary>
        /// This method allows to remove the current preview.
        /// </summary>
        public virtual void DestroyPrefab()
        {
            if (CurrentRemovePreview == null)
            {
                return;
            }

            if (!CurrentRemovePreview.IsDestructible)
            {
                return;
            }

            AllowDestruction = CurrentRemovePreview.CheckDestructionConditions();

            if (!AllowDestruction)
            {
                return;
            }
            if(WorldBuilder.Instance.BuildingState!= WorldBuildingState.None)
                 WorldBuilder.Instance.PickupClaimObject(CurrentRemovePreview.GetComponent<ClaimObject>().ID);
           // Destroy(CurrentRemovePreview.gameObject);
            
         
            if (Source != null)
            {
                if (DestructionClips.Length != 0)
                {
                    Source.PlayOneShot(DestructionClips[UnityEngine.Random.Range(0, DestructionClips.Length)]);
                }
            }

            CurrentSocket = null;

            LastSocket = null;

            AllowDestruction = false;

            HasSocket = false;

            if (ResetModeAfterDestruction)
            {
                ChangeMode(BuildMode.None);
            }
        }

        /// <summary>
        /// This method allows to clear the current remove preview.
        /// </summary>
        public virtual void ClearRemovePreview()
        {
            if (CurrentRemovePreview == null)
            {
                return;
            }

            CurrentRemovePreview.ChangeState(CurrentRemovePreview.LastState);
          //  CurrentRemovePreview.ChangeState(StateType.Queue);

            AllowDestruction = false;

            CurrentRemovePreview = null;
        }

        #endregion Destruction

        #region Edition

        /// <summary>
        /// This method allows to update the edition mode.
        /// </summary>
        public void UpdateEditionPreview()
        {
            AllowEdition = CurrentEditionPreview;

            if (CurrentEditionPreview != null && AllowEdition)
            {
                CurrentEditionPreview.ChangeState(StateType.Edit);
            }

            float Distance = OutOfRangeDistance == 0 ? ActionDistance : OutOfRangeDistance;

            if (Physics.Raycast(GetRay, out RaycastHit Hit, Distance, BuildManager.Instance.FreeLayers))
            {
                PieceBehaviour Part = Hit.collider.GetComponentInParent<PieceBehaviour>();

                if (Part != null)
                {
                    if (CurrentEditionPreview != null)
                    {
                        if (CurrentEditionPreview.GetInstanceID() != Part.GetInstanceID())
                        {
                            ClearEditionPreview();

                            CurrentEditionPreview = Part;
                        }
                    }
                    else
                    {
                        CurrentEditionPreview = Part;
                    }
                }
                else
                {
                    ClearEditionPreview();
                }
            }
            else
            {
                ClearEditionPreview();
            }
        }

        /// <summary>
        /// This method allows to edit the current preview.
        /// </summary>
        public virtual void EditPrefab()
        {
            if (CurrentEditionPreview == null) return;
            
            if (!CurrentEditionPreview.IsEditable)
            {
                return;
            }

            AllowEdition = CurrentEditionPreview.CheckEditionConditions();

            if (!AllowEdition)
            {
                return;
            }

            PieceBehaviour Part = CurrentEditionPreview;

            Part.ChangeState(StateType.Edit);
            var co = CurrentEditionPreview.GetComponent<ClaimObject>();
            WorldBuilder.Instance.SelectedObject = co;
            SelectPrefab(Part);
            if(SelectedPrefab!=null)
                SelectedPrefab.AppearanceIndex = Part.AppearanceIndex;

            ChangeMode(BuildMode.Placement);
        }

        /// <summary>
        /// This method allows to clear the current edition preview.
        /// </summary>
        public void ClearEditionPreview()
        {
            if (CurrentEditionPreview == null)
            {
                return;
            }

            CurrentEditionPreview.ChangeState(CurrentEditionPreview.LastState);
          //  CurrentRemovePreview.ChangeState(StateType.Queue);

            AllowEdition = false;

            CurrentEditionPreview = null;
        }

        #endregion Edition

        /// <summary>
        /// This method allows to update all the builder (Placement, Destruction, Edition).
        /// </summary>
        public virtual void UpdateModes()
        {
         //   Debug.LogWarning("UpdateModes CurrentMode="+CurrentMode+" SelectedPrefab="+SelectedPrefab+" CurrentPreview="+CurrentPreview+" BuildManager="+BuildManager.Instance+" Pices="+(BuildManager.Instance!=null?BuildManager.Instance.Pieces.ToString():"NA"));
            if (BuildManager.Instance == null)
            {
                return;
            }

            if (BuildManager.Instance.Pieces == null)
            {
                return;
            }

            if (CurrentMode == BuildMode.Placement)
            {
               // Debug.LogWarning("UpdateModes SelectedPrefab="+SelectedPrefab+" CurrentPreview="+CurrentPreview);
                if (SelectedPrefab == null)
                {
                    return;
                }

                if (CurrentPreview == null)
                {
                    CreatePreview(SelectedPrefab.gameObject);
                    return;
                }
                else
                {
                    UpdatePreview();
                }
            }
            else if (CurrentMode == BuildMode.Destruction)
            {
                UpdateRemovePreview();
            }
            else if (CurrentMode == BuildMode.Edition)
            {
                UpdateEditionPreview();
            }
            else if (CurrentMode == BuildMode.None)
            {
                ClearPreview();
            }
        }

        /// <summary>
        /// This method allows to change mode.
        /// </summary>
        public void ChangeMode(BuildMode mode)
        {
            if (CurrentMode == mode)
            {
                return;
            }

            if (mode == BuildMode.Placement && !UsePlacementMode)
            {
                return;
            }

            if (mode == BuildMode.Destruction && !UseDestructionMode)
            {
                return;
            }

            if (mode == BuildMode.Edition && !UseEditionMode)
            {
                return;
            }

            if (CurrentMode == BuildMode.Placement)
            {
                ClearPreview();
            }

            if (CurrentMode == BuildMode.Destruction)
            {
                ClearRemovePreview();
            }

            if (mode == BuildMode.None)
            {
                ClearPreview();
                ClearRemovePreview();
                ClearEditionPreview();
            }

            LastMode = CurrentMode;

            CurrentMode = mode;

            BuildEvent.Instance.OnChangedBuildMode.Invoke(CurrentMode);
        }

        /// <summary>
        /// This method allows to change mode.
        /// </summary>
        public void ChangeMode(string modeName)
        {
            if (CurrentMode.ToString() == modeName)
            {
                return;
            }

            if (modeName == BuildMode.Placement.ToString() && !UsePlacementMode)
            {
                return;
            }

            if (modeName == BuildMode.Destruction.ToString() && !UseDestructionMode)
            {
                return;
            }

            if (modeName == BuildMode.Edition.ToString() && !UseEditionMode)
            {
                return;
            }

            if (CurrentMode == BuildMode.Placement)
            {
                ClearPreview();
            }

            if (CurrentMode == BuildMode.Destruction)
            {
                ClearRemovePreview();
            }

            if (modeName == BuildMode.None.ToString())
            {
                ClearPreview();
                ClearRemovePreview();
                ClearEditionPreview();
            }

            LastMode = CurrentMode;

            CurrentMode = (BuildMode)Enum.Parse(typeof(BuildMode), modeName);

            BuildEvent.Instance.OnChangedBuildMode.Invoke(CurrentMode);
        }

        /// <summary>
        /// This method allows to select a prefab.
        /// </summary>
        public void SelectPrefab(PieceBehaviour prefab)
        {
            if (prefab == null)
            {
                return;
            }

            SelectedPrefab = BuildManager.Instance.GetPieceById(prefab.Id);
         //   Debug.LogError("SelectPrefab prefab.Id="+prefab.Id+" SelectedPrefab="+SelectedPrefab);
        }

        private void OnDrawGizmosSelected()
        {
            if (BuilderCamera == null)
            {
                BuilderCamera = GetComponent<Camera>();
                return;
            }

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(GetRay.origin, GetRay.direction * ActionDistance);
        }

        #endregion Methods
    }
}