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


        // �������� ���������
        private void buttonAddEngine_Click(object sender, EventArgs e)
        {

            string engineName = textBoxName.Text;

            // ���������, ��� ��� ��������� �� �������� ������
            if (!string.IsNullOrEmpty(engineName))
            {
                // �������� ����� InsertEngine ��� ���������� ������ ��������� � ��
                dbHelper.InsertEngine(engineName);

                // ��������� ������������ �� �������� ����������
                MessageBox.Show("��������� ������� �������� � ���� ������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ������� ���� ����� ����� ����������
                textBoxName.Text = "";
                UpdateTreeViewData();
            }
            else
            {
                // ��������� ������������, ���� ���� ����� �����
                MessageBox.Show("����������, ������� ��� ���������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // �������� ��������� � �����?
        private void AddComponentsToNode(TreeNode parentNode, Dictionary<Element, int> components)
        {
            foreach (var entry in components)
            {
                Element component = entry.Key;
                int quantity = entry.Value;

                TreeNode componentNode = new TreeNode($"{component.Name} : {quantity} ��");
                parentNode.Nodes.Add(componentNode);

                if (component.ChildElements != null && component.ChildElements.Count > 0)
                {
                    AddComponentsToNode(componentNode, component.ChildElements);
                }
            }

        }


        //�������������
        private void buttonChangeName_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string selectedNodeText = treeView1.SelectedNode.Text;
                string newName = textBoxName.Text;

                // �������� �� ���������� ������ ����� � �������
                if (selectedNodeText == newName)
                {
                    MessageBox.Show("����� ��� ��������� �� ������");
                    return;
                }

                // ��������� �����
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
                    TreeNode componentNode = new TreeNode($"{component.Name} : {quantity} ��");


                    foreach (KeyValuePair<Element, int> subcomponentEntry in component.ChildElements)
                    {
                        Element subcomponent = subcomponentEntry.Key;
                        int subquantity = subcomponentEntry.Value;

                        TreeNode subcomponentNode = new TreeNode($"{subcomponent.Name} : {subquantity} ��");
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
                // �������� ��� ���������� ������ � treeView1
                string parentName = treeView1.SelectedNode.Text;

                string newComponentName = textBoxName.Text.Trim();

                // �������� �� ��, ��� ������������ ������ ���������� �� �����
                if (!string.IsNullOrEmpty(newComponentName))
                {
                    int quantity;

                    // �������� �� ��, ��� textBoxQuantity �� ���� � �� ����� 0
                    if (int.TryParse(textBoxQuantity.Text, out quantity) && quantity > 0)
                    {
                        // �������� ����� InsertEngine ��� ���������� ������ ���������� � ��
                        dbHelper.InsertElement(parentName, newComponentName, quantity);

                        // ��������� ������������ �� �������� ����������
                        MessageBox.Show("��������� ������� �������� � ���� ������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // ������� ���� ����� ����� ����������
                        textBoxName.Text = "";
                        textBoxQuantity.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("����������, ������� ���������� ���������� (������ 0).", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("����������, ������� ��� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("����������, �������� ��������� ��� ��������� ��� ���������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateTreeViewData();

        }




        private void textBoxQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ��������� ���� ������ ����, ������� Backspace � ������� ���������� (��������, Ctrl+C, Ctrl+V)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // ��������� ���� �������
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
                    // ������� ���������
                    dbHelper.DeleteEngine(deletedName);
                }
                else
                {
                    // ������� ���������
                    string parentName = GetParentName(treeView1.SelectedNode);
                    Debug.WriteLine("� �����: ������� ��������� + " + deletedName + " � ��������� " + parentName);
                    dbHelper.DeleteElementWithParent(deletedName, parentName);
                }

                // ��������� ����������� ������ ����� ��������
                UpdateTreeViewData();
            }
        }

        private bool IsEngineNode(TreeNode node)
        {
            // ���������, ���� �� � ���� ��������
            return node.Parent == null;
        }

        private string GetParentName(TreeNode node)
        {
            // �������� ������������ ������ � ������
            TreeNode parentNode = node.Parent;

            // ���� � ���� ��� ��������, ���������� ������ ������
            if (parentNode == null)
            {
                return string.Empty;
            }

            // ���������� ����� ������������ ������
            return parentNode.Text;
        }

        private void buttonReport_Click(object sender, EventArgs e)
        {
            // �������� ���������� ���� � treeView1
            TreeNode selectedNode = treeView1.SelectedNode;

            if (selectedNode != null)
            {
                // �������� ������ ��� ������������ ������
                string componentName = selectedNode.Text;
                Dictionary<string, int> componentDetails = GetComponentDetails(componentName);

                // ������� � ��������� ����� � MS Word
                GenerateWordReport(componentName, componentDetails);
            }
            else
            {
                MessageBox.Show("�������� ��������� ��� ������������ ������.", "��������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private Dictionary<string, int> GetComponentDetails(string componentName)
        {
            // ����� ���������� ������ ��������� ������� ����������
            // ��������, ������ � ���� ������ ��� ������ �������� ������

            // ������ �������� ������ ��� �������
            Dictionary<string, int> details = new Dictionary<string, int>
            {
                { "�������", 2 },
                { "������", 1 },
                { "�������", 1 }
            };

            return details;
        }

        private void GenerateWordReport(string componentName, Dictionary<string, int> componentDetails)
        {
            Word.Application wordApp = new Word.Application();
            wordApp.Visible = true;

            Word.Document doc = wordApp.Documents.Add();
            doc.Content.Font.Size = 12;
            doc.Content.Paragraphs.Add($"����� �� ����������: {componentName}");

            // ��������� �������
            Word.Table table = doc.Tables.Add(doc.Content.Paragraphs[2].Range, componentDetails.Count + 1, 2);
            table.Borders.Enable = 1;

            // ��������� ��������� �������
            table.Cell(1, 1).Range.Text = "���������";
            table.Cell(1, 2).Range.Text = "����������";

            // ��������� ������� �������
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
