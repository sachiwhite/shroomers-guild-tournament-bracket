using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TournamentBracket.Model
{
    public class DataProvider :IDataProvider
    {
        private string savingBracketPath;
        private string openingBracketPath;

        public DataProvider()
        {
            savingBracketPath = string.Empty;
            openingBracketPath = string.Empty;
        }

        private void SetFilePathForSaving()
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
            if (openFileDialog.FileName!=string.Empty)
                openingBracketPath = openFileDialog.FileName;
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
                    Messaging.ShowErrorMessage("Ładowanie drabinki z pliku nie powiodło się.");                     
                }

                
            }

            return Array.Empty<string>();
        }

        public void SaveBracket(string bracketContent)
        {
            throw new NotImplementedException();
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
                        Messaging.ShowErrorMessage("Błąd odczytywania drabinki z pliku. Za mało pamięci ");
                   
                    }
                    catch (IOException e)
                    {
                        Messaging.ShowErrorMessage("Błąd odczytywania drabinki z pliku. Nie można uzyskać dostępu do pliku. ");
                    
                    }
                
                }   
            }
            else
            {
                Messaging.ShowErrorMessage("Nie wybrano pliku do załadowania!");
            }
            

            return string.Empty;
        }
    }
}
