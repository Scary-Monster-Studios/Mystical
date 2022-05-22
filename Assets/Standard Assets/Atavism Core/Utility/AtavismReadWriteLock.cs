//using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Diagnostics;
namespace Atavism
{

    public class AtavismReadWriteLock
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        int readerCount = 0;
        int writerCount = 0;
        int writersWaiting = 0;
#if DEBUG_THREADS
		StackTrace writeLockHolder = null;
#endif

        public AtavismReadWriteLock()
        {
        }

        public void BeginRead()
        {
            Monitor.Enter(this);
            while (writerCount != 0 || writersWaiting != 0)
                Monitor.Wait(this);
            readerCount++;
            Monitor.Exit(this);
        }
        public void EndRead()
        {
            Monitor.Enter(this);
            readerCount--;
            Monitor.PulseAll(this);
            Monitor.Exit(this);
        }
        public void BeginWrite()
        {
            Monitor.Enter(this);
            writersWaiting++;
            while (readerCount != 0 || writerCount != 0)
                Monitor.Wait(this);
            writersWaiting--;
            writerCount++;
#if DEBUG_THREADS
			writeLockHolder = new StackTrace(true);
#endif
            Monitor.Exit(this);
        }
        public void EndWrite()
        {
            Monitor.Enter(this);
            writerCount--;
#if DEBUG_THREADS
			writeLockHolder = null;
#endif
            Monitor.PulseAll(this);
            Monitor.Exit(this);
        }
    }
}