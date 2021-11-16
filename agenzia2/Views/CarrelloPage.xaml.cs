using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

using System.Collections.Generic;


//toast
//using Windows.UI.Notifications;
//using Windows.Data.Xml.Dom;

namespace agenzia2.Views
{
    public sealed partial class CarrelloPage : Page, INotifyPropertyChanged
    {
        public CarrelloPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            //VisualizzaCarrello();

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

        private void VisualizzaCarrello()
        {
            TxtEsente.Text = GlobalData.dCarrelloEsente.ToString("N2");
            TxtImpiva.Text = GlobalData.dCarrelloImpIva.ToString("N2");
            TxtTotale.Text = GlobalData.dCarrelloTotale.ToString("N2");

            double dSoloImponile = 0;
            double dSoloIVA = 0;
            double dScorporoIVA = 1.22;

            dSoloImponile = GlobalData.dCarrelloImpIva / dScorporoIVA;
            dSoloIVA = GlobalData.dCarrelloImpIva - dSoloImponile;
            TxtImponibile.Text = dSoloImponile.ToString("N2");
            TxtIva.Text = dSoloIVA.ToString("N2");
        }

        private void BtnAggiorna_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VisualizzaCarrello();
            //TestListView();
            //FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            GlobalData.DisplayToastNotification("Riepilogo carrello", "Il carrello è stato aggiornato");

        }

        private void BtnSvuota_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente = 0;
            GlobalData.dCarrelloImpIva = 0;
            GlobalData.dCarrelloTotale = 0;

            VisualizzaCarrello();

            //FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

            GlobalData.mylist.Clear();

            GlobalData.DisplayToastNotification("Riepilogo carrello", "Il carrello è stato svuotato");

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VisualizzaCarrello();
            ListCarrello.ItemsSource = GlobalData.mylist;
        }

        //ALLE  TOAST
        //private void displayToastNotification(String caption, String message)
        //{
        //    var toastTemplate = "<toast launch=\"app-defined-string\">" +
        //                        "<visual>" +
        //                          "<binding template =\"ToastGeneric\">" +
        //                            "<text>" + caption + "</text>" +
        //                            "<text>" + message + "</text>" +
        //                          "</binding>" +
        //                        "</visual>" +
        //                        "</toast>";

        //    var xmlDocument = new XmlDocument();

        //    xmlDocument.LoadXml(toastTemplate);

        //    var toastNotification = new ToastNotification(xmlDocument);

        //    var notification = ToastNotificationManager.CreateToastNotifier();

        //    notification.Show(toastNotification);
        //}

        private void TestListView()
        {
            //   List<string> mylist = new List<string>();
            //   mylist.Add("200.00 - Trasferimento autoveicolo");
            //   mylist.Add("70.00 - Duplicato libretto");


            ListCarrello.ItemsSource = GlobalData.mylist;
  //          ListCarrello.Items.Clear();

            //ListCarrello.Items.Add("200.00");
            //ListCarrello.Items.Add("70.00 - Duplicato libretto");
            //ListCarrello.Items.Add("100.00 - Rinnovo patente");

        }




    }

}
