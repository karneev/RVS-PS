using System;
using System.IO;
using System.Windows.Forms;
using Agent.Model;
using Agent.Enums;

namespace Agent.View
{
    public partial class CalculateForm : Form
    {
        bool started=false;
        AgentSystem agent;
        internal CalculateForm(ref AgentSystem agent)
        {
            InitializeComponent();
            this.agent = agent;
            this.agent.RefreshView += refreshForm;
        }
        delegate void refresh();
        private void showAllDataFile() // отобразить все файлы данных в списке файлов
        {
            if (this.InvokeRequired)
            {
                refresh d = new refresh(showAllDataFile);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.dataDiffFileList.Items.Clear(); // очищаем старый список
                this.dataNotDiffFileList.Items.Clear(); // очищаем старый список
                foreach (FileInfo i in agent.GetAllDiffDataFile()) // выводим всё, что есть в агенте
                {
                    this.dataDiffFileList.Items.Add(i.Name);
                }
                foreach (FileInfo i in agent.GetAllNotDiffDataFile()) // выводим всё, что есть в агенте
                {
                    this.dataNotDiffFileList.Items.Add(i.Name);
                }
                this.dataDiffFileList.Items.Add("Добавить файл"); // добавляем опцию "Добавить файл"
                this.dataNotDiffFileList.Items.Add("Добавить файл"); // добавляем опцию "Добавить файл"
            }
        }
        private void refreshMachineSelectPanel() // обновить панель выбора машин
        {
            if (this.InvokeRequired)
            {
                refresh d = new refresh(refreshMachineSelectPanel);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.acceptButton.Enabled = false;
                this.unchekedAllButton.Enabled = false;

                this.checkedMechineListBox.Items.Clear();           // очищаем список машин
                foreach (Contractor i in agent.GetAllContractor())   // заполняем список машин
                {
                    if (i.Selected == false)
                        this.checkedMechineListBox.Items.Add(i.GetIPServer().ToString());
                }
                if (agent.GetAllContractor().Count != 0)
                {
                    this.acceptButton.Enabled = true;
                    this.unchekedAllButton.Enabled = true;
                }
                if(agent.refreshContractor==false)
                {
                    this.machineInfoPanel.Enabled = true;
                    this.filePanel.Enabled = true;
                    this.refreshButton.Enabled = true;
                    this.refreshButton.Text = "Обновить список";
                }
                else
                {
                    this.machineInfoPanel.Enabled = false;
                    this.filePanel.Enabled = false;
                    this.refreshButton.Enabled = false;
                    this.refreshButton.Text = "Идет обновление списка";
                }
            }
        }
        private void refreshMachineInfoPanel() // обновить панель выбранных машинах
        {
            if (this.InvokeRequired)
            {
                refresh d = new refresh(refreshMachineInfoPanel);
                this.Invoke(d, new object[] { });
            }
            else
            {
                int sumRam = 0;
                int sumCpu = 0;
                this.selectedMachineListBox.Items.Clear();       // очищаем список выбранных машин
                foreach (Contractor i in agent.GetAllContractor()) // заполняем список выбранных машин
                {
                    if (i.Selected == true)
                    {
                        this.selectedMachineListBox.Items.Add(i.GetIPServer().ToString());
                        sumRam += i.Info.vRam;
                        sumCpu += i.Info.vCPU;
                    }
                }
                ramTextBox.Text = sumRam.ToString();
                cpuTextBox.Text = sumCpu.ToString();
                this.unselectAllButton.Enabled = this.selectedMachineListBox.Items.Count == 0 ? false : true;
            }
        }
        private void refreshFilePanel() // Обновить панель выбранных файлов
        {
            if (this.InvokeRequired)
            {
                refresh d = new refresh(refreshFilePanel);
                this.Invoke(d, new object[] { });
            }
            else
            {
                bool readyStart = true;
                if (agent.ExeFile != null) // если .exe файл установлен
                    fileNameLabel.Text = agent.ExeFile.Name;
                else
                {
                    fileNameLabel.Text = "не выбран";
                    readyStart = false;
                }
                showAllDataFile();
                readyStart = readyStart && (this.selectedMachineListBox.Items.Count != 0) && !agent.IsCalculate;
                startCalculateButton.Enabled = readyStart && !started;
                if(started==true && agent.IsCalculate==false)
                {
                    startCalculateButton.Text = "Вычисления завершены";
                }
            }
        }

        private void refreshForm()
        {
            if (this.InvokeRequired)
            {
                refresh d = new refresh(refreshForm);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.refreshMachineSelectPanel();
                this.refreshMachineInfoPanel();
                this.refreshFilePanel();
            }
        }

        private void CalculateForm_FormClosed(object sender, FormClosedEventArgs e) // закрытие формы рассчетов (не закончено)
        {
            agent.Status = StatusMachine.Free; // меняем статус на "свободен"
            if(started == true)
                Programm.Reset();
        }

        private void selectRunFileButton_Click(object sender, EventArgs e) // установка файла exe
        {
            this.openFileDialog.Filter = "exe files (*.exe)|*.exe"; 
            if(this.openFileDialog.ShowDialog()==DialogResult.OK)
            {
                FileInfo exe = new FileInfo(openFileDialog.FileName);
                if (exe.Exists)
                {
                    agent.ExeFile = exe;                    // установка файла в агент
                    fileNameLabel.Text = agent.ExeFile.Name;// отображение имени файла на форме
                }
                else
                {
                    MessageBox.Show(this, "Файла не существует.", "Ошибка.", MessageBoxButtons.OK);
                }
            }
        }

