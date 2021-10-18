using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace _3_15_HW_File_Upload_Demo.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Password { get; set; }
        public DateTime TimeUploaded { get; set; }
        public int Views { get; set; }
    }

    public class ImageDb
    {
        private readonly string _connectionString;

        public ImageDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetImages()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images";
                connection.Open();
                List<Image> images = new List<Image>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    images.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        ImageName = (string)reader["ImageName"],
                        TimeUploaded = (DateTime)reader["TimeUploaded"],
                        Views = (int)reader["Views"],
                        Password = (string)reader["password"]
                    });
                }


                return images;
            }
        }
        public Image GetImageById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images where id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                List<Image> images = new List<Image>();
                var reader = cmd.ExecuteReader();
                reader.Read();
                return new Image
                {
                    Id = (int)reader["Id"],
                    ImageName = (string)reader["ImageName"],
                    TimeUploaded = (DateTime)reader["TimeUploaded"],
                    Views = (int)reader["Views"],
                    Password = (string)reader["password"]
                };
            }
        }
        public void AddView(int id,int view)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "update images set Views = @views where id= @id";
                cmd.Parameters.AddWithValue("@views", view);
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void AddImage(string imageName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Images VALUES (@imagename, GETDATE(), 0, @password)";
                cmd.Parameters.AddWithValue("@imagename", imageName);
                cmd.Parameters.AddWithValue("@password", password);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
    public class ViewImagesViewModel
    {
        public bool Viewable { get; set; }
        public Image Image { get; set; }
        public List<Image> Images { get; set; }
        public string Message { get; set; }
    }
}



