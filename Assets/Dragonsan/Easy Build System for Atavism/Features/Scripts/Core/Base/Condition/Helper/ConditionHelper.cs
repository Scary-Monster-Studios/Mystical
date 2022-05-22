using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Condition.Helper
{
    public class ConditionHelper : MonoBehaviour
    {
        #region Methods

        public static List<ConditionAttribute> GetConditions()
        {
            List<ConditionAttribute> ResultAddons = new List<ConditionAttribute>();

            Type[] ActiveBehaviours = GetAllSubTypes(typeof(MonoBehaviour));

            foreach (Type Type in ActiveBehaviours)
            {
                object[] Attributes = Type.GetCustomAttributes(typeof(ConditionAttribute), false);

                if (Attributes != null)
                {
                    for (int i = 0; i < Attributes.Length; i++)
                    {
                        if ((ConditionAttribute)Attributes[i] != null)
                        {
                            ((ConditionAttribute)Attributes[i]).Behaviour = Type;

                            ResultAddons.Add((ConditionAttribute)Attributes[i]);
                        }
                    }
                }
            }

            return ResultAddons;
        }

        public static List<ConditionAttribute> GetConditionsByTarget(ConditionTarget target)
        {
            List<ConditionAttribute> ResultAddons = new List<ConditionAttribute>();

            Type[] ActiveBehaviours = GetAllSubTypes(typeof(MonoBehaviour));

            foreach (Type Type in ActiveBehaviours)
            {
                object[] Attributes = Type.GetCustomAttributes(typeof(ConditionAttribute), false);

                if (Attributes != null)
                {
                    for (int i = 0; i < Attributes.Length; i++)
                    {
                        if ((ConditionAttribute)Attributes[i] != null)
                        {
                            if (((ConditionAttribute)Attributes[i]).Target == target)
                            {
                                ((ConditionAttribute)Attributes[i]).Behaviour = Type;

                                ResultAddons.Add((ConditionAttribute)Attributes[i]);
                            }
                        }
                    }
                }
            }

            return ResultAddons;
        }

        public static Type[] GetAllSubTypes(Type aBaseClass)
        {
            List<Type> Result = new List<Type>();

            Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly Assembly in Assemblies)
            {
                Type[] Types = Assembly.GetTypes();

                foreach (Type T in Types)
                {
                    if (T.IsSubclassOf(aBaseClass))
                    {
                        Result.Add(T);
                    }
                }
            }

            return Result.ToArray();
        }

        #endregion Methods
    }
}