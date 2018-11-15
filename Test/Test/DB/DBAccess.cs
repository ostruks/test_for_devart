using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace Test.DB
{
    public static class DBAccess
    {
        private static String connString = ConfigurationManager.ConnectionStrings["DefaultConnectionStrings"].ConnectionString;

        public static List<string> FilesName()
        {
            List<string> strs = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection(connString))
            {
                if (con == null)
                {
                    MessageBox.Show("conn == null!");
                }
                else
                {
                    SQLiteCommand sqCommand = new SQLiteCommand("SELECT FilesName FROM Files", con);
                    try
                    {
                        con.Open();
                        SQLiteDataReader reader = sqCommand.ExecuteReader(System.Data.CommandBehavior.KeyInfo);
                        while (reader.Read())
                        {
                            strs.Add(reader.GetString(0));
                        }
                        reader.Close();
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            return strs;
        }

        public static string OpenFile(String fileName)
        {
            using (SQLiteConnection con = new SQLiteConnection(connString))
            {
                byte[] b = null;
                SQLiteBlob blob = null;
                SQLiteDataReader reader = null;
                if (con == null)
                {
                    MessageBox.Show("conn == null!");
                }
                else
                {
                    SQLiteCommand sqCommand = new SQLiteCommand("SELECT rowid, File FROM Files WHERE FilesName = '" + fileName + "'", con);
                    try
                    {
                        con.Open();
                        reader = sqCommand.ExecuteReader(System.Data.CommandBehavior.KeyInfo);
                        while (reader.Read())
                        {
                            blob = reader.GetBlob(1, true);
                        }
                        b = new byte[blob.GetCount()];
                        blob.Read(b, blob.GetCount(), 0);
                        reader.Close();
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                return System.Text.Encoding.UTF8.GetString(b).TrimEnd('\0');
            }
        }

        public static void SaveFile(String fileName, byte[] file)
        {
            using (SQLiteConnection con = new SQLiteConnection(connString))
            {
                if (con == null)
                {
                    MessageBox.Show("conn == null!");
                }
                else
                {
                    SQLiteCommand sqCommand = new SQLiteCommand(con);
                    try
                    {
                        if (DBAccess.FilesName().Contains(fileName))
                        {
                            con.Open();
                            sqCommand.CommandText = $"UPDATE Files SET 'File' = @blob WHERE FilesName = '{fileName}';";
                            sqCommand.Parameters.Add("@blob", DbType.Binary, file.Length).Value = file;
                            sqCommand.ExecuteNonQuery();
                            con.Close();
                        }
                        else
                        {
                            con.Open();
                            sqCommand.CommandText = $"INSERT INTO Files ('FilesName', 'File') VALUES ('{fileName}', @blob);";
                            sqCommand.Parameters.Add("@blob", DbType.Binary, file.Length).Value = file;
                            sqCommand.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }
}
