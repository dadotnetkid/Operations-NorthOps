using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace NorthOps.Portal.Repository
{
   /* public class ExcelConnection
    {
        static string ConnectionString(string filename)
        {
            string path = filename;
            return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0", path);
        }

        public static DataTable Datasource(string sql, string filename)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection cn = new OleDbConnection(ConnectionString(filename)))
            {
                cn.Open();
                string sheetname = cn.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                sql = sql.Replace("sheet", "[" + "Sheet1$" + "]");
                using (OleDbCommand cmd = new OleDbCommand(sql, cn))
                {
                    OleDbDataAdapter da = new OleDbDataAdapter();

                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable DataSourceTable(string fields, string filename)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection cn = new OleDbConnection(ConnectionString(filename)))
            {
                cn.Open();
                string sheetname = cn.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                string sql = "select " + fields + " from sheet";
                sql = sql.Replace("sheet", "[" + sheetname + "]");
                using (OleDbCommand cmd = new OleDbCommand(sql, cn))
                {
                    OleDbDataAdapter da = new OleDbDataAdapter();

                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public static int CheckFieldNull(string sql, string filename)
        {
            DataTable dt = new DataTable();
            int returnval = 0;
            using (OleDbConnection cn = new OleDbConnection(ConnectionString(filename)))
            {
                cn.Open();
                string sheetname = cn.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                sql = sql.Replace("sheet", "[" + "Sheet1$" + "]");
                using (OleDbCommand cmd = new OleDbCommand(sql, cn))
                {
                    returnval = (int)cmd.ExecuteScalar();
                }
            }
            return returnval;
        }
        public static DataTable Datasource(string sql, HttpPostedFileBase file, string path)
        {
            var filepath = path + Tools.RandomFileName(file);
            file.SaveAs(filepath);
            DataTable dt = new DataTable();
            using (OleDbConnection cn = new OleDbConnection(ConnectionString(filepath)))
            {
                cn.Open();
                string sheetname = cn.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                sql = sql.Replace("sheet", "[" + sheetname + "]");
                using (OleDbCommand cmd = new OleDbCommand(sql, cn))
                {
                    OleDbDataAdapter da = new OleDbDataAdapter();

                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }

    }*/
    public class Excel
    {
        public Excel(string Filename)
        {
            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Filename};Extended Properties=Excel 12.0";
        }
        public string ConnectionString { get { return connectionString; } set { connectionString = value; } }
        private string connectionString;
        public DataTable ExecuteReader()
        {
            DataTable dt = new DataTable();
            using (OleDbConnection cn = new OleDbConnection(ConnectionString))
            {
                cn.Open();
                string sheetname = cn.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                var query = $"select * from {sheetname}";
                using (OleDbCommand cmd = new OleDbCommand(query, cn))
                {
                    OleDbDataAdapter da = new OleDbDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable ExecuteReader(string query)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection cn = new OleDbConnection(ConnectionString))
            {
                cn.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, cn))
                {
                    OleDbDataAdapter da = new OleDbDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}