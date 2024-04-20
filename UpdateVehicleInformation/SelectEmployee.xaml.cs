/* Title:           Select Employee
 * Date:            6-28-17
 * Author:          Terry Holmes */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UpdateVehicleInformation
{
    /// <summary>
    /// Interaction logic for SelectEmployee.xaml
    /// </summary>
    public partial class SelectEmployee : Window
    {
        public SelectEmployee()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgrResults.ItemsSource = MainWindow.TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName;

            lblTitle.Content = "Select Employee For Vehicle " + Convert.ToString(MainWindow.gintBJCNumber);
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;

            intSelectedIndex = dgrResults.SelectedIndex;

            MainWindow.gintEmployeeID = MainWindow.TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].EmployeeID;

            Close();
        }
    }
}
