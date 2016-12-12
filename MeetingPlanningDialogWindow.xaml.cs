using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SemestralniPrace01
{
    /// <summary>
    /// Interaction logic for MeetingPlanningDialogWindow.xaml
    /// </summary>
    public partial class MeetingPlanningDialogWindow : Window
    {
        public MeetingPlanningDialogWindow()
        {
            InitializeComponent();
        }

        public List<MeetingPlan> newMeeting = new List<MeetingPlan>();
        public List<MeetingPlan> meetingsToCompare = new List<MeetingPlan>();
        public bool meetingChange;

        public int RoomCapacity;
        private bool timeCollision;
        
        private void BtnOkMeeting_Click(object sender, RoutedEventArgs e)
        {
            bool state;
            if (ChbMeetingVideoConferenceMeeting.IsChecked.Value == true)
            {
                state = true;
            }
            else
            {
                state = false;
            }

            DateTime date = DateTime.ParseExact(TbDateNewMeeting.Text, "dd.MM.yyyy", null);
            DateTime timeFrom = DateTime.ParseExact(TbDateNewMeeting.Text + " " + TbMeetingHoursFrom.Text + ":" + TbMeetingMinutesFrom.Text, "dd.MM.yyyy HH:mm", null);
            DateTime timeTo = DateTime.ParseExact(TbDateNewMeeting.Text + " " + TbMeetingHoursTo.Text + ":" + TbMeetingMinutesTo.Text, "dd.MM.yyyy HH:mm", null);

            foreach (MeetingPlan meeting in meetingsToCompare)
            {
                if (((timeFrom >= meeting.TimeFrom) && (timeFrom <= meeting.TimeTo)) || ((timeTo >= meeting.TimeFrom) && (timeTo <= meeting.TimeTo)) 
                    || ((timeFrom < meeting.TimeFrom) && (timeTo > meeting.TimeTo)))
                {
                    timeCollision = true;
                    break;
                }
                else
                {
                    timeCollision = false;
                }               
            }


            if ((TbMeetingHoursFrom.Text.Length == 2) && (TbMeetingMinutesFrom.Text.Length == 2) && (TbMeetingHoursTo.Text.Length == 2) && (TbMeetingMinutesTo.Text.Length == 2) 
                && (timeCollision == false) && (timeFrom < timeTo) && ((Convert.ToInt32(TbMeetingExpectedPersonsCount.Text) <= RoomCapacity) && 
                (Convert.ToInt32(TbMeetingExpectedPersonsCount.Text) >= 1)) && ((TbMeetingCustomer.Text.Length >= 2) && (TbMeetingCustomer.Text.Length <= 100)) 
                && (TbMeetingNote.Text.Length <= 300))
            {
                

                MeetingPlan meeting = new MeetingPlan(TbMeetingCenterNewMeeting.Text, TbMeetingRoomNewMeeting.Text, date, timeFrom,
                timeTo, Convert.ToInt32(TbMeetingExpectedPersonsCount.Text), TbMeetingCustomer.Text, state, TbMeetingNote.Text);

                newMeeting.Add(meeting);

                Close();
                meetingChange = true;
            }
            else
            {
                if ((Convert.ToInt32(TbMeetingExpectedPersonsCount.Text) > RoomCapacity) || (Convert.ToInt32(TbMeetingExpectedPersonsCount.Text) < 1))
                {
                    MessageBox.Show("Expected persons count must be in range: 1-" + RoomCapacity);
                }
                else if ((TbMeetingHoursFrom.Text.Length != 2) || (TbMeetingMinutesFrom.Text.Length != 2) || (TbMeetingHoursTo.Text.Length != 2) || (TbMeetingMinutesTo.Text.Length != 2))
                {
                    MessageBox.Show("Time of the meeting must be in format: HH:mm - HH:mm!");
                }
                else if ((TbMeetingCustomer.Text.Length < 2) || (TbMeetingCustomer.Text.Length > 100))
                {
                    MessageBox.Show("Customer must be 2-100 characters long!");
                }               
                else if (TbMeetingNote.Text.Length > 300)
                {
                    MessageBox.Show("Note must be shorter than 300 characters!");
                }
                else if (timeCollision == true)
                {
                    MessageBox.Show("Time of your meeting collide´s with another one!");
                }
                else if (timeFrom > timeTo)
                {
                    MessageBox.Show("Beginning of meeting must be set sooner than the end!");
                }
            }
            
        }

        private void BtnCancelMeeting_Click(object sender, RoutedEventArgs e)
        {
            Close();
            meetingChange = false;
        }

        
    }
}
