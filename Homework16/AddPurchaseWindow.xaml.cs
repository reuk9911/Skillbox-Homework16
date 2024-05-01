using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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

namespace Homework16
{
    /// <summary>
    /// Логика взаимодействия для AddPurchaseWindow.xaml
    /// </summary>
    public partial class AddPurchaseWindow : Window
    {
        public AddPurchaseWindow()
        {
            InitializeComponent();
        }
        public AddPurchaseWindow(DataRow row) : this()
        {
            cancelBtn.Click += delegate { this.DialogResult = false; };
            okBtn.Click += delegate
            {
                row["email"] = txtEmail.Text;
                row["productCode"] = txtProductCode.Text;
                row["productName"] = txtProductName.Text;
                this.DialogResult = !false;
            };
        }
    }
}
