using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnginesBase.Services
{
    public class TextService
    {
        public string ExtractedElementName(string elementName)
        {
            try
            {
                string[] elementsInfo = elementName.Split(':');
            string extractedElementName = elementsInfo[0].Trim();

            return extractedElementName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при извлечении из " + elementName + " имени компонента: {ex.Message}");
                return elementName;
            }
        }
    }
}
