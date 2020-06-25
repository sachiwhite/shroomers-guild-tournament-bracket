using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TournamentBracket
{
    public static class Messaging
    {
        public static void ShowMessage(string errorMessage,string caption="Błąd!")
        {
            MessageBox.Show(errorMessage, caption);
        }
        
    }
}
