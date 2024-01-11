using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MPTV1._1
{
    internal class dopFuns
    {

        public void logOut()
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
