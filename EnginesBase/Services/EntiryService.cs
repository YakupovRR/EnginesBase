using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnginesBase.Models;

namespace EnginesBase.Services
{
    public class EntiryService
    {
        public Dictionary<int, Engine> GetEngines(string connectionString)
        {
            try
            {
                Dictionary<int, Engine> allEngines = new Dictionary<int, Engine>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    allEngines = GetEnginesFromDatabase(connection);
                }

                return allEngines;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<int, Engine> GetEnginesFromDatabase(SqlConnection connection)
        {
            try
            {
                Dictionary<int, Engine> allEngines = new Dictionary<int, Engine>();

                string engineQuery = "SELECT * FROM Engines";
                SqlCommand engineCommand = new SqlCommand(engineQuery, connection);

                using (SqlDataReader engineReader = engineCommand.ExecuteReader())
                {
                    while (engineReader.Read())
                    {
                        int engId = Convert.ToInt32(engineReader["EngineId"]);
                        Engine engine = new Engine
                        {
                            EngineId = engId,
                            Name = engineReader["Name"].ToString()
                        };

                        allEngines.Add(engId, engine);
                    }

                }

                return allEngines;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<int, Element> GetElements(string connectionString)
        {
            try
            {
                Dictionary<int, Element> allElements = new Dictionary<int, Element>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    allElements = GetElementsFromDatabase(connection);
                }

                return allElements;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<int, Element> GetElementsFromDatabase(SqlConnection connection)
        {
            try
            {
                Dictionary<int, Element> allElements = new Dictionary<int, Element>();

                string elementQuery = "SELECT * FROM Elements";
                SqlCommand elementCommand = new SqlCommand(elementQuery, connection);

                using (SqlDataReader elementReader = elementCommand.ExecuteReader())
                {
                    while (elementReader.Read())
                    {
                        int compId = Convert.ToInt32(elementReader["ElementId"]);
                        Element element = new Element
                        {
                            ElementId = compId,
                            Name = elementReader["Name"].ToString()
                        };

                        allElements.Add(compId, element);
                    }
                }

                return allElements;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Link> GetEngineLinks(string connectionString)
        {
            try
            {
                List<Link> allEngineLinks;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    allEngineLinks = GetEngineLinksFromDatabase(connection);
                }

                return allEngineLinks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Link> GetEngineLinksFromDatabase(SqlConnection connection)
        {
            try
            {
                List<Link> allEngineLinks = new List<Link>();

                string engineLinksQuery = "SELECT * FROM EngineLinks";
                SqlCommand engineLinksCommand = new SqlCommand(engineLinksQuery, connection);

                using (SqlDataReader engineLinksReader = engineLinksCommand.ExecuteReader())
                {
                    while (engineLinksReader.Read())
                    {
                        Link engineLink = new Link
                        {
                            ParentId = Convert.ToInt32(engineLinksReader["EngineId"]),
                            ChildId = Convert.ToInt32(engineLinksReader["ElementId"]),
                            Quantity = Convert.ToInt32(engineLinksReader["Quantity"])
                        };

                        allEngineLinks.Add(engineLink);
                    }
                }

                return allEngineLinks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Link> GetElementLinks(string connectionString)
        {
            try
            {
                List<Link> allElementLinks;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    allElementLinks = GetElementLinksFromDatabase(connection);
                }

                return allElementLinks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<Link> GetElementLinksFromDatabase(SqlConnection connection)
        {
            try
            {
                List<Link> allElementLinks = new List<Link>();

                string elementLinksQuery = "SELECT * FROM ElementLinks";
                SqlCommand elementLinksCommand = new SqlCommand(elementLinksQuery, connection);

                using (SqlDataReader elementLinksReader = elementLinksCommand.ExecuteReader())
                {
                    while (elementLinksReader.Read())
                    {
                        Link elementLink = new Link
                        {
                            ParentId = Convert.ToInt32(elementLinksReader["ParentId"]),
                            ChildId = Convert.ToInt32(elementLinksReader["ChildId"]),
                            Quantity = Convert.ToInt32(elementLinksReader["Quantity"])
                        };

                        allElementLinks.Add(elementLink);
                    }
                }

                return allElementLinks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
