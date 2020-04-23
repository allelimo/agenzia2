using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace agenzia2.Views
{
    public sealed partial class CarrelloPage : Page, INotifyPropertyChanged
    {
        public CarrelloPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            VisualizzaCarrello();

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
        }

        private void BtnSvuota_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GlobalData.dCarrelloEsente = 0;
            GlobalData.dCarrelloImpIva = 0;
            GlobalData.dCarrelloTotale = 0;

            VisualizzaCarrello();

        }
    }
}
