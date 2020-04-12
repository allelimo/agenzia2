using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using agenzia2.Helpers;
using agenzia2.Services;

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

using WinUI = Microsoft.UI.Xaml.Controls;

namespace agenzia2.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        private bool _isBackEnabled;
        private WinUI.NavigationViewItem _selected;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { Set(ref _isBackEnabled, value); }
        }

        public WinUI.NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ShellPage()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
        }

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            navigationView.BackRequested += OnBackRequested;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            KeyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(_backKeyboardAccelerator);
            await Task.CompletedTask;
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            Selected = navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private void OnItemInvoked(WinUI.NavigationView sender, WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }

            var item = navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
            NavigationService.Navigate(pageType);
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
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


        //alle da qui implementazione della searchbox
        private async void NavViewSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                string text = sender.Text;

                if (sender.Text.Length > 1)
                {
                    sender.ItemsSource = this.GetSuggestions(sender.Text);
                }
                else
                {
                    sender.ItemsSource = await Task<string[]>.Run(() => { return this.GetSuggestions(text); }); // new string[] { "No suggestions..." };
                }
            }
        }
        //alle elenco delle parole con cui cercare le pagine
        private string[] suggestions = new string[] {   "art. 94",
                                                        "auto epoca",
                                                        "autocarro",
                                                        "cambio indirizzo",
                                                        "camion",
                                                        "camper",
                                                        "carta di circolazione",
                                                        "cdp",
                                                        "ciclomotore",
                                                        "comodato d'uso",
                                                        "crono",
                                                        "divorzio",
                                                        "duplicato cdp",
                                                        "esportazione",
                                                        "furgone",
                                                        "libretto",
                                                        "motociclo",
                                                        "motorizzazione",
                                                        "mtc",
                                                        "negozio mobile",
                                                        "patente",
                                                        "perdita di possesso",
                                                        "pra",
                                                        "separazione",
                                                        "speciale",
                                                        "targa prova",
                                                        "variazione anagrafica",
                                                        "visura" };
        private string[] GetSuggestions(string text)
        {
            string[] result = null;
            result = suggestions.Where(x => x.Contains(text)).ToArray();
            return result;
        }

        private void NavViewSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string text = (string)args.SelectedItem;
            int i = SelectMyCase(text);

            switch (i)
            {
                case 1:
                    NavigationService.Navigate(typeof(AutoPage));
                    break;
                case 2:
                    NavigationService.Navigate(typeof(MotoPage));
                    break;
                case 3:
                    NavigationService.Navigate(typeof(SpecialePage));
                    break;
                case 4:
                    NavigationService.Navigate(typeof(CamionPage));
                    break;
                case 5:
                    NavigationService.Navigate(typeof(PraPage));
                    break;
                case 6:
                    NavigationService.Navigate(typeof(MtcPage));
                    break;
                case 7:
                    NavigationService.Navigate(typeof(PatentiPage));
                    break;
                case 8:
                    NavigationService.Navigate(typeof(CicloPage));
                    break;

            }
        }
        private int SelectMyCase(string t)
        {
            if (t == "autovettura" || t == "eredità auto" || t == "auto epoca" || t == "separazione" || t == "divorzio")
                return 1;
            else if (t == "motociclo")
                return 2;
            else if (t == "speciale" || t == "camper" || t == "negozio mobile")
                return 3;
            else if (t == "camion" || t == "furgone" || t == "autocarro")
                return 4;
            else if (t == "pra" || t == "duplicato cdp" || t == "cdp" || t == "perdita di possesso" || t == "esportazione" || t == "visura" || t == "crono")
                return 5;
            else if (t == "mtc" || t == "motorizzazione" || t == "carta di circolazione" || t == "libretto" || t == "targa prova" || t == "art. 94" || t == "comodato d'uso" || t == "cambio indirizzo" || t == "variazione anagrafica")
                return 6;
            else if (t == "patente")
                return 7;
            else if (t == "ciclomotore")
                return 8;
            else
                return 0;
        }
    }
}
