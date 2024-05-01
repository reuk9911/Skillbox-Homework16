using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Homework16
{
    public class Database : INotifyPropertyChanged
    {

        public SqlDataAdapter SQLDa;
        public OleDbDataAdapter AccessDa;
        private string sqlConState;
        private string accessConState;

        public SqlConnection SQLCon { get; private set; }
        public OleDbConnection AccessCon { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;


        public DataTable SQLDt;
        public DataTable AccessDt;

        public string SQLConState
        {
            get => this.sqlConState;
            set
            {
                if (sqlConState != value)
                {
                    sqlConState = value;
                    OnPropertyChanged(nameof(SQLConState));
                }

            }

        }

        public string AccessConState
        {
            get => this.accessConState;
            set
            {
                if (accessConState != value)
                {
                    accessConState = value;
                    OnPropertyChanged(nameof(AccessConState));
                }

            }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public Database()
        {
            sqlConState = "Unknown";
            accessConState = "Unknown";

        }

        /// <summary>
        /// Очищает таблицы
        /// </summary>
        public void ClearTables()
        {
            string sql = "DELETE FROM Clients";
            SQLCon.Open();
            SqlCommand sqlCmd = new SqlCommand(sql, SQLCon);
            sqlCmd.ExecuteNonQuery();
            SQLCon.Close();

            sql = "DELETE FROM Purchases";
            AccessCon.Open();
            OleDbCommand accessCmd = new OleDbCommand(sql, AccessCon);
            accessCmd.ExecuteNonQuery();
            AccessCon.Close();

            AccessDt.Clear();
            SQLDt.Clear();

        }
        
        

        /// <summary>
        /// Показывает всех клиентов и их покупки
        /// </summary>
        public void ShowAll()
        {
            string sql = @"SELECT * FROM Clients Order By Clients.Id";
            SQLDa.SelectCommand = new SqlCommand(sql, SQLCon);

            sql = @"SELECT * FROM Purchases";
            AccessDa.SelectCommand = new OleDbCommand(sql, AccessCon);

            AccessDt.Clear();
            SQLDt.Clear();

            AccessDa.Fill(AccessDt);
            SQLDa.Fill(SQLDt);

        }

        /// <summary>
        /// Показывает все покупки по email
        /// </summary>
        /// <param name="email">email</param>
        public void ShowPurchasesByEmail(string email)
        {
            string sql = @"SELECT * FROM Purchases WHERE Purchases.email = @email Order By Purchases.Id";
            AccessDa.SelectCommand = new OleDbCommand(sql, AccessCon);
            AccessDa.SelectCommand.Parameters.AddWithValue("@email", email);
            AccessDt.Clear();
            AccessDa.Fill(AccessDt);
        }

        /// <summary>
        /// Добавляет в таблицу новую покупку
        /// </summary>
        /// <param name="r">покупка</param>
        public void AddPurchase(DataRow r)
        {
            AccessDt.Rows.Add(r);
            AccessDa.Update(AccessDt);
            AccessDt.Clear();
            AccessDa.Fill(AccessDt);
        }

        /// <summary>
        /// Добавляет в таблицу нового клиента
        /// </summary>
        /// <param name="r">Клиент</param>
        public void AddClient(DataRow r)
        {
            SQLDt.Rows.Add(r);
            SQLDa.Update(SQLDt);
            SQLDt.Clear();
            SQLDa.Fill(SQLDt);
        }

        /// <summary>
        /// Получает данные от БД
        /// </summary>
        /// <param name="SQLConString">Строка подключения к MSSQL БД</param>
        /// <param name="AccessConString">Строка подключения к Access БД</param>
        public async Task GetData(string SQLConString, string AccessConString)
        {
            string sql;
            sqlConState = "Unknown";
            accessConState = "Unknown";


            #region SQLCon

            SQLCon = new SqlConnection(SQLConString);
            sql = @"SELECT * FROM Clients Order By Clients.Id";
            SQLDa = new SqlDataAdapter(sql, SQLCon);
            SQLDt = new DataTable(sql, "Clients");
            SQLDa.Fill(SQLDt);
            SetSQLConCommands();
            #endregion

            #region AccessCon

            AccessCon = new OleDbConnection(AccessConString);
            sql = "SELECT * FROM Purchases";
            AccessDa = new OleDbDataAdapter(sql, AccessCon);
            AccessDt = new DataTable("Purchases");
            AccessDa.Fill(AccessDt);
            SetAccessConCommands();
            #endregion

            sqlConState = SQLCon.State.ToString();
            accessConState = AccessCon.State.ToString();
            SQLCon.StateChange += SQLCon_StateChange;
            AccessCon.StateChange += AccessCon_StateChange;

        }

        /// <summary>
        /// Устанавливает команды для SQLDa
        /// </summary>
        private void SetSQLConCommands()
        {

            #region select


            string sql = @"SELECT * FROM Clients Order By Clients.Id";
            SQLDa.SelectCommand = new SqlCommand(sql, SQLCon);

            #endregion

            #region insert

            sql = @"INSERT INTO Clients (lastName,  firstName,  middleName, phone, email) 
                                 VALUES (@lastName, @firstName, @middleName, @phone, @email); 
                     SET @id = @@IDENTITY;";

            SQLDa.InsertCommand = new SqlCommand(sql, SQLCon);

            SQLDa.InsertCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id").Direction = ParameterDirection.Output;
            SQLDa.InsertCommand.Parameters.Add("@lastName", SqlDbType.NVarChar, 40, "lastName");
            SQLDa.InsertCommand.Parameters.Add("@firstName", SqlDbType.NVarChar, 40, "firstName");
            SQLDa.InsertCommand.Parameters.Add("@middleName", SqlDbType.NVarChar, 40, "middleName");
            SQLDa.InsertCommand.Parameters.Add("@phone", SqlDbType.VarChar, 12, "phone");
            SQLDa.InsertCommand.Parameters.Add("@email", SqlDbType.NVarChar, 80, "email");

            #endregion

            #region update

            sql = @"UPDATE Clients SET 
                           lastName = @lastName,
                           firstName = @firstName, 
                           middleName = @middleName,
                           phone = @phone,
                           email = @email
                    WHERE id = @id";

            SQLDa.UpdateCommand = new SqlCommand(sql, SQLCon);
            SQLDa.UpdateCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id").SourceVersion = DataRowVersion.Original;
            SQLDa.UpdateCommand.Parameters.Add("@lastName", SqlDbType.NVarChar, 40, "lastName");
            SQLDa.UpdateCommand.Parameters.Add("@firstName", SqlDbType.NVarChar, 40, "firstName");
            SQLDa.UpdateCommand.Parameters.Add("@middleName", SqlDbType.NVarChar, 40, "middleName");
            SQLDa.UpdateCommand.Parameters.Add("@phone", SqlDbType.VarChar, 12, "phone");
            SQLDa.UpdateCommand.Parameters.Add("@email", SqlDbType.NVarChar, 80, "email");

            #endregion

            #region delete

            //sql = "DELETE FROM Clients WHERE id = @id";
            sql = "DELETE  * FROM Clients WHERE id = @id";
            SQLDa.DeleteCommand = new SqlCommand(sql, SQLCon);
            //SQLDa.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id");

            #endregion

        }

        /// <summary>
        /// Устанавливает команды для AccessDa
        /// </summary>

        private void SetAccessConCommands()
        {

            #region select


            string sql = @"SELECT * FROM Purchases";
            AccessDa.SelectCommand = new OleDbCommand(sql, AccessCon);
            //AccessDa.SelectCommand.Parameters.Add("@email", OleDbType.VarChar, 40, "email");

            #endregion

            #region insert

            sql = @"INSERT INTO Purchases (email,  productCode,  productName) 
                                 VALUES (@email,  @productCode,  @productName);";


            AccessDa.InsertCommand = new OleDbCommand(sql, AccessCon);

            AccessDa.InsertCommand.Parameters.Add("@email", OleDbType.VarChar, 40, "email");
            AccessDa.InsertCommand.Parameters.Add("@productCode", OleDbType.VarChar, 20, "productCode");
            AccessDa.InsertCommand.Parameters.Add("@productName", OleDbType.VarChar, 255, "productName");

            #endregion

            #region update

            sql = @"UPDATE Purchases SET 
                           email = @email,
                           productCode = @productCode, 
                           productName = @productName,
                    WHERE id = @id";

            AccessDa.UpdateCommand = new OleDbCommand(sql, AccessCon);
            AccessDa.UpdateCommand.Parameters.Add("@id", OleDbType.Integer, 4, "id").SourceVersion = DataRowVersion.Original;
            AccessDa.UpdateCommand.Parameters.Add("@email", OleDbType.VarChar, 40, "email");
            AccessDa.UpdateCommand.Parameters.Add("@productCode ", OleDbType.VarChar, 20, "productCode");
            AccessDa.UpdateCommand.Parameters.Add("@productName", OleDbType.VarChar, 255, "productName");

            #endregion

            #region delete

            //sql = "DELETE FROM Purchases WHERE id = @id";
            sql = "DELETE * FROM Purchases";

            AccessDa.DeleteCommand = new OleDbCommand(sql, AccessCon);
            //AccessDa.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id");

            #endregion

        }





        private void AccessCon_StateChange(object sender, StateChangeEventArgs e)
        {
            this.AccessConState = e.CurrentState.ToString();
        }

        private void SQLCon_StateChange(object sender, StateChangeEventArgs e)
        {
            this.SQLConState = e.CurrentState.ToString();
        }
    }
}
