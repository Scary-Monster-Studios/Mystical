using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
namespace Atavism
{

    abstract public class AtavismPathInterpolator
    {

        public AtavismPathInterpolator(long oid, long startTime, float speed, String terrainString, List<Vector3> path)
        {
            this.oid = oid;
            this.startTimeUsed = startTime / 1000f;
            //this.startTime = startTime;
            this.startTime = Time.time - 0.1f; // 6/8/2017 - added the - 0.1f
            this.speed = speed;
            this.terrainString = terrainString;
            this.path = path;
        }

        // abstract public string ToString();

        abstract public PathLocAndDir Interpolate(float systemTime);

        public Vector3 ZeroYIfOnTerrain(Vector3 loc, int pointIndex)
        {
            //System.Debug.Assert (pointIndex >= 0 && pointIndex < terrainString.Length - 1);
            // If either the previous point was on terrain or the
            // current point is on terrain, then the path element is a 
            if (terrainString[pointIndex] == 'T' || terrainString[pointIndex + 1] == 'T')
                loc.y = 0f;
            return loc;
        }

        public string StringPath()
        {
            string s = "";
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 p = path[i];
                if (s.Length > 0)
                    s += ", ";
                s += "#" + i + ": " + p;
            }
            return s;
        }

        public long Oid
        {
            get
            {
                return oid;
            }
        }

        public float Speed
        {
            get
            {
                return speed;
            }
        }

        public string TerrainString
        {
            get
            {
                return terrainString;
            }
        }

        public float StartTime
        {
            get
            {
                return startTime;
            }
        }

        public float TotalTime
        {
            get
            {
                return totalTime;
            }
        }

