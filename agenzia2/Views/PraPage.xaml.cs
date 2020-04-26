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
        private bool bPRA, bDeterioramento, bCrono, bExtraUE, bRaccomandata = false;

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
                    TswRaccomandata.IsOn = false;
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
            if (MyRdbScelta == "RdbCDP" && !bDeterioramento)
                return 0; //!duplicato cdp
            else if (MyRdbScelta == "RdbCDP" && bDeterioramento)
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

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (RdbCDP.IsChecked == false &&
                RdbPposs.IsChecked == false &&
                RdbEsp.IsChecked == false &&
                RdbRinnAuto.IsChecked == false &&
                RdbRinnMoto.IsChecked == false &&
                RdbRettifica.IsChecked == false &&
                RdbVisura.IsChecked == false &&
                RdbCrono.IsChecked == false)

                RdbCDP.IsChecked = true;
        }



        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            //ScegliCaso();
            //recupera il valore di nota pra
            double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            double dbRaccomandata = double.Parse(GlobalData.arrayauto[10]);

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
            GlobalData.DisplayToastNotification("Il totale è stato aggiunto al carrello", "E' possibile eseguire un altro preventivo");


            int i = ScegliCaso();
            string mystring = null;



            if (i == 0)
                mystring = "duplicato cdp";
            if (i == 1)
                mystring = "duplicato cdp deterioramento";
            else if (i == 2)
                mystring = "perdita possesso";
            else if (i == 3)
                mystring = "perdita possesso + crono";
            else if (i == 4)
                mystring = "esportazione UE";
            else if (i == 5)
                mystring = "esportazione extra UE";
            else if (i == 6)
                mystring = "rinnovo auto";
            else if (i == 7)
                mystring = "rinnovo moto";
            else if (i == 8)
                mystring = "rettifica";
            else if (i == 9)
                mystring = "visura";
            else if (i == 10)
                mystring = "crono";


           // GlobalData.mylist.Add(TxtTotale.Text + " - " + mystring);

            ArticoliCarrello artcar = new ArticoliCarrello() { Prezzo = TxtTotale.Text, Tipo = "Pratiche Pra", Descrizione = mystring };

            GlobalData.mylist.Add(artcar);



        }

    }
}
