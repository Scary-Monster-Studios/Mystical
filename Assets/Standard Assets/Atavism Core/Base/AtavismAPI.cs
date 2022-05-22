using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
namespace Atavism
{

    public delegate void WorldInitializedHandler(object sender, EventArgs e);

    public delegate void FrameStartedHandler(object sender, ScriptingFrameEventArgs e);

    public delegate void FrameEndedHandler(object sender, ScriptingFrameEventArgs e);

    public delegate void TimeSecondHandler(object sender, ScriptingFrameEventArgs e);

    public delegate void ProgressUpdateHandler(object sender, ProgressUpdateEventArgs e);

    public delegate void WaterEventHandler(object sender, WaterEventArgs e);

    public delegate void ZoneEventHandler(object sender, ZoneEventArgs e);

    public delegate void RegionEventHandler(object sender, RegionEventArgs e);

    public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

    public class ScriptingFrameEventArgs : EventArgs
    {
        protected float time;

        public ScriptingFrameEventArgs(float time)
                : base()
        {
            this.time = time;
        }

        public float TimeSinceLastFrame
        {
            get
            {
                return time;
            }
        }
    }

    public class ProgressUpdateEventArgs : EventArgs
    {
        protected float percentage;

        public ProgressUpdateEventArgs(float percentage)
                : base()
        {
            this.percentage = percentage;
        }

        public float Percentage
        {
            get
            {
                return percentage;
            }
        }
    }

    public class WaterEventArgs : EventArgs
    {
        protected bool underwater;

        public WaterEventArgs(bool underwater)
                : base()
        {
            this.underwater = underwater;
        }

        public bool Underwater
        {
            get
            {
                return underwater;
            }
        }
    }

    public class ZoneEventArgs : EventArgs
    {
        protected string zoneName;
        protected string zoneType;

        public ZoneEventArgs(string zoneName, string zoneType)
                : base()
        {
            this.zoneName = zoneName;
            this.zoneType = zoneType;
        }

        public string ZoneName
        {
            get
            {
                return zoneName;
            }
        }

        public string ZoneType
        {
            get
            {
                return zoneType;
            }
        }
    }

    public class RegionEventArgs : EventArgs
    {
        protected string regionName;
        protected bool entering;

        public RegionEventArgs(string regionName, bool entering)
                : base()
        {
            this.regionName = regionName;
            this.entering = entering;
        }

        public string RegionName
        {
            get
            {
                return regionName;
            }
        }

        public bool Entering
        {
            get
            {
                return entering;
            }
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        protected string errorMessage;

        public ErrorEventArgs(string errorMessage)
                : base()
        {
            this.errorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }
    }

    public class AtavismAPI
    {

        protected static IGameWorld betaWorld;
        protected static bool initialized;
        protected static SortedList<long, List<object>> effectWorkQueue;
        protected static YieldEffectHandler yieldHandler;
        protected static int currentSecond;

        public static event WorldInitializedHandler WorldInitialized;
        public static event FrameStartedHandler FrameStarted;
        public static event FrameStartedHandler InterfaceFrameStarted;
        public static event FrameEndedHandler FrameEnded;
        public static event FrameEndedHandler InterfaceFrameEnded;
        public static event TimeSecondHandler NewSecond;
        public static event EventHandler WorldConnect;
        public static event EventHandler WorldDisconnect;
        public static event EventHandler AdminWorldFilesChanged;
        public static event EventHandler WorldFileUpdateStarted;
        public static event ProgressUpdateHandler WorldFileUpdateProgress;
        public static event EventHandler WorldFileUpdateCompleted;
        public static event ErrorEventHandler WorldFileUpdateFailed;
        public static event EventHandler WorldFileUploadStarted;
        public static event ProgressUpdateHandler WorldFileUploadProgress;
        public static event EventHandler WorldFileUploadCompleted;
        public static event ErrorEventHandler WorldFileUploadFailed;
        public static event EventHandler LoginStarted;
        public static event WaterEventHandler PlayerWaterEvent;
        public static event WaterEventHandler CameraWaterEvent;
        public static event ZoneEventHandler ZoneEvent;
        public static event RegionEventHandler RegionEvent;

        static AtavismAPI()
        {
            effectWorkQueue = new SortedList<long, List<object>>();
        }

