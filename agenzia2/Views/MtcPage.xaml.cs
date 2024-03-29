﻿using System;
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
    public sealed partial class MtcPage : Page, INotifyPropertyChanged
    {
        public MtcPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // alle qui: caricare i dati negli array
            GlobalData.LoadDatainArray("mtc_impiva.txt", GlobalData.arraymtc_impiva);
            GlobalData.LoadDatainArray("mtc_esente.txt", GlobalData.arraymtc_esente);

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
        private bool bDeterioramento, bAzienda, bRinnovo, bRaccomandata = false;

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
                    TswAzienda.IsOn = false;
                    TswRinnovo.IsOn = false;
                    TswRaccomandata.IsOn = false;
                }

            }
            catch
            {

            }
        }
        private void Tsw_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bDeterioramento = TswDeterioramento.IsOn;
            bAzienda = TswAzienda.IsOn;
            bRaccomandata = TswRaccomandata.IsOn;
            bRinnovo = TswRinnovo.IsOn;
            //! per usare messagedialog serve windows.ui.popups
            //! aggiungere async
            //alle dialogo da cancellare una volta controllato il funzionamento
            //var mydlg = new MessageDialog("pra " + bPRA + " deterioramento " + bDeterioramento + " crono " + bCrono + " extraue" + bExtraUE);
            //await mydlg.ShowAsync();
        }

        //alle controlla bottoni e switch per decidere il tipo di tarsferimento richiesto
        private int ScegliCaso()
        {
            if (MyRdbScelta == "RdbCCagg")
                return 0; //!aggiornamento cc
            else if (MyRdbScelta == "RdbCCdupl" && !bDeterioramento)
                return 1; //!duplicato cc furto
            else if (MyRdbScelta == "RdbCCdupl" && bDeterioramento)
                return 2; //!duplicato cc deterioramento
            else if (MyRdbScelta == "RdbVendita")
                return 3; //!vendita rimorchio
            else if (MyRdbScelta == "RdbArt94" && !bAzienda)
                return 4; //!art.94
            else if (MyRdbScelta == "RdbArt94" && bAzienda)
                return 5; //!art.94 per azienda
            else if (MyRdbScelta == "RdbTargarip")
                return 6; //!targa ripetitrice
            else if (MyRdbScelta == "RdbRevisione")
                return 7; //! inserimento per revisione
            else if (MyRdbScelta == "RdbTargaProva" && !bRinnovo)
                return 8; //!targa prova nuova
            else if (MyRdbScelta == "RdbTargaProva" && bRinnovo)
                return 9; //!targa prova rinnovo
            else
                return 10;
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
            string strImpiva = GlobalData.arraymtc_impiva[myScelta];
            string strEsente = GlobalData.arraymtc_esente[myScelta];
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
            if (RdbCCagg.IsChecked == false &&
                RdbCCdupl.IsChecked == false &&
                RdbArt94.IsChecked == false &&
                RdbVendita.IsChecked == false &&
                RdbTargarip.IsChecked == false &&
                RdbRevisione.IsChecked == false &&
                RdbTargaProva.IsChecked == false)

                RdbCCagg.IsChecked = true;

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
            GlobalData.DisplayToastNotification("Motorizzazione", "Il totale è stato aggiunto al carrello");

            int i = ScegliCaso();
            string mystring = null;

            if (i == 0)
                mystring = "Aggiornamento carta di circolazione";
            else if (i == 1)
                mystring = "Duplicato cc per furto/smarrimento";
            else if (i == 2)
                mystring = "Duplicato cc per deterioramento";
            else if (i == 3)
                mystring = "Vendita rimorchio/caravan";
            else if (i == 4)
                mystring = "Artciolo 94";
            else if (i == 5)
                mystring = "Articolo 94 per azienda";
            else if (i == 6)
                mystring = "Targa ripetitrice";
            else if (i == 7)
                mystring = "Inserimento per revisione";
            else if (i == 8)
                mystring = "Targa prova - rilascio";
            else if (i == 9)
                mystring = "Targa prova - rinnovo";

            ArticoliCarrello artcar = new ArticoliCarrello(TxtTotale.Text, "Pratica MTC", mystring);
            GlobalData.mylist.Add(artcar);

            // carrello pieno/vuoto
            GlobalData.bCarrelloPieno = true;
            BtnCarrello.Content = "Carrello pieno";
            BtnCarrello.Foreground = new SolidColorBrush(Colors.White);
            BtnCarrello.Background = new SolidColorBrush(Colors.Red);


        }


    }
}
