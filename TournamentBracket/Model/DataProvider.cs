using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace TournamentBracket.Model
{
    public class DataProvider :IDataProvider
    {
        private string savingBracketPath;
        private string openingBracketPath;
        private string screenshotPath;

        public DataProvider()
        {
            savingBracketPath = string.Empty;
            openingBracketPath = string.Empty;
            screenshotPath = string.Empty;
        }

        private void SetFilePathForSavingScreenshot()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Wybierz, gdzie zapisać screenshot drabinki";
            saveFileDialog.Filter = "Plik PNG |*.png|Wszystkie pliki|*.*";

            saveFileDialog.ShowDialog();
         
            if (saveFileDialog.FileName!=string.Empty)
                screenshotPath = saveFileDialog.FileName;
        }
        private void SetFilePathForSavingBracket()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Wybierz, gdzie zapisać drabinkę";
            saveFileDialog.Filter = "JSON z drabinką Gildii Grzybiarzy |*.grzyb|Plik tekstowy|*.txt|Wszystkie pliki|*.*";

            saveFileDialog.ShowDialog();
         
            if (saveFileDialog.FileName!=string.Empty)
                savingBracketPath = saveFileDialog.FileName;
        }

        private string ReturnFilePathForCreatingNewBracket()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Wybierz, którą drabinkę otworzyć";
            openFileDialog.Filter = "Plik tekstowy|*.txt|Wszystkie pliki|*.*";

            openFileDialog.ShowDialog();
         
            return openFileDialog.FileName;
        }
        private void SetFilePathForOpening()
        {
          OpenFileDialog openFileDialog = new OpenFileDialog();
          openFileDialog.Title = "Wybierz, którą drabinkę otworzyć";
          openFileDialog.Filter = "Zapisany stan drabinki |*.grzyb|Wszystkie pliki|*.*";

          openFileDialog.ShowDialog();
         
          if (openFileDialog.FileName!=string.Empty)
              openingBracketPath = openFileDialog.FileName;
        }
        public string[] ReturnStartingNicknames()
        {
            var path = ReturnFilePathForCreatingNewBracket();
            if (path!=string.Empty)
            {
                
                try
                {
                    var startingBracket=File.ReadAllLines(path).ToArray();
                    return startingBracket;
                }
                catch (Exception ex)
                {
                    Messaging.ShowMessage("Ładowanie drabinki z pliku nie powiodło się.");                     
                }

                
            }

            return Array.Empty<string>();
        }

        public void SaveBracket(string bracketContent)
        {
            SetFilePathForSavingBracket();
            if (savingBracketPath!=String.Empty)
            {
                using (StreamWriter writer=new StreamWriter(savingBracketPath,false))
                {
                    try
                    {
                        writer.WriteLine(bracketContent);
                    }
                    catch (IOException e)
                    {
                        Messaging.ShowMessage("Nie można było uzyskać dostępu do pliku, w którym chcesz zapisać drabinkę. ");
                    }
                    catch (Exception e)
                    {
                        Messaging.ShowMessage("Wystąpił nieznany błąd przy zapisywaniu drabinki.");
                    }
                    
                }
            }
        }

        public string LoadBracket()
        {
            SetFilePathForOpening();
            if (openingBracketPath!=string.Empty)
            {
                using (StreamReader reader = new StreamReader(openingBracketPath))
                {
                    try
                    {
                        var bracketJson = reader.ReadToEnd();
                        return bracketJson;
                    }
                    catch (OutOfMemoryException e)
                    {
                        Messaging.ShowMessage("Błąd odczytywania drabinki z pliku. Za mało pamięci ");
                   
                    }
                    catch (IOException e)
                    {
                        Messaging.ShowMessage("Błąd odczytywania drabinki z pliku. Nie można uzyskać dostępu do pliku. ");
                    
                    }
                
                }   
            }
            else
            {
                Messaging.ShowMessage("Nie wybrano pliku do załadowania!");
            }
            

            return string.Empty;
        }

        private Stream ReturnStreamForSavingFiles(string path)
        {
            try
            {
                return File.Create(path);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public void SaveScreenshotOfBracket(PngBitmapEncoder encoder)
        {
            SetFilePathForSavingScreenshot();
            if (screenshotPath != String.Empty)
            {
                try
                {
                    var fileStream = ReturnStreamForSavingFiles(screenshotPath);
                    encoder.Save(fileStream);

                }
                catch (Exception ex)
                {
                    Messaging.ShowMessage("Zapisywanie screenshota drabinki nie powiodło się");
                }
                
                
            }
            Messaging.ShowMessage("Screenshot zapisano!","Sukces");
        }
    }
}
