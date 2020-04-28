using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace agenzia2.Views
{
    public sealed partial class PatentiPage : Page, INotifyPropertyChanged
    {
        public PatentiPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("patente_impiva.txt", GlobalData.arraypatente_impiva);
            GlobalData.LoadDatainArray("patente_esente.txt", GlobalData.arraypatente_esente);

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
        private bool bDati, bSmarrita, bDati2, bVecchio = false;

        private void RdbGruppo_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton rbd = sender as RadioButton;
            MyRdbScelta = rbd.Name;

            try
            {
                if (TswDati == null)
                    return;
                else
                {
                    TswDati.IsOn = false;
                    TswSmarrita.IsOn = false;
                    TswDati2.IsOn = false;
                    TswVecchio.IsOn = false;
                }

            }
            catch
            {

            }
        }
        private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bDati = TswDati.IsOn;
            bSmarrita = TswSmarrita.IsOn;
            bDati2 = TswDati2.IsOn;
            bVecchio = TswVecchio.IsOn;


            //bAzienda = TswAzienda.IsOn;
            //! per usare messagedialog serve windows.ui.popups
            //! aggiungere async
            //alle dialogo da cancellare una volta controllato il funzionamento
            //var mydlg = new MessageDialog("pra " + bPRA + " deterioramento " + bDeterioramento + " crono " + bCrono + " extraue" + bExtraUE);
            //await mydlg.ShowAsync();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ScegliCaso();
            //var mydlg = new MessageDialog(ScegliCaso().ToString());
            //await mydlg.ShowAsync();
            CalcolaIptFissa();

        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (RdbVisita.IsChecked == false &&
                RdbRinnovo.IsChecked == false &&
                RdbSmarrimento.IsChecked == false &&
                RdbDeterioramento.IsChecked == false &&
                RdbRiclassificazione.IsChecked == false &&
                RdbInternazionale.IsChecked == false &&
                RdbEstera.IsChecked == false &&
                RdbPermessoCM.IsChecked == false &&
                RdbVisitaFatta.IsChecked == false)

                RdbVisita.IsChecked = true;
        }

        private void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            HyperlinkButton hpl = sender as HyperlinkButton;
            GlobalData.OpenSettingFile(hpl.Name + ".txt");
        }


        //alle controlla bottoni e switch per decidere il tipo di tarsferimento richiesto
        private int ScegliCaso()
        {
            if (MyRdbScelta == "RdbVisita")
                return 0; //!solo visita
            else if (MyRdbScelta == "RdbRinnovo" && !bDati && !bSmarrita)
                return 1; //!rinnovo 
            else if (MyRdbScelta == "RdbRinnovo" && bDati || bSmarrita)
                return 2; //!rinnovo + dati + smarrita
            else if (MyRdbScelta == "RdbSmarrimento")
                return 3; //!smarrimento o furto
            else if (MyRdbScelta == "RdbDeterioramento" && !bDati2)
                return 4; //!smarrim/furto con dati 
            else if (MyRdbScelta == "RdbDeterioramento" && bDati2)
                return 4; //!deterioramento con o senza dati
            else if (MyRdbScelta == "RdbRiclassificazione")
                return 5; //!riclassificazione
            else if (MyRdbScelta == "RdbInternazionale")
                return 6; //! internazionale
            else if (MyRdbScelta == "RdbEstera")
                return 7; //!conversione patente estera
            else if (MyRdbScelta == "RdbPermessoCM")
                return 8; //!permesso per vista CML
            else if (MyRdbScelta == "RdbVisitaFatta" && !bVecchio)
                return 9; //!visita fatta - nuovo CM
            else if (MyRdbScelta == "RdbVisitaFatta" && bVecchio)
                return 10; //!visita fatta - vecchio CM
            else
                return 11;
        }
        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            //recupera il valore di nota pra
            //double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            //controlla il caso che dobbiamo calcolare
            int myScelta = ScegliCaso();
            //alle recupera i valori da file - l'indice dell'array è -2 perchè l'array paete da 0 e il primo caso è return 0
            string strImpiva = GlobalData.arraypatente_impiva[myScelta];
            string strEsente = GlobalData.arraypatente_esente[myScelta];
            // trasformiamo in numeri
            double dbImpiva = double.Parse(strImpiva);
            double dbEsente = double.Parse(strEsente);
            // calcoliamo il totale
            double dbTotale = dbImpiva + dbEsente;
            // aggiorniamo la view
            TxtImpiva.Text = strImpiva;
            TxtEsente.Text = dbEsente.ToString("N2");
            TxtTotale.Text = dbTotale.ToString("N2");
        }

        private void BtnCarrello_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente += double.Parse(TxtEsente.Text);
            GlobalData.dCarrelloImpIva += double.Parse(TxtImpiva.Text);
            GlobalData.dCarrelloTotale += double.Parse(TxtTotale.Text);
            GlobalData.DisplayToastNotification("Patenti", "Il totale è stato aggiunto al carrello");

            int i = ScegliCaso();
            string mystring = null;

            if (i == 0)
                mystring = "Solo visita per rilascio";
            else if (i == 1)
                mystring = "Rinnovo";
            else if (i == 2)
                mystring = "Rinnovo per smarrimento e correzione dati";
            else if (i == 3)
                mystring = "Smarrimento o furto";
            else if (i == 4)
                mystring = "Deterioramento/furto e correzione dati";
            else if (i == 5)
                mystring = "Riclassificazione";
            else if (i == 6)
                mystring = "Internazionale";
            else if (i == 7)
                mystring = "Conversione estera";
            else if (i == 8)
                mystring = "Permesso per vista CML";
            else if (i == 9)
                mystring = "Rinnovo visita già fatta - nuovo CM";
            else if (i == 10)
                mystring = "Rinnovo visita già fatta - vecchio CM";

            ArticoliCarrello artcar = new ArticoliCarrello(TxtTotale.Text, "Patente", mystring);
            GlobalData.mylist.Add(artcar);

        }

    }
}
