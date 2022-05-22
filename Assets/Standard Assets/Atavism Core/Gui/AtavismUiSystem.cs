using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{

    /// <summary>
    /// User interface system. Keeps track of what frames the users mouse is over etc.
    /// </summary>
    public class AtavismUiSystem
    {

        static bool overrideFrameDetection;
        static bool mouseOverFrame;

        static Dictionary<string, Rect> frames = new Dictionary<string, Rect>();
        //static List<Rect> frames = new List<Rect>();
        static List<string> uiWindows = new List<string>();

        public static void AddFrame(string frameName, Rect frame)
        {
            if (!frames.ContainsKey(frameName))
            {
                frames.Add(frameName, frame);
                //Debug.Log("Adding frame: " + frameName + " with rect: " + frame);
            }
            //frames.Add(frame);
        }
         
        public static void RemoveFrame(string frameName, Rect frame)
        {
            if (frames.ContainsKey(frameName))
                frames.Remove(frameName);
            //frames.Remove(frame);
        }

      
        public static List<Rect> GetFrames(Vector2 point)
        {
            List<Rect> framesHit = new List<Rect>();
            foreach (Rect frame in frames.Values)
            {
                if (frame.Contains(point))
                    framesHit.Add(frame);
            }
            return framesHit;
        }

        public static bool IsMouseOverFrame()
        {
            if (overrideFrameDetection)
            {
                return mouseOverFrame;
            }
            // Subtract the mouse position y from the screen height as it seems to use the inverse of the gui y direction
            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            foreach (Rect frame in frames.Values)
            {
                //Debug.Log("Checking mouse position " + mousePosition + " against frame: " + frame.xMin + "/" + frame.yMin 
                //	+ " " + frame.xMax + "/" + frame.yMax + ". contains point = " + frame.Contains(mousePosition));
                if (frame.Contains(mousePosition))
                    return true;
            }

            return IsOverUiWindow();
        }

        public static void AddMouseOverWindow(string windowName)
        {
            uiWindows.Add(windowName);
        }

        public static void RemoveMouseOverWindow(string windowName)
        {
            uiWindows.Remove(windowName);
        }

        public static bool IsOverUiWindow()
        {
            if (uiWindows.Count > 0)
                return true;
            else
                return false;
        }

        public static bool OverrideFrameDetection
        {
            get
            {
                return overrideFrameDetection;
            }
            set
            {
                overrideFrameDetection = value;
            }
        }

        public static bool MouseOverFrame
        {
            get
            {
                return mouseOverFrame;
            }
            set
            {
                mouseOverFrame = value;
            }
        }
    }
}