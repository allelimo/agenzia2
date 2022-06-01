using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

using System.Linq;

//carrello flyout
using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI;


namespace agenzia2.Views
{
    public sealed partial class CmlPage : Page, INotifyPropertyChanged
    {
        public CmlPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("cml_impiva.txt", GlobalData.arraycml_impiva);
            GlobalData.LoadDatainArray("cml_esente.txt", GlobalData.arraycml_esente);


        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // alle variabili per radiobutton
        private string MyRdbScelta = null;
        // alle variabili per toggleswitch
        private bool bRaccomandata = false;

        private void RdbGruppo_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton rbd = sender as RadioButton;
            MyRdbScelta = rbd.Name;

            try
            {
                if (TswRaccomandata == null)
                    return;
                else
                {
                    TswRaccomandata.IsOn = false;
                    TswRaccomandata.IsOn = false;
                }

            }
            catch
            {
            }
        }
        private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bRaccomandata = TswRaccomandata.IsOn;
            //! per usare messagedialog serve windows.ui.popups
            //! aggiungere async
            //alle dialogo da cancellare una volta controllato il funzionamento
            //var mydlg = new MessageDialog("pra " + bPRA + " deterioramento " + bDeterioramento + " crono " + bCrono + " extraue" + bExtraUE);
            //await mydlg.ShowAsync();
        }

        //alle controlla bottoni e switch per decidere il tipo di tarsferimento richiesto
        private int ScegliCaso()
        {
            if (MyRdbScelta == "RdbPatduplicatocc")
                return 0; //!duplicato patente attraverso carabinieri
            if (MyRdbScelta == "RdbCmlvisita")
                return 1; //!pagamento per visita cml già prenotata
            else if (MyRdbScelta == "RdbCmlprenotazione")
                return 2; //!prenotazione visita cml
            else if (MyRdbScelta == "RdbDuduplicatocc")
                return 3; //!duplicato DU attraverso carabinieri
            else if (MyRdbScelta == "RdbPNRichiesta")
                return 4; //!patente nautica - solo richiesta tagliando
            else if (MyRdbScelta == "RdbPNVisita")
                return 5; //!patente nautica - solo visita
            else if (MyRdbScelta == "RdbPNTutto")
                return 6; //!patente nautica - visita e richiesta
            else
                return 7;
        }

        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            //recupera il valore di nota pra
            //double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            double dbRaccomandata = double.Parse(GlobalData.arrayauto[10]);
            //controlla il caso che dobbiamo calcolare
            int myScelta = ScegliCaso();
            //alle recupera i valori da file - l'indice dell'array è -2 perchè l'array paete da 0 e il primo caso è return 0
            string strImpiva = GlobalData.arraycml_impiva[myScelta];
            string strEsente = GlobalData.arraycml_esente[myScelta];
            // trasformiamo in numeri
            double dbImpiva = double.Parse(strImpiva);
            double dbEsente = double.Parse(strEsente);

            if (bRaccomandata)
            {
                dbEsente += dbRaccomandata;
            }

            // calcoliamo il totale
            double dbTotale = dbImpiva + dbEsente;
            // aggiorniamo la view
            TxtImpiva.Text = strImpiva;
            TxtEsente.Text = dbEsente.ToString("N2");
            TxtTotale.Text = dbTotale.ToString("N2");
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (RdbPatduplicatocc.IsChecked == false &&
                RdbCmlvisita.IsChecked == false &&
                RdbCmlprenotazione.IsChecked == false &&
                RdbDuduplicatocc.IsChecked == false &&
                RdbPNRichiesta.IsChecked == false &&
                RdbPNVisita.IsChecked == false &&
                RdbPNTutto.IsChecked == false)

                RdbPatduplicatocc.IsChecked = true;

            // carrello pieno/vuoto
            if (GlobalData.bCarrelloPieno == true)
            {
                BtnCarrello.Content = "Carrello pieno";
                BtnCarrello.Foreground = new SolidColorBrush(Colors.White);
                BtnCarrello.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                BtnCarrello.Content = "Carrello";
                BtnCarrello.Foreground = new SolidColorBrush(Colors.Black);
                BtnCarrello.Background = new SolidColorBrush(Colors.LightGray);
            }


        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ScegliCaso();
            //var mydlg = new MessageDialog(ScegliCaso().ToString());
            //await mydlg.ShowAsync();
            CalcolaIptFissa();

        }

        private void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            HyperlinkButton hpl = sender as HyperlinkButton;
            GlobalData.OpenSettingFile(hpl.Name + ".txt");
        }

        private void BtnCarrello_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente += double.Parse(TxtEsente.Text);
            GlobalData.dCarrelloImpIva += double.Parse(TxtImpiva.Text);
            GlobalData.dCarrelloTotale += double.Parse(TxtTotale.Text);
            GlobalData.DisplayToastNotification("PagoPA/CML", "Il totale è stato aggiunto al carrello");

            int i = ScegliCaso();
            string mystring = null;

            if (i == 0)
                mystring = "Duplicato patente tramite Carabinieri";
            else if (i == 1)
                mystring = "Bollettini per visita presso CML già prenotata";
            else if (i == 2)
                mystring = "Prenotazione visita presso CML";
            else if (i == 3)
                mystring = "Duplicato DU tramite Carabinieri";
            else if (i == 4)
                mystring = "Richiesta Patente nautica - visita già fatta";
            else if (i == 5)
                mystring = "Patente nautica - solo visita";
            else if (i == 6)
                mystring = "Patente nautica - visita e richiesta";
            else if (i == 7)
                mystring = "n/a";
            else if (i == 8)
                mystring = "n/a";

            ArticoliCarrello artcar = new ArticoliCarrello(TxtTotale.Text, "PagoPA/CML", mystring);
            GlobalData.mylist.Add(artcar);

            // carrello pieno/vuoto
            GlobalData.bCarrelloPieno = true;
            BtnCarrello.Content = "Carrello pieno";
            BtnCarrello.Foreground = new SolidColorBrush(Colors.White);
            BtnCarrello.Background = new SolidColorBrush(Colors.Red);
        }

    }
}
