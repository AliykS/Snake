using Npgsql;
using System;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Snake
{

    class DB
    {
        public void GetPlayers()
        {
            string connString =
                "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=admin";
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Подключение к БД выполнено!");

                    string sql = "SELECT * FROM players";

                    NpgsqlCommand command = new NpgsqlCommand(sql, conn);

                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int score = reader.GetInt32(2);

                        Console.WriteLine($"ID: {id}");
                        Console.WriteLine($"Имя: {name}");
                        Console.WriteLine($"Счёт: {score}");
                        Console.WriteLine("----------------");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }
        public void AddPlayer(string name)
        {
            string connString =
                "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=admin";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql =
                    "INSERT INTO players(name, score) VALUES(@name, 0)";

                NpgsqlCommand command = new NpgsqlCommand(sql, conn);

                command.Parameters.AddWithValue("name", name);

                command.ExecuteNonQuery();
            }
        }
        public void UpdateScore(string name, int score)
        {
            string connString =
                "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=admin";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql = "UPDATE players SET score = @score WHERE name = @name";
                NpgsqlCommand command = new NpgsqlCommand(sql, conn);

                command.Parameters.AddWithValue("score", score);
                command.Parameters.AddWithValue("name", name);

                command.ExecuteNonQuery();
            }
        }

    }
}