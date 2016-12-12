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
using System.Xml;


namespace SemestralniPrace01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MeetingCenter selectedCenter = null;
        private MeetingRoom selectedRoom = null;
        private MeetingPlan selectedMeeting = null;

        private string selectedCenterPlanning = null;
        private string selectedRoomPlanning = null;    
        
        public MainWindow()
        {
            
            InitializeComponent();

            if (File.Exists("save.csv"))
            {
                LoadData("save.csv");
            }
            if (File.Exists("xmlMeetings.xml"))
            {
                LoadXmlData();
            }           

            LoadCenters();
            FillCenterComboBox();

            saveState = false;
            changes = false;
        }

        //private ObservableCollection<MeetingCenter> meetingCenters = new ObservableCollection<MeetingCenter>();
        List<MeetingCenter> meetingCenters = new List<MeetingCenter>();
        List<MeetingRoom> meetingRooms = new List<MeetingRoom>();
        List<MeetingPlan> meetingPlans = new List<MeetingPlan>();

        Dictionary<string, string[]> jsonData = new Dictionary<string, string[]>();

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

        //Nacte vsechna Meeting Centers do comboboxu v Meetings Planning
        public void FillCenterComboBox()
        {
            CbMeetingCenterPlanning.Items.Clear();
            foreach (MeetingCenter center in meetingCenters)
            {
                if (center != null)
                {
                    CbMeetingCenterPlanning.Items.Add(center.Code);
                }
            }
        }

        //Nacte vsechna Meeting Centers, ktera nalezi vybranemu Meeting Center do comboboxu v Meetings Planning
        public void FillRoomComboBox(string centerCode)
        {
            CbMeetingRoomPlanning.Items.Clear();
            foreach (MeetingRoom room in meetingRooms)
            {
                if (room.CentreCode == centerCode)
                {
                    CbMeetingRoomPlanning.Items.Add(room.Code);
                }
            }
        }

        //Nacteni dat z kolekce rooms do listboxu
        //Iterace skrze vsechny Meeting Rooms v kolekci
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

        //Nacteni dat z kolekce meetings do listboxu
        //Iterace skrze vsechny Meetingy v kolekci
        private void LoadMeetings(string roomCode)
        {
            LboxMeetings.Items.Clear();
            meetingPlans.Sort((x, y) => DateTime.Compare(x.TimeFrom, y.TimeFrom));

            foreach (MeetingPlan meeting in meetingPlans)
            {
                if (meeting.MeetingRoomCode == roomCode)
                {
                    if (meeting.Date.Date == DpDate.SelectedDate)
                    {
                        LboxMeetings.Items.Add(meeting);
                    }                    
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

        //Vymaze data z textbnoxu Meetings Detail
        private void ClearMeetingsDetailTb()
        {
            TbHoursFrom.Clear();
            TbMinutesFrom.Clear();
            TbHoursTo.Clear();
            TbMinutesTo.Clear();
            TbExpectedPersonsCount.Clear();
            TbCustomer.Clear();
            ChbVideoConferencePlanning.IsChecked = false;
            TbNote.Clear();
        }

        //Ulozeni dat do souboru
        //Vytvori csv soubor s kodovanim UTF-8.
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
            selectedCenter = (MeetingCenter)LboxMeetingCenters.SelectedItem;
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

            LoadRooms(selectedCenter.Code);
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
                MessageBox.Show("You must select a Meeting Center!");
            }
            
        }

        //Vyvola editaci vybrane mistnosti
        private void BtnEditMR_Click(object sender, RoutedEventArgs e)
        {
            selectedRoom = (MeetingRoom)LboxMeetingRooms.SelectedItem;
            selectedCenter = (MeetingCenter)LboxMeetingCenters.SelectedItem;
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

                LoadRooms(selectedCenter.Code);
            }
            else
            {
                MessageBox.Show("You must select a Meeting Room!");
            }
            
           
        }

        //Smaze vybrane centrum
        private void BtnDeleteMc_Click(object sender, RoutedEventArgs e)
        {
            selectedCenter = (MeetingCenter)LboxMeetingCenters.SelectedItem;

            if (selectedCenter != null)
            {
                if (MessageBox.Show("Are you sure?", "Delete Meeting Center", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    RemoveRooms(selectedCenter.Code);

                    meetingCenters.Remove(selectedCenter);
                    LboxMeetingCenters.Items.Remove(selectedCenter);

                    ClearRoomTb();

                    TbNameMc.Clear();
                    TbCodeMc.Clear();
                    TbDescriptionMc.Clear();

                    changes = true;
                }
            }
            else
            {
                MessageBox.Show("You must select a Meeting Center to delete first!");
            }
            
        }

        //Smaze vybranou mistnost
        private void BtnDeleteMR_Click(object sender, RoutedEventArgs e)
        {
            selectedRoom = (MeetingRoom)LboxMeetingRooms.SelectedItem;

            if (selectedRoom != null)
            {
                if (MessageBox.Show("Are you sure?", "Delete Meeting Room", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    meetingRooms.Remove(selectedRoom);
                    LboxMeetingRooms.Items.Remove(selectedRoom);

                    ClearRoomTb();

                    changes = true;
                }
            }
            else
            {
                MessageBox.Show("You must select a Meeting Room to delete first!");
            }
            
        }

        //Ulozeni dat do souboru pres tlacitko v menu
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveToFile();
            SaveToXml();
            saveState = true;
        }

        //Pri uzavreni hlavniho okna se zepta na ulozeni dat pokud doslo k neajkym zmenam
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (saveState == false && changes == true)
            {
                if (MessageBox.Show("Do you wish to save changes?", "Save changes", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveToFile();
                    SaveToXml();
                }
            }
        }

        //Otevre dialogove okno pro vytvoreni noveho meetingu a nacte vybrana data (Meeting Center, Meeting Room a datum)
        private void BtnNewMeeting_Click(object sender, RoutedEventArgs e)
        {
            MeetingPlanningDialogWindow newMeetingPlan = new MeetingPlanningDialogWindow();
            selectedRoomPlanning = (string)CbMeetingRoomPlanning.SelectedItem;
            

            newMeetingPlan.TbMeetingCenterNewMeeting.Text = CbMeetingCenterPlanning.Text;
            newMeetingPlan.TbMeetingRoomNewMeeting.Text = CbMeetingRoomPlanning.Text;
            newMeetingPlan.TbDateNewMeeting.Text = DpDate.Text;

            foreach (MeetingRoom room in meetingRooms)
            {
                if (selectedRoomPlanning == room.Code)
                {
                    newMeetingPlan.RoomCapacity = room.Capacity;
                }
            }

            newMeetingPlan.meetingsToCompare.Clear();
            foreach (MeetingPlan meeting in meetingPlans)
            {
                if (meeting.Date.Date == DpDate.SelectedDate)
                {
                    newMeetingPlan.meetingsToCompare.Add(meeting);
                }
            }

            newMeetingPlan.ShowDialog();

            foreach (MeetingPlan plan in newMeetingPlan.newMeeting)
            {
                meetingPlans.Add(plan);                
            }

            if (newMeetingPlan.meetingChange == true)
                changes = true;

            LoadMeetings(selectedRoomPlanning);
        }

        //Pri vybrani Meeting Centra v zalozce pro planovani, vyvola nacteni prislusicich Meeting Rooms do comboboxu
        private void CbMeetingCenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCenterPlanning = (string)CbMeetingCenterPlanning.SelectedItem;

            if (selectedCenterPlanning != null)
            {
                FillRoomComboBox(selectedCenterPlanning);
            }
        }
       
        private void CbMeetingRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRoomPlanning = (string)CbMeetingRoomPlanning.SelectedItem;

            if (selectedRoomPlanning != null)
            {
                LoadMeetings(selectedRoomPlanning);
            }
            
        }

        private void LboxMeetings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMeeting = (MeetingPlan)LboxMeetings.SelectedItem;

            if (selectedMeeting != null)
            {
                TbHoursFrom.Text = selectedMeeting.TimeFrom.ToString("HH");
                TbMinutesFrom.Text = selectedMeeting.TimeFrom.ToString("mm");
                TbHoursTo.Text = selectedMeeting.TimeTo.ToString("HH");
                TbMinutesTo.Text = selectedMeeting.TimeTo.ToString("mm");

                TbExpectedPersonsCount.Text = selectedMeeting.ExpectedPersonsCount.ToString();
                TbCustomer.Text = selectedMeeting.Customer;
                ChbVideoConferencePlanning.IsChecked = selectedMeeting.VideoConferencePlan;
                TbNote.Text = selectedMeeting.Note;
            }

        }

        private void BtnEditMeeting_Click(object sender, RoutedEventArgs e)
        {
            selectedMeeting = (MeetingPlan)LboxMeetings.SelectedItem;
            selectedRoomPlanning = (string)CbMeetingRoomPlanning.SelectedItem;
            MeetingPlanningDialogWindow editMeeting = new MeetingPlanningDialogWindow();

            if (selectedMeeting != null)
            {
                editMeeting.TbMeetingCenterNewMeeting.Text = CbMeetingCenterPlanning.Text;
                editMeeting.TbMeetingRoomNewMeeting.Text = CbMeetingRoomPlanning.Text;
                editMeeting.TbDateNewMeeting.Text = DpDate.Text;

                editMeeting.TbMeetingHoursFrom.Text = selectedMeeting.TimeFrom.ToString("HH");
                editMeeting.TbMeetingMinutesFrom.Text = selectedMeeting.TimeFrom.ToString("mm");
                editMeeting.TbMeetingHoursTo.Text = selectedMeeting.TimeTo.ToString("HH");
                editMeeting.TbMeetingMinutesTo.Text = selectedMeeting.TimeTo.ToString("mm");

                editMeeting.TbMeetingExpectedPersonsCount.Text = selectedMeeting.ExpectedPersonsCount.ToString();
                editMeeting.TbMeetingCustomer.Text = selectedMeeting.Customer;
                editMeeting.ChbMeetingVideoConferenceMeeting.IsChecked = selectedMeeting.VideoConferencePlan;
                editMeeting.TbMeetingNote.Text = selectedMeeting.Note;

                foreach (MeetingRoom room in meetingRooms)
                {
                    if (selectedRoomPlanning == room.Code)
                    {
                        editMeeting.RoomCapacity = room.Capacity;
                    }
                }

                editMeeting.meetingsToCompare.Clear();
                foreach (MeetingPlan meeting in meetingPlans)
                {                    
                    if (meeting.Date.Date == DpDate.SelectedDate)
                    {
                        if (selectedMeeting != meeting)
                        {
                            editMeeting.meetingsToCompare.Add(meeting);
                        }
                        
                    }
                }


                editMeeting.ShowDialog();

                foreach (MeetingPlan item in editMeeting.newMeeting)
                {
                    int index = meetingPlans.IndexOf(selectedMeeting);
                    meetingPlans[index] = item;
                }

                LoadMeetings(selectedRoomPlanning);

                if (editMeeting.meetingChange == true)
                    changes = true;
            }
            else
            {
                MessageBox.Show("You must select a Meeting!");
            }
        }

        private void BtnDeleteMeeting_Click(object sender, RoutedEventArgs e)
        {
            selectedMeeting = (MeetingPlan)LboxMeetings.SelectedItem;

            if (selectedMeeting != null)
            {
                if (MessageBox.Show("Are you sure?", "Delete Meeting Room", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    meetingPlans.Remove(selectedMeeting);
                    LboxMeetings.Items.Remove(selectedMeeting);

                    ClearMeetingsDetailTb();

                    changes = true;
                }
            }
            else
            {
                MessageBox.Show("You must select a Meeting to delete first!");
            }
            
        }

        private void DpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRoomPlanning = (string)CbMeetingRoomPlanning.SelectedItem;

            LoadMeetings(selectedRoomPlanning);

            ClearMeetingsDetailTb();
        }


        private void SaveToXml()
        {
            XmlWriter writer = null;

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = false;
                settings.NewLineOnAttributes = false;
                settings.Encoding = Encoding.UTF8;

                writer = XmlWriter.Create(new StreamWriter("xmlMeetings.xml"), settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("PlannedMeetings");
                foreach (MeetingPlan meeting in meetingPlans)
                {
                    writer.WriteStartElement("Meeting");

                    writer.WriteStartElement("MeetingCenter");
                    writer.WriteValue(meeting.MeetingCenterCode);
                    writer.WriteEndElement();
                    writer.WriteStartElement("MeetingRoom");
                    writer.WriteValue(meeting.MeetingRoomCode);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Date");
                    writer.WriteValue(meeting.Date);
                    writer.WriteEndElement();
                    writer.WriteStartElement("TimeFrom");
                    writer.WriteValue(meeting.TimeFrom);
                    writer.WriteEndElement();
                    writer.WriteStartElement("TimeTo");
                    writer.WriteValue(meeting.TimeTo);
                    writer.WriteEndElement();
                    writer.WriteStartElement("ExpectedPersonsCount");
                    writer.WriteValue(meeting.ExpectedPersonsCount);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Customer");
                    writer.WriteValue(meeting.Customer);
                    writer.WriteEndElement();
                    writer.WriteStartElement("VideoConference");
                    writer.WriteValue(meeting.VideoConferencePlan);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Note");
                    writer.WriteValue(meeting.Note);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                                   
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            catch (FileLoadException xmlLoadExc)
            {

                throw xmlLoadExc;
            }
            finally
            {
                writer.Close();
            }
        }


        string meetingCenter;
        string meetingRoom;
        DateTime date;
        DateTime timeFrom;
        DateTime timeTo;
        string customer;
        int persons;
        bool videoConference;
        string note;

        public object JsonConvert { get; private set; }

        private void LoadXmlData()
        {
            XmlDocument loadDoc = new XmlDocument();
            loadDoc.Load("xmlMeetings.xml");

            XmlNode root = loadDoc.DocumentElement;
            XmlNodeList childs = root.ChildNodes;

            meetingPlans.Clear();

            foreach (XmlNode node in root.ChildNodes)
            {
                
                foreach (XmlNode data in node)
                {                    
                    switch (data.Name)
                    {
                        case "MeetingCenter":
                            meetingCenter = data.InnerText;
                            break;
                        case "MeetingRoom":
                            meetingRoom = data.InnerText;
                            break;
                        case "Date":
                            string[] dateSplit = (data.InnerText).Split('T');
                            date = DateTime.ParseExact(dateSplit[0], "yyyy-MM-dd", null);
                            date.ToString("dd.MM.yyyy");
                            break;
                        case "TimeFrom":
                            string[] timeFromSplit = (data.InnerText).Split('T');
                            timeFrom = DateTime.ParseExact(timeFromSplit[0] + " " + timeFromSplit[1], "yyyy-MM-dd HH:mm:ss", null);
                            timeFrom.ToString("dd.MM.yyyy HH:mm");
                            break;
                        case "TimeTo":
                            string[] timeToSplit = (data.InnerText).Split('T');
                            timeTo = DateTime.ParseExact(timeToSplit[0] + " " + timeToSplit[1], "yyyy-MM-dd HH:mm:ss", null);
                            timeTo.ToString("dd.MM.yyyy HH:mm");
                            break;
                        case "Customer":
                            customer = data.InnerText;
                            break;
                        case "ExpectedPersonsCount":
                            persons = Convert.ToInt32(data.InnerText);
                            break;
                        case "VideoConference":
                            videoConference = Convert.ToBoolean(data.InnerText);
                            break;
                        case "Note":
                            note = data.InnerText;
                            break;
                    }                    
                }

                MeetingPlan loadedMeeting = new MeetingPlan(meetingCenter, meetingRoom, date, timeFrom, timeTo, persons, customer, videoConference, note);
                meetingPlans.Add(loadedMeeting);
            }            
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < meetingPlans.Count; i++)
            //{
            //    jsonData.Add("data", new string[] {meetingPlans[i].MeetingCenterCode, meetingPlans[i].MeetingRoomCode,
            //        new Dictionary<string, Dictionary<string, string[]>> {"reservation", new Dictionary<string, string[]> {meetingPlans[i].Date.ToString(), new string[] { } } } });

            //}

            
        }
    }
}
