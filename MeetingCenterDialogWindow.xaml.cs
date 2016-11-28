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
    /// Interaction logic for MeetingCenterDialogWindow.xaml
    /// </summary>
    public partial class MeetingCenterDialogWindow : Window
    {
        
        public MeetingCenterDialogWindow()
        {
            InitializeComponent();
        }

        public List<MeetingCenter> newCenter = new List<MeetingCenter>();
        public bool centersChange;
        //Regularni vyraz pro Code
        Regex rgx = new Regex("[a-zA-Z_:-]{5,50}");

        //Pri  stisknuti talcitka OK overi delku vsech retezcu a regularni vyraz. 
        //Nacte data z textboxu, vytvori instanci tridy MeetingCenter a ulozi ji do kolekce newCenter. 
        //Pokud nektera z validaci selze, tak je zobrazeno okno s popisem chyby. 
        private void BtnOkMc_Click(object sender, RoutedEventArgs e)
        {
            
            if (((TbNameDialogMc.Text.Length >= 2) && (TbNameDialogMc.Text.Length <= 100)) && (rgx.IsMatch(TbCodeDialogMc.Text)) 
                && ((TbDescriptionDialogMc.Text.Length >= 10) && (TbDescriptionDialogMc.Text.Length <= 300)))
            {
                MeetingCenter center = new MeetingCenter(TbNameDialogMc.Text, TbCodeDialogMc.Text, TbDescriptionDialogMc.Text);
                newCenter.Add(center);
                Close();
                centersChange = true;
            }
            else
            {
                if ((TbNameDialogMc.Text.Length < 2) || (TbNameDialogMc.Text.Length > 100))
                {                    
                    MessageBox.Show("Name must have 2-100 characters!");
                }
                else if ((!rgx.IsMatch(TbCodeDialogMc.Text)))
                {
                    MessageBox.Show("Code must have 5-50 characters (a-z, A-Z, -, :, _) !");
                }
                else if ((TbDescriptionDialogMc.Text.Length < 10) || (TbDescriptionDialogMc.Text.Length > 300))
                {
                    MessageBox.Show("Description must have 10-300 characters!");
                }                
            }

        }

        //Pri stisknuti Cancel se okno uzavre.
        private void BtnCancelMc_Click(object sender, RoutedEventArgs e)
        {
            Close();
            centersChange = false;
        }
    }
}
