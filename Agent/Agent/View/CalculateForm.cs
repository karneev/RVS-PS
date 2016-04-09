using System;
using System.IO;
using System.Windows.Forms;
using Agent.Model;

namespace Agent.View
{
    public partial class CalculateForm : Form
    {
        bool started=false;
        AgentSystem agent;
        public CalculateForm(ref AgentSystem agent)
        {
            InitializeComponent();
            this.agent = agent;
            this.agent.refreshView += refreshForm;
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
                this.dataFileList.Items.Clear(); // очищаем старый список
                foreach (FileInfo i in agent.GetAllDataFile()) // выводим всё, что есть в агенте
                {
                    this.dataFileList.Items.Add(i.Name);
                }
                this.dataFileList.Items.Add("Добавить файл"); // добавляем опцию "Добавить файл"
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
                        this.checkedMechineListBox.Items.Add(i.GetIpServer().ToString());
                }
                if (agent.GetAllContractor().Count != 0)
                {
                    this.acceptButton.Enabled = true;
                    this.unchekedAllButton.Enabled = true;
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
                        this.selectedMachineListBox.Items.Add(i.GetIpServer().ToString());
                        sumRam += i.Info.vRam;
                        sumCpu += i.Info.vCPU;
                    }
                }
                ramTextBox.Text = sumRam.ToString();
                cpuTextBox.Text = sumRam.ToString();
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
                    this.dataFileList.Enabled = false;
                    this.startCalculateButton.Enabled = false;
                    this.exitButton.Enabled = false;
                    this.machineInfoPanel.Enabled = false;
                    this.machineSelectPanel.Enabled = false;
                    this.startCalculateButton.Text = "Идут вычисления";
                    this.started = true;
                    agent.StartCalculate(startInInitiator.Checked);
                    this.exitButton.Enabled = true;
                    this.startCalculateButton.Text = "Вычисления завершаются";
                }
            }
            catch (Exception ex)
            {
                //Programm.ShowMessage(ex.Message + " in startCalculateButton_Click");
            }
        }

        private void controlButton_Click(object sender, EventArgs e) // НЕ РАБОТАЕТ
        {

        }

        private void acceptButton_Click(object sender, EventArgs e) // выбрать выделенные машины для вычислений
        {
            CheckedListBox.CheckedIndexCollection col = checkedMechineListBox.CheckedIndices;
            int[] Arr = new int[col.Count];
            for (int i = 0; i < col.Count; i++)
                Arr[i] = col[i];
            for (int i = Arr.Length - 1; i >= 0; i--)
                agent.SelectContractor(Arr[i]);
        }

        private void unchekedAllButton_Click(object sender, EventArgs e) // снять всё выделение
        {
            foreach (int t in checkedMechineListBox.CheckedIndices)
                checkedMechineListBox.SetItemChecked(t, false);
        }

        private void refreshButton_Click(object sender, EventArgs e) // обновить список машин
        {
            this.machineInfoPanel.Enabled = false;
            this.filePanel.Enabled = false;
            this.refreshButton.Enabled = false;
            this.refreshButton.Text = "Идет обновление списка";
            agent.RefreshContractorList();
            this.machineInfoPanel.Enabled = true;
            this.filePanel.Enabled = true;
            this.refreshButton.Enabled = true;
            this.refreshButton.Text = "Обновить список";
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataFileList_MouseDoubleClick(object sender, MouseEventArgs e) // Пытаемся добавить или заменить файл данных
        {
            this.openFileDialog.Filter = "txt files (*.txt)|*.txt";
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo txt = new FileInfo(openFileDialog.FileName);
                if (txt.Exists)
                {
                    if (this.dataFileList.Text.CompareTo("Добавить файл") == 0)
                        agent.AddDataFile(txt);             // добавляем файл
                    else
                    {
                        string old = this.dataFileList.Text;
                        agent.ReplaseDataFile(old, txt);   // заменяем файл
                    }
                }
                else
                {
                    MessageBox.Show(this, "Файла не существует.", "Ошибка.", MessageBoxButtons.OK);
                }
            }
        }

        private void dataFileList_KeyDown(object sender, KeyEventArgs e) // Пытаемся удалить файл данных
        {
            if(e.KeyCode==Keys.Delete && this.dataFileList.Text.CompareTo("Добавить файл") != 0) // нажали на Delete, но при этом выбрал не "Добавить файл"
            {
                agent.RemoveDataFile(this.dataFileList.Text);   // удаляем файл
            }
        }

        private void unselectButton_Click(object sender, EventArgs e) // отменить выбор отмеченных машин
        {
            CheckedListBox.CheckedIndexCollection col = checkedMechineListBox.CheckedIndices;
            for (int i = 0; i < col.Count; i++)
                agent.UnSelectContractor(col[i]);
        }

        private void unselectAllButton_Click(object sender, EventArgs e) // отменить выбор всех машин
        {
            agent.UnSelectAll();
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
    }
}
