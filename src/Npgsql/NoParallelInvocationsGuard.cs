using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    internal class NoParallelInvocationsGuard
    {
#if !NETSTANDARD
        private string StackTrace;
#endif
        private int busy;
        public object Data { get; private set; }
        private void Start(object data = null)
        {
            if (Interlocked.Increment(ref busy) != 1)
                System.Diagnostics.Debugger.Break();
#if !NETSTANDARD
            StackTrace = new StackTrace().ToString();
#endif
            this.Data = data;

        }

        public IDisposable Scope(object data = null)
        {
#if true
            return null;
#else
            Start(data);
            return new DelegateDisposable() { Action = () => End() };
#endif
        }


        private void End()
        {
            Interlocked.Decrement(ref busy);
        }

        private class DelegateDisposable : IDisposable
        {
            public Action Action;
            public void Dispose()
            {
                var k = Interlocked.Exchange(ref Action, null);
                if (k != null) k();
            }
        }
    }






}
