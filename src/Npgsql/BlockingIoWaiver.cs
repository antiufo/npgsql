using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shaman.Runtime
{
    public class BlockingIoWaiver : IDisposable
    {
        [ThreadStatic]
        private static int waiverDepth;

        private bool disposed;


        public static void Check()
        {
            if (waiverDepth == 0 && SynchronizationContext.Current != null)
            {
                Debugger.Break();
            }
        }


        public static IDisposable Create()
        {
            return new BlockingIoWaiver();
        }

        private BlockingIoWaiver()
        {
            waiverDepth++;
        }


        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                waiverDepth--;
            }
        }

    }
}
