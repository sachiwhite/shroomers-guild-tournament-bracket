using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.CommandWpf;

namespace TournamentBracket.Model
{
    public interface IBracketHolder
    {
        ObservableCollection<ObservableCollection<string>> Brackets { get; }
        void PopulateBracket();
        int NumberOfColumns { get; }
        void LoadBracket();
        void SaveScreenshotOfBracket(PngBitmapEncoder encoder);

    }
}