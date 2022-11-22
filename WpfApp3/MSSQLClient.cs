using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf3Dapp
{
    // Microsoft SQL Client
    static class MSSQLClient
    {
        static public string SQLConnect = ""; //"user id=Админ;password=Xibs95r8#;server=ARM-MilkCount\\SQLEXPRESS;Trusted_Connection=yes;database=Milk;connection timeout=30";
        static public bool ERRSQL = false;

        // client:
        //static private SqlConnection client = new SqlConnection();

        // check connection:
        //static public bool state_Open { get => (client != null) ? client.State == System.Data.ConnectionState.Open : false; }
        //static public bool state_Closed { get => (client != null) ? client.State == System.Data.ConnectionState.Closed : false; }

        // Thread Synchronization
        //static private object sync = new object();

        // Get SQL table
        static public DataSet get_DataSet(string SQLCreq, string table = "mytable")
        {
            DataSet _Ds = new DataSet();
            try
            {
                SqlConnection _ConnDB = new SqlConnection(SQLConnect);
                SqlDataAdapter _sqlDataAdap = new SqlDataAdapter(SQLCreq, _ConnDB);
                SqlCommandBuilder _sqlCmdBld = new SqlCommandBuilder(_sqlDataAdap);

                _sqlDataAdap.Fill(_Ds, table);

                _ConnDB.Close();
                ERRSQL = false;
            }
            catch (Exception ex)
            {
                ERRSQL = true;
                MessageBox.Show($"{ex.Message}", "Ошибка получения данных");
            }
            return _Ds;
        }

        // Send SQL commands
        static public bool set_Control(string SQLreq)
        {
            bool OK = false;
            SqlConnection conn = new SqlConnection(SQLConnect);
            try
            {
                conn.Open();
                SqlCommand myCommand = new SqlCommand(SQLreq, conn);
                myCommand.CommandTimeout = conn.ConnectionTimeout + 5;
                SqlDataReader myReader = myCommand.ExecuteReader();
                myReader.Close();
                conn.Close();
                OK = true;
                ERRSQL = false;
            }
            catch (Exception ex)
            {
                ERRSQL = true;
                MessageBox.Show($"{ex.Message}", "Ошибка запроса к БД");
            }
            return OK;
        }

        // Get SQL data
        static public string get_Receive(string SQLreq, int Column = -1, char DelimCol = ',', char DelimRow = ';')
        {
            string Receive = "";
            SqlConnection conn = new SqlConnection(SQLConnect);
            try
            {
                conn.Open();
                SqlCommand myCommand = new SqlCommand(SQLreq, conn);
                myCommand.CommandTimeout = conn.ConnectionTimeout + 5;
                SqlDataReader myReader = myCommand.ExecuteReader();
                int Index = 0;
                while (myReader.Read())
                {
                    if (Index > 0)
                    {
                        Receive += DelimRow;
                    }
                    Index++;
                    if (Column >= 0)
                    {
                        Receive += myReader[Column].ToString();
                    }
                    else
                    {
                        for (var i = 0; i < myReader.FieldCount; i++)
                        {
                            if (i > 0)
                            {
                                Receive += DelimCol;
                            }
                            Receive += myReader[i].ToString();
                        }

                    }
                }
                myReader.Close();
                conn.Close();
                ERRSQL = false;
            }
            catch (Exception ex)
            {
                ERRSQL = true;
                MessageBox.Show($"{ex.Message}", "Ошибка работы в БД");
            }
            return Receive;
        }

        // Get one SQL variable
        static public dynamic get_ONEReceive(string SQLreq)
        {
            dynamic Value = null;
            SqlConnection conn = new SqlConnection(SQLConnect);
            try
            {
                conn.Open();
                SqlCommand myCommand = new SqlCommand(SQLreq, conn);
                myCommand.CommandTimeout = conn.ConnectionTimeout + 5;
                Value = myCommand.ExecuteScalar();

                conn.Close();
                ERRSQL = false;
            }
            catch (Exception ex)
            {
                ERRSQL = true;
                MessageBox.Show($"{ex.Message}", "Ошибка работы в БД");
            }
            return Value;
        }

        // ====================== Test ========================
        static public void Test()
        {
            Console.WriteLine("----------------- Microsoft SQL --------------");
            SqlConnection conn = new SqlConnection("user id=DM;" +
                                       "password=65536;server=WIN-6D9BE2IKJQB\\SQLEXPRESS;" +
                                       "Trusted_Connection=yes;" +
                                       "database=MSDataBase1; " +
                                       "connection timeout=30");
            try
            {
                conn.Open();

                SqlCommand myCommand = new SqlCommand("select * from Table1", conn);
                SqlDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    Console.WriteLine(myReader[0].ToString() + " " + myReader[1].ToString() + " " + myReader[2].ToString());
                }
                myReader.Close();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }



        }

        // ======== Sample =====================

        //// Sample CONNECT:
        //static public int Connect(string ConnString)
        //{
        //    try
        //    {
        //        client = new SqlConnection(ConnString);
        //        return (client != null) ? 0 : -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.HResult;
        //    }
        //}

        //// Sample OPEN:
        //static public void Open()
        //{
        //    if (state_Closed)
        //        client.Open();
        //}

        //// Sample CLOSE:
        //static public void Close()
        //{
        //    if (state_Open)
        //        client.Close();
        //}

    }
}
