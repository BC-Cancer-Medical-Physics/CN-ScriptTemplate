using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    public partial class ScriptWindow : Window
    {
        // This is the WPF gui you can use to get any user input you want when your script runs.  
        public ScriptWindow(Patient p_in, PlanSetup pl_in)
        {
            InitializeComponent();
            label_PlanID.Content = pl_in.Id; // display the plan Id
        }

        private void Close_GUI(object sender, RoutedEventArgs e)
        {
            Close(); // this closes the script GUI
        }

        // Add more buttons here:
    }
}