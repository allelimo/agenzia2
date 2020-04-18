using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

using System.Linq;

namespace agenzia2.Views
{
    public sealed partial class CamionPage : Page, INotifyPropertyChanged
    {
        public CamionPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("camion_esente.txt", GlobalData.arraycamion_esente);
            GlobalData.LoadDatainArray("camion_impiva.txt", GlobalData.arraycamion_impiva);

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
        private bool bPRA, bEpoca, bDoppia = false;
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
            }
        }

        private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bPRA = TswPra.IsOn;
            bEpoca = TswEpoca.IsOn;
            bDoppia = TswDoppia.IsOn;
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
                CalcolaIptFissa();
        }

        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            //recupera il valore di nota pra
            double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            //controlla il caso che dobbiamo calcolare
            int myScelta = ScegliCaso();
            //alle recupera i valori da file - l'indice dell'array è 0
            string strImpiva = GlobalData.arraycamion_impiva[myScelta];
            string strEsente = GlobalData.arraycamion_esente[myScelta];
            // trasformiamo in numeri
            double dbImpiva = double.Parse(strImpiva);
            double dbEsente = double.Parse(strEsente);
            // conrrolliamo se c'è la nota PRA
            if (bPRA)
            {
                dbEsente += dbNotaPra;
            }
            // controlliamo se c'è succcessione doppia

            if (bDoppia && MyRdbScelta == "RdbSuccessione")
            {
                dbImpiva += 100;
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
            if (!string.IsNullOrEmpty(TxtKwh.Text))
            {
                int numKwh = int.Parse(TxtKwh.Text);

                if (MyRdbScelta == "RdbTrasferimento" ||
                    MyRdbScelta == "RdbSuccessione")
                //trasferimento e successione
                {
                    if (numKwh < 701)
                        return 0;
                    else if (numKwh > 700 && numKwh < 1501)
                        return 1;
                    else if (numKwh > 1500 && numKwh < 3001)
                        return 2;
                    else if (numKwh > 3000 && numKwh < 4501)
                        return 3;
                    else if (numKwh > 4500 && numKwh < 6001)
                        return 4;
                    else if (numKwh > 6000 && numKwh < 8001)
                        return 5;
                    else if (numKwh > 8000)
                        return 6;
                }
                else if (MyRdbScelta == "RdbDini")
                    return 7; //!dini normale
                else if (MyRdbScelta == "RdbAtto")
                    return 8;//!separazione 
                else
                    return 9;
            }
            return 9;

        }


        //alle si assicura che nel campo kwh possano essere digitati solo numeri (use linq) 
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

    }
}
