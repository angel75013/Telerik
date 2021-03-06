﻿using System;
using System.Data.SqlClient;
using System.IO;

namespace GetImageFromDb
{
    public class Startup
    {
        public static void Main()
        {
            var isExsist = Directory.Exists("../../images");

            if (!isExsist)
            {
                Directory.CreateDirectory("../../images");
            }

            ExtractImagesFromDB(
                                @"Server=.\SQLEXPRESS;" +
                                        "Database=NORTHWND;" +
                                        "Integrated Security=true;",
                                "SELECT CategoryName, Picture FROM Categories",
                                "CategoryName",
                                "Picture",
                                ".jpg",
                                "../../images/");
        }

        private static void WriteBinaryFile(string fileName, int offset, byte[] fileContents)
        {
            FileStream stream = File.OpenWrite(fileName);
            using (stream)
            {
                stream.Write(fileContents, offset, fileContents.Length - offset);
            }
        }

        private static void ExtractImagesFromDB(
                                                string connectionString,
                                                string query,
                                                string nameColumn,
                                                string pictureColumn,
                                                string format,
                                                string path)
        {
            SqlConnection dbConn = new SqlConnection(connectionString);
            dbConn.Open();
            using (dbConn)
            {
                SqlCommand cmd = new SqlCommand(query, dbConn);
                SqlDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        var name = (string)reader[nameColumn];
                        var escapedName = name.Replace(@"/", @"-");
                        var picture = (byte[])reader[pictureColumn];
                        Console.WriteLine("Creating " + escapedName + format);
                        WriteBinaryFile(path + escapedName + format, 78, picture);
                    }
                }
            }
        }
    }
}
