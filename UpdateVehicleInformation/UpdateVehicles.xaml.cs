/* Title:           Update Vehicles
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
using NewEventLogDLL;
using NewEmployeeDLL;
using NewVehicleDLL;
using VehicleAssignmentDLL;

namespace UpdateVehicleInformation
{
    /// <summary>
    /// Interaction logic for UpdateVehicles.xaml
    /// </summary>
    public partial class UpdateVehicles : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();

        //setting up the data
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindCurrentAssignedVehicleByVehicleIDDataSet TheFindCurrentAssignedVehjicleByVehicleIDDataSet = new FindCurrentAssignedVehicleByVehicleIDDataSet();
        FindActiveVehiclesDataSet TheFindActiveVehiclesDataSet = new FindActiveVehiclesDataSet();
        
        ImportVehiclesDataSet TheImportVehiclesDataSet;
        ImportVehiclesDataSetTableAdapters.vehiclesforimportTableAdapter TheImportVehiclesTableAdapter;

        public UpdateVehicles()
        {
            InitializeComponent();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the data
            try
            {
                TheImportVehiclesDataSet = new ImportVehiclesDataSet();
                TheImportVehiclesTableAdapter = new ImportVehiclesDataSetTableAdapters.vehiclesforimportTableAdapter();
                TheImportVehiclesTableAdapter.Fill(TheImportVehiclesDataSet.vehiclesforimport);

                dgrResults.ItemsSource = TheImportVehiclesDataSet.vehiclesforimport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Update Vehicle Information // Update Vehicles // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strVINNumber;
            int intVehicleID;
            string strLastName;
            int intRecordReturned;
            string strLicensePlate;
            int intOilChangeOdometer;
            DateTime datOilChangeDate;
            string strNotes;
            bool blnActive;
            string strAssignedOffice;
            bool blnFatalError;
            string strFirstName;
            bool blnUpdateVehicle;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheImportVehiclesDataSet.vehiclesforimport.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    MainWindow.gintBJCNumber = Convert.ToInt32(TheImportVehiclesDataSet.vehiclesforimport[intCounter].BJCNUMBER);
               
                    TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(MainWindow.gintBJCNumber);

                    intRecordReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                    blnUpdateVehicle = false;

                    if(intRecordReturned > 0)
                    {
                        strVINNumber = TheImportVehiclesDataSet.vehiclesforimport[intCounter].VINNUMBER;
                        intVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;
                        strLastName = TheImportVehiclesDataSet.vehiclesforimport[intCounter].LASTNAME;
                        strFirstName = TheImportVehiclesDataSet.vehiclesforimport[intCounter].FIRSTNAME;
                        strLicensePlate = TheImportVehiclesDataSet.vehiclesforimport[intCounter].LICENSEPLATE;
                        intOilChangeOdometer = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].OilChangeOdometer;
                        datOilChangeDate = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].OilChangeDate;
                        strNotes = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].Notes;
                        blnActive = true;
                        strAssignedOffice = TheImportVehiclesDataSet.vehiclesforimport[intCounter].HOMEOFFICE;

                        MainWindow.TheVerifyEmployeeDataSet = TheEmployeeClass.VerifyEmployee(strFirstName, strLastName);

                        intRecordReturned = MainWindow.TheVerifyEmployeeDataSet.VerifyEmployee.Rows.Count;

                        if((intRecordReturned == 0) || (intRecordReturned > 1))
                        {
                            MainWindow.TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strLastName);

                            intRecordReturned = MainWindow.TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count;

                            if(intRecordReturned == 0)
                            {
                                MainWindow.TheVerifyEmployeeDataSet = TheEmployeeClass.VerifyEmployee(strAssignedOffice, "WAREHOUSE");

                                MainWindow.gintEmployeeID = MainWindow.TheVerifyEmployeeDataSet.VerifyEmployee[0].EmployeeID;
                            }
                            else if(intRecordReturned == 1)
                            {
                                MainWindow.gintEmployeeID = MainWindow.TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[0].EmployeeID;
                            }
                            else if(intRecordReturned > 1)
                            {

                                SelectEmployee SelectEmployee = new SelectEmployee();
                                SelectEmployee.ShowDialog();
                            }

                        }
                        else if(intRecordReturned == 1)
                        {
                            MainWindow.gintEmployeeID = MainWindow.TheVerifyEmployeeDataSet.VerifyEmployee[0].EmployeeID;
                        }

                        TheFindCurrentAssignedVehjicleByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleByVehicleID(intVehicleID);

                        intRecordReturned = TheFindCurrentAssignedVehjicleByVehicleIDDataSet.FindCurrentAssignedVehicleByVehicleID.Rows.Count;
                        
                        if(intRecordReturned == 0)
                        {
                            blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(intVehicleID, MainWindow.gintEmployeeID);
                        }

                        if(strVINNumber != TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VINNumber)
                        {
                            blnUpdateVehicle = true;
                        }
                        if(strLicensePlate != TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].LicensePlate)
                        {
                            blnUpdateVehicle = true;
                        }
                        if(strAssignedOffice != TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].AssignedOffice)
                        {
                            blnUpdateVehicle = true;
                        }

                        if(blnUpdateVehicle == true)
                        {
                            blnFatalError = TheVehicleClass.UpdateVehicleEdit(intVehicleID, strLicensePlate, intOilChangeOdometer, datOilChangeDate, strVINNumber, strNotes, blnActive, strAssignedOffice);
                        }
                    }
                }

                CheckActiveVehiclesForEntry();
            }
            catch (Exception Ex)
            { 
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Update Vehicle Information // Update Vehicles // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();

        }
        private void CheckActiveVehiclesForEntry()
        {
            //creating local variables
            int intCounter;
            int intNumberOfRecords;
            int intVehicleID;
            int intRecordsReturned;
            string strFirstName;
            string strLastName = "WAREHOUSE";
            int intEmployeeID;
            bool blnFatalError;

            try
            {
                TheFindActiveVehiclesDataSet = TheVehicleClass.FindActiveVehicles();

                intNumberOfRecords = TheFindActiveVehiclesDataSet.FindActiveVehicles.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intVehicleID = TheFindActiveVehiclesDataSet.FindActiveVehicles[intCounter].VehicleID;

                    TheFindCurrentAssignedVehjicleByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleByVehicleID(intVehicleID);

                    intRecordsReturned = TheFindCurrentAssignedVehjicleByVehicleIDDataSet.FindCurrentAssignedVehicleByVehicleID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        strFirstName = TheFindActiveVehiclesDataSet.FindActiveVehicles[intCounter].AssignedOffice;

                        MainWindow.TheVerifyEmployeeDataSet = TheEmployeeClass.VerifyEmployee(strFirstName, strLastName);

                        intEmployeeID = MainWindow.TheVerifyEmployeeDataSet.VerifyEmployee[0].EmployeeID;

                        blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(intVehicleID, intEmployeeID);

                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("There Was a Massive Problem, Contact IT");
                            return;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Update vehicle Information // Update Vehicles // " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
    }
}
