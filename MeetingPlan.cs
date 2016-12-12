using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemestralniPrace01
{
    public class MeetingPlan
    {
        public string MeetingCenterCode { get; set; }
        public string MeetingRoomCode { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public int ExpectedPersonsCount { get; set; }
        public string Customer { get; set; }
        public bool VideoConferencePlan { get; set; }
        public string Note { get; set; }

        public MeetingPlan(string meetingCenterCode, string meetingRoomCode, DateTime date, DateTime timeFrom, DateTime timeTo,
            int expectedPerosnsCount, string customer, bool videoConferencePlan, string note)
        {
            MeetingCenterCode = meetingCenterCode;
            MeetingRoomCode = meetingRoomCode;
            Date = date;
            TimeFrom = timeFrom;
            TimeTo = timeTo;
            ExpectedPersonsCount = expectedPerosnsCount;
            Customer = customer;
            VideoConferencePlan = videoConferencePlan;
            Note = note;
        }


        public override string ToString()
        {
            return "From: " + TimeFrom.ToString("HH:mm") + "  -  " + "To: " + TimeTo.ToString("HH:mm") + "  -  " + "Persons: " + ExpectedPersonsCount ;
        }


    }
}
