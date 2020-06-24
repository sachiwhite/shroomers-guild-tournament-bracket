﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentBracket.Model
{
    public class DebugDataProvider : IDataProvider
    {
        private string ReturnPathToBracketsTextFile()
        {
            return "testbrackets.txt";
        }
        private string ReturnPathForLoadingBracket()
        {
            return "jsoned_bracket.txt";
        }

        private string ReturnPathForSavingBracket()
        {
            return "jsoned_bracket.txt";
        }

        public void SaveBracket(string bracketContent)
        {
            var path = ReturnPathForSavingBracket();
            using (StreamWriter writer=new StreamWriter(path,false))
            {
                writer.WriteLine(bracketContent);
            }
        }

        public string LoadBracket()
        {
            var path = ReturnPathForLoadingBracket();
            using (StreamReader reader = new StreamReader(path))
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

            return string.Empty;
        }

        public string[] ReturnStartingNicknames()
        {
            string path =ReturnPathToBracketsTextFile();
           string[] startingBracket = Array.Empty<string>();
            try
            {
                startingBracket=File.ReadAllLines(path).ToArray();
            }
            catch (Exception ex)
            {
                #warning implement proper exception handling
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                
            }

          return startingBracket;
        }
    }
}
