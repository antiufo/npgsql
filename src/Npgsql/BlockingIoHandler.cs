using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql
{
    public static class BlockingIoHandler
    {
        public static void Check()
        {
            var c = CheckDelegate;
            if (c != null) c();
        }

        public static Action CheckDelegate;
    }
}
