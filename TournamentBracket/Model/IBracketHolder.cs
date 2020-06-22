﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace TournamentBracket.Model
{
    public interface IBracketHolder
    {
        ObservableCollection<ObservableCollection<string>> Brackets { get; }
        string[] PopulateBracket();
        int NumberOfColumns { get; }
        RelayCommand<int[]> SetWinnerCommand { get;}
    }
}