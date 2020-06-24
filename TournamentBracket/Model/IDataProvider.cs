using System.Runtime.CompilerServices;

namespace TournamentBracket.Model
{
    public interface IDataProvider
    {
        //string ReturnPathToBracketsTextFile();
        string[] ReturnStartingNicknames();
        void SaveBracket(string bracketContent);
        string LoadBracket();
        
    }
}