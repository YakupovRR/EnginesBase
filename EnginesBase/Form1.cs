using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using EnginesBase.Models;
using Word = Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;


namespace EnginesBase
{
    public partial class FormQuantity : System.Windows.Forms.Form
    {
        private DatabaseFacade dbFacade;


        public FormQuantity()
        {
            InitializeComponent();
            dbFacade = new DatabaseFacade();
            UpdateTreeViewData();
            textBoxQuantity.KeyPress += new KeyPressEventHandler(textBoxQuantity_KeyPress);

        }


        private void buttonAddEngine_Click(object sender, EventArgs e)
        {
            string engineName = textBoxName.Text;

            if (!string.IsNullOrEmpty(engineName))
            {
                dbFacade.InsertEngine(engineName);
                MessageBox.Show("Двигатель успешно добавлен.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxName.Text = "";
                UpdateTreeViewData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите имя двигателя.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


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


        private void buttonChangeName_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string selectedNodeText = treeView1.SelectedNode.Text;
                string newName = textBoxName.Text;

                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show("Введите новое имя");
                    return;
                }

                if (selectedNodeText == newName)
                {
                    MessageBox.Show("Новое имя совпадает со старым");
                    return;
                }

                if (dbFacade.checkNewNameExist(newName))
                {
                    MessageBox.Show("Имя уже используется. Введите другое");
                    return;
                }

                try
                {
                    dbFacade.ChangeName(selectedNodeText, newName);
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
            List<Engine> engines = dbFacade.EngineAssembly();

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

                string parentName = treeView1.SelectedNode.Text;

                string newComponentName = textBoxName.Text.Trim();

                if (!string.IsNullOrEmpty(newComponentName))
                {
                    int quantity;

                    if (int.TryParse(textBoxQuantity.Text, out quantity) && quantity > 0)
                    {
                        dbFacade.InsertElement(parentName, newComponentName, quantity);
                        MessageBox.Show("Компонент успешно добавлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxName.Text = "";
                        textBoxQuantity.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, введите корректное количество (больше 0).",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите имя компонента.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите Двигатель или Компонент для добавления компонента.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateTreeViewData();
        }

        private void textBoxQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
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
                    dbFacade.DeleteEngine(deletedName);
                    MessageBox.Show("Двигатель удалён.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string parentName = GetParentName(treeView1.SelectedNode);
                    dbFacade.DeleteElement(deletedName, parentName);
                    MessageBox.Show("Компонент удалён.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


                UpdateTreeViewData();
            }
        }

        private bool IsEngineNode(TreeNode node)
        {
            return node.Parent == null;
        }

        private string GetParentName(TreeNode node)
        {
            TreeNode parentNode = node.Parent;

            if (parentNode == null)
            {
                return string.Empty;
            }

            return parentNode.Text;
        }

        private void buttonReport_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;

            if (selectedNode != null)
            {
                string componentName = selectedNode.Text;
                Dictionary<string, int> componentDetails = dbFacade.generateReportTable(componentName);


                GenerateWordReport(componentDetails);
            }
            else
            {
                MessageBox.Show("Выберите компонент для формирования отчета.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void GenerateWordReport(Dictionary<string, int> componentDetails)
        {
            if (componentDetails.Count == 0)
            {
                MessageBox.Show("Для данного элемента отсутствуют компоненты");
                return;
            }

            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Document doc = word.Documents.Add();

            Table table = doc.Tables.Add(doc.Content, componentDetails.Count, 2);
            table.Borders.Enable = 1;

            int row = 1;
            foreach (var kvp in componentDetails)
            {
                table.Cell(row, 1).Range.Text = kvp.Key;
                table.Cell(row, 2).Range.Text = $"{kvp.Value} шт";
                row++;
            }

            word.Visible = true;
        }



    }
}
