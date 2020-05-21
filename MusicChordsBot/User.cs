using System;
using System.Data;
using System.Data.SqlClient;

namespace AccordsBot
{
    internal class User
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }


        /// <summary>
        /// Текущее состояние
        /// </summary>
        public State.Categories State { get; set; }

        /// <summary>
        /// Стадия категории
        /// </summary>
        public State.Stage Stage { get; set; }

        /// <summary>
        /// Id последнего сообщения
        /// </summary>
        public int LimitResultsNumber { get; set; }


        public User(long id)
        {
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);
            try
            {
                connection.Open();
                var command = new SqlCommand("select * from users where UserId=@UserId", connection);
                command.Parameters.AddWithValue("UserId", id);
                var reader = command.ExecuteReader();

                this.id = id;

                if (!reader.HasRows)
                {
                    CreateUser(id);

                }
                else
                {
                    reader.Read();

                    FirstName = reader["FirstName"] == DBNull.Value ? string.Empty: (string)reader["FirstName"];
                    LastName = reader["LastName"] == DBNull.Value ? string.Empty: (string)reader["LastName"];

                    State = reader["CurrentState"] == DBNull.Value ? 0 : (State.Categories)reader["CurrentState"];
                    Stage = reader["StageOfCategory"] == DBNull.Value ? 0 : (State.Stage)reader["StageOfCategory"];

                    LimitResultsNumber = reader["NumberOfResults"] == DBNull.Value ? 0 : (int)reader["NumberOfResults"];
                }

                reader.Close();
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
        /// Создание нового пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        private void CreateUser(long id)
        {
            // DataBase.CreateTable(id);
            DataBase.InsertToUsersTable(id);

            FirstName = string.Empty;
            LastName = string.Empty;

            State = (State.Categories)0;
            Stage = (State.Stage)0;

            LimitResultsNumber = 5;
        }

        /// <summarary>
        /// Сохранение данных пользователя
        /// </summary>
        public void SaveUser()
        {
            var connection = new SqlConnection(AppSettings.MSSQLConnectionString);

            try
            {
                connection.Open();
                Console.WriteLine($"______[{FirstName}  {LastName} - {id}]" +
                    $" {State} : {Stage}");
                var command = new SqlCommand($"UPDATE Users SET " +
                                             $"FirstName= @FirstName, " +
                                             $"LastName= @LastName, " +
                                             $"CurrentState=@CurrentState," +
                                             $"StageOfCategory=@StageOfCategory, " +
                                             $"NumberOfResults=@NumberOfResults " +
                                             $"FROM users WHERE UserId= @userId", 
                                             connection);

                command.Parameters.AddWithValue("userId", id);
                command.Parameters.AddWithValue("FirstName", FirstName);
                command.Parameters.AddWithValue("LastName", LastName);
                command.Parameters.AddWithValue("CurrentState", State);
                command.Parameters.AddWithValue("StageOfCategory", Stage);
                command.Parameters.AddWithValue("NumberOfResults", LimitResultsNumber);

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