        protected long oid;
        protected float speed;
        protected String terrainString;
        protected List<Vector3> path;
        protected float totalTime;  // In seconds from start time
        protected float startTime;
        protected float startTimeUsed;

    }


    // A PathSpline is a Catmull-Rom spline, which is related to a
    // BSpline.  Paths produced by the pathing code are interpolated as
    // Catmull-Rom splines
    public class PathSpline : AtavismPathInterpolator
    {

        public PathSpline(long oid, long startTime, float speed, String terrainString, List<Vector3> path) :
                    base(oid, startTime, speed, terrainString, path)
        {
            // Add two points before the zeroeth point, and one after the
            // count-1 point, to allow us to access from -2 to +1 points.
            // Add one point before the zeroeth point, and two after the
            // count-1 point, to allow us to access from -1 to +2 points.
            path.Insert(0, path[0]);
            Vector3 last = path[path.Count - 1];
            path.Add(last);
            path.Add(last);
            int count = path.Count;
            timeVector = new float[count];
            timeVector[0] = 0f;
            float t = 0;
            Vector3 curr = path[0];
            for (int i = 1; i < count; i++)
            {
                Vector3 next = path[i];
                Vector3 diff = next - curr;
                float diffTime = diff.magnitude;
                t = t + diffTime / speed;
                timeVector[i] = t;
                curr = next;
            }
            totalTime = t;
        }

        public override string ToString()
        {
            return "[PathSpline oid = " + oid + "; speed = " + speed + "; timeVector = " + timeVector + "; path = " + StringPath() + "]";
        }

        public override PathLocAndDir Interpolate(float systemTime)
        {
            float t = (float)(systemTime - startTime);
            if (t < 0)
                t = 0;
            else if (t >= totalTime)
                return null;
            // Find the point number whose time is less than or equal to t
            int count = path.Count;
            // A bit of trickiness - - the first two points and the last
            // point are dummies, inserted only to ensure that we have -2
            // to +1 points at every real point.
            int pointNumber = -2;
            for (int i = 0; i < count; i++)
            {
                if (timeVector[i] > t)
                {
                    pointNumber = i - 1;
                    break;
                }
            }
            if (pointNumber == -1)
            {
                pointNumber = 1;
            }
            Vector3 loc;
            Vector3 dir;
            float timeFraction = 0;
            // If we're beyond the last time, return the last point, and a
            // (0,0,0) direction
            if (pointNumber == -2)
            {
                loc = path[count - 1];
                dir = new Vector3(0f, 0f, 0f);
            }
            else
            {
                float timeAtPoint = timeVector[pointNumber];
                float timeSincePoint = t - timeAtPoint;
                timeFraction = timeSincePoint / (timeVector[pointNumber + 1] - timeAtPoint);
                loc = evalPoint(pointNumber, timeFraction);
                dir = evalDirection(loc, pointNumber, timeFraction) * speed;
            }
            // A bit tricky - - if there were n elements in the _original_
            // path, there are n-1 characters in the terrain string.
            // We've added _three_ additional path elements, one before
            // and two after.
            int pathNumber = (pointNumber == -2 ? count - 4 : pointNumber);
            if (terrainString[pathNumber] == 'T' || terrainString[pathNumber + 1] == 'T')
            {
                loc.y = 0f;
                dir.y = 0f;
            }
            return new PathLocAndDir(loc, dir, speed * (totalTime - t));
        }

        // Catmull-Rom spline is just like a B spline, only with a different basis
        protected float basisFactor(int degree, float t)
        {
            switch (degree)
            {
                case -1:
                    return ((-t + 2f) * t - 1f) * t / 2f;
                case 0:
                    return (((3f * t - 5f) * t) * t + 2f) / 2f;
                case 1:
                    return ((-3f * t + 4f) * t + 1f) * t / 2f;
                case 2:
                    return ((t - 1f) * t * t) / 2f;
            }
            return 0f; //we only get here if an invalid i is specified
        }

        // evaluate a point on the spline.  t is the time since we arrived
        // at point pointNumber.
        protected Vector3 evalPoint(int pointNumber, float t)
        {
            float px = 0;
            float py = 0;
            float pz = 0;
            for (int degree = -1; degree <= 2; degree++)
            {
                float basis = basisFactor(degree, t);
                Vector3 pathPoint = path[pointNumber + degree];
                px += basis * pathPoint.x;
                py += basis * pathPoint.y;
                pz += basis * pathPoint.z;
            }
            return new Vector3(px, py, pz);
        }

       

      
        protected float directionTimeOffset = .01f;

        // evaluate the direction on the spline.  t is the time since we
        // arrived at point pointNumber.
        protected Vector3 evalDirection(Vector3 p, int pointNumber, float t)
        {
            Vector3 next = evalPoint(pointNumber, t + directionTimeOffset);
            Vector3 n = next - p;
            n.y = 0;
            n.Normalize();
            return n;
        }

        protected float[] timeVector;

    }

    // A linear interpolator of a sequence of points
    public class PathLinear : AtavismPathInterpolator
    {
        AtavismMobNode node;
        public PathLinear(long oid, AtavismMobNode node, long startTime, float speed, String terrainString, List<Vector3> path) :
                    base(oid, startTime, speed, terrainString, path)
        {
           if (AtavismLogger.logLevel <= LogLevel.Debug)
               AtavismLogger.LogInfoMessage("PathLinear: node pos=" + node.Position + " for mob: " + oid + " new path=" + string.Join(";", path.ToArray())+ " speed="+ speed+ " terrainString=" + terrainString+ " startTime="+ startTime);
           if (ClientAPI.Instance.mobOidDebug > 0L && oid == ClientAPI.Instance.mobOidDebug)
           {
               UnityEngine.Debug.LogError("PathLinear: node pos=" + node.Position + " for mob: " + oid + " new path=" + string.Join(";", path.ToArray()) + " speed=" + speed + " terrainString=" + terrainString + " startTime=" + startTime);
           }
          // UnityEngine.Debug.LogError("PathLinear: node pos=" + node.Position + " for mob: " + oid + " new path=" + string.Join(";", path.ToArray()) + " speed=" + speed + " terrainString=" + terrainString + " startTime=" + startTime);
           this.node = node;
            // If the mob is sufficiently away from the first point, update the position
            if (path.Count > 0)
                if (AtavismClient.Instance.ResyncMobs && Vector2.Distance(new Vector2(path[0].x, path[0].z), new Vector2(node.Position.x, node.Position.z)) > 15)
                {
                    // We need to re-sync position, but not sure of where the ground is at. It's not the most effective system, but do a raycast to find the ground
                    Vector3 newPos = new Vector3(path[0].x, path[0].y, path[0].z);
                    if (node.Position.y > path[0].y)
                    {
                        newPos.y = node.Position.y;
                    }
                    if (node.MobController != null)
                    {
                        bool isAboveTerrain = IsAboveTerrain(newPos, node.MobController.groundLayers);
                        //AtavismLogger.LogWarning("Raycast from: " + newPos + " is above terrain? " + isAboveTerrain);
                        int attempts = 0;
                        while (!isAboveTerrain && attempts < 300 && node.MobController.groundLayers > 0)
                        {
                            newPos += new Vector3(0, 0.5f, 0);
                            isAboveTerrain = IsAboveTerrain(newPos, node.MobController.groundLayers);
                            //AtavismLogger.LogWarning("Raycast from: " + newPos + " is above terrain? " + isAboveTerrain);
                            attempts++;
                        }
                    }
                    if (AtavismLogger.logLevel <= LogLevel.Debug)
                        AtavismLogger.LogWarning("Distance between node: " + node.Position + " and pos 0: " + path[0] + " is greater than 5 for mob: " + oid);
                   node.Position = new Vector3(path[0].x, newPos.y, path[0].z);

                   
                }
            // Get direction from current to first point
          
            float pathDistance = 0f;
            float characterDistance = 0f;
            if (path.Count > 1)
            {
                path[0] = node.Position;
                pathDistance = Vector3.Distance(path[0], path[1]);
                if (node.runChangePos != null)
                {
                    characterDistance = Vector3.Distance(node.rubChangeToPos, path[1]);
                }
                else
                {
                    characterDistance = Vector3.Distance(node.Position, path[1]);
                }
            }
            else if (path.Count == 1)
            {
                if (node.runChangePos != null)
                {
                    characterDistance = Vector3.Distance(node.rubChangeToPos, path[0]);
                }
                else
                {
                    characterDistance = Vector3.Distance(node.Position, path[0]);
                }
            }

            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogInfoMessage("Distance between node: " + node.Position + " and pos 0: " + (path.Count > 0 ? path[0] + "" : "none") + " going to point: " + (path.Count>1?path[1]+"":"none") + " pathDistance: " + pathDistance + " characterDistance: " + characterDistance+ " speed="+ speed);

            float differenceRatio = 1f;
            if (pathDistance > 0)
                differenceRatio = characterDistance / pathDistance * 1f;
            if (differenceRatio < 0.75f)
            {
                differenceRatio = 0.75f;
            }
            else if (differenceRatio > 1.25f)
            {
                differenceRatio = 1.25f;
            }
            this.speed = speed * differenceRatio;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogInfoMessage("Distance between node: " + node.Position + " rubChangeToPos="+ node.rubChangeToPos + "and pos 0: " + (path.Count > 0 ? path[0] + "" : "none") + " going to point: " + (path.Count > 1 ? path[1] + "" : "none") + "with ratio: " + differenceRatio + " for mob: " + oid+ " speed="+ speed+" speed2="+this.speed);
  
            if (path.Count < 2)
            {
                path.Insert(0, node.Position);
                if(path.Count==1)
                    path.Insert(0, node.Position);
            }
            else
            {
                if (node.runChangePos != null)
                {
                    float dis = Vector3.Distance(path[0], node.rubChangeToPos);
                    if (dis > 0.2f)
                    {
                        path[0] = node.rubChangeToPos;
                    }
                }
                else
                {
                    path[0] = node.Position;
                }
            }
            float cumm = 0f;
            Vector3 curr = path[0];
            
            Vector3 dst = path[1];
            if (node != null && node.GameObject != null)
            {
               // Debug.LogError("PathLinear "+oid+" Pos "+node.GameObject.transform.position+" Rot "+node.GameObject.transform.rotation.eulerAngles+" LookAt "+dst);
                dst.y = node.GameObject.transform.position.y;
              // Debug.LogError("PathLinear "+oid+" Pos "+node.GameObject.transform.position+" Rot "+node.GameObject.transform.rotation.eulerAngles+" dist="+(dst - node.GameObject.transform.position).magnitude+" Agnle "+Quaternion.Angle(node.GameObject.transform.rotation, rotation));
                if ((dst - node.GameObject.transform.position).magnitude > 0.1f)
                {
                    Quaternion rotation = Quaternion.LookRotation(dst - node.GameObject.transform.position, Vector3.up);
                    node.SetOrientation(rotation);
                }
                // Debug.LogError("PathLinear "+oid+" Pos "+node.GameObject.transform.position+" Rot "+node.GameObject.transform.rotation.eulerAngles+" After "+dst+" rotation="+rotation);

            }

            for (int i = 1; i < path.Count; i++)
            {
                Vector3 next = path[i];
                Vector3 diff = next - curr;
                float dist = (next - curr).magnitude;
                float diffTime = dist / this.speed;
                cumm += diffTime;
            }
            totalTime = cumm + 0.1f;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogInfoMessage("PathLinear: node pos=" + node.Position + " for mob: " + oid + " new path=" + string.Join(";", path.ToArray()) + " speed=" + speed + " terrainString=" + terrainString + " startTime=" + startTime+ " totalTime="+ totalTime);



        }

        bool IsAboveTerrain(Vector3 position, LayerMask groundLayers)
        {
            // Make sure the corpse isn't underground
            Ray ray = new Ray(position, Vector3.down);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            return Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayers);
        }

        // Evaluate the position and direction on the path.  The
        // argument is the millisecond system time
        public override PathLocAndDir Interpolate(float systemTime)
        {

            float t = (float)(systemTime - startTime);
            if (t < 0)
                t = 0;
            else if (t >= totalTime)
            {
                return null;
            }
            float cumm = 0f;
            Vector3 curr = path[0];
            for (int i = 1; i < path.Count; i++)
            {
                Vector3 next = path[i];
                Vector3 diff = next - curr;
                float dist = (next - curr).magnitude;

                float diffTime = dist / speed;
                if (t <= cumm + diffTime)
                {
                    float frac = (t - cumm) / diffTime;
                    Vector3 loc = curr + (diff * frac); // ZeroYIfOnTerrain(curr + (diff * frac), i - 1);
                    diff.Normalize();
                    Vector3 dir = diff * speed;
                    
                    if (node != null && node.GameObject != null)
                    {
                        // Debug.LogError("PathLinear "+oid+" Pos "+node.GameObject.transform.position+" Rot "+node.GameObject.transform.rotation.eulerAngles+" LookAt "+dst);
                        next.y = node.GameObject.transform.position.y;
                        // Debug.LogError("PathLinear "+oid+" Pos "+node.GameObject.transform.position+" Rot "+node.GameObject.transform.rotation.eulerAngles+" dist="+(dst - node.GameObject.transform.position).magnitude+" Agnle "+Quaternion.Angle(node.GameObject.transform.rotation, rotation));
                        if ((next - node.GameObject.transform.position).magnitude > 0.1f)
                        {
                            Quaternion rotation = Quaternion.LookRotation(next - node.GameObject.transform.position, Vector3.up);
                            node.SetOrientation(rotation);
                        }
                        // Debug.LogError("PathLinear "+oid+" Pos "+node.GameObject.transform.position+" Rot "+node.GameObject.transform.rotation.eulerAngles+" After "+dst+" rotation="+rotation);

                    }
                    return new PathLocAndDir(loc, dir, speed * (totalTime - t));
                }
                cumm += diffTime;
                curr = next;

            }

            // Didn't find the time, so return the last point, and a dir
            // of zero
            return new PathLocAndDir(path[path.Count - 1], new Vector3(0f, 0f, 0f), 0f);
        }

        public override String ToString()
        {
            return "[PathLinear oid = " + oid + "; speed = " + speed + "; path = " + StringPath() + "]";
        }

    }

    // A class to encapsulate the return values from path
    // interpolators
    public class PathLocAndDir
    {

        public PathLocAndDir(Vector3 location, Vector3 direction, float lengthLeft)
        {
            this.location = location;
            this.direction = direction;
            this.lengthLeft = lengthLeft;
        }

        public Vector3 Location
        {
            get
            {
                return location;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
        }

        public float LengthLeft
        {
            get
            {
                return lengthLeft;
            }
        }

        public override string ToString()
        {
            return string.Format("[PathLocAndDir loc {0} dir {1} lengthLeft {2}]", location, direction, lengthLeft);
        }

        protected Vector3 location;
        protected Vector3 direction;
        protected float lengthLeft;
    }
}