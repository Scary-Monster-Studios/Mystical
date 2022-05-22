using UnityEngine;
using System.Collections;
namespace Atavism
{

    public class IntVector3
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public int x;
        public int y;
        public int z;

        public IntVector3()
        {
        }

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public IntVector3(Vector3 vec)
        {
            this.x = (int)vec.x;
            this.y = (int)vec.y;
            this.z = (int)vec.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return string.Format("IntVector3({0}, {1}, {2})", x, y, z);
        }
    }
}