using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class AtavismEffect 
    {

        public int id;
        public long stateid;
        public string name;
        public Sprite icon;
        public string tooltip;
        public bool isBuff;
        public int stackLimit = 1;
        public bool stackTime = false;
        public bool allowMultiple = false;
        public long startTime = 0L;
        int stackSize = 1;
        float length;
        float expiration = -1;
        bool active = false;
        bool passive = false;

        // Use this for initialization
        void Start()
        {

        }

        public override string ToString()
        {
            return "[AtavismEffect: "+ name+ " isBuff="+ isBuff+ " stackSize="+ stackSize+ " length="+ length+ " expiration="+ expiration+ " active="+ active+ " passive="+ passive+" ]";
        }

        public AtavismEffect Clone( )
        {
            AtavismEffect clone = new AtavismEffect();
            clone.id = id;
            clone.name = name;
            clone.icon = icon;
            clone.tooltip = tooltip;
            clone.Length = Length;
            clone.isBuff = isBuff;
            clone.stackLimit = stackLimit;
            clone.stackTime = stackTime;
            clone.allowMultiple = allowMultiple;
            return clone;
        }

        public int StackSize
        {
            get
            {
                return stackSize;
            }
            set
            {
                stackSize = value;
            }
        }

        public float Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        public float Expiration
        {
            get
            {
                return expiration;
            }
            set
            {
                expiration = value;
            }
        }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
        public bool Passive
        {
            get
            {
                return passive;
            }
            set
            {
                passive = value;
            }
        }
    }
}