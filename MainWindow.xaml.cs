using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SemestralniPrace01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MeetingCenter selectedCenter = null;
        private MeetingRoom selectedRoom = null;
                
        
        public MainWindow()
        {
            
            InitializeComponent();

            if (File.Exists("save.csv"))
            {
                LoadData("save.csv");
            }            

            LoadCenters();

            saveState = false;
            changes = false;
        }

        //private ObservableCollection<MeetingCenter> meetingCenters = new ObservableCollection<MeetingCenter>();
        List<MeetingCenter> meetingCenters = new List<MeetingCenter>();
        List<MeetingRoom> meetingRooms = new List<MeetingRoom>();
        
        //Podle savState je zjisteno, jestli byla data uz ulozena
        private bool saveState;
        //Podle changes je zjisteno, jestli byly provedeny zmeny v datech
        private bool changes;

        //Nacteni dat z kolekce centers do listboxu
        //Iterace zkrze vsechna Meeting Centra v kolekci
        public void LoadCenters()
        {
            LboxMeetingCenters.Items.Clear();
            foreach (MeetingCenter center in meetingCenters)
            {
                LboxMeetingCenters.Items.Add(center);
            }
           
        }
        //Nacteni dat z kolekce rooms do listboxu
        //Iterace zkrze vsechny Meeting Rooms v kolekci
        private void LoadRooms(string centerCode)
        {
            LboxMeetingRooms.Items.Clear();
            foreach (MeetingRoom room in meetingRooms)
            {
                if (room.CentreCode == centerCode)
                {
                    LboxMeetingRooms.Items.Add(room);
                }
            }
        }

        //Pri smazani centra smaze k nemu nalezici mistnosti
        private void RemoveRooms(string removedCenterCode)
        {            
            meetingRooms.RemoveAll(x => (x.CentreCode != null) && (x.CentreCode == removedCenterCode));
            LboxMeetingRooms.Items.Clear();
        }
        
        //Vymaze data z textboxu Rooms       
        private void ClearRoomTb()
        {
            TbNameMr.Clear();
            TbCodeMr.Clear();
            TbDescriptionMr.Clear();
            TbCapacity.Clear();
            ChbVideoConference.IsChecked = false;
        }

        //Ulozeni dat do souboru
        //Vytvori csv soubor s kodovani UTF-8.
        //Nejdrive zapise vsechny Meeting Centra a pak vsechny Meeting Rooms
        //Po dokonceni operace nebo vyskytu chyby uzavre soubor
        private void SaveToFile()
        {
            TextWriter tw = null;

            try
            {
                tw = new StreamWriter(new FileStream("save.csv", FileMode.Create), Encoding.UTF8);
                tw.WriteLine("MEETING_CENTRES,");
                foreach (MeetingCenter center in meetingCenters)
                {                   
                    tw.WriteLine("{0},{1},{2},", center.Name, center.Code, center.Description);
                }

                tw.WriteLine("MEETING_ROOMS,");
                foreach (MeetingRoom room in meetingRooms)
                {

                    tw.WriteLine("{0},{1},{2},{3},{4},{5},", room.Name, room.Code, room.Description, room.Capacity, (room.VideoConference == true) ? "YES" : "NO", room.CentreCode);
                }

                MessageBox.Show("Save was successfull.");
            }
            catch (IOException e)
            {
                
                MessageBox.Show("Error during writing!");
                throw e;
            }
            finally
            {
                if (tw != null)
                    tw.Close();
            }
        }

        //Nacteni dat z csv souboru
        //Nacte data po radcich do pole a pote headeru vytvori prislusne instance objektu
        //a ulozi je do kolekce
        
        private void LoadData(string path)
        {
            try
            {
                string[] reader = File.ReadAllLines(path);
                List<Array> arrayList = new List<Array>();

                foreach (var item in reader)
                {
                    var values = item.Split(',');

                    string[] records = values;
                    arrayList.Add(records);
                }

                meetingCenters.Clear();
                meetingRooms.Clear();

                bool meetingType = true;
                foreach (Array item in arrayList)
                {
                    if ((item.GetValue(0).ToString() == "MEETING_CENTRES"))
                    {
                        meetingType = true;
                    }
                    else if (item.GetValue(0).ToString() == "MEETING_ROOMS")
                    {
                        meetingType = false;
                    }

                    if ((item.GetValue(0).ToString() != "MEETING_CENTRES") && (item.GetValue(0).ToString() != "MEETING_ROOMS"))
                    {
                        if (meetingType == true)
                        {
                            MeetingCenter center = new MeetingCenter(item.GetValue(0).ToString(), item.GetValue(1).ToString(), item.GetValue(2).ToString());
                            meetingCenters.Add(center);
                        }
                        else
                        {

                            MeetingRoom room = new MeetingRoom(item.GetValue(0).ToString(), item.GetValue(1).ToString(), item.GetValue(2).ToString(),
                                Convert.ToInt32(item.GetValue(3)), (item.GetValue(4).ToString() == "YES") ? true : false, item.GetValue(5).ToString());
                            meetingRooms.Add(room);
                        }
                    }
                }

                LoadCenters();
            }
            catch (FileLoadException e)
            {
                throw e;
            }
        }


        //Otevreni dialogoveho okna pro zadani noveho Meeting Center
        //Po vyplneni noveho Meeting Centra projde kolekci do ktere byla ulozena jeho instance
        //a prida ho k ostatnim
        //Pote je zaznamenana zmena v datech a jsou nactena centra z aktualizovane kolekce
        private void BtnNewMc_Click(object sender, RoutedEventArgs e)
        {
            MeetingCenterDialogWindow newMeetingCenter = new MeetingCenterDialogWindow();
            newMeetingCenter.ShowDialog();
            
            foreach (MeetingCenter item in newMeetingCenter.newCenter)
            {
                meetingCenters.Add(item);
            }
            if (newMeetingCenter.centersChange == true)
                changes = true;
            LoadCenters();
        }

        //Otevreni dialogoveho okna pro zadani nove Meeting Room        
        //Do comboboxu jsou nactena Meeting Centra, ktera jsou k dispozici
        //Po vyplneni nove Meeting Room projde kolekci do ktere byla ulozena jeho instance
        //a prida ho k ostatnim
        //Pote je zaznamenana zmena v datech
        private void BtnNewMR_Click(object sender, RoutedEventArgs e)
        {
            MeetingRoomDialogWindow newMeetingRoom = new MeetingRoomDialogWindow();
            foreach (MeetingCenter center in meetingCenters)
            {
                newMeetingRoom.CbMeetingCenterDialogMr.Items.Add(center.Code);
            }
            
            newMeetingRoom.ShowDialog();

            foreach (MeetingRoom item in newMeetingRoom.newRoom)
            {
                meetingRooms.Add(item);
            }

            if (newMeetingRoom.roomsChange == true)
                changes = true;
            
        }

        //Otevreni okna pro vyber souboru s daty a jejich nacteni
        //Import dat ze souboru a jejich zpracovani        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV files (*.csv)|*.csv";
            bool? result = ofd.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                string path = ofd.FileName;
                // Vyjimky                
                LoadData(path);                
            }            
        }


        //Zobrazeni detailu vybraneho centra a k nemu nalezijicich mistnosti
        //Detaily o Meeting Centru jsou propsany do textboxu
        private void LboxMeetingCenters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCenter = (MeetingCenter)LboxMeetingCenters.SelectedItem;

            if (selectedCenter != null)
            {
                TbNameMc.Text = selectedCenter.Name;
                TbCodeMc.Text = selectedCenter.Code;
                TbDescriptionMc.Text = selectedCenter.Description;

                LoadRooms(selectedCenter.Code);
            }   
        }

        //Zobrazeni detailu vybrane mistnosti do textboxu       
        private void LboxMeetingRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRoom = (MeetingRoom)LboxMeetingRooms.SelectedItem;

            if (selectedRoom != null)
            {
                TbNameMr.Text = selectedRoom.Name;
                TbCodeMr.Text = selectedRoom.Code;
                TbDescriptionMr.Text = selectedRoom.Description;
                TbCapacity.Text = selectedRoom.Capacity.ToString();
                ChbVideoConference.IsChecked = selectedRoom.VideoConference;                
            }
        }

        //Vyvola editaci vybraneho centra
        private void BtnEditMc_Click(object sender, RoutedEventArgs e)
        {
            selectedCenter = (MeetingCenter)LboxMeetingCenters.SelectedItem;
            MeetingCenterDialogWindow editMeetingCenter = new MeetingCenterDialogWindow();

            if (selectedCenter != null)
            {
                editMeetingCenter.TbNameDialogMc.Text = selectedCenter.Name;
                editMeetingCenter.TbCodeDialogMc.Text = selectedCenter.Code;
                editMeetingCenter.TbDescriptionDialogMc.Text = selectedCenter.Description;

                editMeetingCenter.ShowDialog();

                foreach (MeetingCenter item in editMeetingCenter.newCenter)
                {
                    int index = meetingCenters.IndexOf(selectedCenter);
                    meetingCenters[index] = item;
                }

                if (editMeetingCenter.centersChange == true)
                    changes = true;

                LoadCenters();
            }
            else
            {
                MessageBox.Show("You have select a Meeting Center!");
            }
            
        }

        //Vyvola editaci vybrane mistnosti
        private void BtnEditMR_Click(object sender, RoutedEventArgs e)
        {
            selectedRoom = (MeetingRoom)LboxMeetingRooms.SelectedItem;
            MeetingRoomDialogWindow editMeetingRoom = new MeetingRoomDialogWindow();

            if (selectedRoom != null)
            {
                foreach (MeetingCenter center in meetingCenters)
                {
                    editMeetingRoom.CbMeetingCenterDialogMr.Items.Add(center.Code);
                }

                editMeetingRoom.TbNameDialogMr.Text = selectedRoom.Name;
                editMeetingRoom.TbCodeDialogMr.Text = selectedRoom.Code;
                editMeetingRoom.TbDescriptionDialogMr.Text = selectedRoom.Description;
                editMeetingRoom.TbCapacityDialogMr.Text = selectedRoom.Capacity.ToString();
                editMeetingRoom.CbMeetingCenterDialogMr.SelectedValue = selectedRoom.CentreCode;
                editMeetingRoom.ChbVideoConferenceDialogMr.IsChecked = selectedRoom.VideoConference;

                editMeetingRoom.ShowDialog();

                foreach (MeetingRoom item in editMeetingRoom.newRoom)
                {
                    int index = meetingRooms.IndexOf(selectedRoom);
                    meetingRooms[index] = item;
                }

                if (editMeetingRoom.roomsChange == true)
                    changes = true;
            }
            else
            {
                MessageBox.Show("You have select a Meeting Room!");
            }
            
           
        }

        //Smaze vybrane centrum
        private void BtnDeleteMc_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete Meeting Center", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                selectedCenter = (MeetingCenter)LboxMeetingCenters.SelectedItem;
                if (selectedCenter != null)
                {
                    RemoveRooms(selectedCenter.Code);
                }
                meetingCenters.Remove(selectedCenter);
                LboxMeetingCenters.Items.Remove(selectedCenter);

                ClearRoomTb();

                TbNameMc.Clear();
                TbCodeMc.Clear();
                TbDescriptionMc.Clear();

                changes = true;
            }
        }

        //Smaze vybranou mistnost
        private void BtnDeleteMR_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete Meeting Room", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                selectedRoom = (MeetingRoom)LboxMeetingRooms.SelectedItem;
                meetingRooms.Remove(selectedRoom);
                LboxMeetingRooms.Items.Remove(selectedRoom);

                ClearRoomTb();

                changes = true;
            }
            
        }

        //Ulozeni dat do souboru pres tlacitko v menu
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveToFile();
            saveState = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (saveState == false && changes == true)
            {
                if (MessageBox.Show("Do you wish to save changes?", "Save changes", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveToFile();
                }
            }
        }
    }
}
