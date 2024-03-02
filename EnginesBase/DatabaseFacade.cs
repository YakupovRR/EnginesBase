using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using EnginesBase.Models;
using EnginesBase.Services;


namespace EnginesBase
{
    public class DatabaseFacade
    {
        private GetFromDbService getFromDbService = new GetFromDbService();
        private ChangeNameService changeNameService = new ChangeNameService();
        private InsertService insertService = new InsertService();
        private DeleteService deleteService = new DeleteService();
        private GenerateReportService generateReportService = new GenerateReportService();

        private static string serverName = "DESKTOP-LMC9S02\\SQLEXPRESS";
        private static string databaseName = "EnginesDb";
        private static string integratedSecurity = "true";
        private static string trustServerCertificate = "true";
        private string connectionString = "Server=" + serverName +
                       ";Database=" + databaseName +
                       ";Integrated Security=" + integratedSecurity +
                       ";TrustServerCertificate=" + trustServerCertificate +
                       ";";



        public void InsertEngine(string name)
        {
            insertService.InsertEngine(name, connectionString);
        }

        public void InsertElement(string parentName, string newElementName, int quantity)
        {
            insertService.InsertElement(parentName, newElementName, quantity, connectionString);
        }

        public List<Engine> EngineAssembly()
        {
            return getFromDbService.EngineAssembly(connectionString);
        }

        public void ChangeName(string oldName, string newName)
        { changeNameService.ChangeName(oldName, newName, connectionString); }

        public void DeleteEngine(string deletedName)
        { deleteService.DeleteEngine(deletedName, connectionString); }

        public void DeleteElement(string deletedName, string parentName)
        { deleteService.DeleteElementWithParent(deletedName, parentName, connectionString); }

        public Dictionary<string, int> generateReportTable(string name)
        { return generateReportService.generateReportTable(name, connectionString); }


    }
}
