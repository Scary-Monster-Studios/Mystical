using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Group;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Event
{
    [DefaultExecutionOrder(-999)]
    public class BuildEvent : MonoBehaviour
    {
        #region Fields

        public static BuildEvent Instance;

        [Serializable] public class StorageSaving : UnityEvent { }
        public StorageSaving OnStorageSaving;

        [Serializable] public class StorageLoading : UnityEvent { }
        public StorageLoading OnStorageLoading;

        [Serializable] public class StorageLoadingResult : UnityEvent<PieceBehaviour[]> { }
        public StorageLoadingResult OnStorageLoadingResult;

        [Serializable] public class StorageSavingResult : UnityEvent<PieceBehaviour[]> { }
        public StorageSavingResult OnStorageSavingResult;

        [Serializable] public class PieceInstantiated : UnityEvent<PieceBehaviour, SocketBehaviour> { }
        public PieceInstantiated OnPieceInstantiated;

        [Serializable] public class PieceDestroyed : UnityEvent<PieceBehaviour> { }
        public PieceDestroyed OnPieceDestroyed;

        [Serializable] public class PieceChangedState : UnityEvent<PieceBehaviour, StateType> { }
        public PieceChangedState OnPieceChangedState;

        [Serializable] public class PieceChangedAppearance : UnityEvent<PieceBehaviour, int> { }
        public PieceChangedAppearance OnPieceChangedApperance;

        [Serializable] public class GroupInstantiated : UnityEvent<GroupBehaviour> { }
        public GroupInstantiated OnGroupInstantiated;

        [Serializable] public class GroupUpdated : UnityEvent<GroupBehaviour> { }
        public GroupUpdated OnGroupUpdated;

        [Serializable] public class ChangedBuildMode : UnityEvent<BuildMode> { }
        public ChangedBuildMode OnChangedBuildMode;

        [Serializable] public class ChangedSocketState : UnityEvent<SocketBehaviour, bool> { }
        public ChangedSocketState OnChangedSocketState;

        #endregion

        #region Methods

        private void Awake()
        {
            Instance = this;
        }

        #endregion
    }
}