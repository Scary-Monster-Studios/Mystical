//using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
namespace Atavism
{

    public interface IWorldManager
    {

        AtavismNetworkHelper NetworkHelper
        {
            get;
            set;
        }

        bool PlayerInitialized
        {
            get;
        }

        long PlayerId
        {
            set;
            get;
        }

        // Kludge: This is referenced by interface IWorldManager, and
        // interfaces can't refer to static properties, make
        // implementors define this non-static property
        long CurrentTimeValue
        {
            get;
        }

        //string ServerCapabilities {
        //    set;
        //}

        void AdjustTimestamp(long timestamp);

        void RequestShutdown(string message);
    }
}