        private void startCalculateButton_Click(object sender, EventArgs e) // запуск вычислений
        {
            try
            {
                if(agent.IsCalculate==false) // если они не запущены ранее
                {
                    this.selectRunFileButton.Enabled = false;
                    this.dataDiffFileList.Enabled = false;
                    this.startCalculateButton.Enabled = false;
                    this.exitButton.Enabled = false;
                    this.machineInfoPanel.Enabled = false;
                    this.machineSelectPanel.Enabled = false;
                    this.startCalculateButton.Text = "Идут вычисления";
                    this.started = true;
                    agent.StartCalculate(notDeleteFilesCheckBox.CheckState.Equals(CheckState.Checked));
                    this.exitButton.Enabled = true;
                    this.startCalculateButton.Text = "Вычисления завершаются";
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void acceptButton_Click(object sender, EventArgs e) // выбрать выделенные машины для вычислений
        {
            CheckedListBox.CheckedIndexCollection col = checkedMechineListBox.CheckedIndices;
            int[] Arr = new int[col.Count];
            for (int i = 0; i < col.Count; i++)
                Arr[i] = col[i];
            for (int i = Arr.Length - 1; i >= 0; i--)
                agent.SelectContractor(Arr[i]);
            this.refreshForm();
        }

        private void unchekedAllButton_Click(object sender, EventArgs e) // снять всё выделение
        {
            CheckedListBox.CheckedIndexCollection coll= checkedMechineListBox.CheckedIndices;
            foreach (int t in coll)
                checkedMechineListBox.SetItemChecked(t, false);
        }

        private void refreshButton_Click(object sender, EventArgs e) // обновить список машин
        {
            agent.RefreshContractorList();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataDiffFileList_MouseDoubleClick(object sender, MouseEventArgs e) // Пытаемся добавить или заменить файл данных
        {
            this.openFileDialog.Filter = "txt files (*.txt)|*.txt";
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo txt = new FileInfo(openFileDialog.FileName);
                if (txt.Exists)
                {
                    if (this.dataDiffFileList.Text.CompareTo("Добавить файл") == 0)
                        agent.AddDiffDataFile(txt);             // добавляем файл
                    else
                    {
                        string old = this.dataDiffFileList.Text;
                        agent.ReplaceDiffDataFile(old, txt);   // заменяем файл
                    }
                }
                else
                {
                    MessageBox.Show(this, "Файла не существует.", "Ошибка.", MessageBoxButtons.OK);
                }
            }
        }

        private void dataDiffFileList_KeyDown(object sender, KeyEventArgs e) // Пытаемся удалить файл данных
        {
            if(e.KeyCode==Keys.Delete && this.dataDiffFileList.Text.CompareTo("Добавить файл") != 0) // нажали на Delete, но при этом выбрал не "Добавить файл"
            {
                agent.RemoveDiffDataFile(this.dataDiffFileList.Text);   // удаляем файл
            }
        }
        private void dataNotDiffFileList_MouseDoubleClick(object sender, MouseEventArgs e) // Пытаемся добавить или заменить файл данных
        {
            this.openFileDialog.Filter = "txt files (*.txt)|*.txt";
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo txt = new FileInfo(openFileDialog.FileName);
                if (txt.Exists)
                {
                    if (this.dataDiffFileList.Text.CompareTo("Добавить файл") == 0)
                        agent.AddNotDiffDataFile(txt);             // добавляем файл
                    else
                    {
                        string old = this.dataDiffFileList.Text;
                        agent.ReplaceNotDiffDataFile(old, txt);   // заменяем файл
                    }
                }
                else
                {
                    MessageBox.Show(this, "Файла не существует.", "Ошибка.", MessageBoxButtons.OK);
                }
            }
        }

        private void dataNotDiffFileList_KeyDown(object sender, KeyEventArgs e) // Пытаемся удалить файл данных
        {
            if (e.KeyCode == Keys.Delete && this.dataDiffFileList.Text.CompareTo("Добавить файл") != 0) // нажали на Delete, но при этом выбрал не "Добавить файл"
            {
                agent.RemoveNotDiffDataFile(this.dataDiffFileList.Text);   // удаляем файл
            }
        }

        private void unselectButton_Click(object sender, EventArgs e) // отменить выбор отмеченных машин
        {
            CheckedListBox.CheckedIndexCollection col = selectedMachineListBox.CheckedIndices;
            for (int i = 0; i < col.Count; i++)
                agent.UnSelectContractor(col[i]);
            this.refreshForm();
        }

        private void unselectAllButton_Click(object sender, EventArgs e) // отменить выбор всех машин
        {
            agent.UnSelectAll();
            this.refreshForm();
        }

        private void CalculateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (agent.IsCalculate == true)
            {
                var result = MessageBox.Show("Точно выйти?\n Вычисления еще не закончены!", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void findIP_Click(object sender, EventArgs e)
        {
            new FindIPForm(agent).ShowDialog();
            this.refreshMachineInfoPanel();
        }

        private void checkedMechineListBox_MouseClick(object sender, MouseEventArgs e)
        {
            CheckedListBox clb = (CheckedListBox)sender;
            if(clb.SelectedIndex!=-1)
                checkedMechineListBox.SetItemChecked(clb.SelectedIndex, !clb.GetItemChecked(clb.SelectedIndex));
        }
    }
}
