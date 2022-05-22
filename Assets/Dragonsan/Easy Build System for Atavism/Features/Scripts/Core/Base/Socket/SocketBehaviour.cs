using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Data;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket.Data;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket.Enums;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Socket
{
    [AddComponentMenu("Easy Build System/Features/Buildings Behaviour/Socket Behaviour")]
    public class SocketBehaviour : MonoBehaviour
    {
        #region Fields

        public SocketType Type;
        public float Radius = 0.5f;
        public Bounds AttachmentBounds;
        public List<Offset> PartOffsets = new List<Offset>();
        public List<Occupancy> BusySpaces = new List<Occupancy>();

        public PieceBehaviour ParentPiece;

        private Collider _CachedCollider;
        public Collider CachedCollider
        {
            get
            {
                if (_CachedCollider == null)
                    _CachedCollider = GetComponent<Collider>();

                return _CachedCollider;
            }
        }

        public bool IsDisabled;

        #endregion Fields

        #region Methods

        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer(Constants.LAYER_SOCKET);

            ParentPiece = GetComponentInParent<PieceBehaviour>();

            if (Type == SocketType.Socket)
            {
                gameObject.AddSphereCollider(Radius);
            }
            else
            {
                gameObject.AddBoxCollider(AttachmentBounds.extents, AttachmentBounds.center);
            }
        }

        private void Start()
        {
            BuildManager.Instance.AddSocket(this);
            UpdateOccupancy(true);
        }

        private void OnDestroy()
        {
            BuildManager.Instance.RemoveSocket(this);
            UpdateOccupancy(false);
        }

        public void OnDrawGizmos()
        {
            if (IsDisabled)
            {
                return;
            }

            if (BusySpaces.Count != 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(transform.position, Vector3.one / 6);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(transform.position, Vector3.one / 6);
            }

            if (Type == SocketType.Socket)
            {
                Gizmos.DrawWireCube(transform.position, Vector3.one / 6);
                Gizmos.DrawWireCube(transform.position, Vector3.one * Radius);
            }
            else
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(AttachmentBounds.center, Vector3.one / 6);
                Gizmos.DrawWireCube(AttachmentBounds.center, AttachmentBounds.extents);
            }
        }

        /// <summary>
        /// This method allows to disable the collider of the socket.
        /// </summary>
        public void DisableSocketCollider()
        {
            IsDisabled = true;

            if (CachedCollider != null)
            {
                CachedCollider.gameObject.layer = Physics.IgnoreRaycastLayer;
            }
        }

        /// <summary>
        /// This method allows to enable the collider of the socket.
        /// </summary>
        public void EnableSocketCollider()
        {
            IsDisabled = false;

            if (CachedCollider != null)
            {
                CachedCollider.gameObject.layer = LayerMask.NameToLayer(Constants.LAYER_SOCKET);
            }
        }

        /// <summary>
        /// This method allows to check if the piece is contains in the offset list of this socket.
        /// </summary>
        public bool AllowPiece(PieceBehaviour piece)
        {
            if (piece == null) return false;

            for (int i = 0; i < PartOffsets.Count; i++)
            {
                if (PartOffsets[i] != null)
                {
                    if (PartOffsets[i].Referer == OffsetRefererType.ByPiece)
                    {
                        if (PartOffsets[i].Piece != null && PartOffsets[i].Piece.Id == piece.Id)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (PartOffsets[i].Category == piece.Category)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This method allows to check if the piece is placed on this socket.
        /// </summary>
        public bool CheckOccupancy(PieceBehaviour piece)
        {
             for (int i = 0; i < BusySpaces.Count; i++)
            {
                if (BusySpaces[i].Piece.Category == piece.Category)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method allows to add a occupancy on this socket.
        /// </summary>
        public void AddOccupancy(PieceBehaviour piece)
        {
            if (!CheckOccupancy(piece))
            {
                if (piece != ParentPiece)
                {
                    BusySpaces.Add(new Occupancy(piece));
                }
            }
        }

        /// <summary>
        /// This method allows to remove a occupancy from this socket.
        /// </summary>
        public void RemoveOccupancy(PieceBehaviour piece)
        {
            if (CheckOccupancy(piece))
            {
                //Dragonsan
                BusySpaces.Remove(BusySpaces.Find(entry => entry.Piece.Category == piece.Category));
             }
        }

        /// <summary>
        /// This method allows to change the current socket state.
        /// </summary>
        public void ChangeOccupancy(bool busy, PieceBehaviour piece)
        { 
            if (busy)
            {
                if (!CheckOccupancy(piece))
                {
                    AddOccupancy(piece);
                }
            }
            else
            {
                if (CheckOccupancy(piece))
                {
                    RemoveOccupancy(piece);
                }
            }
            
            BuildEvent.Instance.OnChangedSocketState.Invoke(this, busy);
        }

        /// <summary>
        /// This method allows to change the state of the sockets that collide the mesh bounds to avoid multiple placement.
        /// </summary>
        private void UpdateOccupancy(bool busy)
        {
            SocketBehaviour[] Sockets = PhysicExtension.GetNeighborsTypeBySphere<SocketBehaviour>(transform.position, Radius, LayerMask.GetMask(Constants.LAYER_SOCKET));

            for (int i = 0; i < Sockets.Length; i++)
            {
                if (Sockets[i] != null)
                {
                    if (Sockets[i].ParentPiece != ParentPiece)
                    {
                        if (AllowPiece(Sockets[i].ParentPiece) && Sockets[i].AllowPiece(ParentPiece))
                        {
                            Sockets[i].ChangeOccupancy(busy, ParentPiece);
                            ChangeOccupancy(busy, Sockets[i].ParentPiece);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method allows to get the piece offset wich allowed on this socket.
        /// </summary>
        public Offset GetOffset(PieceBehaviour piece)
        {
            for (int i = 0; i < PartOffsets.Count; i++)
            {
                if (PartOffsets[i].Referer == OffsetRefererType.ByPiece)
                {
                    if (PartOffsets[i].Piece != null && PartOffsets[i].Piece.Id == piece.Id)
                    {
                        return PartOffsets[i];
                    }
                }
                else
                {
                    if (PartOffsets[i].Category == piece.Category)
                    {
                        return PartOffsets[i];
                    }
                }
            }

            return null;
        }

        #endregion Methods
    }
}