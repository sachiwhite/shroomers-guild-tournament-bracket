using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;

namespace TournamentBracket.Model
{
    public class BracketHolder : ObservableObject, IBracketHolder
    {
        private readonly IDataProvider dataProvider;

        private ObservableCollection<ObservableCollection<string>> brackets;
        public ObservableCollection<ObservableCollection<string>> Brackets
        {
            get=>brackets;
            private set => Set(()=>Brackets,ref brackets,value);
        }

        public RelayCommand<int[]> SetWinnerCommand { get;}
        public RelayCommand SaveBracketCommand { get; }

        public int NumberOfColumns => Brackets.Count;

        public BracketHolder(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            SetWinnerCommand = new RelayCommand<int[]>(SetWinnerOfBracket);
            SaveBracketCommand = new RelayCommand(SaveBracket);
            brackets = new ObservableCollection<ObservableCollection<string>>();
        }

        public void SetWinnerOfBracket(int[] indices)
        {
            int columnFrom = indices[0];
            int rowFrom = indices[1];
            if(columnFrom<Brackets.Count-1)
                Brackets[columnFrom + 1][rowFrom / 2] = Brackets[columnFrom][rowFrom];
        }

        
        private int ReturnNumberOfColumns(string[] nicknames)
        {
            int tempNumber = nicknames.Length;
            int numberOfColumnsToAdd = 0;

            while (tempNumber > 1)
            {
                numberOfColumnsToAdd++;
                tempNumber /= 2;
            }

            if (IsNicknamesNumberPowerOfTwo(nicknames.Length))
                numberOfColumnsToAdd++;
            else
                numberOfColumnsToAdd += 2;

            return numberOfColumnsToAdd;
        }
        
        private bool IsNicknamesNumberPowerOfTwo(int x)
        {
            if (x == 0) 
                return false;
           
            return (x & (x - 1)) == 0;
        }
       
        public void PopulateBracket()
        {
            var startingNicknames = dataProvider.ReturnStartingNicknames();
            var columnNumber = ReturnNumberOfColumns(startingNicknames);
            if (!startingNicknames.Equals(Array.Empty<string>()))
            {
                InitializeBrackets(startingNicknames.Length,columnNumber);
                ChooseStartingNicknames(startingNicknames);
            }
           
        }
        
        private void InitializeBrackets(int numberOfNicknames, int numberOfColumns)
        {
            for (int i = 0; i < numberOfColumns; i++)
                brackets.Add(new ObservableCollection<string>());

            int power = 0;
            numberOfNicknames = numberOfNicknames * 2 - 1;
            for (int i = numberOfColumns - 1; i >= 0; i--)
            {
                var positionsToAdd = 1 << power;
                if (numberOfNicknames - positionsToAdd < 0)
                    positionsToAdd = numberOfNicknames;
                
                for (int j = 0; j < positionsToAdd; j++)
                    brackets[i].Add(string.Empty);

                numberOfNicknames -= positionsToAdd;
                power++;
            }
        }

        public void SaveBracket()
        {
            try
            {
                var bracketToSave = JsonConvert.SerializeObject(this.Brackets);
                dataProvider.SaveBracket(bracketToSave);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }

        }

        public void LoadBracket()
        {
            try
            {
                #warning todo: refactor
                string jsonBracket = dataProvider.LoadBracket();
                var bracketToLoad = JsonConvert.DeserializeObject<ObservableCollection<ObservableCollection<string>>>(jsonBracket);
                this.brackets = new ObservableCollection<ObservableCollection<string>>();
                for (int i = 0; i < bracketToLoad.Count; i++)
                {
                    var currentlyLoadedBracket = bracketToLoad[i];
                    brackets.Add(new ObservableCollection<string>());
                    for (int j = 0; j < currentlyLoadedBracket.Count; j++)
                    {
                      brackets[i].Add(currentlyLoadedBracket[j]);
                    }
                }


            }
            catch (Exception e)
            {
                Messaging.ShowErrorMessage("Nie można było załadować drabinki."); 
            }
            
        }
        private void ChooseStartingNicknames(string[] nicknames)
        {
            Random random = new Random();
            var randomizedNicknameIndices = Enumerable.Range(0, nicknames.Length).OrderBy(a=>random.NextDouble()).ToArray();
            
            var startingNicknames = brackets[0];
            int randomizedNicknameIndex = 0;
            for (randomizedNicknameIndex = 0; randomizedNicknameIndex < startingNicknames.Count; randomizedNicknameIndex++)
            {
                var indexToAssign = randomizedNicknameIndices[randomizedNicknameIndex];
                startingNicknames[randomizedNicknameIndex] = nicknames[indexToAssign];
            }

            if (randomizedNicknameIndices.Length!=startingNicknames.Count)
            {
                int startingIndex = startingNicknames.Count / 2;
                var startingBracketIfTheirCountIsNotPowerOfTwo = brackets[1];
                for (int i = startingIndex; i < startingBracketIfTheirCountIsNotPowerOfTwo.Count; i++)
                {
                    var indexToAssign = randomizedNicknameIndices[randomizedNicknameIndex++];
                    startingBracketIfTheirCountIsNotPowerOfTwo[i] = nicknames[indexToAssign];

                }
            }


        }
    }
}
