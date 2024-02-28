using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using EnginesBase.Models;
using Word = Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;


namespace EnginesBase
{
    public partial class FormQuantity : System.Windows.Forms.Form
    {
        private DatabaseHelper dbHelper;


        public FormQuantity()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            UpdateTreeViewData();
            textBoxQuantity.KeyPress += new KeyPressEventHandler(textBoxQuantity_KeyPress);

        }


        // Добавить двигатель
        private void buttonAddEngine_Click(object sender, EventArgs e)
        {

            string engineName = textBoxName.Text;

            // Проверяем, что имя двигателя не является пустым
            if (!string.IsNullOrEmpty(engineName))
            {
                // Вызываем метод InsertEngine для добавления нового двигателя в БД
                dbHelper.InsertEngine(engineName);

                // Оповещаем пользователя об успешном добавлении
                MessageBox.Show("Двигатель успешно добавлен в базу данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Очищаем поле ввода после добавления
                textBoxName.Text = "";
                UpdateTreeViewData();
            }
            else
            {
                // Оповещаем пользователя, если поле ввода пусто
                MessageBox.Show("Пожалуйста, введите имя двигателя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ДОБАВИТЬ КОМПОННЕТ В ДРЕВО?
        private void AddComponentsToNode(TreeNode parentNode, Dictionary<Element, int> components)
        {
            foreach (var entry in components)
            {
                Element component = entry.Key;
                int quantity = entry.Value;

                TreeNode componentNode = new TreeNode($"{component.Name} : {quantity} шт");
                parentNode.Nodes.Add(componentNode);

                if (component.ChildElements != null && component.ChildElements.Count > 0)
                {
                    AddComponentsToNode(componentNode, component.ChildElements);
                }
            }

        }


        //Переименовать
        private void buttonChangeName_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string selectedNodeText = treeView1.SelectedNode.Text;
                string newName = textBoxName.Text;

                // Проверка на совпадение нового имени с текущим
                if (selectedNodeText == newName)
                {
                    MessageBox.Show("Новое имя совпадает со старым");
                    return;
                }

                // Изменение имени
                try
                {
                    dbHelper.ChangeName(selectedNodeText, newName);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            UpdateTreeViewData();

        }


        private void button1_Click(object sender, EventArgs e)
        {
            UpdateTreeViewData();

        }



        public void UpdateTreeViewData()
        {
            List<Engine> engines = dbHelper.EngineAssembly();

            treeView1.Nodes.Clear();

            foreach (Engine engine in engines)
            {
                TreeNode engineNode = new TreeNode(engine.Name);

                foreach (KeyValuePair<Element, int> kvp in engine.ChildElements)

                {

                    Element component = kvp.Key;
                    int quantity = kvp.Value;
                    TreeNode componentNode = new TreeNode($"{component.Name} : {quantity} шт");


                    foreach (KeyValuePair<Element, int> subcomponentEntry in component.ChildElements)
                    {
                        Element subcomponent = subcomponentEntry.Key;
                        int subquantity = subcomponentEntry.Value;

                        TreeNode subcomponentNode = new TreeNode($"{subcomponent.Name} : {subquantity} шт");
                        componentNode.Nodes.Add(subcomponentNode);
                    }

                    engineNode.Nodes.Add(componentNode);
                }

                treeView1.Nodes.Add(engineNode);
            }
            treeView1.ExpandAll();
            Console.ReadLine();

        }

        private void buttonAddComponent_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                // Получаем имя выделенной записи в treeView1
                string parentName = treeView1.SelectedNode.Text;

                string newComponentName = textBoxName.Text.Trim();

                // Проверка на то, что наименование нового компонента не пусто
                if (!string.IsNullOrEmpty(newComponentName))
                {
                    int quantity;

                    // Проверка на то, что textBoxQuantity не пуст и не равно 0
                    if (int.TryParse(textBoxQuantity.Text, out quantity) && quantity > 0)
                    {
                        // Вызываем метод InsertEngine для добавления нового компонента в БД
                        dbHelper.InsertElement(parentName, newComponentName, quantity);

                        // Оповещаем пользователя об успешном добавлении
                        MessageBox.Show("Компонент успешно добавлен в базу данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Очищаем поля ввода после добавления
                        textBoxName.Text = "";
                        textBoxQuantity.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, введите корректное количество (больше 0).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите имя компонента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите Двигатель или Компонент для добавления компонента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateTreeViewData();

        }




        private void textBoxQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем ввод только цифр, клавиши Backspace и клавиши управления (например, Ctrl+C, Ctrl+V)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Запрещаем ввод символа
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string deletedName = treeView1.SelectedNode.Text;

                bool isEngine = IsEngineNode(treeView1.SelectedNode);

                if (isEngine)
                {
                    // Удаляем двигатель
                    dbHelper.DeleteEngine(deletedName);
                }
                else
                {
                    // Удаляем компонент
                    string parentName = GetParentName(treeView1.SelectedNode);
                    Debug.WriteLine("В форме: удаляем компонент + " + deletedName + " с родителем " + parentName);
                    dbHelper.DeleteElementWithParent(deletedName, parentName);
                }

                // Обновляем отображение дерева после удаления
                UpdateTreeViewData();
            }
        }

        private bool IsEngineNode(TreeNode node)
        {
            // Проверяем, есть ли у узла родитель
            return node.Parent == null;
        }

        private string GetParentName(TreeNode node)
        {
            // Получаем родительскую запись в дереве
            TreeNode parentNode = node.Parent;

            // Если у узла нет родителя, возвращаем пустую строку
            if (parentNode == null)
            {
                return string.Empty;
            }

            // Возвращаем текст родительской записи
            return parentNode.Text;
        }

        private void buttonReport_Click(object sender, EventArgs e)
        {
            // Получаем выделенный узел в treeView1
            TreeNode selectedNode = treeView1.SelectedNode;

            if (selectedNode != null)
            {
                // Получаем данные для формирования отчета
                string componentName = selectedNode.Text;
                Dictionary<string, int> componentDetails = GetComponentDetails(componentName);

                // Создаем и сохраняем отчет в MS Word
                GenerateWordReport(componentName, componentDetails);
            }
            else
            {
                MessageBox.Show("Выберите компонент для формирования отчета.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private Dictionary<string, int> GetComponentDetails(string componentName)
        {
            // Здесь реализуйте логику получения деталей компонента
            // Например, запрос к базе данных или другой источник данных

            // Вернем тестовые данные для примера
            Dictionary<string, int> details = new Dictionary<string, int>
            {
                { "Цилиндр", 2 },
                { "Статор", 1 },
                { "Поршень", 1 }
            };

            return details;
        }

        private void GenerateWordReport(string componentName, Dictionary<string, int> componentDetails)
        {
            Word.Application wordApp = new Word.Application();
            wordApp.Visible = true;

            Word.Document doc = wordApp.Documents.Add();
            doc.Content.Font.Size = 12;
            doc.Content.Paragraphs.Add($"Отчет по компоненту: {componentName}");

            // Добавляем таблицу
            Word.Table table = doc.Tables.Add(doc.Content.Paragraphs[2].Range, componentDetails.Count + 1, 2);
            table.Borders.Enable = 1;

            // Заполняем заголовки таблицы
            table.Cell(1, 1).Range.Text = "Компонент";
            table.Cell(1, 2).Range.Text = "Количество";

            // Заполняем таблицу данными
            int row = 2;
            foreach (var detail in componentDetails)
            {
                table.Cell(row, 1).Range.Text = detail.Key;
                table.Cell(row, 2).Range.Text = detail.Value.ToString();
                row++;
            }
        }
    

}
}
