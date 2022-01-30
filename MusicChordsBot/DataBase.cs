using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AccordsBot
{
    public class DataBase
    {
        // soon will be implemented with EF

        public static void AddSongToLibrary(string fullName, string url)
        {
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);
            try
            {
                connection.Open();

                var insertCommand = new SqlCommand(
                    $"insert into  SONGSLIBRARY (SONG, URL) values(@song, @url)",
                    connection);
                insertCommand.Parameters.AddWithValue("song", fullName);
                insertCommand.Parameters.AddWithValue("url", url);
                insertCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Вставка значения поля в таблицу
        /// </summary>
        /// <param name="nameTable">Имя таблицы</param>
        /// <param name="field">Имя поля</param>
        /// <param name="value">Значение поля</param>
        public static void InsertString(long userId, string field, string value)
        {
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);
            try
            {
                connection.Open();

                var command = new SqlCommand($"INSERT INTO MusicBase ({ field }, userid) " +
                                            $"VALUES(@fieldValue, @userid)",
                                            connection);
                command.Parameters.AddWithValue("fieldValue", value);
                command.Parameters.AddWithValue("userid", userId);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception EX)
                {
                    Console.WriteLine(EX.Message);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public static Dictionary<string, string> GetMusicLibrary(long userid)
        {
            var library = new Dictionary<string, string>();
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);
            try
            {
                connection.Open();

                var command = new SqlCommand($"SELECT url, song FROM MusicBase where IsFavourite=1 and UserId = @userid",
                                            connection);
                command.Parameters.AddWithValue("userid", userid);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var url = (reader["url"] != DBNull.Value) ? (string)reader["url"] : string.Empty;
                    var songName = (reader["song"] != DBNull.Value) ? (string)reader["song"] : string.Empty;
                    if (url != string.Empty && !library.ContainsKey(url))
                    {
                        library.Add(url, songName);
                    }


                }

                reader.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return library;
        }

        public static bool CheckSong(string fullName, out string fileId)
        {
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);
            fileId = string.Empty;
            try
            {
                connection.Open();
                var selectCommand = new SqlCommand(
                    $"select top (1) url from SONGSLIBRARY where song = @fullname",
                    connection);
                selectCommand.Parameters.AddWithValue("fullname", fullName);
                var value = selectCommand.ExecuteScalar();
                if (value != DBNull.Value && !string.IsNullOrWhiteSpace((string)value))
                {
                    fileId = (string)value;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return true;
        }


        /// <summary>
        /// Обновление данных строки в таблице за критерием
        /// </summary>
        /// <param name="name_Table">Имя таблицы</param>
        /// <param name="field">Имя поля для обновления</param>
        /// <param name="value">Значение поля для обновления </param>
        /// <param name="indexField">Имя поля критерия</param>
        /// <param name="fieldvalue">Значение поля критерия</param>
        public static void UpdateString(long userId, string field, string value, string indexField, string fieldvalue)
        {
            SqlConnection connection = new SqlConnection(AppSettings.MSSQLConnectionString);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand($"UPDATE MusicBase " +
                                                    $"SET { field }=@fieldValue " +
                                                    $"WHERE { indexField }=@valueFieldValue and " +
                                                    $"UserId = @userId",
                                                    connection);
                command.Parameters.AddWithValue("fieldValue", value.Trim());
                command.Parameters.AddWithValue("valueFieldValue", fieldvalue);
                command.Parameters.AddWithValue("userId", userId);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception EX)
                {
                    Console.WriteLine(EX.Message);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// Проверка наличия пользователя в таблице 
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        public static bool IsUserExist(long id)
        {
            int countRows = 0;
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);

            try
            {
                connection.Open();
                var command = new SqlCommand("SELECT count(1) FROM users " +
                                             "WHERE UserId=@userId",
                                             connection);
                command.Parameters.AddWithValue("userId", id);
                countRows = (int)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return countRows != 0;
        }

        /// <summary>
        /// Добавление пользователя в таблицу
        /// </summary>
        /// <param name="id">Id пользователя</param>
        public static void InsertToUsersTable(long id)
        {
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);
            try
            {
                connection.Open();
                var command = new SqlCommand($"INSERT INTO users(UserId)" +
                                            $" VALUES (@userId)",
                                            connection);
                command.Parameters.AddWithValue("userId", id);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
