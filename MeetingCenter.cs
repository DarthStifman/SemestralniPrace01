using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemestralniPrace01
{
    class MeetingCenter : Building
    {

        public List<MeetingRoom> MeetingRooms { get; set; } = new List<MeetingRoom>();

        public MeetingCenter(string name, string code, string description, List<MeetingRoom> meetingRooms) : base(name, code, description)
        {
            MeetingRooms = meetingRooms;
        }
    }
}
