using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Windows.UI.Popups;

using System.Collections.ObjectModel;

// toast?
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//toast
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace agenzia2
{
    public  class GlobalData
    {
        // alle inserire qui gli array comuni per le diverse view
        public static string[] arrayauto = new string[11];
        public static string[] arrayauto_esente = new string[7];
        public static string[] arrayauto_impiva = new string[7];
        public static string[] arraypra_impiva = new string[11];
        public static string[] arraypra_esente = new string[11];
        public static string[] arraymtc_impiva = new string[10];
        public static string[] arraymtc_esente = new string[10];
        public static string[] arraymoto_impiva = new string[8];
        public static string[] arraymoto_esente = new string[8];
        public static string[] arrayspeciale = new string[7];
        public static string[] arrayspeciale_esente = new string[4];
        public static string[] arrayspeciale_impiva = new string[4];
        public static string[] arraypatente_esente = new string[11];
        public static string[] arraypatente_impiva = new string[11];
        public static string[] arraycamion_esente = new string[9];
        public static string[] arraycamion_impiva = new string[9];
        public static string[] arrayciclo_esente = new string[9];
        public static string[] arrayciclo_impiva = new string[9];
        public static string[] arraycml_esente = new string[9];
        public static string[] arraycml_impiva = new string[9];

        // alle tre variabili per la gestione del carrello
        public static double dCarrelloEsente = 0;
        public static double dCarrelloImpIva = 0;
        public static double dCarrelloTotale = 0;
        // alle nuova variabile per bottone
        public static bool bCarrelloPieno;

        public static ObservableCollection<ArticoliCarrello> mylist = new ObservableCollection<ArticoliCarrello>();

        // alle prova per lista del carrello
        //public static List<string> mylist = new List<string>();
        //public static ObservableCollection<String> mylist = new ObservableCollection<string>();


        //? inserito il controllo dell'essitenza del file
        /// vediamo sea ppare qeuata linea
        public static async void LoadDatainArray(string myfile_to_open, Array my_array_to_load)
        {
            int i = 0;
            string mystring = null;
            //alle accedere al file nello storage di windows
            try
            {
                Windows.Storage.StorageFolder strFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile strFile = await strFolder.GetFileAsync(myfile_to_open);

                if (strFile != null)
                {
                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(strFile);
                    using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                    {
                        //alle scrivi il file in un buffer
                        mystring += dataReader.ReadString(buffer.Length);
                    }
                    //! using System.IO; //per stringreader qui sotto
                    //alle usare i dati nel buffer per popolare gli array
                    using (StringReader sr = new StringReader(mystring))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            //alle scrivi ogni linea in un valore dell'array
                            my_array_to_load.SetValue(line, i);
                            i++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageDialog mydlg = new MessageDialog("File di configurazione non presenti in caricamento");
                await mydlg.ShowAsync();
            }
        }

        //alle apri il file txt pee l'editing diretto
        public static async void OpenSettingFile(string myfile_to_open)
        {
            try
            {
                Windows.Storage.StorageFolder strFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile strFile = await strFolder.GetFileAsync(myfile_to_open);

                if (strFile != null)
                {
                    //alle apri il dile con la'pplicazione di default
                    var success = await Windows.System.Launcher.LaunchFileAsync(strFile);
                }
            }
            catch (Exception)
            {
                MessageDialog mydlg = new MessageDialog("File di configurazione non presenti in apertura");
                await mydlg.ShowAsync();
            }
        }


        public static void DisplayToastNotification(String caption, String message)
        {
            var toastTemplate = "<toast launch=\"app-defined-string\">" +
                "<header id = \"160663\" title = \"Carrello\" arguments = \"action=openConversation&amp;id=6289\" />" +   //added header per raggruppamento in notifiche
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
            toastNotification.ExpirationTime = DateTime.Now.AddSeconds(30); //alle aggiunta expiration time solo per centro notifiche
            notification.Show(toastNotification);
        }

    }
}
