using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Logging
{
    class ILog
    {

        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public void DebugFormat(string str, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(str, args);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public void Warn(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public void Warn(string str, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public void WarnFormat(string str, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(str, args);
        }
        [System.Diagnostics.Conditional("TRACE")]
        public void Trace(string str)
        {
            System.Diagnostics.Trace.WriteLine(str);
        }
        [System.Diagnostics.Conditional("TRACE")]
        public void TraceFormat(string str, params object[] args)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(str, args));
        }


        public bool IsTraceEnabled
        {
            get
            {
#if TRACE
                return true;
#else
                return false;
#endif
            }
        }
        
    }

    class LogManager : ILog
    {
        private readonly static ILog dummy = new ILog();
        public static ILog GetCurrentClassLogger()
        {
            return dummy;
        }


    }

}
