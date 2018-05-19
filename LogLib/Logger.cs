using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLib
{
    public class Logger : ILogger
    {
        public void Wright(LogLvl lvl, object sender, string message)
        {
        }

        public void WrightFormat(LogLvl lvl, object sender, params object[] message)
        {
        }
    }
}
