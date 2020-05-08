using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

using System.Linq;

namespace agenzia2.Views
{
    public sealed partial class SpecialePage : Page, INotifyPropertyChanged
    {
        public SpecialePage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("speciale.txt", GlobalData.arrayspeciale);
            GlobalData.LoadDatainArray("speciale_esente.txt", GlobalData.arrayspeciale_esente);
            GlobalData.LoadDatainArray("speciale_impiva.txt", GlobalData.arrayspeciale_impiva);

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
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ScegliCaso() == 0)
                CalcolaIptProporzionale();
            else
                CalcolaIptFissa();
        }

        //alle calcola ipt proporzionale usando i dati degli array
        private void CalcolaIptProporzionale()
        {
            //carica i valori dell'array auto
            double dbPra = double.Parse(GlobalData.arrayspeciale[0]);
            double dbMtc = double.Parse(GlobalData.arrayspeciale[1]);
            double dbAltre = double.Parse(GlobalData.arrayspeciale[2]);
            double dbCorrispettivi = double.Parse(GlobalData.arrayspeciale[3]);
            double db53esente = double.Parse(GlobalData.arrayspeciale[4]);
            double db53impiva = double.Parse(GlobalData.arrayspeciale[5]);
            double dbNotaPra = double.Parse(GlobalData.arrayspeciale[6]);
            //int sconto1 = int.Parse(GlobalData.arrayauto[7]);
            //int sconto2 = int.Parse(GlobalData.arrayauto[8]);
            //int sconto3 = int.Parse(GlobalData.arrayauto[9]);
            double dbRaccomandata = double.Parse(GlobalData.arrayauto[10]);
            int sconto = 0;
            //alle controlla se c'è input, se no annulla la funzione
            if (!string.IsNullOrEmpty(TxtKwh.Text))
            {
                int numKwh = int.Parse(TxtKwh.Text);

                if (numKwh < 54)
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
                    //alle arrotonda normale - diviso 4, cioè un quarto ipt per speciali
                    double dbIpt = Math.Round(numKwh * 4.57 / 4);

                    //? tolto sconto momentaneamente
                    //if (numKwh < 121)
                    //    sconto = sconto1;
                    //else if (numKwh < 231)
                    //    sconto = sconto2;
                    //else
                    //    sconto = sconto3;

                    double dbTemptot = dbIpt + dbMtc + dbPra + dbAltre + dbCorrispettivi - sconto;
                    //alle arrotonda al 10 piu vicino per speciali
                    //? andrebbe al 10 piu alto, proviamo prima
                    double dbTotale = 10 * (int)Math.Round(dbTemptot / 10);
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

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (RdbTrasferimento.IsChecked == false &&
                RdbSuccessione.IsChecked == false &&
                RdbDini.IsChecked == false &&
                RdbAtto.IsChecked == false)

                RdbTrasferimento.IsChecked = true;

        }

        private void TxtKwh_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;

                if (TxtKwh.Text != "")
                {
                    if (ScegliCaso() == 0)
                        CalcolaIptProporzionale();
                    else
                        CalcolaIptFissa();
                }
            }

        }


        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            //recupera il valore di nota pra
            double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            double dbRaccomandata = double.Parse(GlobalData.arrayauto[10]);

            //controlla il caso che dobbiamo calcolare
            int myScelta = ScegliCaso();
            //alle recupera i valori da file - l'indice dell'array è -1 perchè l'array paete da 0 e il primo caso è return 0
            string strImpiva = GlobalData.arrayspeciale_impiva[myScelta - 1];
            string strEsente = GlobalData.arrayspeciale_esente[myScelta - 1];
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
                return 0; //!epoca
            else if (MyRdbScelta == "RdbSuccessione" && !bEpoca && !bDoppia)
                return 1; //!successione normale
            else if (MyRdbScelta == "RdbSuccessione" && bEpoca && !bDoppia)
                return 1; //!successione epoca
            else if (MyRdbScelta == "RdbSuccessione" && bDoppia)
                return 2; //!successione doppia
            else if (MyRdbScelta == "RdbDini")
                return 3; //!dini nromale
            else if (MyRdbScelta == "RdbAtto")
                return 4;//!separazione epoca
            else
                return 5;
        }

        private void BtnCarrello_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente += double.Parse(TxtEsente.Text);
            GlobalData.dCarrelloImpIva += double.Parse(TxtImpiva.Text);
            GlobalData.dCarrelloTotale += double.Parse(TxtTotale.Text);
            GlobalData.DisplayToastNotification("Veicolo speciale", "Il totale è stato aggiunto al carrello");

            int i = ScegliCaso();
            string mystring = null;

            //if (i == 0)
            //    mystring = "duplicato cdp";
            if (i == 0)
                mystring = "Trasferimento di proprietà/epoca";
            else if (i == 1)
                mystring = "Successione/epoca";
            else if (i == 2)
                mystring = "Successione doppia";
            else if (i == 3)
                mystring = "Legge Dini";
            else if (i == 4)
                mystring = "Atto diovrzio/separazione";

            ArticoliCarrello artcar = new ArticoliCarrello(TxtTotale.Text, "Veicolo speciale", mystring);
            GlobalData.mylist.Add(artcar);

        }

    }

}
