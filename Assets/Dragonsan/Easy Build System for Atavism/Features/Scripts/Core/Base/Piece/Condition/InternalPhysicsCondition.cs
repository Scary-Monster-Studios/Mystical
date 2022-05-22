using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Linq;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Piece.Condition
{
    [System.Serializable]
    public class Detection
    {
        #region Fields

        public Bounds DetectionBounds;
        public LayerMask RequireLayer;
        public bool RequireSupport;
        public string[] RequirePieceType;

        #endregion Fields

        #region Methods

        public bool CheckType(string type)
        {
            return RequirePieceType.Contains(type);
        }

        #endregion Methods
    }

    [Condition("Internal Physics Condition", "Check and denies the actions, if this piece is not stable.\n" +
        "A new rigidbody component is added when the support is destroyed during the runtime.\n" +
        "Convex feature is included for the MeshColliders to avoid the physics errors messages.\n" +
        "You can find more information about of conditions in the online documentation.", ConditionTarget.PieceBehaviour)]
    public class InternalPhysicsCondition : ConditionBehaviour
    {
        #region Fields

        public bool RequireStableSupport = false;
        public bool ConvexMeshColliders = true;
        public bool CheckStabilitySometime;
        public float CheckStabilityInterval = 0.5f;
        public Detection[] Detections;

        public bool AffectedByPhysics { get; set; }

        private Rigidbody Rigidbody;

        #endregion Fields

        #region Methods

        private void Start()
        {
            if (Piece.CurrentState != StateType.Placed)
                return;

            if (!CheckStability())
            {
                ApplyPhysics();
            }

            BuildEvent.Instance.OnPieceDestroyed.AddListener((PieceBehaviour piece) =>
            {
                if (piece.CurrentState != StateType.Remove) return;

                if (!CheckStability())
                {
                    ApplyPhysics();
                }
            });

            if (CheckStabilitySometime)
            {
                InvokeRepeating("CheckStabilityWithInterval", CheckStabilityInterval, CheckStabilityInterval);
            }
        }

        private void OnDestroy()
        {
            if (IsQuitting) return;

            if (Piece == null) return;

            if (Piece.CurrentState == StateType.Preview) return;

            for (int i = 0; i < Piece.LinkedPieces.Count; i++)
            {
                if (Piece != null && Piece.LinkedPieces[i] != null)
                {
                    InternalPhysicsCondition LinkedCondition = Piece.LinkedPieces[i].GetComponent<InternalPhysicsCondition>();

                    if (LinkedCondition != null)
                        if (!LinkedCondition.CheckStability())
                            LinkedCondition.ApplyPhysics();
                }
            }
        }

        private bool IsQuitting;
        private void OnApplicationQuit()
        {
            IsQuitting = true;
        }

        public override bool CheckForPlacement()
        {
            if (RequireStableSupport)
                return CheckStability();
            else
                return true;
        }

        public void ApplyPhysics()
        {
            if (AffectedByPhysics)
            {
                return;
            }

            if (Piece.CurrentState == StateType.Queue)
            {
                return;
            }

            if (Rigidbody == null)
            {
                Rigidbody = gameObject.AddRigibody(false, false);

                if (ConvexMeshColliders)
                {
                    for (int i = 0; i < Piece.Colliders.Count; i++)
                    {
                        if (Piece.Colliders[i].GetComponent<MeshCollider>() != null)
                        {
                            Piece.Colliders[i].GetComponent<MeshCollider>().convex = true;
                        }
                    }
                }

                Rigidbody.useGravity = true;
                Rigidbody.isKinematic = false;
                Rigidbody.AddForce(Random.insideUnitSphere, ForceMode.Impulse);
            }

            AffectedByPhysics = true;
            PhysicExtension.SetLayerRecursively(gameObject, Physics.IgnoreRaycastLayer);
         //   Debug.LogError("Destroy");
            Destroy(Piece);
            Destroy(gameObject, 5f);
        }

        public bool CheckStability()
        {
         //   Debug.LogError("CheckStability "+Detections.Length);
            if (Detections.Length != 0)
            {
                bool[] Results = new bool[Detections.Length];

                for (int i = 0; i < Detections.Length; i++)
                {
                    if (Detections[i] != null)
                    {
                        PieceBehaviour[] Pieces = PhysicExtension.GetNeighborsTypeByBox<PieceBehaviour>(transform.TransformPoint(Detections[i].DetectionBounds.center),
                            Detections[i].DetectionBounds.extents, transform.rotation, Detections[i].RequireLayer);

                        for (int p = 0; p < Pieces.Length; p++)
                        {
                            PieceBehaviour CollapsePiece = Pieces[p].GetComponent<PieceBehaviour>();

                            if (CollapsePiece != null)
                            {
                                if (CollapsePiece != Piece)
                                {
                                    if (CollapsePiece.CurrentState != StateType.Queue && Detections[i].CheckType(CollapsePiece.Category))
                                    {
                                        Results[i] = true;
                                    }
                                }
                            }
                        }

                        Collider[] Colliders = PhysicExtension.GetNeighborsTypeByBox<Collider>(transform.TransformPoint(Detections[i].DetectionBounds.center),
                            Detections[i].DetectionBounds.extents, transform.rotation, Detections[i].RequireLayer);

                        for (int x = 0; x < Colliders.Length; x++)
                        {
                            if (Detections[i].RequireSupport)
                            {
                                if (BuildManager.Instance.IsSupport(Colliders[x]))
                                {
                                    Results[i] = true;
                                }
                            }
                        }
                    }
                }

                return Results.All(result => result);
            }

            return false;
        }
          public bool CheckStability(PieceBehaviour _p)
        {
            Debug.LogError("CheckStability "+Detections.Length);
            if (Detections.Length != 0)
            {
                bool[] Results = new bool[Detections.Length];

                for (int i = 0; i < Detections.Length; i++)
                {
                    if (Detections[i] != null)
                    {
                        PieceBehaviour[] Pieces = PhysicExtension.GetNeighborsTypeByBox<PieceBehaviour>(transform.TransformPoint(Detections[i].DetectionBounds.center),
                            Detections[i].DetectionBounds.extents, transform.rotation, Detections[i].RequireLayer);

                        for (int p = 0; p < Pieces.Length; p++)
                        {
                            PieceBehaviour CollapsePiece = Pieces[p].GetComponent<PieceBehaviour>();

                            if (CollapsePiece != null)
                            {
                                if (CollapsePiece != Piece)
                                {
                                    if (CollapsePiece.CurrentState != StateType.Queue && Detections[i].CheckType(CollapsePiece.Category))
                                    {
                                        if (_p == CollapsePiece)
                                            return true;
                                    }
                                }
                            }
                        }

                        Collider[] Colliders = PhysicExtension.GetNeighborsTypeByBox<Collider>(transform.TransformPoint(Detections[i].DetectionBounds.center),
                            Detections[i].DetectionBounds.extents, transform.rotation, Detections[i].RequireLayer);

                        for (int x = 0; x < Colliders.Length; x++)
                        {
                            if (Detections[i].RequireSupport)
                            {
                                if (BuildManager.Instance.IsSupport(Colliders[x]))
                                {
                                    Results[i] = true;
                                }
                            }
                        }
                    }
                }

                return Results.All(result => result);
            }

            return false;
        }

        private void CheckStabilityWithInterval()
        {
            if (!CheckStability())
                ApplyPhysics();
        }

        private void OnDrawGizmosSelected()
        {
            if (Detections == null || Detections.Length == 0) return;

            for (int i = 0; i < Detections.Length; i++)
            {
                Gizmos.DrawWireCube(transform.TransformPoint(Detections[i].DetectionBounds.center), Detections[i].DetectionBounds.extents * 2);
            }
        }

        #endregion
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(InternalPhysicsCondition), true)]
    public class InternalPhysicsConditionInspector : UnityEditor.Editor
    {
        private InternalPhysicsCondition Target;

        private void OnEnable()
        {
            Target = (InternalPhysicsCondition)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("Detections.Array.size"), new GUIContent("Detection Array Size"));
            for (int i = 0; i < serializedObject.FindProperty("Detections").arraySize; i++)
            {
                GUI.color = Color.black / 4;
                GUILayout.BeginVertical("helpBox");
                GUI.color = Color.white;
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("Detections").GetArrayElementAtIndex(i).FindPropertyRelative("DetectionBounds"), new GUIContent("Detection Bounds"));
                UnityEditor.SceneView.RepaintAll();
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("Detections").GetArrayElementAtIndex(i).FindPropertyRelative("RequireLayer"), new GUIContent("Require Layer(s)"));
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("Detections").GetArrayElementAtIndex(i).FindPropertyRelative("RequireSupport"), new GUIContent("Require Support"));

                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("Detections").GetArrayElementAtIndex(i).FindPropertyRelative("RequirePieceType.Array.size"), new GUIContent("Require Piece Type Array Size"));

                for (int x = 0; x < serializedObject.FindProperty("Detections").GetArrayElementAtIndex(i).FindPropertyRelative("RequirePieceType").arraySize; x++)
                {
                    GUI.color = Color.black / 4;
                    GUILayout.BeginVertical("helpBox");
                    GUI.color = Color.white;
                    UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("Detections").GetArrayElementAtIndex(i).FindPropertyRelative("RequirePieceType").GetArrayElementAtIndex(x));
                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }

            GUI.color = new Color(0f, 1f, 1f);

            if (GUILayout.Button("Check Support Stability"))
            {
                Debug.Log("<b>Easy Build System</b> : The piece is " + (Target.CheckStability() ? "stable" : "unstable"));
            }

            GUI.color = Color.white;

            GUILayout.Space(3f);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}