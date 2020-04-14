using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Windows.UI.Popups;

namespace agenzia2
{
    public static class GlobalData
    {
        // alle inserire qui gli array comuni per le diverse view
        public static string[] arrayauto = new string[10];
        public static string[] arrayauto_esente = new string[10];
        public static string[] arrayauto_impiva = new string[10];
        public static string[] arraypra_impiva = new string[12];
        public static string[] arraypra_esente = new string[12];
        public static string[] arraymtc_impiva = new string[8];
        public static string[] arraymtc_esente = new string[8];
        public static string[] arraymoto_impiva = new string[9];
        public static string[] arraymoto_esente = new string[9];
        public static string[] arrayspeciale = new string[10];
        public static string[] arrayspeciale_esente = new string[10];
        public static string[] arrayspeciale_impiva = new string[10];

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
    }
}
