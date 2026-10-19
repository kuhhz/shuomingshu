using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorProofing
{
    internal class SystemConfig
    {
        public static int MaxQueueSize { get; set; }
        public static int LoadMaxQueueSize()
        {
            string value = ConfigurationManager.AppSettings["MaxQueueSize"];

            if (int.TryParse(value, out int result) && result > 0)
                return result;

            return 3;
        }
    }

}
