using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnginesBase.Services
{
    public class ChangeNameService
    {
        private CheckService checkService = new CheckService();
        private TextService textService = new TextService();


        public void ChangeName(string oldName, string newName, string connectionString)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    bool isEngine = checkService.CheckIfEngine(connection, oldName);
                    if (!isEngine) { oldName = textService.ExtractedElementName(oldName); }

                    string checkQuery = "SELECT COUNT(*) FROM Engines WHERE Name = @NewName " +
                                        "UNION " +
                                        "SELECT COUNT(*) FROM Elements WHERE Name = @NewName";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@NewName", newName);

                    int nameCount = (int)checkCommand.ExecuteScalar();

                    if (nameCount > 0)
                    {
                        throw new InvalidOperationException("Новое имя уже существует");
                    }


                    if (isEngine) { ChangeEngineName(connection, oldName, newName); }
                    else { ChangeElementName(connection, oldName, newName); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошло исключение: {ex.Message}");
                }
            }
        }

        private void ChangeEngineName(SqlConnection connection, string oldName, string newName)
        {

            try
            {

                string updateEngineQuery = "UPDATE Engines SET Name = @NewName WHERE Name = @OldName";
                SqlCommand updateEngineCommand = new SqlCommand(updateEngineQuery, connection);
                updateEngineCommand.Parameters.AddWithValue("@NewName", newName);
                updateEngineCommand.Parameters.AddWithValue("@OldName", oldName);

                updateEngineCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }

        private void ChangeElementName(SqlConnection connection, string oldName, string newName)
        {

            try
            {
                string updateElementQuery = "UPDATE Elements SET Name = @NewName WHERE Name = @OldName";
                SqlCommand updateElementCommand = new SqlCommand(updateElementQuery, connection);
                updateElementCommand.Parameters.AddWithValue("@NewName", newName);
                updateElementCommand.Parameters.AddWithValue("@OldName", oldName);

                updateElementCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }

    }
}
