using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemestralniPrace01
{
    class MeetingRoom: Building
    {
        private int Capacity { get; set; }
        private bool VideoConference { get; set; }
        public List<MeetingCenter> MeetingCenters { get; set; } = new List<MeetingCenter>();

        public MeetingRoom(string name, string code, string description, int capacity, bool videoConference, List<MeetingCenter> meetingCenters) : base(name, code, description)
        {
            Capacity = capacity;
            VideoConference = videoConference;
            MeetingCenters = meetingCenters;
        }
    }
}
