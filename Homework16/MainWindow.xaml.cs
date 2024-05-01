using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.OleDb;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Security.Cryptography;
using System.ComponentModel;

namespace Homework16
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataRowView row;

        Database Db;

        public MainWindow()
        {
            InitializeComponent();

        }

        private async Task Preparing()
        {

            var SQLConString = new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                TrustServerCertificate = true,
                MultipleActiveResultSets = true,
                //Encrypt = false,
                //InitialCatalog = "ClientsDb",
                AttachDBFilename = 
                @"C:\Users\reuk\source\repos\Skillbox-Homework16-ADO.net-master\Db\ClientsDb.mdf",
                UserID = "user0",
                Password = "123"
            };

            var AccConString = new OleDbConnectionStringBuilder
            {
                Provider = "Microsoft.ACE.OLEDB.12.0",
                DataSource =
                @"C:\Users\reuk\source\repos\Skillbox-Homework16-ADO.net-master\Db\PurchasesDb.accdb"

            };

            Db = new Database();

            Db.GetData(SQLConString.ConnectionString,
                AccConString.ConnectionString);

            TextBlockSQLConState.DataContext = Db;
            TextBlockAccessConState.DataContext = Db;

            sqlGridView.DataContext = Db.SQLDt.DefaultView;
            accessGridView.DataContext = Db.AccessDt.DefaultView;

        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Preparing();
        }

        private void sqlGridView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            row = (DataRowView)sqlGridView.SelectedItem;
            row.BeginEdit();

        }

        private void sqlGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            if (sqlGridView.SelectedItem is DataRowView)
            {
                row.EndEdit();
                if (row!=null)
                    Db.SQLDa.Update(Db.SQLDt);
            }
        }

        ///// <summary>
        ///// Удаление клиента
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MenuItemDeleteClientClick(object sender, RoutedEventArgs e)
        //{
        //    row = (DataRowView)sqlGridView.SelectedItem;
        //    row.Row.Delete();
        //    Db.SQLDa.Update(Db.SQLDt);
        //}


        //private void MenuItemDeletePurchaseClick(object sender, RoutedEventArgs e)
        //{
        //    row = (DataRowView)sqlGridView.SelectedItem;
        //    row.Row.Delete();
        //    Db.AccessDa.Update(Db.AccessDt);
        //}


        private void sqlGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sqlGridView.SelectedItem is DataRowView)
            {
                row = (DataRowView)sqlGridView.SelectedItem;
                if (row != null)
                    Db.ShowPurchasesByEmail(row["email"].ToString());
            }
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow r = Db.SQLDt.NewRow();
            AddClientWindow add = new AddClientWindow(r);
            add.ShowDialog();


            if (add.DialogResult.Value)
                Db.AddClient(r);

        }

        private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow r = Db.AccessDt.NewRow();
            AddPurchaseWindow add = new AddPurchaseWindow(r);
            add.ShowDialog();


            if (add.DialogResult.Value)
                Db.AddPurchase(r);
        }

        private void ClearTablesButton_Click(object sender, RoutedEventArgs e)
        {
            Db.ClearTables();
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            Db.ShowAll();
        }


    }
}
