using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

//toast
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

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
        }

        private void BtnAggiorna_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VisualizzaCarrello();

            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

        }

        private void BtnSvuota_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente = 0;
            GlobalData.dCarrelloImpIva = 0;
            GlobalData.dCarrelloTotale = 0;

            VisualizzaCarrello();

            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

            displayToastNotification("Il carrello è stato correttamente svuotato", "E' possibile calcolare un nuovo preventivo");

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VisualizzaCarrello();
        }

        //ALLE  TOAST
        private void displayToastNotification(String caption, String message)
        {
            var toastTemplate = "<toast launch=\"app-defined-string\">" +
                                "<visual>" +
                                  "<binding template =\"ToastGeneric\">" +
                                    "<text>" + caption + "</text>" +
                                    "<text>" + message + "</text>" +
                                  "</binding>" +
                                "</visual>" +
                                "</toast>";

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(toastTemplate);

            var toastNotification = new ToastNotification(xmlDocument);

            var notification = ToastNotificationManager.CreateToastNotifier();

            notification.Show(toastNotification);
        }

        //Remove toast history
        private void removeToastHistory()
        {
            var toastHistory = ToastNotificationManager.History;
            toastHistory.Clear();
        }

    }
}
