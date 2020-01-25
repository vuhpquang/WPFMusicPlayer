using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3_PlaylistMusic
{
    class Utils
    {
        public static string GetExtension(string filename)
        {
            string result = "";

            int index = filename.LastIndexOf('.');
            result = filename.Substring(index + 1);

            return result;
        }
    }
}
