using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

using System.Linq;

namespace agenzia2.Views
{
    public sealed partial class MotoPage : Page, INotifyPropertyChanged
    {
        public MotoPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("moto_esente.txt", GlobalData.arraymoto_esente);
            GlobalData.LoadDatainArray("moto_impiva.txt", GlobalData.arraymoto_impiva);




        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
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
        private bool bPRA, bEpoca, bDoppia, bRaccomandata = false;
        //alle variabili per ipt con default

        private void RdbGruppo_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton rbd = sender as RadioButton;
            MyRdbScelta = rbd.Name;
            if (MyRdbScelta == "RdbSuccessione")
            {
                TswPra.IsOn = true;
                bPRA = true;
                TswEpoca.IsOn = false;
                bEpoca = false;
                TswDoppia.IsOn = false;
                bDoppia = false;
                TswRaccomandata.IsOn = false;
                bRaccomandata = false;
            }
            else
            {
                if (TswPra != null)
                {
                    TswPra.IsOn = false;
                    bPRA = false;
                }
                if (TswEpoca != null)
                {
                    TswEpoca.IsOn = false;
                    bEpoca = false;
                }
                if (TswDoppia != null)
                {
                    TswDoppia.IsOn = false;
                    bDoppia = false;
                }
                if (TswRaccomandata != null)
                {
                    TswRaccomandata.IsOn = false;
                    bRaccomandata = false;
                }

            }
        }
            private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bPRA = TswPra.IsOn;
            bEpoca = TswEpoca.IsOn;
            bDoppia = TswDoppia.IsOn;
            bRaccomandata = TswRaccomandata.IsOn;
        }
        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //if (ScegliCaso() == 1)
            //    CalcolaIptProporzionale();
            //else
            CalcolaIptFissa();
        }
        //alle si assicura ceh nel campo kwh possano essere digitati solo numeri
        private void TxtKwh_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        //alle apre i file di configurazione. usa il trucco di chiamare il bottone come il file da aprire
        private void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            HyperlinkButton hpl = sender as HyperlinkButton;
            GlobalData.OpenSettingFile(hpl.Name + ".txt");
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (RdbTrasferimento.IsChecked == false &&
                RdbSuccessione.IsChecked == false &&
                RdbDini.IsChecked == false &&
                RdbAtto.IsChecked == false)

                RdbTrasferimento.IsChecked = true;

        }

        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            //recupera il valore di nota pra
            double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            double dbRaccomandata = double.Parse(GlobalData.arrayauto[10]);
            //controlla il caso che dobbiamo calcolare
            int myScelta = ScegliCaso();
            //alle recupera i valori da file partire da zero
            string strImpiva = GlobalData.arraymoto_impiva[myScelta];
            string strEsente = GlobalData.arraymoto_esente[myScelta];
            // trasformiamo in numeri
            double dbImpiva = double.Parse(strImpiva);
            double dbEsente = double.Parse(strEsente);
            // conrrolliamo se c'è la nota PRA
            if (bPRA)
            {
                dbEsente += dbNotaPra;
            }
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

        //alle controlla bottoni e switch per decidere il tipo di tarsferimento richiesto
        private int ScegliCaso()
        {
            if (MyRdbScelta == "RdbTrasferimento" && !bEpoca)
                return 0; //!normale
            if (MyRdbScelta == "RdbTrasferimento" && bEpoca)
                return 1; //!epoca
            else if (MyRdbScelta == "RdbSuccessione" && !bEpoca && !bDoppia)
                return 2; //!successione normale
            else if (MyRdbScelta == "RdbSuccessione" && bEpoca && !bDoppia)
                return 3; //!successione epoca
            else if (MyRdbScelta == "RdbSuccessione" && bDoppia && !bEpoca)
                return 4; //!successione doppia
            else if (MyRdbScelta == "RdbSuccessione" && bDoppia && bEpoca)
                return 5; //!successione doppia d'epoca
            else if (MyRdbScelta == "RdbDini")
                return 6; //!dini nromale
            else if (MyRdbScelta == "RdbAtto")
                return 7;//!separazione epoca
            else
                return 8;
        }

        private void BtnCarrello_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente += double.Parse(TxtEsente.Text);
            GlobalData.dCarrelloImpIva += double.Parse(TxtImpiva.Text);
            GlobalData.dCarrelloTotale += double.Parse(TxtTotale.Text);
            GlobalData.DisplayToastNotification("Il totale è stato aggiunto al carrello", "E' possibile eseguire un altro preventivo");
        }

    }
}
