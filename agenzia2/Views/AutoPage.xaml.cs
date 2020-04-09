using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

using System.Linq;

namespace agenzia2.Views
{
    public sealed partial class AutoPage : Page, INotifyPropertyChanged
    {
        public AutoPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("art15.txt", GlobalData.myarray1);
            //! solo test
            GlobalData.OpenSettingFile("art14.txt");

            // alle selezionare la text box dei kwh
            // ? non funziona: TxtKwh.Focus(FocusState.Programmatic);
            TxtKwh.SelectAll();
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
        private double dbPra = 76.60;
        private double dbMtc = 29.76;
        private double dbAltre = 6;
        private double dbCorrispettivi = 118.34;
        private double db53esente = 0;
        private double db53impiva = 92.44;
        private double dbPraDini = 63.1;
        private double dbNotaPra = 20;

        private int sconto1 = 15;
        private int sconto2 = 10;
        private int sconto3 = 5;

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
            }
            else
            {
                TswPra.IsOn = false;
                bPRA = false;

            }
        }
        private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bPRA = TswPra.IsOn;
            bEpoca = TswEpoca.IsOn;
            bDoppia = TswDoppia.IsOn;
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
            //alle controlla se c'è input, se no annulla la funzione
            if (!string.IsNullOrEmpty(TxtKwh.Text))
            {
                int numKwh = int.Parse(TxtKwh.Text);
                db53esente = 307.56;

                if (numKwh <54)
                {
                    if (bPRA)
                        db53esente += dbNotaPra;

                    double db53totale = db53esente + db53impiva;
                    TxtEsente.Text = db53esente.ToString();
                    TxtImpiva.Text = db53impiva.ToString();
                    TxtTotale.Text = db53totale.ToString();

                }
                else
                {
                    //arrotonda normale
                    double dbIpt = Math.Round(numKwh * 4.57);

                    double dbTemptot = dbIpt + dbMtc + dbPra + dbAltre + dbCorrispettivi - sconto1;
                    //arrotonda al 5 piu vicino
                    double dbTotale = 5 * (int)Math.Round(dbTemptot / 5);
                    double dbEsente = dbIpt + dbMtc + dbPra;

                    if (bPRA)
                    {
                        dbEsente += dbNotaPra;
                        dbTotale += dbNotaPra;             
                    }

                    double dbImpiva = dbTotale - dbEsente;

                    TxtTotale.Text = dbTotale.ToString();
                    TxtImpiva.Text = dbImpiva.ToString();
                    TxtEsente.Text = dbEsente.ToString();
                }
            }
        }

        //private void TxtKwh_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        //{
        //    sender.Text = new String(sender.Text.Where(char.IsDigit)ToArray());
        //}

        private void TxtKwh_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            string strImpiva;
            string strEsente;
            //numKwh = int.Parse(TxtKwh.Text);
            int myScelta = ScegliCaso();

            strImpiva = GlobalData.myarray1[myScelta];
            strEsente = GlobalData.myarray1[myScelta];

            TxtImpiva.Text = "strImpiva";
            TxtEsente.Text = "strEsente";


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
            else if (MyRdbScelta == "RdbSuccessione" && bDoppia)
                return 5; //!successione doppia
            else if (MyRdbScelta == "RdbDini" && !bEpoca)
                return 6; //!dini nromale
            else if (MyRdbScelta == "RdbDini" && bEpoca)
                return 7; //!dini epoca
            else if (MyRdbScelta == "RdbAtto" && !bEpoca)
                return 8; //!separazione normale
            else if (MyRdbScelta == "RdbAtto" && bEpoca)
                return 9;//!separazione epoca
            else
                return 0;
        }
    }
}