        public static void OnWorldInitialized()
        {
            WorldInitializedHandler handler = WorldInitialized;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        public static void OnWorldConnect()
        {
            if (WorldConnect != null)
                WorldConnect(null, new EventArgs());
        }

        public static void OnWorldDisconnect()
        {
            if (WorldDisconnect != null)
                WorldDisconnect(null, new EventArgs());
        }

        /// <summary>
        /// The next few functions relate to world file updating/uploading
        /// </summary>
        public static void OnAdminWorldFilesChanged()
        {
            if (AdminWorldFilesChanged != null)
                AdminWorldFilesChanged(null, new EventArgs());
        }

        public static void OnWorldFileUpdateStarted()
        {
            if (WorldFileUpdateStarted != null)
                WorldFileUpdateStarted(null, new EventArgs());
        }

        public static void OnWorldFileUpdateProgress(float progress)
        {
            ProgressUpdateHandler handler = WorldFileUpdateProgress;
            if (handler != null)
            {
                handler(null, new ProgressUpdateEventArgs(progress));
            }
        }

        public static void OnWorldFileUpdateCompleted()
        {
            if (WorldFileUpdateCompleted != null)
                WorldFileUpdateCompleted(null, new EventArgs());
        }

        public static void OnWorldFileUpdateFailed(string errorMessage)
        {
            ErrorEventHandler handler = WorldFileUpdateFailed;
            if (handler != null)
            {
                handler(null, new ErrorEventArgs(errorMessage));
            }
        }

        public static void OnWorldFileUploadStarted()
        {
            if (WorldFileUploadStarted != null)
                WorldFileUploadStarted(null, new EventArgs());
        }

        public static void OnWorldFileUploadProgress(float progress)
        {
            ProgressUpdateHandler handler = WorldFileUploadProgress;
            if (handler != null)
            {
                handler(null, new ProgressUpdateEventArgs(progress));
            }
        }

        public static void OnWorldFileUploadCompleted()
        {
            if (WorldFileUploadCompleted != null)
                WorldFileUploadCompleted(null, new EventArgs());
        }

        public static void OnWorldFileUploadFailed(string errorMessage)
        {
            ErrorEventHandler handler = WorldFileUploadFailed;
            if (handler != null)
            {
                handler(null, new ErrorEventArgs(errorMessage));
            }
        }

        public static void OnLoginStarted()
        {
            if (LoginStarted != null)
                LoginStarted(null, new EventArgs());
        }

        public static void OnPlayerWaterEvent(bool underwater)
        {
            WaterEventHandler handler = PlayerWaterEvent;
            if (handler != null)
            {
                handler(null, new WaterEventArgs(underwater));
            }
        }

        public static void OnCameraWaterEvent(bool underwater)
        {
            WaterEventHandler handler = CameraWaterEvent;
            if (handler != null)
            {
                handler(null, new WaterEventArgs(underwater));
            }
        }

        public static void OnZoneEvent(string zoneName, string zoneType)
        {
            ZoneEventHandler handler = ZoneEvent;
            if (handler != null)
            {
                handler(null, new ZoneEventArgs(zoneName, zoneType));
            }
        }

        public static void OnRegionEvent(string zoneName, bool entering)
        {
            RegionEventHandler handler = RegionEvent;
            if (handler != null)
            {
                handler(null, new RegionEventArgs(zoneName, entering));
            }
        }

        public static void OnFrameStarted(float time)
        {
            FrameStartedHandler handler = FrameStarted;
            if (handler != null)
            {
                handler(null, new ScriptingFrameEventArgs(time));
            }
            FrameStartedHandler handler2 = InterfaceFrameStarted;
            if (handler2 != null)
            {
                handler2(null, new ScriptingFrameEventArgs(time));
            }
            // Determine if we have entered a new second
            if (DateTime.Now.Second != currentSecond)
            {
                TimeSecondHandler handler3 = NewSecond;
                if (handler3 != null)
                {
                    handler3(null, new ScriptingFrameEventArgs(time));
                }
            }
        }

        public static void OnFrameEnded(float time)
        {
            FrameEndedHandler handler = FrameEnded;
            if (handler != null)
            {
                handler(null, new ScriptingFrameEventArgs(time));
            }
            FrameEndedHandler handler2 = InterfaceFrameEnded;
            if (handler2 != null)
            {
                handler2(null, new ScriptingFrameEventArgs(time));
            }
        }

        public static void InitAPI(IGameWorld gameWorld)
        {
            
            betaWorld = gameWorld;
            initialized = true;
        }

        public static bool AddInterfaceEventHandler(string eventName, EventHandler d)
        {
            if (eventName == "AdminWorldFilesChanged")
            {
                AdminWorldFilesChanged += d;
                return true;
            }
            else if (eventName == "WorldFileUpdateStarted")
            {
                WorldFileUpdateStarted += d;
                return true;
            }
            else if (eventName == "WorldFileUpdateCompleted")
            {
                WorldFileUpdateCompleted += d;
                return true;
            }
            if (eventName == "WorldFileUploadStarted")
            {
                WorldFileUploadStarted += d;
                return true;
            }
            else if (eventName == "WorldFileUploadCompleted")
            {
                WorldFileUploadCompleted += d;
                return true;
            }
            else if (eventName == "LoginStarted")
            {
                LoginStarted += d;
                return true;
            }
            return false;
        }

        public static void AddProgressEventHandler(string eventName, ProgressUpdateHandler d)
        {
            if (eventName == "WorldFileUpdateProgress")
                WorldFileUpdateProgress += d;
            else if (eventName == "WorldFileUploadProgress")
                WorldFileUploadProgress += d;
        }

        public static void AddErrorEventHandler(string eventName, ErrorEventHandler d)
        {
            if (eventName == "WorldFileUpdateFailed")
                WorldFileUpdateFailed += d;
            else if (eventName == "WorldFileUploadFailed")
                WorldFileUploadFailed += d;
        }

        /// <summary>
        /// Clears the Interface Event Handlers when the UI is reloaded to 
        /// prevent old events from firing off.
        /// </summary>
        public static void ClearInterfaceEventHandlers()
        {
            if (InterfaceFrameStarted != null)
            {
                foreach (Delegate d in InterfaceFrameStarted.GetInvocationList())
                {
                    InterfaceFrameStarted -= (FrameStartedHandler)d;
                }
            }
            if (InterfaceFrameEnded != null)
            {
                foreach (Delegate d in InterfaceFrameEnded.GetInvocationList())
                {
                    InterfaceFrameEnded -= (FrameEndedHandler)d;
                }
            }
            if (AdminWorldFilesChanged != null)
            {
                foreach (Delegate d in AdminWorldFilesChanged.GetInvocationList())
                {
                    AdminWorldFilesChanged -= (EventHandler)d;
                }
            }
            if (WorldFileUpdateStarted != null)
            {
                foreach (Delegate d in WorldFileUpdateStarted.GetInvocationList())
                {
                    WorldFileUpdateStarted -= (EventHandler)d;
                }
            }
            if (WorldFileUpdateProgress != null)
            {
                foreach (Delegate d in WorldFileUpdateProgress.GetInvocationList())
                {
                    WorldFileUpdateProgress -= (ProgressUpdateHandler)d;
                }
            }
            if (WorldFileUpdateCompleted != null)
            {
                foreach (Delegate d in WorldFileUpdateCompleted.GetInvocationList())
                {
                    WorldFileUpdateCompleted -= (EventHandler)d;
                }
            }
            if (WorldFileUpdateFailed != null)
            {
                foreach (Delegate d in WorldFileUpdateFailed.GetInvocationList())
                {
                    WorldFileUpdateFailed -= (ErrorEventHandler)d;
                }
            }
            if (WorldFileUploadStarted != null)
            {
                foreach (Delegate d in WorldFileUploadStarted.GetInvocationList())
                {
                    WorldFileUploadStarted -= (EventHandler)d;
                }
            }
            if (WorldFileUploadProgress != null)
            {
                foreach (Delegate d in WorldFileUploadProgress.GetInvocationList())
                {
                    WorldFileUploadProgress -= (ProgressUpdateHandler)d;
                }
            }
            if (WorldFileUploadCompleted != null)
            {
                foreach (Delegate d in WorldFileUploadCompleted.GetInvocationList())
                {
                    WorldFileUploadCompleted -= (EventHandler)d;
                }
            }
            if (WorldFileUploadFailed != null)
            {
                foreach (Delegate d in WorldFileUploadFailed.GetInvocationList())
                {
                    WorldFileUploadFailed -= (ErrorEventHandler)d;
                }
            }
            if (LoginStarted != null)
            {
                foreach (Delegate d in LoginStarted.GetInvocationList())
                {
                    LoginStarted -= (EventHandler)d;
                }
            }
        }

        public delegate int YieldEffectHandler(object generator);

        public static void QueueYieldEffect(object generator, int milliseconds)
        {
            long time = milliseconds + AtavismTimeTool.CurrentTime;
            List<object> timeList;

            if (effectWorkQueue.ContainsKey(time))
            {
                timeList = effectWorkQueue[time];
            }
            else
            {
                timeList = new List<object>();
                effectWorkQueue[time] = timeList;
            }
            timeList.Add(generator);
        }

        public static void ProcessYieldEffectQueue()
        {
            long currentTime = AtavismTimeTool.CurrentTime;

           
        }

        protected static bool triggerWorldInitialized;

        public static bool TriggerWorldInitialized
        {
            get
            {
                return triggerWorldInitialized;
            }
            set
            {
                triggerWorldInitialized = true;
                OnWorldInitialized();
            }
        }

        protected static Dictionary<string, object> deprecatedCalls = new Dictionary<string, object>();

        public static void ScriptDeprecated(string version, string oldMethod, string newMethod)
        {
            StackTrace t = new StackTrace(true);
            StackFrame f = null;
            int i = 0;
            bool foundPython = false;

            // look for the first stack frame that appears to be in a python script
            for (; i < t.FrameCount; i++)
            {
                f = t.GetFrame(i);
                string filename = f.GetFileName();
                if ((filename != null) && filename.ToLowerInvariant().EndsWith(".py"))
                {
                    foundPython = true;
                    break;
                }
            }

            if (foundPython)
            {
                // generate a string that uniquely identifies a call to a deprecated interface
                //   from a particular line of script code.  We will use this string to avoid
                //   printing the same deprecated message over and over for the same line of code.
                string instanceID = oldMethod + f.GetFileName() + f.GetFileLineNumber().ToString();

                if (!deprecatedCalls.ContainsKey(instanceID))
                {
                    // mark the call instance ID in the 
                    deprecatedCalls[instanceID] = null;

                    // continue from the index where the previous loop found a python file
                    for (; i < t.FrameCount; i++)
                    {
                        f = t.GetFrame(i);
                        string filename = f.GetFileName();
                    }
                }
            }
        }
    }
}