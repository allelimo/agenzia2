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


namespace agenzia2.Views
{
    public sealed partial class AutoPage : Page, INotifyPropertyChanged
    {
        public AutoPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("auto.txt", GlobalData.arrayauto);
            GlobalData.LoadDatainArray("auto_esente.txt", GlobalData.arrayauto_esente);
            GlobalData.LoadDatainArray("auto_impiva.txt", GlobalData.arrayauto_impiva);

            //! solo test
            //GlobalData.OpenSettingFile("auto.txt");

            // alle selezionare la text box dei kwh
            // ? non funziona:
            //TxtKwh.Focus(FocusState.Programmatic);
            //TxtKwh.Focus(true); // SetValue(IsFocusedProperty, true);
            //TxtKwh.Focus(FocusState.Keyboard);
            //TxtKwh.SelectAll();


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
        //private double dbPra = 76.60;
        //private double dbMtc = 29.76;
        //private double dbAltre = 6;
        //private double dbCorrispettivi = 118.34;
        //private double db53esente = 0;
        //private double db53impiva = 92.44;
        //private double dbPraDini = 63.1;
        //private double dbNotaPra = 20;

        //private int sconto1 = 15;
        //private int sconto2 = 10;
        //private int sconto3 = 5;

        private void RdbGruppo_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton rbd = sender as RadioButton;
            MyRdbScelta = rbd.Name;
            //! per usare messagedialog serve windows.ui.popups
            //alle dialogo da cancellare una volta controllato il funzionamento
            //var mydlg = new MessageDialog(MyRdbScelta + " selected");
            //await mydlg.ShowAsync();
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
            if (MyRdbScelta != "RdbTrasferimento")
            {
                TxtSoloIpt.Text = "n/a";
                TxtPreRound.Text = "n/a";
            }
        }
        private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bPRA = TswPra.IsOn;
            bEpoca = TswEpoca.IsOn;
            bDoppia = TswDoppia.IsOn;
            bRaccomandata = TswRaccomandata.IsOn;
            //! per usare messagedialog serve windows.ui.popups
            //! aggiungere async
            //alle dialogo da cancellare una volta controllato il funzionamento
            //var mydlg = new MessageDialog("pra " + bPRA + " epoca " + bEpoca + " doppia " + bDoppia);
            //await mydlg.ShowAsync();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ScegliCaso() == 1)
                CalcolaIptProporzionale();
            else
                CalcolaIptFissa();
        }

        //alle calcola ipt proporzionale usando i dati degli array
        private void CalcolaIptProporzionale()
        {
            //carica i valori dell'array auto
            double dbPra = double.Parse(GlobalData.arrayauto[0]);
            double dbMtc = double.Parse(GlobalData.arrayauto[1]);
            double dbAltre = double.Parse(GlobalData.arrayauto[2]);
            double dbCorrispettivi = double.Parse(GlobalData.arrayauto[3]);
            double db53esente = double.Parse(GlobalData.arrayauto[4]);
            double db53impiva = double.Parse(GlobalData.arrayauto[5]);
            double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            int sconto1 = int.Parse(GlobalData.arrayauto[7]);
            int sconto2 = int.Parse(GlobalData.arrayauto[8]);
            int sconto3 = int.Parse(GlobalData.arrayauto[9]);
            double dbRaccomandata = double.Parse(GlobalData.arrayauto[10]);
            int sconto = 0;
            //alle controlla se c'è input, se no annulla la funzione
            if (!string.IsNullOrEmpty(TxtKwh.Text))
            {
                int numKwh = int.Parse(TxtKwh.Text);
 //               db53esente = 307.56;

                if (numKwh <54)
                {
                    if (bPRA)
                        db53esente += dbNotaPra;
                    if (bRaccomandata)
                        db53esente += dbRaccomandata;

                    double db53totale = db53esente + db53impiva;
                    TxtEsente.Text = db53esente.ToString("N2");
                    TxtImpiva.Text = db53impiva.ToString("N2");
                    TxtTotale.Text = db53totale.ToString("n2");

                }
                else
                {
                    //arrotonda normale
                    double dbIpt = Math.Round(numKwh * 4.57);

                    if (numKwh < 121)
                        sconto = sconto1;
                    else if (numKwh < 231)
                        sconto = sconto2;
                    else
                        sconto = sconto3;

                    double dbTemptot = dbIpt + dbMtc + dbPra + dbAltre + dbCorrispettivi - sconto;
                    //arrotonda al 5 piu vicino
                    double dbTotale = 5 * (int)Math.Round(dbTemptot / 5);
                    double dbEsente = dbIpt + dbMtc + dbPra;

                    if (bPRA)
                    {
                        dbEsente += dbNotaPra;
                        dbTotale += dbNotaPra;             
                    }
                    if (bRaccomandata)
                    {
                        dbEsente += dbRaccomandata;
                        dbTotale += dbRaccomandata;
                    }

                    double dbImpiva = dbTotale - dbEsente;

                    TxtTotale.Text = dbTotale.ToString("N2");
                    TxtImpiva.Text = dbImpiva.ToString("N2");
                    TxtEsente.Text = dbEsente.ToString("N2");
                    //alle visualizza solo ipt e totale pre-arrotondamento
                    TxtSoloIpt.Text = dbIpt.ToString("N2");
                    TxtPreRound.Text = dbTemptot.ToString("N2");

                }
            }
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

        private void TxtKwh_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TxtKwh.SelectAll();
        }


        //if nothing is selected, select the first radiobutton
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
            //alle recupera i valori da file - l'indice dell'array è -2 perchè l'array paete da 0 e il primo caso è return 2
            string strImpiva = GlobalData.arrayauto_impiva[myScelta - 2];
            string strEsente = GlobalData.arrayauto_esente[myScelta - 2];
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
            if(MyRdbScelta == "RdbTrasferimento" && !bEpoca)
                return 1; //!normale
            if (MyRdbScelta == "RdbTrasferimento" && bEpoca)
                return 2; //!epoca
            else if (MyRdbScelta == "RdbSuccessione" && !bEpoca && !bDoppia)
                return 3; //!successione normale
            else if (MyRdbScelta == "RdbSuccessione" && bEpoca && !bDoppia)
                return 4; //!successione epoca
            else if (MyRdbScelta == "RdbSuccessione" && bDoppia && !bEpoca)
                return 5; //!successione doppia
            else if (MyRdbScelta == "RdbSuccessione" && bDoppia && bEpoca)
                return 6; //!successione doppia d'epoca
            else if (MyRdbScelta == "RdbDini")
                return 7; //!dini nromale
            else if (MyRdbScelta == "RdbAtto")
                return 8;//!separazione epoca
            else
                return 0;
        }

        private void BtnCarrello_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente += double.Parse(TxtEsente.Text);
            GlobalData.dCarrelloImpIva += double.Parse(TxtImpiva.Text);
            GlobalData.dCarrelloTotale += double.Parse(TxtTotale.Text);

            //FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            GlobalData.DisplayToastNotification("Il totale è stato aggiunto al carrello", "E' possibile eseguire un altro preventivo");

        }

    }
}
