using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnginesBase.Services
{
    public class GenerateReportService
    {

        CheckService checkService = new CheckService();
        GetFromDbService getFromDbService = new GetFromDbService();
        TextService textService = new TextService();

        public Dictionary<string, int> generateReportTable(string name, string connectionString)
        {
            Dictionary<string, int> report = new Dictionary<string, int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    if (checkService.CheckIfEngine(connection, name))
                    {
                        Debug.WriteLine("Вызываем метод отчёта для двигателя");
                        report = GenerateReportEngine(connection, name);
                    }
                    else
                    {
                        Debug.WriteLine("Вызываем метод отчёта для компонента");
                        report = GenerateReportElementByName(connection, name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }


            return report;

        }


        public Dictionary<string, int> GenerateReportEngine(SqlConnection connection, string engineName)
        {

            Dictionary<string, int> report = new Dictionary<string, int>();

            try
            {
                int engineId = getFromDbService.GetEngineIdByName(connection, engineName);

                if (checkService.CheckEngineHasChildElements(connection, engineId))
                {
                    List<int> childElemetId = getFromDbService.GetElementsByEngineId(connection, engineId);

                    foreach (int elementId in childElemetId)
                    {
                        if (checkService.CheckElementHasChildElements(connection, elementId))
                        {
                            int elementQuantity = getFromDbService.GetElementOfEngineQuantity(connection, engineId, elementId);
                            Dictionary<string, int> dictionaryOfElement = generateReportElementById(connection, elementId);
                            report = CombineDictionaries(report, dictionaryOfElement, elementQuantity);

                        }
                        else
                        {
                            string elementName = getFromDbService.GetElementNameById(connection, elementId);
                            int quantity = getFromDbService.GetElementOfEngineQuantity(connection, engineId, elementId);
                            report.Add(elementName, quantity);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
            return report;

        }


        public Dictionary<string, int> GenerateReportElementByName(SqlConnection connection, string elementName)
        {
            Dictionary<string, int> report = new Dictionary<string, int>();

            try
            {
                int elementId = getFromDbService.GetElementIdByName(connection, textService.ExtractedElementName(elementName));


                if (checkService.CheckElementHasChildElements(connection, elementId))
                {
                    List<int> childElemetsId = getFromDbService.GetChildElements(connection, elementId);

                    foreach (int childId in childElemetsId)
                    {
                        int childElementQuantity = getFromDbService.GetChildElementOfElementQuantity(connection, elementId, childId);
                        Dictionary<string, int> dictionaryOfElement = generateReportElementById(connection, childId);
                        report = CombineDictionaries(report, dictionaryOfElement, childElementQuantity);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }

            return report;
        }


        public Dictionary<string, int> generateReportElementById(SqlConnection connection, int elementId)
        {

            Dictionary<string, int> report = new Dictionary<string, int>();

            try
            {
                if (checkService.CheckElementHasChildElements(connection, elementId))
                {
                    List<int> childElementsId = getFromDbService.GetChildElements(connection, elementId);
                    foreach (int id in childElementsId)
                    {
                        int quantity = getFromDbService.GetChildElementOfElementQuantity(connection, elementId, id);
                        report = CombineDictionaries(report, generateReportElementById(connection, id), quantity);
                    }
                }
                else
                {
                    string elementName = getFromDbService.GetElementNameById(connection, elementId);
                    report.Add(elementName, 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
            return report;

        }


        static Dictionary<string, int> CombineDictionaries(Dictionary<string, int> dict1, Dictionary<string, int> dict2, int multiplier)
        {
            try
            {
                var mergedDict = dict1.Concat(dict2.ToDictionary(pair => pair.Key, pair => pair.Value * multiplier));

                return mergedDict.ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}

