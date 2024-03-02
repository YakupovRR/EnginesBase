namespace EnginesBase
{
    partial class FormQuantity
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonAddEngine = new Button();
            treeView1 = new TreeView();
            textBoxName = new TextBox();
            button1 = new Button();
            labelName = new Label();
            labelQuantity = new Label();
            textBoxQuantity = new TextBox();
            buttonChangeName = new Button();
            buttonAddComponent = new Button();
            buttonDelete = new Button();
            buttonReport = new Button();
            SuspendLayout();
            // 
            // buttonAddEngine
            // 
            buttonAddEngine.Location = new Point(452, 173);
            buttonAddEngine.Name = "buttonAddEngine";
            buttonAddEngine.Size = new Size(156, 23);
            buttonAddEngine.TabIndex = 0;
            buttonAddEngine.Text = "Добавить двигатель";
            buttonAddEngine.UseVisualStyleBackColor = true;
            buttonAddEngine.Click += buttonAddEngine_Click;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(137, 52);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(223, 318);
            treeView1.TabIndex = 1;
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(452, 70);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(156, 23);
            textBoxName.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(137, 390);
            button1.Name = "button1";
            button1.Size = new Size(223, 37);
            button1.TabIndex = 3;
            button1.Text = "Обновить";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new Point(452, 52);
            labelName.Name = "labelName";
            labelName.Size = new Size(96, 15);
            labelName.TabIndex = 4;
            labelName.Text = "Наименоваение";
            // 
            // labelQuantity
            // 
            labelQuantity.AutoSize = true;
            labelQuantity.Location = new Point(452, 117);
            labelQuantity.Name = "labelQuantity";
            labelQuantity.Size = new Size(72, 15);
            labelQuantity.TabIndex = 6;
            labelQuantity.Text = "Количество";
            // 
            // textBoxQuantity
            // 
            textBoxQuantity.Location = new Point(452, 135);
            textBoxQuantity.Name = "textBoxQuantity";
            textBoxQuantity.Size = new Size(156, 23);
            textBoxQuantity.TabIndex = 5;
            // 
            // buttonChangeName
            // 
            buttonChangeName.Location = new Point(452, 253);
            buttonChangeName.Name = "buttonChangeName";
            buttonChangeName.Size = new Size(156, 23);
            buttonChangeName.TabIndex = 7;
            buttonChangeName.Text = "Переименовать";
            buttonChangeName.UseVisualStyleBackColor = true;
            buttonChangeName.Click += buttonChangeName_Click;
            // 
            // buttonAddComponent
            // 
            buttonAddComponent.Location = new Point(452, 213);
            buttonAddComponent.Name = "buttonAddComponent";
            buttonAddComponent.Size = new Size(156, 23);
            buttonAddComponent.TabIndex = 8;
            buttonAddComponent.Text = "Добавить компонент";
            buttonAddComponent.UseVisualStyleBackColor = true;
            buttonAddComponent.Click += buttonAddComponent_Click;
            // 
            // buttonDelete
            // 
            buttonDelete.Location = new Point(452, 296);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(156, 23);
            buttonDelete.TabIndex = 9;
            buttonDelete.Text = "Удалить";
            buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete.Click += buttonDelete_Click;
            // 
            // buttonReport
            // 
            buttonReport.Location = new Point(452, 339);
            buttonReport.Name = "buttonReport";
            buttonReport.Size = new Size(156, 23);
            buttonReport.TabIndex = 10;
            buttonReport.Text = "Вывести отчет";
            buttonReport.UseVisualStyleBackColor = true;
            buttonReport.Click += buttonReport_Click;
            // 
            // FormQuantity
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(676, 450);
            Controls.Add(buttonReport);
            Controls.Add(buttonDelete);
            Controls.Add(buttonAddComponent);
            Controls.Add(buttonChangeName);
            Controls.Add(labelQuantity);
            Controls.Add(textBoxQuantity);
            Controls.Add(labelName);
            Controls.Add(button1);
            Controls.Add(textBoxName);
            Controls.Add(treeView1);
            Controls.Add(buttonAddEngine);
            Name = "FormQuantity";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonAddEngine;
        private TreeView treeView1;
        private TextBox textBoxName;
        private Button button1;
        private Label labelName;
        private Label labelQuantity;
        private TextBox textBoxQuantity;
        private Button buttonChangeName;
        private Button buttonAddComponent;
        private Button buttonDelete;
        private Button buttonReport;
    }
}
