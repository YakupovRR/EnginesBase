using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnginesBase.Services
{
    public class InsertService
    {
        CheckService checkService = new CheckService();
        GetFromDbService getFromDbService = new GetFromDbService();
        DeleteService deleteService = new DeleteService();


        public void InsertEngine(string name, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Engines (Name) VALUES (@Name)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", name);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }

        public void InsertElement(string parentName, string newElementName, int quantity, string connectionString)
        {

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    bool parentIsEngine = checkService.CheckIfEngine(connection, parentName);
                    bool elementAlreadyExist = checkService.CheckElementExist(newElementName, connectionString);

                    int newElementId;

                    if (elementAlreadyExist)
                    {
                        newElementId = getFromDbService.GetElementIdByName(connection, newElementName);
                    }
                    else
                    {
                        newElementId = CreateElementInDb(connection, newElementName);
                    }

                    if (parentIsEngine)
                    {
                        int parentId = getFromDbService.GetEngineIdByName(connection, parentName);

                        InsertEngineLink(connection, parentId, newElementId, quantity);
                    }
                    else
                    {
                        int parentId = getFromDbService.GetElementIdByName(connection, parentName);

                        if (checkService.CheckRecursiveAddition(connection, newElementId, parentId))
                        {
                            InsertElementLink(connection, parentId, newElementId, quantity);
                        }
                        else
                        {
                            Debug.WriteLine("Рекурсивное добавление отменено");
                            if (!elementAlreadyExist)
                            {
                                deleteService.DeleteElementWithoutParent(connection, newElementId, connectionString);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }

        public int CreateElementInDb(SqlConnection connection, string elementName)
        {
            try
            {
                Debug.WriteLine("Создаём в БД новый компонент с названием " + elementName);

                string insertElementQuery = "INSERT INTO Elements (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertElementQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", elementName);
                    int generatedElementId = Convert.ToInt32(command.ExecuteScalar());
                    return generatedElementId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
                throw;
            }
        }

        public void InsertEngineLink(SqlConnection connection, int engineId, int elementId, int quantity)
        {
            try
            {
                string insertEngineLinkQuery = "INSERT INTO EngineLinks (EngineId, ElementId, Quantity)" +
                    " VALUES (@EngineId, @ElementId, @Quantity)";
                SqlCommand insertEngineLinkCommand = new SqlCommand(insertEngineLinkQuery, connection);
                insertEngineLinkCommand.Parameters.AddWithValue("@EngineId", engineId);
                insertEngineLinkCommand.Parameters.AddWithValue("@ElementId", elementId);
                insertEngineLinkCommand.Parameters.AddWithValue("@Quantity", quantity);

                insertEngineLinkCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }

        public void InsertElementLink(SqlConnection connection, int parentId, int childId, int quantity)
        {
            try
            {
                string insertElementLinkQuery = "INSERT INTO ElementLinks (ParentId, ChildId, Quantity)" +
                    " VALUES (@ParentId, @ChildId, @Quantity)";
                SqlCommand insertElementLinkCommand = new SqlCommand(insertElementLinkQuery, connection);
                insertElementLinkCommand.Parameters.AddWithValue("@ParentId", parentId);
                insertElementLinkCommand.Parameters.AddWithValue("@ChildId", childId);
                insertElementLinkCommand.Parameters.AddWithValue("@Quantity", quantity);

                insertElementLinkCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }




    }
}
