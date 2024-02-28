using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using EnginesBase.Models;


namespace EnginesBase
{
    public class DatabaseHelper
    {
        private readonly string connectionString = "Server=DESKTOP-LMC9S02\\SQLEXPRESS;Database=EnginesDb;Integrated Security=True;TrustServerCertificate=True;";





        public void InsertEngine(string name)
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




        public List<Engine> EngineAssembly()
        {

            Dictionary<int, Element> elements = new Dictionary<int, Element>();
            elements = GetElements();
            List<Link> elementLinks = new List<Link>();
            elementLinks = GetElementLinks();

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
            engines = GetEngines();
            List<Link> engineLinks = new List<Link>();
            engineLinks = GetEngineLinks();


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


        // Начало получение всех сущностей
        // Сервис
        public Dictionary<int, Engine> GetEngines()
        {
            Dictionary<int, Engine> allEngines = new Dictionary<int, Engine>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                allEngines = GetEnginesFromDatabase(connection);
            }

            return allEngines;
        }


        private Dictionary<int, Engine> GetEnginesFromDatabase(SqlConnection connection)
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

                    // Добавляем engine в список allEngines

                    allEngines.Add(engId, engine);
                }
            }

            return allEngines;
        }

        // Сервис
        public Dictionary<int, Element> GetElements()
        {
            Dictionary<int, Element> allElements = new Dictionary<int, Element>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                allElements = GetElementsFromDatabase(connection);
            }

            return allElements;
        }



        private Dictionary<int, Element> GetElementsFromDatabase(SqlConnection connection)
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

                    // Добавляем element в список allElements
                    allElements.Add(compId, element);
                }
            }

            return allElements;
        }



        // Сервис
        public List<Link> GetEngineLinks()
        {
            List<Link> allEngineLinks;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                allEngineLinks = GetEngineLinksFromDatabase(connection);
            }

            return allEngineLinks;
        }


        private List<Link> GetEngineLinksFromDatabase(SqlConnection connection)
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

                    // Добавляем engineLink в список allEngineLinks
                    allEngineLinks.Add(engineLink);
                }
            }

            return allEngineLinks;
        }


        public List<Link> GetElementLinks()
        {
            List<Link> allElementLinks;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                allElementLinks = GetElementLinksFromDatabase(connection);
            }

            return allElementLinks;
        }


        private List<Link> GetElementLinksFromDatabase(SqlConnection connection)
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

                    // Добавляем elementLink в список allElementLinks
                    allElementLinks.Add(elementLink);
                }
            }

            return allElementLinks;
        }

        // конец получения всех сущностей





        // Изменение названия
        public void ChangeName(string oldName, string newName)
        {
            Debug.WriteLine("Метод изменения названия, oldName = " + oldName + "новое имя = " + newName);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Определение типа по старому имени
                bool isEngine = CheckIfEngine(connection, oldName);

                if (!isEngine)
                {
                    oldName = ExtractedElementName(oldName);
                }

                // Проверка на совпадение нового имени с существующими именами в таблицах Engines и Elements
                string checkQuery = "SELECT COUNT(*) FROM Engines WHERE Name = @NewName " +
                                    "UNION " +
                                    "SELECT COUNT(*) FROM Elements WHERE Name = @NewName";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@NewName", newName);

                int nameCount = (int)checkCommand.ExecuteScalar();

                if (nameCount > 0)
                {
                    // Новое имя уже существует, выбросить исключение или обработать по вашему усмотрению
                    throw new InvalidOperationException("Новое имя уже существует");
                }


                if (isEngine)
                {
                    ChangeEngineName(connection, oldName, newName);
                }
                else
                {
                    ChangeElementName(connection, oldName, newName);
                }

            }
        }

        private bool CheckIfEngine(SqlConnection connection, string name)
        {
            string checkEngineQuery = "SELECT COUNT(*) FROM Engines WHERE Name = @Name";
            SqlCommand checkEngineCommand = new SqlCommand(checkEngineQuery, connection);
            checkEngineCommand.Parameters.AddWithValue("@Name", name);

            int engineCount = (int)checkEngineCommand.ExecuteScalar();
            return engineCount > 0;
        }

        private void ChangeEngineName(SqlConnection connection, string oldName, string newName)
        {
            Debug.WriteLine("Метод изменения названия двигателя");
            string updateEngineQuery = "UPDATE Engines SET Name = @NewName WHERE Name = @OldName";
            SqlCommand updateEngineCommand = new SqlCommand(updateEngineQuery, connection);
            updateEngineCommand.Parameters.AddWithValue("@NewName", newName);
            updateEngineCommand.Parameters.AddWithValue("@OldName", oldName);

            updateEngineCommand.ExecuteNonQuery();
        }

        private void ChangeElementName(SqlConnection connection, string oldName, string newName)
        {
            Debug.WriteLine("Метод изменения названия компонента");

            string updateElementQuery = "UPDATE Elements SET Name = @NewName WHERE Name = @OldName";
            SqlCommand updateElementCommand = new SqlCommand(updateElementQuery, connection);
            updateElementCommand.Parameters.AddWithValue("@NewName", newName);
            updateElementCommand.Parameters.AddWithValue("@OldName", oldName);

            updateElementCommand.ExecuteNonQuery();
        }

        // конец изменения названия




        // добавление компонента

        public void InsertElement(string parentName, string newElementName, int quantity)
        {
            Debug.WriteLine("Метод добавления компонента к " + parentName + " с названием = " + newElementName);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool parentIsEngine = CheckIfEngine(connection, parentName);
                bool elementAlreadyExist = isElementExist(connection, newElementName);

                int newElementId;

                if (elementAlreadyExist)
                {
                    newElementId = GetElementIdByName(connection, newElementName);
                }
                else
                {
                    newElementId = CreateElementInDb(connection, newElementName);
                }

                if (parentIsEngine)
                {
                    Debug.WriteLine("Родитель - двигатель ");
                    int parentId = GetEngineIdByName(connection, parentName);

                    InsertEngineLink(connection, parentId, newElementId, quantity);
                }
                else
                {
                    Debug.WriteLine("Родитель - компонент ");
                    int parentId = GetElementIdByName(connection, parentName);

                    if (IsRecursiveAddition(connection, newElementId, parentId))
                    {
                        InsertElementLink(connection, parentId, newElementId, quantity);
                                            } 
                    else
                    {
                        Debug.WriteLine("Рекурсивное добавление отменено");
                        if (!elementAlreadyExist) { DeleteElementWithoutParent(connection, newElementId); }

                    }

                }
            }
        }


        private bool isElementExist (SqlConnection connection, string elementName)
        {

            string query = "SELECT COUNT(*) FROM Elements WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", elementName);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }

        }



        public int CreateElementInDb(SqlConnection connection, string elementName)
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


        private int GetEngineIdByName(SqlConnection connection, string engineName)
        {
            // Получаем EngineId по имени двигателя
            string getEngineIdQuery = "SELECT EngineId FROM Engines WHERE Name = @Name";
            SqlCommand getEngineIdCommand = new SqlCommand(getEngineIdQuery, connection);
            getEngineIdCommand.Parameters.AddWithValue("@Name", engineName);

            int engineId = (int)getEngineIdCommand.ExecuteScalar();

            return engineId;
        }

        private int GetElementIdByName(SqlConnection connection, string elementName)
        {
            string extractedElementName = ExtractedElementName(elementName);
            Debug.WriteLine("Название компонента после очистки от количества " + extractedElementName);

            // Получаем ElementId по имени компонента
            string getElementIdQuery = "SELECT ElementId FROM Elements WHERE Name = @Name";
            SqlCommand getElementIdCommand = new SqlCommand(getElementIdQuery, connection);
            getElementIdCommand.Parameters.AddWithValue("@Name", extractedElementName);

            int elementId = (int)getElementIdCommand.ExecuteScalar();

            return elementId;
        }


        private void InsertEngineLink(SqlConnection connection, int engineId, int elementId, int quantity)
        {
            // Вставляем запись в таблицу EngineLinks
            string insertEngineLinkQuery = "INSERT INTO EngineLinks (EngineId, ElementId, Quantity) VALUES (@EngineId, @ElementId, @Quantity)";
            SqlCommand insertEngineLinkCommand = new SqlCommand(insertEngineLinkQuery, connection);
            insertEngineLinkCommand.Parameters.AddWithValue("@EngineId", engineId);
            insertEngineLinkCommand.Parameters.AddWithValue("@ElementId", elementId);
            insertEngineLinkCommand.Parameters.AddWithValue("@Quantity", quantity);

            insertEngineLinkCommand.ExecuteNonQuery();
        }

        private void InsertElementLink(SqlConnection connection, int parentId, int childId, int quantity)
        {
            // Вставляем запись в таблицу ElementLinks
            string insertElementLinkQuery = "INSERT INTO ElementLinks (ParentId, ChildId, Quantity) VALUES (@ParentId, @ChildId, @Quantity)";
            SqlCommand insertElementLinkCommand = new SqlCommand(insertElementLinkQuery, connection);
            insertElementLinkCommand.Parameters.AddWithValue("@ParentId", parentId);
            insertElementLinkCommand.Parameters.AddWithValue("@ChildId", childId);
            insertElementLinkCommand.Parameters.AddWithValue("@Quantity", quantity);

            insertElementLinkCommand.ExecuteNonQuery();
        }


        // закончили добавление компонента


        // почистить название компонента от количество

        public string ExtractedElementName(string elementName)
        {
            string[] elementsInfo = elementName.Split(':');
            string extractedElementName = elementsInfo[0].Trim();

            return extractedElementName;
        }





        // проверка что компонент кем-то используется

        public bool IsElementUnused(int elementId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool isUsedInElementLinks = IsElementUsedInElementLinks(connection, elementId);
                bool isUsedInEngineLinks = IsElementUsedInEngineLinks(connection, elementId);

                // Вернуть true, если компонент не используется нигде
                return !isUsedInElementLinks && !isUsedInEngineLinks;
            }
        }

        private bool IsElementUsedInElementLinks(SqlConnection connection, int elementId)
        {
            string query = "SELECT COUNT(*) FROM ElementLinks WHERE ChildId = @ElementId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ElementId", elementId);

            int count = (int)command.ExecuteScalar();
            Debug.WriteLine("Компонент используется другими компонентами " + count + " раз");
            // Вернуть true, если компонент используется в таблице ElementLinks
            return count > 0;
        }

        private bool IsElementUsedInEngineLinks(SqlConnection connection, int elementId)
        {
            string query = "SELECT COUNT(*) FROM EngineLinks WHERE ElementId = @ElementId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ElementId", elementId);

            int count = (int)command.ExecuteScalar();
            Debug.WriteLine("Компонент используется двигателями :" + count + " раз");

            // Вернуть true, если компонент используется в таблице EngineLinks
            return count > 0;
        }


        //удаление 


        public void DeleteEngine(string engineName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int engineId = GetEngineIdByName(connection, engineName);
                Debug.WriteLine("Удаляем двигатель " + engineName + "с id " + engineId);

                List<int> idChildElementsList = GetElementsByEngineId(connection, engineId);
                DeleteAllEngineLinks(connection, engineId);
                DeleteEngineFromDb(connection, engineId);
                DeleteUnusedChildElementList(connection, idChildElementsList);
            }

        }

        public void DeleteAllEngineLinks(SqlConnection connection, int engineId)
        {
            Debug.WriteLine("Удаляем все EngineLinks двигателя с id " + engineId);

            string deleteEngineLinksQuery = "DELETE FROM EngineLinks WHERE EngineId = @EngineId";
            SqlCommand deleteEngineLinksCommand = new SqlCommand(deleteEngineLinksQuery, connection);
            deleteEngineLinksCommand.Parameters.AddWithValue("@EngineId", engineId);
            deleteEngineLinksCommand.ExecuteNonQuery();

        }

        public void DeleteEngineFromDb(SqlConnection connection, int engineId)
        {
            Debug.WriteLine("Удаляем из БД двигатель с id " + engineId);

            string deleteEngineQuery = "DELETE FROM Engines WHERE EngineId = @EngineId";
            SqlCommand deleteEngineCommand = new SqlCommand(deleteEngineQuery, connection);
            deleteEngineCommand.Parameters.AddWithValue("@EngineId", engineId);
            deleteEngineCommand.ExecuteNonQuery();

        }

        public void DeleteElementWithParent(string deletedElementName, string parentName)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int deletedElementId = GetElementIdByName(connection, deletedElementName);
                Debug.WriteLine("Удаляем компонент " + deletedElementName + "с id " + deletedElementId);
                Boolean parentIsEngine = CheckIfEngine(connection, parentName);
                int parentId;

                if (parentIsEngine)
               {
                    parentId = GetEngineIdByName(connection, parentName);
                    Debug.WriteLine("Его родитель двигатель " + parentName + " с id " + parentId);
                    DeleteEngineLink(connection, parentId, deletedElementId);
                }
                else
                {
                    parentId = GetElementIdByName(connection, parentName);
                    Debug.WriteLine("Его а " + parentName + " с id " + parentId);
                    DeleteElementLink(connection, parentId, deletedElementId);
                }

                DeleteElementWithoutParent(connection, deletedElementId);
            }
        }

        public void DeleteEngineLink(SqlConnection connection, int engineId, int elementId)
        {
            Debug.WriteLine("Удаляем EngineLink двигателя с id " + engineId + "и компонента с id " + elementId);

            string query = "DELETE FROM EngineLinks WHERE EngineId = @EngineId AND ElementId = @ElementId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EngineId", engineId);
                command.Parameters.AddWithValue("@ElementId", elementId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteElementLink(SqlConnection connection, int parentId, int childId)
        {
            Debug.WriteLine("Удаляем ElementLink род. компонента id " + parentId + "и доч. компонента id " + childId);

            string query = "DELETE FROM ElementLinks WHERE ParentId = @ParentId AND ChildId = @ChildId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ParentId", parentId);
                command.Parameters.AddWithValue("@ChildId", childId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteUnusedChildElementList(SqlConnection connection, List<int> childElementsIds)
        {
            foreach (int elementId in childElementsIds)
            {
                if (IsElementUnused(elementId)) { DeleteElementWithoutParent(connection, elementId); }
            }

        }

        public void DeleteElementWithoutParent(SqlConnection connection, int elementId)
        {
            Debug.WriteLine("Удаляем компонент без родителей с id " + elementId);

            List<int> childElements = GetChildElements(connection, elementId);

            string query = "DELETE FROM Elements WHERE ElementId = @ElementId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ElementId", elementId);
                command.ExecuteNonQuery();
            }

            DeleteUnusedChildElementList(connection, childElements);

        }

        public List<int> GetElementsByEngineId(SqlConnection connection, int engineId)
        {
            Debug.WriteLine("Получаем компоненты двигателя с id " + engineId);

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

        public List<int> GetChildElements(SqlConnection connection, int elementId)
        {

            Debug.WriteLine("Получаем доч. компоненты компонента с id " + elementId);


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

        //проверка на рекурсию

        public bool IsRecursiveAddition(SqlConnection connection, int idChildComponent, int idParentComponent)
        {
            if (idChildComponent != idParentComponent)
            {
                if (ElementLinkDoesNotExist(connection, idChildComponent, idParentComponent))
                {
                    List<int> childIds = GetChildElements(connection, idChildComponent);

                    return AreChildIdsValid(connection, childIds, idParentComponent);
                }
            }

            return false;
        }

    
        public bool ElementLinkDoesNotExist(SqlConnection connection, int parentId, int childId)
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

        private bool AreChildIdsValid(SqlConnection connection, List<int> childIds, int idParentComponent)
        {
            foreach (int childId in childIds)
            {
                if (!ElementLinkDoesNotExist(connection, childId, idParentComponent))
                {
                    return false;
                }
            }

            return true;
        }


        //конец проверки на рекурсию





    }
}
