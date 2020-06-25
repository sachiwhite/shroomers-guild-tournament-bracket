using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace TournamentBracket.Model
{
    public interface IDataProvider
    {
        //string ReturnPathToBracketsTextFile();
        string[] ReturnStartingNicknames();
        void SaveBracket(string bracketContent);
        string LoadBracket();

        void SaveScreenshotOfBracket(PngBitmapEncoder encoder);
    }
}