using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3_PlaylistMusic
{
    class Song
    {
        private int _stt;
        private string _name;
        private string time;
        public int STT { get => _stt; set => _stt = value; }
        public string Name { get => _name; set => _name = value; }
        public string Time { get => time; set => time = value; }
    }
}
