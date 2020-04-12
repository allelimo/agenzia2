﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

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
        private bool bDeterioramento, bAzienda = false;

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
                return 0; //!duplicato cdp
            if (MyRdbScelta == "RdbCCdupl" && !bDeterioramento)
                return 1; //!duplicato cdp deterioramento
            else if (MyRdbScelta == "RdbCCdupl" && bDeterioramento)
                return 2; //!perdita possesso
            else if (MyRdbScelta == "RdbVendita")
                return 3; //!perdita possesso + crono
            else if (MyRdbScelta == "RdbArt94" && !bAzienda)
                return 4; //!esportazione UE
            else if (MyRdbScelta == "RdbArt94" && bAzienda)
                return 5; //!esportazione extra UE
            else if (MyRdbScelta == "RdbTargarip")
                return 6; //!esportazione extra UE

            else
                return 7;
        }
        //alle calcola ipt con valori fissi
        private void CalcolaIptFissa()
        {
            //recupera il valore di nota pra
            //double dbNotaPra = double.Parse(GlobalData.arrayauto[6]);
            //controlla il caso che dobbiamo calcolare
            int myScelta = ScegliCaso();
            //alle recupera i valori da file - l'indice dell'array è -2 perchè l'array paete da 0 e il primo caso è return 2
            string strImpiva = GlobalData.arraymtc_impiva[myScelta];
            string strEsente = GlobalData.arraymtc_esente[myScelta];
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


    }
}