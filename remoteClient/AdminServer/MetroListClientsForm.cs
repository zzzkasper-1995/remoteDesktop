using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdminServer
{
    public partial class MetroListClientsForm : MetroForm
    {
        private bool line = true;//Поле указывает приклеена ли данная форма к главной форме

        public bool Line
        {
            get { return line; }
            set
            {
                line = value;
            }
        }

        public MetroListClientsForm()
        {
            InitializeComponent();
            
            int x = Program.MainForm.Location.X + Program.MainForm.Size.Width;
            int y = Program.MainForm.Location.Y;

            Point XY = new Point(x, y);
            Location = XY;
            StartPosition = FormStartPosition.Manual;

            metroTextBox_find.ForeColor = Color.Gray;
            metroTextBox_find.Text = "Поиск...";

            Style = Program.MainForm.Style;

            // ассоциируем контекстное меню с таблицей
            dataGridView_lid.ContextMenuStrip = contextMenuStrip_lid;
            dataGridView_find.ContextMenuStrip = contextMenuStrip_find;
            dataGridView_online.ContextMenuStrip = contextMenuStrip_online;

            //сортироватьтаблицу с поиском по клиентам онлайн
            System.ComponentModel.ListSortDirection direction;
            direction = System.ComponentModel.ListSortDirection.Descending;
            dataGridView_find.Sort(dataGridViewCheckBoxColumn2, direction);

            //при клике по ячейке выделять строку целиком
            dataGridView_find.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_online.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_lid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            metroTextBox_find.GotFocus += new EventHandler(TextGotFocus);
            metroTextBox_find.LostFocus += new EventHandler(TextLostFocus);
        }

        //-----------------------------------Поодсказка (Hint) в текстовом поле---------------------------
        private void TextGotFocus(object sender, EventArgs e)
        {
            if (metroTextBox_find.Text == "" || metroTextBox_find.Text == "Поиск...")
            {
                metroTextBox_find.Text = "";
            }

            metroTextBox_find.ForeColor = Color.Black;
        }

        private void TextLostFocus(object sender, EventArgs e)
        {
            if (metroTextBox_find.Text == "" || metroTextBox_find.Text == "Поиск...")
            {
                metroTextBox_find.Text = "Поиск...";
            }

            metroTextBox_find.ForeColor = Color.Gray;
        }
        //-----------------------------------КОНЕЦ Поодсказка (Hint) в текстовом поле---------------------------

        public void ManagerPanel()//отвечает за сворачивание и разворот блоков поиска, ПК онлайн и все ПК
        {
            ChangeHeightDataGrid();
            if (checkBox_online.Checked)
            {
                try
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        panel_find.Size = new Size(panel_find.Size.Width, metroTextBox_find.Location.Y + metroTextBox_find.Size.Height + 3);
                        if (dataGridView_online.Rows.Count != 0)
                        {
                            panel_online.Size = new Size(panel_online.Size.Width, checkBox_online.Location.Y + checkBox_online.Size.Height + dataGridView_online.Size.Height + 3);
                        }
                        else
                        {
                            panel_online.Size = new Size(panel_online.Size.Width, checkBox_online.Location.Y + checkBox_online.Size.Height + 3);
                        }
                        panel_lid.Size = new Size(panel_lid.Size.Width, checkBox_lid.Location.Y + checkBox_lid.Size.Height + 3);

                        panel_online.Location = new Point(panel_online.Location.X, panel_find.Location.Y + panel_find.Size.Height + 3);
                        panel_lid.Location = new Point(panel_lid.Location.X, panel_online.Location.Y + panel_online.Size.Height + 3);
                    }));
                }
                catch { }

                return;
            }

            if (checkBox_lid.Checked)
            {
                try
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        panel_find.Size = new Size(panel_find.Size.Width, metroTextBox_find.Location.Y + metroTextBox_find.Size.Height + 3);
                        panel_online.Size = new Size(panel_online.Size.Width, checkBox_online.Location.Y + checkBox_online.Size.Height + 3);
                        if (dataGridView_lid.Rows.Count != 0)
                        {
                            panel_lid.Size = new Size(panel_lid.Size.Width, checkBox_lid.Location.Y + checkBox_lid.Size.Height + dataGridView_lid.Size.Height + 3);
                        }
                        else
                        {
                            panel_lid.Size = new Size(panel_lid.Size.Width, checkBox_lid.Location.Y + checkBox_lid.Size.Height + 3);
                        }

                        panel_online.Location = new Point(panel_online.Location.X, panel_find.Location.Y + panel_find.Size.Height + 3);
                        panel_lid.Location = new Point(panel_lid.Location.X, panel_online.Location.Y + panel_online.Size.Height + 3);
                    }));
                }
                catch { }
                return;
            }

            if (metroTextBox_find.Text != "" || metroTextBox_find.Text != "Поиск...")
            {
                try
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        if (dataGridView_find.Rows.Count != 0)
                        {
                            panel_find.Size = new Size(panel_find.Size.Width, metroTextBox_find.Location.Y + metroTextBox_find.Size.Height + dataGridView_find.Size.Height + 3);
                        }
                        else
                        {
                            panel_find.Size = new Size(panel_find.Size.Width, metroTextBox_find.Location.Y + metroTextBox_find.Size.Height + 3);
                        }
                        panel_online.Size = new Size(panel_online.Size.Width, checkBox_online.Location.Y + checkBox_online.Size.Height + 3);
                        panel_lid.Size = new Size(panel_lid.Size.Width, checkBox_lid.Location.Y + checkBox_lid.Size.Height + 3);

                        panel_online.Location = new Point(panel_online.Location.X, panel_find.Location.Y + panel_find.Size.Height + 3);
                        panel_lid.Location = new Point(panel_lid.Location.X, panel_online.Location.Y + panel_online.Size.Height + 3);
                    }));
                }
                catch { }
            }
        }

        private void ChangeHeightDataGrid()//Меняем высоту таблиц в соответствии с содержимым
        {
            try
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    try
                    {
                        // меняем высоту таблицу по высоте всех строк
                        dataGridView_online.Height = dataGridView_online.Rows.GetRowsHeight(DataGridViewElementStates.Visible) +
                                           dataGridView_online.ColumnHeadersHeight * 2;
                        if (dataGridView_online.Height > 341) dataGridView_online.Height = 341;
                        if (dataGridView_online.Rows.Count == 0) dataGridView_online.Height = 0;
                        label_Num_online.Text = "(" + dataGridView_online.Rows.Count.ToString() + ")";

                        dataGridView_lid.Height = dataGridView_lid.Rows.GetRowsHeight(DataGridViewElementStates.Visible) +
                           dataGridView_lid.ColumnHeadersHeight * 2;
                        if (dataGridView_lid.Height > 341) dataGridView_lid.Height = 341;
                        if (dataGridView_lid.Rows.Count == 0) dataGridView_lid.Height = 0;
                        label_Num_lid.Text = "(" + dataGridView_online.Rows.Count.ToString() + "/" + dataGridView_lid.Rows.Count.ToString() + ")";

                        dataGridView_find.Height = dataGridView_find.Rows.GetRowsHeight(DataGridViewElementStates.Visible) +
                           dataGridView_find.ColumnHeadersHeight * 2;
                        if (dataGridView_find.Height > 341) dataGridView_find.Height = 341;
                        if (dataGridView_find.Rows.Count == 0) dataGridView_find.Height = 0;
                    }
                    catch { }
                }));
            }
            catch { }
        }

        public void UpdataGridView_online()//Обновление содержимого таблицы "компьютеры в сети"
        {
            // удаление всех строк
            try
            {
                if (dataGridView_online.Rows.Count != 0)
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        try
                        {
                            dataGridView_online.Rows.Clear();
                        }
                        catch { }
                    }));
                }
            }
            catch { }

            if (MetroMainForm.ListLid.Count != 0 && MetroMainForm.ListClientsID.Count != 0)
            {
                //Заполняем таблицу
                try
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        foreach (string id in MetroMainForm.ListClientsID)
                        {
                            Lid lid = MetroMainForm.ListLid.Find(x => x.ID.Equals(id));
                            if (lid != null)
                            {
                                try
                                {
                                    dataGridView_online.Rows.Add();
                                    dataGridView_online.Rows[dataGridView_online.Rows.Count - 1].Cells[0].Value = lid.ID;
                                    dataGridView_online.Rows[dataGridView_online.Rows.Count - 1].Cells[1].Value = lid.ADDRESS;
                                    dataGridView_online.Rows[dataGridView_online.Rows.Count - 1].Cells[2].Value = lid.LAST_NAME + " "
                                                                                                   + lid.NAME + " " + lid.SECOND_NAME;
                                    dataGridView_online.Rows[dataGridView_online.Rows.Count - 1].Cells[3].Value = lid.PHONE;
                                }
                                catch (Exception exc) { }
                            }
                        }
                    }));
                }
                catch (Exception exc) { Program.logger.Error("UpdataGridView_online(): " + exc); }
            }

            ManagerPanel();
        }

        public void UpdataGridView_lid_stat()//Обновление статуса онлайн/оффлайн в таблице "все"
        {
            if (MetroMainForm.ListClientsID.Count != 0)
            {
                try
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        for (int i = 0; i < dataGridView_lid.Rows.Count; i++)
                        {
                            //смотрим есть ли лид с таким ID в online
                            string id = MetroMainForm.ListClientsID.Find(x => x.Equals((string)dataGridView_lid.Rows[i].Cells[0].Value));
                            if (id != null)
                            {
                                try
                                {
                                    dataGridView_lid.Rows[i].Cells[4].Value = true;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                dataGridView_lid.Rows[i].Cells[4].Value = false;
                            }
                        }
                    }));
                }
                catch (Exception exc) { Program.logger.Error("UpdataGridView_lid_stat(): " + exc); }
            }
            else
            {
                try
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        for (int i = 0; i < dataGridView_lid.Rows.Count; i++)
                        {
                            dataGridView_lid.Rows[i].Cells[4].Value = false;
                        }
                    }));
                }
                catch { }
            }

            ManagerPanel();
        }

        public void UpdataGridView_lid()//Обновление таблицы "все"
        {
            if (dataGridView_lid.Rows.Count != MetroMainForm.ListLid.Count)
            {
                try
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        try
                        {
                            dataGridView_lid.Rows.Clear();  //Удаляем все строки
                        }
                        catch { }

                        for (int i = 0; i < MetroMainForm.ListLid.Count; i++)//Заполняем таблицу
                        {
                            dataGridView_lid.Rows.Add();

                            dataGridView_lid.Rows[i].Cells[0].Value = MetroMainForm.ListLid[i].ID;
                            dataGridView_lid.Rows[i].Cells[1].Value = MetroMainForm.ListLid[i].ADDRESS;
                            dataGridView_lid.Rows[i].Cells[2].Value = MetroMainForm.ListLid[i].LAST_NAME + " " + 
                                      MetroMainForm.ListLid[i].NAME + " " + MetroMainForm.ListLid[i].SECOND_NAME;
                            dataGridView_lid.Rows[i].Cells[3].Value = MetroMainForm.ListLid[i].PHONE;
                        }
                    }));
                }
                catch { }
            }
            ManagerPanel();
        }

        //--------------------------------События контролов------------------------------------
        private void MetroListClientsForm_LocationChanged(object sender, EventArgs e)
        {
            if (Math.Abs(Program.MainForm.Location.X + Program.MainForm.Size.Width - Location.X) < 20
                    && Math.Abs(Program.MainForm.Location.Y - Location.Y) < 20)
            {
                int x = Program.MainForm.Location.X + Program.MainForm.Size.Width;
                int y = Program.MainForm.Location.Y;

                Point XY = new Point(x, y);
                Location = XY;
                Line = true;
            }
            else
            {
                Line = false;
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dataGridView_online.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void checkBox_lid_CheckedChanged(object sender, EventArgs e)
        {
            bool c = checkBox_lid.Checked;
            if (c)
            {
                checkBox_online.Checked = false;
            }
            ManagerPanel();
        }

        private void checkBox_online_CheckedChanged(object sender, EventArgs e)
        {
            bool c = checkBox_online.Checked;
            if (c)
            {
                checkBox_lid.Checked = false;
            }
            ManagerPanel();
        }

        private void metroTextBox_find_TextChanged(object sender, EventArgs e)
        {
            checkBox_online.Checked = false;
            checkBox_lid.Checked = false;
            ManagerPanel();
        }

        private void checkBox_online_MouseMove(object sender, MouseEventArgs e)
        {
            checkBox_online.ForeColor = Color.MidnightBlue;
            label_Num_online.ForeColor = Color.MidnightBlue;
        }

        private void checkBox_online_MouseLeave(object sender, EventArgs e)
        {
            checkBox_online.ForeColor = Color.Black;
            label_Num_online.ForeColor = Color.Black;
        }

        private void checkBox_lid_MouseMove(object sender, MouseEventArgs e)
        {
            checkBox_lid.ForeColor = Color.MidnightBlue;
            label_Num_lid.ForeColor = Color.MidnightBlue;
        }

        private void checkBox_lid_MouseLeave(object sender, EventArgs e)
        {
            checkBox_lid.ForeColor = Color.Black;
            label_Num_lid.ForeColor = Color.Black;
        }

        private void metroTextBox_find_KeyUp(object sender, KeyEventArgs e)//Обновление содержимого таблицы поиска при изменении текста в поле поиска
        {
            if (e.KeyValue == 13)
            {
                checkBox_online.Checked = false;
                checkBox_lid.Checked = false;
                ManagerPanel();
            }

            try
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    try// удаление всех строк из таблицы поиска
                    {
                        dataGridView_find.Rows.Clear();  
                    }
                    catch { }

                    for (int i = 0; i < MetroMainForm.ListLid.Count; i++)
                    {
                        String s = MetroMainForm.ListLid[i].ID;
                        if (MetroMainForm.ListLid[i].ID.Contains(metroTextBox_find.Text) || MetroMainForm.ListLid[i].PHONE.Contains(metroTextBox_find.Text))
                        {
                            try
                            {
                                dataGridView_find.Rows.Add();

                                dataGridView_find.Rows[dataGridView_find.Rows.Count - 1].Cells[0].Value = MetroMainForm.ListLid[i].ID;
                                dataGridView_find.Rows[dataGridView_find.Rows.Count - 1].Cells[1].Value = MetroMainForm.ListLid[i].ADDRESS;
                                dataGridView_find.Rows[dataGridView_find.Rows.Count - 1].Cells[2].Value = MetroMainForm.ListLid[i].LAST_NAME + " " + MetroMainForm.ListLid[i].NAME + " " + MetroMainForm.ListLid[i].SECOND_NAME;
                                dataGridView_find.Rows[dataGridView_find.Rows.Count - 1].Cells[3].Value = MetroMainForm.ListLid[i].PHONE;

                                string id = MetroMainForm.ListClientsID.Find(x => x.Equals((string)dataGridView_find.Rows[dataGridView_find.Rows.Count - 1].Cells[0].Value));
                                if (id != null)
                                {
                                    dataGridView_find.Rows[dataGridView_find.Rows.Count - 1].Cells[4].Value = true;
                                }
                                else
                                {
                                    dataGridView_find.Rows[dataGridView_find.Rows.Count - 1].Cells[4].Value = false;
                                }
                            }
                            catch (Exception exc) { }
                        }
                    }

                    ManagerPanel();
                    System.ComponentModel.ListSortDirection direction;
                    direction = System.ComponentModel.ListSortDirection.Descending;
                    dataGridView_find.Sort(dataGridViewCheckBoxColumn2, direction);
                }));
            }
            catch { }
        }

        private void dataGridView_lid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)//Клик правой кнопкой мыши по таблице "Все"
        {
            if (!e.RowIndex.Equals(-1) && !e.ColumnIndex.Equals(-1) && e.Button.Equals(MouseButtons.Right))
            {
                dataGridView_lid.CurrentCell = dataGridView_lid[e.ColumnIndex, e.RowIndex];
                dataGridView_lid.CurrentRow.Selected = true;

                if (e.Button == MouseButtons.Right)
                {
                    int index = dataGridView_lid.CurrentRow.Index;
                    if (Repeater.ConnectServer)
                    {
                        if (MetroMainForm.remoteC.id == (string)dataGridView_lid.Rows[index].Cells[0].Value )
                        {
                            connectToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            connectToolStripMenuItem.Enabled = false;
                        }

                        if (Program.MainForm.IsFindID)//подключиться можно только после поиска,так проверяем был ли поиск
                        {
                            findToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            connectToolStripMenuItem.Enabled = false;
                        }

                        bool b = false;
                        foreach(string id in MetroMainForm.ListClientsID)
                        {
                            if((string)dataGridView_lid.Rows[index].Cells[0].Value==id)
                            { b = true; break; }
                        }
                        if(!b)
                        {
                            connectToolStripMenuItem.Enabled = false;
                        }
                    }
                    else
                    {
                        connectToolStripMenuItem.Enabled = false;
                        findToolStripMenuItem.Enabled = false;
                    }
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)//Контекстное меню, найти
        {
            int index = dataGridView_lid.CurrentRow.Index;
            Program.MainForm.FindID = (string)dataGridView_lid.Rows[index].Cells[0].Value;
            Program.MainForm.FindId(Program.MainForm.FindID);
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)//Контекстное меню "Подключиться"
        {
            Program.MainForm.ConnectToClient(Program.MainForm.FindID);
        }

        private void MetroListClientsForm_FormClosing(object sender, FormClosingEventArgs e)//Закрытие формы
        {
            try
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    Close();
                }));
            }
            catch { }
        }

        private void dataGridView_online_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)//Клик правой кнопкой мыши по таблице "Онлайн"
        {
            if (!e.RowIndex.Equals(-1) && !e.ColumnIndex.Equals(-1) && e.Button.Equals(MouseButtons.Right))
            {
                dataGridView_online.CurrentCell = dataGridView_online[e.ColumnIndex, e.RowIndex];
                dataGridView_online.CurrentRow.Selected = true;

                if (e.Button == MouseButtons.Right)
                {
                    int index = dataGridView_online.CurrentRow.Index;
                    if (Repeater.ConnectServer)
                    {
                        if (MetroMainForm.remoteC.id == (string)dataGridView_online.Rows[index].Cells[0].Value)
                        {
                            ConnectToolStripMenuItem_online.Enabled = true;
                        }
                        else
                        {
                            ConnectToolStripMenuItem_online.Enabled = false;
                        }

                        if (Program.MainForm.IsFindID)//подключиться можно только после поиска,так проверяем был ли поиск
                        {
                            FindToolStripMenuItem_online.Enabled = true;
                        }
                        else
                        {
                            ConnectToolStripMenuItem_online.Enabled = false;
                        }

                        bool b = false;
                        foreach (string id in MetroMainForm.ListClientsID)
                        {
                            if ((string)dataGridView_online.Rows[index].Cells[0].Value == id)
                            { b = true; break; }
                        }
                        if (!b)
                        {
                            ConnectToolStripMenuItem_online.Enabled = false;
                        }
                    }
                    else
                    {
                        ConnectToolStripMenuItem_online.Enabled = false;
                        FindToolStripMenuItem_online.Enabled = false;
                    }
                }
            }
        }

        private void dataGridView_find_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)//Клик правой кнопкой мыши по таблице посика
        {
            if (!e.RowIndex.Equals(-1) && !e.ColumnIndex.Equals(-1) && e.Button.Equals(MouseButtons.Right))
            {
                dataGridView_find.CurrentCell = dataGridView_find[e.ColumnIndex, e.RowIndex];
                dataGridView_find.CurrentRow.Selected = true;

                if (e.Button == MouseButtons.Right)
                {
                    int index = dataGridView_find.CurrentRow.Index;
                    if (Repeater.ConnectServer)
                    {
                        if (MetroMainForm.remoteC.id == (string)dataGridView_find.Rows[index].Cells[0].Value)
                        {
                            ConnectToolStripMenuItem_find.Enabled = true;
                        }
                        else
                        {
                            ConnectToolStripMenuItem_find.Enabled = false;
                        }

                        if (Program.MainForm.IsFindID)//подключиться можно только после поиска,так проверяем был ли поиск
                        {
                            FindToolStripMenuItem_find.Enabled = true;
                        }
                        else
                        {
                            ConnectToolStripMenuItem_find.Enabled = false;
                        }

                        bool b = false;
                        foreach (string id in MetroMainForm.ListClientsID)
                        {
                            if ((string)dataGridView_find.Rows[index].Cells[0].Value == id)
                            { b = true; break; }
                        }
                        if (!b)
                        {
                            ConnectToolStripMenuItem_find.Enabled = false;
                        }
                    }
                    else
                    {
                        ConnectToolStripMenuItem_find.Enabled = false;
                        FindToolStripMenuItem_find.Enabled = false;
                    }
                }
            }
        }

        private void FindToolStripMenuItem_online_Click(object sender, EventArgs e)
        {
            int index = dataGridView_online.CurrentRow.Index;
            Program.MainForm.FindID = (string)dataGridView_online.Rows[index].Cells[0].Value;
            Program.MainForm.FindId(Program.MainForm.FindID);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            int index = dataGridView_find.CurrentRow.Index;
            Program.MainForm.FindID = (string)dataGridView_find.Rows[index].Cells[0].Value;
            Program.MainForm.FindId(Program.MainForm.FindID);
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            Program.MainForm.ConnectToClient(Program.MainForm.FindID);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Program.MainForm.ConnectToClient(Program.MainForm.FindID);
        }

        private void MetroListClientsForm_Load(object sender, EventArgs e)
        {
            MetroMainForm.GetListLid();
            MetroMainForm.GetListClients();
            UpdataGridView_lid();
            UpdataGridView_lid_stat();
            UpdataGridView_online();
        }
        //--------------------------------КОНЕЦ События контролов------------------------------------
    }
}
