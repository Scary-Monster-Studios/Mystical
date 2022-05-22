using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using System;

namespace EasyBuildSystem.Features.Scripts.Core.Addons
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddonAttribute : Attribute
    {
        #region Fields

        public readonly string Name;
        public readonly AddonTarget Target;
        public Type Behaviour;

        #endregion Fields

        #region Methods

        public AddonAttribute(string name, AddonTarget target)
        {
            Name = name;
            Target = target;
        }

        #endregion Methods
    }
}