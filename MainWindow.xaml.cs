using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SemestralniPrace01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
               

        public MainWindow()
        {
            
            InitializeComponent();

            List<MeetingCenter> meetingCenter = new List<MeetingCenter>();
            List<MeetingRoom> meetingRoom = new List<MeetingRoom>();
        }

        //Otevreni dialogoveho okna pro zadani noveho Meeting Center
        private void BtnNewMc_Click(object sender, RoutedEventArgs e)
        {
            MeetingCenterDialogWindow newMeetingCenter = new MeetingCenterDialogWindow();
            newMeetingCenter.ShowDialog();
        }

        //Otevreni dialogoveho okna pro zadani nove Meeting Room
        private void BtnNewMR_Click(object sender, RoutedEventArgs e)
        {
            MeetingRoomDialogWindow newMeetingRoom = new MeetingRoomDialogWindow();
            newMeetingRoom.ShowDialog();
        }

        //Otevreni okna pro vyber souboru s daty a jejich nacteni
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            bool? result = ofd.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                string path = ofd.FileName;
                // Vyjimky
                

                try
                {
                    string[] reader = File.ReadAllLines(path);

                    foreach (var item in reader)
                    {
                         
                        if (item.Contains("MEETING_CENTRES"))
                        {

                            var pole = item.Split(',');
                        }
                        Console.WriteLine();
                    }

                    MessageBox.Show("Zdarec");

                }
                catch (Exception )
                {

                    throw new IOException();
                    
                }
                // SaveFileDialog
            }


        }
    }
}
