using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace agenzia2.Views
{
    public sealed partial class CicloPage : Page, INotifyPropertyChanged
    {
        public CicloPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("ciclo_impiva.txt", GlobalData.arrayciclo_impiva);
            GlobalData.LoadDatainArray("ciclo_esente.txt", GlobalData.arrayciclo_esente);


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
        private bool bTarga, bContestuale, bRaccomandata = false;

        private void RdbGruppo_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton rbd = sender as RadioButton;
            MyRdbScelta = rbd.Name;

            try
            {
                if (TswTargaNuova == null)
                    return;
                else
                {
                    TswTargaNuova.IsOn = false;
                    TswContestuale.IsOn = false;
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
            bTarga = TswTargaNuova.IsOn;
            bContestuale = TswContestuale.IsOn;
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
            if (MyRdbScelta == "RdbTrasferimento" && !bTarga)
                return 0; //!vendita con targa NUOVA
            if (MyRdbScelta == "RdbTrasferimento" && bTarga)
                return 1; //!vendita con targa vecchia
            else if (MyRdbScelta == "RdbSospensione" && !bContestuale)
                return 2; //!sospensione per vendita
            else if (MyRdbScelta == "RdbSospensione" && bContestuale)
                return 3; //!sospensione volontaria
            else if (MyRdbScelta == "RdbSuccessione")
                return 4; //!successione
            else if (MyRdbScelta == "Rdb1")
                return 5; //!art.94 per azienda
            else if (MyRdbScelta == "Rdb2")
                return 6; //!targa ripetitrice
            else if (MyRdbScelta == "Rdb3")
                return 7; //! inserimento per revisione
            else if (MyRdbScelta == "Rdb4")
                return 8; //!targa prova nuova
            else
                return 9;
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
            string strImpiva = GlobalData.arrayciclo_impiva[myScelta];
            string strEsente = GlobalData.arrayciclo_esente[myScelta];
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
            if (RdbTrasferimento.IsChecked == false &&
                RdbSospensione.IsChecked == false &&
                RdbSuccessione.IsChecked == false &&
                Rdb1.IsChecked == false &&
                Rdb2.IsChecked == false &&
                Rdb3.IsChecked == false &&
                Rdb4.IsChecked == false)

                RdbTrasferimento.IsChecked = true;

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
            GlobalData.DisplayToastNotification("Ciclomotore", "Il totale è stato aggiunto al carrello");

            int i = ScegliCaso();
            string mystring = null;

            if (i == 0)
                mystring = "Vendita con targa NUOVA";
            else if (i == 1)
                mystring = "Vendita con targa già in possesso";
            else if (i == 2)
                mystring = "Sospensione per vendita contestuale";
            else if (i == 3)
                mystring = "Sospensione volontaria";
            else if (i == 4)
                mystring = "Successione";
            else if (i == 5)
                mystring = "n/a";
            else if (i == 6)
                mystring = "n/a";
            else if (i == 7)
                mystring = "n/a";
            else if (i == 8)
                mystring = "n/a";

            ArticoliCarrello artcar = new ArticoliCarrello(TxtTotale.Text, "Ciclomotore", mystring);
            GlobalData.mylist.Add(artcar);

        }

    }
}
