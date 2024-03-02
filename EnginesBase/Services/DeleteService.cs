using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EnginesBase.Services
{
    public class DeleteService
    {
        GetFromDbService getFromDbService = new GetFromDbService();
        CheckService checkService = new CheckService();

        public void DeleteEngine(string engineName, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int engineId = getFromDbService.GetEngineIdByName(connection, engineName);
                    List<int> idChildElementsList = getFromDbService.GetElementsByEngineId(connection, engineId);
                    DeleteAllEngineLinks(connection, engineId);
                    DeleteEngineFromDb(connection, engineId);
                    DeleteUnusedChildElementList(connection, idChildElementsList, connectionString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }
        }

        public void DeleteAllEngineLinks(SqlConnection connection, int engineId)
        {
            try
            {
                string deleteEngineLinksQuery = "DELETE FROM EngineLinks WHERE EngineId = @EngineId";
                SqlCommand deleteEngineLinksCommand = new SqlCommand(deleteEngineLinksQuery, connection);
                deleteEngineLinksCommand.Parameters.AddWithValue("@EngineId", engineId);
                deleteEngineLinksCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }

        }

        public void DeleteEngineFromDb(SqlConnection connection, int engineId)
        {
            try
            {
                string deleteEngineQuery = "DELETE FROM Engines WHERE EngineId = @EngineId";
                SqlCommand deleteEngineCommand = new SqlCommand(deleteEngineQuery, connection);
                deleteEngineCommand.Parameters.AddWithValue("@EngineId", engineId);
                deleteEngineCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }
        }

        public void DeleteElementWithParent(string deletedElementName, string parentName, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int deletedElementId = getFromDbService.GetElementIdByName(connection, deletedElementName);
                    Boolean parentIsEngine = checkService.CheckIfEngine(connection, parentName);
                    int parentId;

                    if (parentIsEngine)
                    {
                        parentId = getFromDbService.GetEngineIdByName(connection, parentName);
                        DeleteEngineLink(connection, parentId, deletedElementId);
                    }
                    else
                    {
                        parentId = getFromDbService.GetElementIdByName(connection, parentName);
                        DeleteElementLink(connection, parentId, deletedElementId);
                    }

                    if (checkService.ChekElementUnused(deletedElementId, connectionString))
                    {
                        DeleteElementWithoutParent(connection, deletedElementId, connectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }
        }

        public void DeleteEngineLink(SqlConnection connection, int engineId, int elementId)
        {
            try
            {
                string query = "DELETE FROM EngineLinks WHERE EngineId = @EngineId AND ElementId = @ElementId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EngineId", engineId);
                    command.Parameters.AddWithValue("@ElementId", elementId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }
        }

        public void DeleteElementLink(SqlConnection connection, int parentId, int childId)
        {
            try
            {
                string query = "DELETE FROM ElementLinks WHERE ParentId = @ParentId AND ChildId = @ChildId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParentId", parentId);
                    command.Parameters.AddWithValue("@ChildId", childId);
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }
        }

        public void DeleteUnusedChildElementList(SqlConnection connection, List<int> childElementsIds, string connectionString)
        {
            try
            {
                foreach (int elementId in childElementsIds)
                {
                    if (checkService.ChekElementUnused(elementId, connectionString))
                    {
                        DeleteElementWithoutParent(connection, elementId, connectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }
        }

        public void DeleteElementWithoutParent(SqlConnection connection, int elementId, string connectionString)
        {
            try
            {
                List<int> childElements = getFromDbService.GetChildElements(connection, elementId);

                string query = "DELETE FROM Elements WHERE ElementId = @ElementId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ElementId", elementId);
                    command.ExecuteNonQuery();
                }

                DeleteUnusedChildElementList(connection, childElements, connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");

            }
        }

    }
}
