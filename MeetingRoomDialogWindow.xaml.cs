using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MeetingRoomDialogWindow.xaml
    /// </summary>
    public partial class MeetingRoomDialogWindow : Window
    {
        public MeetingRoomDialogWindow()
        {
            InitializeComponent();
        }

        public List<MeetingRoom> newRoom = new List<MeetingRoom>();
        public bool roomsChange;
        //Regularni vyraz pro Code
        Regex rgx = new Regex("[a-zA-Z_:-]{5, 50}");

        //Pri  stisknuti talcitka OK overi delku vsech retezcu a regularni vyraz. 
        //Nacte data z textboxu, vytvori instanci tridy MeetingRoom a ulozi ji do kolekce newRoom. 
        //Pokud nektera z validaci selze, tak je zobrazeno okno s popisem chyby.        
        private void BtnOkMr_Click(object sender, RoutedEventArgs e)
        {
            bool state;
            if (ChbVideoConferenceDialogMr.IsChecked.Value == true)
            {
                state = true;
            }
            else
            {
                state = false;
            }
            if (((TbNameDialogMr.Text.Length >= 2) && (TbNameDialogMr.Text.Length <= 100)) && (rgx.IsMatch(TbCodeDialogMr.Text)) 
                && ((TbDescriptionDialogMr.Text.Length >= 10) && (TbDescriptionDialogMr.Text.Length <= 300)) && 
                ((TbCapacityDialogMr.Text.Length >= 1) && (TbCapacityDialogMr.Text.Length <= 100)) && (CbMeetingCenterDialogMr.SelectedValue != null))
            {
                MeetingRoom room = new MeetingRoom(TbNameDialogMr.Text, TbCodeDialogMr.Text, TbDescriptionDialogMr.Text, Convert.ToInt32(TbCapacityDialogMr.Text),
                   state, CbMeetingCenterDialogMr.Text);
                newRoom.Add(room);
                Close();
                roomsChange = true;
            }
            else
            {
                if ((TbNameDialogMr.Text.Length < 2) || (TbNameDialogMr.Text.Length > 100))
                {
                    MessageBox.Show("Name must have 2-100 characters!");
                }                
                else if ((!rgx.IsMatch(TbCodeDialogMr.Text)))
                {
                    MessageBox.Show("Code must have 5-50 characters (a-z, A-Z, -, :, _) !");
                }
                else if ((TbDescriptionDialogMr.Text.Length < 10) || (TbDescriptionDialogMr.Text.Length > 300))
                {
                    MessageBox.Show("Code must have 10-300 characters!");
                }
                else if ((TbCapacityDialogMr.Text.Length < 1) || (TbCapacityDialogMr.Text.Length > 100))
                {
                    MessageBox.Show("Code must have 1-100 characters!");
                }
                else if (CbMeetingCenterDialogMr.SelectedValue == null)
                {
                    MessageBox.Show("Choose Meeting Center!");
                }
            }
        }

        //Pri stisknuti Cancel se okno uzavre.
        private void BtnCancelMr_Click(object sender, RoutedEventArgs e)
        {
            Close();
            roomsChange = false;
        }

        private void CbMeetingCenterDialogMr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
        }
    }
}
