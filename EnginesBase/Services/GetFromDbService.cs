using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnginesBase.Models;

namespace EnginesBase.Services
{
    public class GetFromDbService
    {
        private TextService textService = new TextService();
        private EntiryService entiryService = new EntiryService();


        public int GetEngineIdByName(SqlConnection connection, string engineName)
        {
            try
            {
                string getEngineIdQuery = "SELECT EngineId FROM Engines WHERE Name = @Name";
                SqlCommand getEngineIdCommand = new SqlCommand(getEngineIdQuery, connection);
                getEngineIdCommand.Parameters.AddWithValue("@Name", engineName);

                int engineId = (int)getEngineIdCommand.ExecuteScalar();

                return engineId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetEngineIdByName: {ex.Message}");
                throw;
            }
        }

        public int GetElementIdByName(SqlConnection connection, string elementName)
        {
            try
            {
                string extractedElementName = textService.ExtractedElementName(elementName);
                Debug.WriteLine("Название компонента после очистки от количества " + extractedElementName);
                string getElementIdQuery = "SELECT ElementId FROM Elements WHERE Name = @Name";
                SqlCommand getElementIdCommand = new SqlCommand(getElementIdQuery, connection);
                getElementIdCommand.Parameters.AddWithValue("@Name", extractedElementName);

                int elementId = (int)getElementIdCommand.ExecuteScalar();

                return elementId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElementIdByName: {ex.Message}");
                throw;
            }
        }

        public string GetElementNameById(SqlConnection connection, int elementId)
        {
            try
            {
                string query = "SELECT Name FROM Elements WHERE ElementId = @ElementId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ElementId", elementId);
                string elementName = (string)command.ExecuteScalar();

                return elementName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElementNameById: {ex.Message}");
                throw;
            }
        }

        public int GetElementOfEngineQuantity(SqlConnection connection, int engineId, int elementId)
        {
            try
            {
                int quantity = 0;

                string query = "SELECT Quantity FROM EngineLinks WHERE EngineId = @EngineId AND ElementId = @ElementId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EngineId", engineId);
                    command.Parameters.AddWithValue("@ElementId", elementId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantity = Convert.ToInt32(reader["Quantity"]);
                        }
                    }
                }

                return quantity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElementNameById: {ex.Message}");
                throw;
            }
        }


        public int GetChildElementOfElementQuantity(SqlConnection connection, int parentId, int childId)
        {
            try
            {
                int quantity = 0;

                string query = "SELECT Quantity FROM ElementLinks WHERE ParentId = @ParentId AND ChildId = @ChildId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParentId", parentId);
                    command.Parameters.AddWithValue("@ChildId", childId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantity = Convert.ToInt32(reader["Quantity"]);
                        }
                    }
                }

                return quantity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElementNameById: {ex.Message}");
                throw;
            }
        }

        public List<int> GetElementsByEngineId(SqlConnection connection, int engineId)
        {
            try
            {

                List<int> idElementsList = new List<int>();
                string query = "SELECT ElementId FROM EngineLinks WHERE EngineId = @EngineId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EngineId", engineId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int elementId = Convert.ToInt32(reader["ElementId"]);
                        idElementsList.Add(elementId);
                    }
                }

                return idElementsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElementNameById: {ex.Message}");
                throw;
            }
        }

        public List<int> GetChildElements(SqlConnection connection, int elementId)
        {
            try
            {
                List<int> childIdList = new List<int>();
                string query = "SELECT ChildId FROM ElementLinks WHERE ParentId = @ParentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParentId", elementId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int childId = Convert.ToInt32(reader["ChildId"]);
                        childIdList.Add(childId);
                    }
                }

                return childIdList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElementNameById: {ex.Message}");
                throw;
            }
        }

        public List<Engine> EngineAssembly(string connectionString)
        {
            try
            {
                Dictionary<int, Element> elements = new Dictionary<int, Element>();
                elements = entiryService.GetElements(connectionString);
                List<Link> elementLinks = new List<Link>();
                elementLinks = entiryService.GetElementLinks(connectionString);

                foreach (Link link in elementLinks)
                {
                    int parentId = link.ParentId;
                    int childId = link.ChildId;
                    int quantity = link.Quantity;

                    Element parentElement = elements[parentId];
                    Element childElement = elements[childId];
                    parentElement.ChildElements.Add(childElement, quantity);
                    elements[parentId] = parentElement;
                }

                Dictionary<int, Engine> engines = new Dictionary<int, Engine>();
                engines = entiryService.GetEngines(connectionString);
                List<Link> engineLinks = new List<Link>();
                engineLinks = entiryService.GetEngineLinks(connectionString);


                foreach (Link link in engineLinks)
                {
                    int parentId = link.ParentId;
                    int childId = link.ChildId;
                    int quantity = link.Quantity;

                    Engine parentElement = engines[parentId];
                    Element childElement = elements[childId];
                    parentElement.ChildElements.Add(childElement, quantity);
                    engines[parentId] = parentElement;

                }

                List<Engine> assembledEngines = engines.Values.ToList();

                return assembledEngines;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetElementNameById: {ex.Message}");
                throw;
            }
        }

    }
}
