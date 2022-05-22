//using UnityEngine;
//using System.Collections;
namespace Atavism
{

    public class AtavismTimeTool
    {

        /// <summary>
        ///   Used to override the client's idea of time, for use in
        ///   some automation tasks.
        /// </summary>
        protected static long timeOverride = 0;
        /// <summary>
        ///   Keep track of the last timestamp, so that we can have
        ///   monotonically non-decreasing timestamps
        /// </summary>
        protected static long lastTimestamp = 0;
        /// <summary>
        ///   Stash this number here, so we don't have to recalculate it
        /// </summary>
        protected const long intRange = (long)int.MaxValue - (long)int.MinValue + 1;

        public static long TimeOverride
        {
            get
            {
                return timeOverride;
            }
            set
            {
                timeOverride = value;
            }
        }

        public static long CurrentTime
        {
            get
            {
                long timestamp;
                if (timeOverride == 0)
                {
                    timestamp = System.Environment.TickCount;
                    while (timestamp < lastTimestamp)
                    {
                        timestamp += intRange;
                    }
                    lastTimestamp = timestamp;
                }
                else
                {
                    timestamp = timeOverride;
                }
                return timestamp;
            }
        }
    }
}