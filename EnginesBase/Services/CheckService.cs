using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnginesBase.Services
{
    public class CheckService
    {
        GetFromDbService getFromDbService = new GetFromDbService();


        public bool CheckIfEngine(SqlConnection connection, string name)
        {
            try
            {
                string checkEngineQuery = "SELECT COUNT(*) FROM Engines WHERE Name = @Name";
                SqlCommand checkEngineCommand = new SqlCommand(checkEngineQuery, connection);
                checkEngineCommand.Parameters.AddWithValue("@Name", name);

                int engineCount = (int)checkEngineCommand.ExecuteScalar();
                return engineCount > 0;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckElementExist(string elementName, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Elements WHERE Name = @Name";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", elementName);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckEngineExist(string engineName, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Engines WHERE Name = @Name";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", engineName);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                throw;
            }
        }


        public bool ChekElementUnused(int elementId, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    bool isUsedInElementLinks = CheckElementUsedInElementLinks(connection, elementId);
                    bool isUsedInEngineLinks = CheckElementUsedInEngineLinks(connection, elementId);
                    return !isUsedInElementLinks && !isUsedInEngineLinks;
                }
            }
            catch
            {
                throw;
            }
        }

        private bool CheckElementUsedInElementLinks(SqlConnection connection, int elementId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM ElementLinks WHERE ChildId = @ElementId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ElementId", elementId);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckElementUsedInEngineLinks(SqlConnection connection, int elementId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM EngineLinks WHERE ElementId = @ElementId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ElementId", elementId);

                int count = (int)command.ExecuteScalar();
                Debug.WriteLine("Компонент используется двигателями :" + count + " раз");
                return count > 0;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckEngineHasChildElements(SqlConnection connection, int engineId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM EngineLinks WHERE EngineId = @EngineId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("EngineId", engineId);

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
            catch
            {
                throw;
            }

        }

        public bool CheckElementHasChildElements(SqlConnection connection, int parentElementId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM ElementLinks WHERE ParentId = @ParentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("ParentId", parentElementId);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
            catch
            {
                throw;
            }

        }

        public bool CheckRecursiveAddition(SqlConnection connection, int idChildComponent, int idParentComponent)
        {
            try
            {
                if (idChildComponent != idParentComponent)
                {
                    if (CheckElementLinkDoesNotExist(connection, idChildComponent, idParentComponent))
                    {
                        List<int> childIds = getFromDbService.GetChildElements(connection, idChildComponent);

                        return CheckChildIdsValid(connection, childIds, idParentComponent);
                    }
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckElementLinkDoesNotExist(SqlConnection connection, int parentId, int childId)
        {
            try
            {
                string checkElementLinkQuery = "SELECT COUNT(*) FROM ElementLinks WHERE ParentId = @ParentId AND ChildId = @ChildId";

                using (SqlCommand command = new SqlCommand(checkElementLinkQuery, connection))
                {
                    command.Parameters.AddWithValue("@ParentId", parentId);
                    command.Parameters.AddWithValue("@ChildId", childId);
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // Если количество записей равно 0, значит, связи нет
                    return count == 0;
                }
            }
            catch
            {
                throw;
            }
        }

        private bool CheckChildIdsValid(SqlConnection connection, List<int> childIds, int idParentComponent)
        {
            try
            {
                foreach (int childId in childIds)
                {
                    if (!CheckElementLinkDoesNotExist(connection, childId, idParentComponent))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

    }
}
