using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

using Windows.UI.Popups;

namespace agenzia2.Views
{
    public sealed partial class PraPage : Page, INotifyPropertyChanged
    {
        public PraPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("pra_impiva.txt", GlobalData.arraypra_impiva);
            GlobalData.LoadDatainArray("pra_esente.txt", GlobalData.arraypra_esente);

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
        private bool bPRA, bDeterioramento, bCrono, bExtraUE = false;

        private void RdbGruppo_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton rbd = sender as RadioButton;
            MyRdbScelta = rbd.Name;

            try
            {
                if (TswDeterioramento == null)
                    return;
                else
                {
                    TswDeterioramento.IsOn = false;
                    TswCrono.IsOn = false;
                    TswExtraUE.IsOn = false;
                    TswPra.IsOn = false;
                }

            }
            catch
            {

            }
            //    TswCrono.IsOn = false;
            //    TswExtraUE.IsOn = false;

            //}
            //! per usare messagedialog serve windows.ui.popups
            //alle dialogo da cancellare una volta controllato il funzionamento
            //var mydlg = new MessageDialog(MyRdbScelta + " selected");
            //await mydlg.ShowAsync();
            //if (MyRdbScelta == "RdbSuccessione")
            //{
            //    TswPra.IsOn = true;
            //    bPRA = true;
            //}
            //else
            //{
            //    TswPra.IsOn = false;
            //    bPRA = false;

            //}
        }
        private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bPRA = TswPra.IsOn;
            bDeterioramento = TswDeterioramento.IsOn;
            bCrono = TswCrono.IsOn;
            bExtraUE = TswExtraUE.IsOn;
            //! per usare messagedialog serve windows.ui.popups
            //! aggiungere async
            //alle dialogo da cancellare una volta controllato il funzionamento
            //var mydlg = new MessageDialog("pra " + bPRA + " deterioramento " + bDeterioramento + " crono " + bCrono + " extraue" + bExtraUE);
            //await mydlg.ShowAsync();
        }

        //alle controlla bottoni e switch per decidere il tipo di tarsferimento richiesto
        private int ScegliCaso()
        {
            if (MyRdbScelta == "RdbCDP" && !bDeterioramento)
                return 0; //!duplicato cdp
            if (MyRdbScelta == "RdbCDP" && bDeterioramento)
                return 1; //!duplicato cdp deterioramento
            else if (MyRdbScelta == "RdbPposs" && !bCrono)
                return 2; //!perdita possesso
            else if (MyRdbScelta == "RdbPposs" && bCrono)
                return 3; //!perdita possesso + crono
            else if (MyRdbScelta == "RdbEsp" && !bExtraUE)
                return 4; //!esportazione UE
            else if (MyRdbScelta == "RdbEsp" && bExtraUE)
                return 5; //!esportazione extra UE
            else if (MyRdbScelta == "RdbRinnAuto")
                return 6; //!rinnovo auto
            else if (MyRdbScelta == "RdbRinnMoto")
                return 7; //!rinnovo moto
            else if (MyRdbScelta == "RdbRettifica")
                return 8;//!rettifica
            else if (MyRdbScelta == "RdbVisura")
                return 9; //!visura
            else if (MyRdbScelta == "RdbCrono")
                return 10;//!crono

            else
                return 11;
        }

        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            ScegliCaso();
            //recupera il valore di nota pra
            double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            //controlla il caso che dobbiamo calcolare
            int myScelta = ScegliCaso();
            //alle recupera i valori da file - l'indice dell'array è -2 perchè l'array paete da 0 e il primo caso è return 2
            string strImpiva = GlobalData.arraypra_impiva[myScelta];
            string strEsente = GlobalData.arraypra_esente[myScelta];
            // trasformiamo in numeri
            double dbImpiva = double.Parse(strImpiva);
            double dbEsente = double.Parse(strEsente);
            // conrrolliamo se c'è la nota PRA
            if (bPRA)
            {
                dbEsente += dbNotaPra;
            }
            // calcoliamo il totale
            double dbTotale = dbImpiva + dbEsente;
            // aggiorniamo la view
            TxtImpiva.Text = strImpiva;
            TxtEsente.Text = dbEsente.ToString("N2");
            TxtTotale.Text = dbTotale.ToString("N2");
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ScegliCaso();
            var mydlg = new MessageDialog(ScegliCaso().ToString());
            await mydlg.ShowAsync();
            CalcolaIptFissa();

        }

        private void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
