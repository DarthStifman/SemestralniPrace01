using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemestralniPrace01
{
    public class MeetingRoom: MeetingCenter
    {
        public int Capacity { get; set; }
        public bool VideoConference { get; set; }
        public string CentreCode { get; set; }

        public MeetingRoom(string name, string code, string description, int capacity, bool videoConference, string centerCode) : base(name, code, description)
        {
            Capacity = capacity;
            VideoConference = videoConference;
            CentreCode = centerCode;
        }

        //Prepsani metody ToString() tak, aby v listboxu vypsala Name, Code, Capacity a CentreCode
        public override string ToString()
        {
            return Name + "  " + Code + "  " + Capacity + "  " + CentreCode;
        }
    }
}
