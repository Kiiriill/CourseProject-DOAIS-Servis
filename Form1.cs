using Npgsql;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace ИДЗБД
{


    public partial class Form1 : Form
    {
        string sqlconnect = "Server=localhost;Port=5432;Database=servis; User Id =postgres; Password=RBHBkk2002;";
        private Dictionary<string, string> columnNamesMapping = new Dictionary<string, string>
        {
            { "order_id", "Номер заказа" },
            { "staff_id", "Номер сотрудника" },
            { "client_id", "Номер клиента" },
            { "technic_id", "Номер техники" },
            { "order_datecreation", "Дата создания заказа" },
            { "order_status", "Статус выполнения заказа" },
            { "order_mileage", "Пробег" },
            { "order_cost", "Стоимоть" },
            { "order_datecompletion", "Дата завершения" },
            { "order_description", "Описание заказа" },
            { "complworks_datetime", "Дата создания заказа" },
            { "complworks_duration", "Дата завершения заказа " },
            { "complworks_jobcontent", "Проделанные работы " },
            { "client_name","Имя" },
            { "client_surname","Фамилия" },
            { "client_patronymic","Отчество" },
            { "client_phonenumber","Номер" },
            { "client_adress","Адрес" },
            { "supplier_adress","Адрес поставщика" },
            { "supply_id","Номер поставки" },
            { "repairparts_id","Номер запчасти" },
            { "supplier_company","Компания поставщика" },
            { "supply_date","Дата" },
            { "supply_quantity","Количество" },
            { "supply_cost","Стоимость" },
            { "repairparts_title","Название" },
            { "repairparts_marks","Марка" },
            { "repairparts_model","Модель" },
            { "repairparts_quantity","Количество" },
            { "staff_name","Имя" },
            { "staff_surname","Фамилия" },
            { "staff_patronymic","Отчество" },
            { "staff_phonenumber","Номер телефона" },
            { "staff_adress","Адрес" },
            { "staff_post","Должность" },
            { "supplier_phonenumber","Телефон" },
            { "technic_mark","Марка" },
            { "technic_model","Модель" },
            { "technic_serialnumber","Серийный номер" },
            { "technic_dateproduction","Дата производства" },
            { "technic_condition","Состояние" },
            { "technic_self","Собственность" },
            { "technic_mileage","Пробег" }
        };

        public Form1()
        {
            InitializeComponent();

            LoadDataForOrders();
        }


        #region --- Переключение таблиц по меню ---

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;

            string selected = listBox1.SelectedItem.ToString();
            switch (selected)
            {
                case "Заказы":
                    LoadDataForOrders();
                    break;
                case "Выполненные работы":
                    LoadDataForCompletedWorks();
                    break;
                case "Техника":
                    LoadDataForTechnic();
                    break;
                case "Запчасти":
                    LoadDataForRepairParts();
                    break;
                case "Клиенты":
                    LoadDataForClients();
                    break;
                case "Сотрудники":
                    LoadDataForStaff();
                    break;
                case "Поставщики":
                    LoadDataForSuppliers();
                    break;
                case "История поставок":
                    LoadDataForSupplyHistory();
                    break;
            }
        }

        #endregion

        #region --- Загрузка данных для каждой таблицы ---

        public void LoadDataForOrders()
        {
            string query = @"
        SELECT 
            o.order_id AS ""Номер заказа"",
            o.staff_id AS ""staff_id"",          -- скрытая колонка
            o.client_id AS ""client_id"",        -- скрытая колонка
            o.technic_id AS ""technic_id"",      -- скрытая колонка
            s.staff_surname || ' ' || s.staff_name || ' ' || COALESCE(s.staff_patronymic, '') AS ""ФИО сотрудника"",
            c.client_surname || ' ' || c.client_name || ' ' || COALESCE(c.client_patronymic, '') AS ""ФИО клиента"",
            t.technic_mark || ' ' || t.technic_model AS ""Марка и модель техники"",
            o.order_datecreation AS ""Дата создания заказа"",
            CASE o.order_status
                WHEN 0 THEN 'В работе'
                WHEN 1 THEN 'Выполнен'
                WHEN 2 THEN 'Отменён'
                ELSE 'Неизвестно'
            END AS ""Статус заказа"",
            o.order_datecompletion AS ""Дата завершения заказа"",
            o.order_description AS ""Описание заказа"",
            o.order_cost AS ""Стоимость ремонта"",
            o.order_mileage AS ""Пробег техники""
        FROM ""Order"" o
        LEFT JOIN staff s ON o.staff_id = s.staff_id
        LEFT JOIN client c ON o.client_id = c.client_id
        LEFT JOIN technic t ON o.technic_id = t.technic_id
        ORDER BY o.order_id ASC";

            LoadData(query);

            // Скрываем технические колонки в DataGridView
            if (dataGridView1.Columns.Contains("staff_id"))
                dataGridView1.Columns["staff_id"].Visible = false;
            if (dataGridView1.Columns.Contains("client_id"))
                dataGridView1.Columns["client_id"].Visible = false;
            if (dataGridView1.Columns.Contains("technic_id"))
                dataGridView1.Columns["technic_id"].Visible = false;
        }

        public void LoadDataForCompletedWorks()
        {
            string query = @"
                SELECT 
                    cw.order_id AS ""Номер заказа"",
                    cw.staff_id AS ""staff_id"", 
                    s.staff_surname || ' ' || s.staff_name || ' ' || COALESCE(s.staff_patronymic, '') AS ""ФИО сотрудника"",
                    cw.complworks_datetime AS ""Дата начала работы"",
                    cw.complworks_jobcontent AS ""Проведённые работы"",
                    cw.complworks_duration AS ""Дата завершения работы""
                FROM completed_works cw
                LEFT JOIN staff s ON cw.staff_id = s.staff_id
                ORDER BY cw.order_id, cw.complworks_datetime";

            LoadData(query);

        }

        public void LoadDataForTechnic()
        {
            string query = @"
                SELECT 
                    technic_id AS ""technic_id"",
                    technic_mark AS ""Марка"",
                    technic_model AS ""Модель"",
                    technic_serialnumber AS ""Серийный номер"",
                    technic_dateproduction AS ""Год выпуска"",
                    technic_condition AS ""Состояние"",
                    CASE technic_self
                        WHEN 0 THEN 'Чужая'
                        WHEN 1 THEN 'Своя'
                        ELSE 'Неизвестно'
                    END AS ""Собственная техника"",
                    technic_mileage AS ""Пробег""
                FROM technic
                ORDER BY technic_id";

            LoadData(query);

            if (dataGridView1.Columns.Contains("technic_id"))
            {
                dataGridView1.Columns["technic_id"].Visible = false;
            }
        }

        public void LoadDataForRepairParts()
        {
            string query = @"
                SELECT 
                    repairparts_id AS ""repairparts_id"",
                    repairparts_title AS ""Запчасть"",
                    repairparts_marks AS ""Марка"",
                    repairparts_model AS ""Модель"",
                    repairparts_quantity AS ""Количество""
                FROM repairparts
                ORDER BY repairparts_id";

            LoadData(query);

            if (dataGridView1.Columns.Contains("repairparts_id"))
            {
                dataGridView1.Columns["repairparts_id"].Visible = false;
            }
        }

        public void LoadDataForClients()
        {
            string query = @"
                SELECT 
                    client_id AS ""client_id"",
                    client_name AS ""Имя"",
                    client_surname AS ""Фамилия"",
                    client_patronymic AS ""Отчество"",
                    client_phonenumber AS ""Номер телефона"",
                    client_adress AS ""Адрес проживания""
                FROM client
                ORDER BY client_id";

            LoadData(query);

            if (dataGridView1.Columns.Contains("client_id"))
            {
                dataGridView1.Columns["client_id"].Visible = false;
            }
        }

        public void LoadDataForStaff()
        {
            string query = @"
                SELECT 
                    staff_id AS ""staff_id"",
                    staff_name AS ""Имя"",
                    staff_surname AS ""Фамилия"",
                    staff_patronymic AS ""Отчество"",
                    staff_post AS ""Должность"",
                    staff_phonenumber AS ""Номер телефона"",
                    staff_adress AS ""Адрес проживания""
                FROM staff
                ORDER BY staff_id";

            LoadData(query);

            if (dataGridView1.Columns.Contains("staff_id"))
            {
                dataGridView1.Columns["staff_id"].Visible = false;
            }
        }

        public void LoadDataForSuppliers()
        {
            string query = @"SELECT supplier_company AS ""Компания"", supplier_adress AS ""Адрес"", supplier_phonenumber AS ""Номер"" FROM supplier";
            LoadData(query);
        }

        private void LoadDataForSupplyHistory()
        {
            string query = @"
                    SELECT
                        hs.supply_id AS ""Номер поставки"",
                        hs.repairparts_id AS ""repairparts_id"",  
                        rp.repairparts_title || ' (' || rp.repairparts_marks || ' ' || rp.repairparts_model || ')' AS ""Запчасть"",
                        hs.supplier_company AS ""Поставщик"",
                        hs.supply_date AS ""Дата поставки"",
                        hs.supply_quantity AS ""Количество"",
                        hs.supply_cost AS ""Стоимость""
                    FROM histotysupply hs
                    LEFT JOIN repairparts rp ON hs.repairparts_id = rp.repairparts_id
                    ORDER BY hs.supply_date DESC, hs.supply_id DESC";

            LoadData(query);

            // Скрываем техническую колонку, чтобы пользователь её не видел
            if (dataGridView1.Columns.Contains("repairparts_id"))
            {
                dataGridView1.Columns["repairparts_id"].Visible = false;
            }
        }

        private void LoadData(string query)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(sqlconnect))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        #endregion

        #region --- Фильтры и поиск ---

        private void ApplyFilters()
        {
            if (!(dataGridView1.DataSource is DataTable dt)) return;

            DataView dv = dt.DefaultView;
            System.Collections.Generic.List<string> filters = new System.Collections.Generic.List<string>();

            // Фильтр по статусу
            if (comboBoxStatus.SelectedItem != null && comboBoxStatus.SelectedItem.ToString() != "Все")
            {
                string status = comboBoxStatus.SelectedItem.ToString().Replace("'", "''");
                filters.Add($"[Статус заказа] = '{status}'");
            }

            // Фильтр по дате создания
            if (checkBoxDateFilter.Checked && dt.Columns.Contains("Дата создания заказа"))
            {
                string from = dateTimePickerFrom.Value.ToString("yyyy-MM-dd");
                string to = dateTimePickerTo.Value.ToString("yyyy-MM-dd");
                filters.Add($"[Дата создания заказа] >= #{from}# AND [Дата создания заказа] <= #{to}#");
            }

            dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";
        }

        private void ClearFilters()
        {
            textBoxSearch.Clear();
            comboBoxStatus.SelectedItem = "Все";
            checkBoxDateFilter.Checked = false;
            if (dataGridView1.DataSource is DataTable dt)
                dt.DefaultView.RowFilter = "";
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource is not DataTable dt) return;

            string term = textBoxSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
            {
                ClearFilters();
                return;
            }

            term = term.Replace("'", "''");
            System.Collections.Generic.List<string> conditions = new System.Collections.Generic.List<string>();

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (!col.Visible) continue;
                string colName = col.HeaderText;

                if (!dt.Columns.Contains(colName)) continue;

                var colType = dt.Columns[colName].DataType;

                if (colType == typeof(string))
                {
                    conditions.Add($"[{colName}] LIKE '%{term}%'");
                }
                else if (colType == typeof(int) || colType == typeof(long) || colType == typeof(decimal))
                {
                    if (int.TryParse(term, out _) || decimal.TryParse(term, out _))
                        conditions.Add($"[{colName}] = {term}");
                }
                else if (colType == typeof(DateTime))
                {
                    if (DateTime.TryParse(term, out DateTime date))
                        conditions.Add($"[{colName}] = #{date:yyyy-MM-dd}#");
                }
            }

            if (conditions.Count > 0)
                dt.DefaultView.RowFilter = string.Join(" OR ", conditions);
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите таблицу в меню слева.", "Выбор таблицы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string selectedTable = listBox1.SelectedItem.ToString();

            Form formToOpen = null;

            switch (selectedTable)
            {
                case "Заказы":
                    formToOpen = new FormAdd();  // Твоя существующая форма для заказов
                    break;

                case "Выполненные работы":
                    formToOpen = new FormAddCompletedWork();
                    break;

                case "Техника":
                    formToOpen = new FormAddTechnic();  // Потом создашь
                    break;

                case "Запчасти":
                    formToOpen = new FormAddRepairPart();
                    break;

                case "Клиенты":
                    formToOpen = new FormAddClient();
                    break;

                case "Сотрудники":
                    formToOpen = new FormAddStaff();
                    break;

                case "Поставщики":
                    formToOpen = new FormAddSupplier();
                    break;

                case "История поставок":
                    formToOpen = new FormAddSupply();
                    break;

                default:
                    MessageBox.Show("Добавление записи для этой таблицы ещё не реализовано.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
            }

            if (formToOpen != null)
            {
                formToOpen.ShowDialog();  // Открываем модально

                // После закрытия формы обновляем текущую таблицу
                RefreshCurrentTable();
            }
        }

        // Вспомогательный метод — обновляет данные в зависимости от выбранной таблицы
        private void RefreshCurrentTable()
        {
            if (listBox1.SelectedItem == null) return;

            string selected = listBox1.SelectedItem.ToString();

            switch (selected)
            {
                case "Заказы":
                    LoadDataForOrders();
                    break;
                case "Выполненные работы":
                    LoadDataForCompletedWorks();
                    break;
                case "Техника":
                    LoadDataForTechnic();
                    break;
                case "Запчасти":
                    LoadDataForRepairParts();
                    break;
                case "Клиенты":
                    LoadDataForClients();
                    break;
                case "Сотрудники":
                    LoadDataForStaff();
                    break;
                case "Поставщики":
                    LoadDataForSuppliers();
                    break;
                case "История поставок":
                    LoadDataForSupplyHistory();
                    break;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу!");
                return;
            }

            string selectedTable = listBox1.SelectedItem.ToString();
            DataGridViewRow row = dataGridView1.SelectedRows[0];

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись?\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            bool success = false;

            switch (selectedTable)
            {
                case "Заказы":
                    success = DeleteOrder(row);
                    break;
                case "Выполненные работы":
                    success = DeleteCompletedWork(row);
                    break;
                case "Техника":
                    success = DeleteTechnic(row);
                    break;
                case "Клиенты":
                    success = DeleteClient(row);
                    break;
                case "Сотрудники":
                    success = DeleteStaff(row);
                    break;
                case "Запчасти":
                    success = DeleteRepairPart(row);
                    break;
                case "Поставщики":
                    success = DeleteSupplier(row);
                    break;
                case "История поставок":
                    success = DeleteSupply(row);
                    break;
                default:
                    MessageBox.Show("Удаление для этой таблицы не реализовано.");
                    return;
            }

            if (success)
            {
                MessageBox.Show("Запись успешно удалена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshCurrentTable();
            }
        }

        private bool DeleteOrder(DataGridViewRow row)
        {
            int orderId = Convert.ToInt32(row.Cells["Номер заказа"].Value);
            return ExecuteDelete("DELETE FROM \"Order\" WHERE order_id = @id", "@id", orderId);
        }

        private bool DeleteCompletedWork(DataGridViewRow row)
        {
            int orderId = Convert.ToInt32(row.Cells["Номер заказа"].Value);
            int staffId = Convert.ToInt32(row.Cells["staff_id"]?.Value ?? 0); // Если есть скрытая колонка
            DateTime startDate = Convert.ToDateTime(row.Cells["Дата начала работы"].Value);

            using (NpgsqlConnection conn = new NpgsqlConnection(sqlconnect))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                DELETE FROM completed_works 
                WHERE order_id = @order_id 
                  AND staff_id = @staff_id 
                  AND complworks_datetime = @start";

                    cmd.Parameters.AddWithValue("@order_id", orderId);
                    cmd.Parameters.AddWithValue("@staff_id", staffId);
                    cmd.Parameters.AddWithValue("@start", startDate);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private bool DeleteTechnic(DataGridViewRow row)
        {
            int id = Convert.ToInt32(row.Cells["technic_id"].Value);
            return ExecuteDelete("DELETE FROM technic WHERE technic_id = @id", "@id", id);
        }

        private bool DeleteClient(DataGridViewRow row)
        {
            int id = Convert.ToInt32(row.Cells["client_id"].Value);
            return ExecuteDelete("DELETE FROM client WHERE client_id = @id", "@id", id);
        }

        private bool DeleteStaff(DataGridViewRow row)
        {
            int id = Convert.ToInt32(row.Cells["staff_id"].Value);
            return ExecuteDelete("DELETE FROM staff WHERE staff_id = @id", "@id", id);
        }

        private bool DeleteRepairPart(DataGridViewRow row)
        {
            int id = Convert.ToInt32(row.Cells["repairparts_id"].Value);
            return ExecuteDelete("DELETE FROM repairparts WHERE repairparts_id = @id", "@id", id);
        }

        private bool DeleteSupplier(DataGridViewRow row)
        {
            string company = row.Cells["Компания"].Value.ToString();
            return ExecuteDelete(
                    "DELETE FROM supplier WHERE supplier_company = @company",
                    "@company",
                    company);
        }

        private bool DeleteSupply(DataGridViewRow row)
        {
            int id = Convert.ToInt32(row.Cells["Номер поставки"].Value);
            return ExecuteDelete("DELETE FROM histotysupply WHERE supply_id = @id", "@id", id);
        }

        // Универсальный метод удаления
        private bool ExecuteDelete(string query, string parameterName, object parameterValue)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(sqlconnect))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(parameterName, parameterValue);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503") // Нарушение внешнего ключа
            {
                MessageBox.Show("Нельзя удалить запись — на неё ссылаются другие данные (например, история поставок).",
                                "Ошибка целостности", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || listBox1.SelectedItem.ToString() != "Заказы")
            {
                MessageBox.Show("Для генерации отчёта выберите таблицу \"Заказы\" и строку с заказом.", "Отчёт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заказ для генерации отчёта!", "Отчёт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int orderId = Convert.ToInt32(row.Cells["Номер заказа"].Value);

            // Генерируем отчёт
            ReportGenerator report = new ReportGenerator(sqlconnect);
            report.GenerateReportForOrder(orderId);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для редактирования!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу!");
                return;
            }

            string selectedTable = listBox1.SelectedItem.ToString();
            DataGridViewRow row = dataGridView1.SelectedRows[0];

            Form formToOpen = null;

            switch (selectedTable)
            {
                case "Заказы":
                    formToOpen = new FormEditOrder(row);  // Создадим ниже
                    break;

                case "Выполненные работы":
                    formToOpen = new FormEditCompletedWork(row);
                    break;

                case "Техника":
                    formToOpen = new FormEditTechnic(row);
                    break;

                case "Клиенты":
                    formToOpen = new FormEditClient(row);
                    break;

                case "Сотрудники":
                    formToOpen = new FormEditStaff(row);
                    break;

                case "Запчасти":
                    formToOpen = new FormEditRepairPart(row);
                    break;

                case "Поставщики":
                    formToOpen = new FormEditSupplier(row);
                    break;

                case "История поставок":
                    formToOpen = new FormEditSupply(row);
                    break;

                default:
                    MessageBox.Show("Редактирование для этой таблицы ещё не реализовано.");
                    return;
            }

            if (formToOpen != null)
            {
                formToOpen.ShowDialog();
                RefreshCurrentTable();  // Обновляем таблицу после редактирования
            }


        }



        private void buttonClear_Click(object sender, EventArgs e)
        {

            textBoxSearch.Clear();
            comboBoxStatus.SelectedItem = "Все";
            checkBoxDateFilter.Checked = false;
            if (dataGridView1.DataSource is DataView dv)
                dv.RowFilter = "";

        }

        private void comboBoxStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dateTimePickerFrom_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dateTimePickerTo_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void checkBoxDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void отчётToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void отчётПоЗаказуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || listBox1.SelectedItem.ToString() != "Заказы")
            {
                MessageBox.Show("Для генерации отчёта выберите таблицу \"Заказы\" в меню слева.",
                              "Отчёт по заказу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заказ для генерации отчёта!",
                              "Отчёт по заказу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int orderId = Convert.ToInt32(row.Cells["Номер заказа"].Value);

            // Генерируем отчёт
            ReportGenerator report = new ReportGenerator(sqlconnect);
            report.GenerateReportForOrder(orderId);
        }

        private void месячныйОтчётToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormMonthSelect monthForm = new FormMonthSelect())
            {
                if (monthForm.ShowDialog() == DialogResult.OK)
                {
                    int year = monthForm.SelectedYear;
                    int month = monthForm.SelectedMonth;

                    // Генерируем отчёт
                    ReportGenerator report = new ReportGenerator(sqlconnect);
                    report.GenerateMonthlyProfitReport(year, month);
                }
            }
        }
        private string GetMonthName(int month)
        {
            string[] months = {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
            "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };
            return month >= 1 && month <= 12 ? months[month - 1] : "Неизвестный месяц";
        }
    }
    public class ReportGenerator
    {
        private readonly string connectionString;

        public ReportGenerator(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void GenerateReportForOrder(int orderId)
        {
            DataSet dataSet = GetOrderData(orderId);
            string filePath = GetSaveFilePath();
            if (string.IsNullOrEmpty(filePath)) return;

            CreatePdfReport(filePath, dataSet, orderId);
            MessageBox.Show("Отчёт успешно создан!\nФайл: " + filePath, "Отчёт", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DataSet GetOrderData(int orderId)
        {
            DataSet ds = new DataSet();

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // 1. Основные данные заказа
                string queryOrder = @"
                SELECT 
                    o.order_id,
                    o.order_datecreation,
                    o.order_datecompletion,
                    o.order_description,
                    o.order_mileage,
                    o.order_cost,
                    CASE o.order_status
                        WHEN 0 THEN 'В работе'
                        WHEN 1 THEN 'Выполнен'
                        WHEN 2 THEN 'Отменён'
                    END AS status_text,
                    c.client_surname || ' ' || c.client_name || ' ' || COALESCE(c.client_patronymic, '') AS client_fio,
                    c.client_phonenumber AS client_phone,
                    c.client_adress AS client_address,
                    t.technic_mark || ' ' || t.technic_model AS technic_full,
                    t.technic_serialnumber,
                    t.technic_condition,
                    s.staff_surname || ' ' || s.staff_name || ' ' || COALESCE(s.staff_patronymic, '') AS staff_fio,
                    s.staff_post
                FROM ""Order"" o
                LEFT JOIN client c ON o.client_id = c.client_id
                LEFT JOIN technic t ON o.technic_id = t.technic_id
                LEFT JOIN staff s ON o.staff_id = s.staff_id
                WHERE o.order_id = @id";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryOrder, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@id", orderId);
                    DataTable dtOrder = new DataTable("OrderInfo");
                    adapter.Fill(dtOrder);
                    ds.Tables.Add(dtOrder);
                }

                // 2. Выполненные работы с JOIN на staff для ФИО
                string queryWorks = @"
                SELECT 
                    cw.complworks_datetime AS start_date,
                    cw.complworks_duration AS end_date,
                    cw.complworks_jobcontent AS job_content,
                    s.staff_surname || ' ' || s.staff_name || ' ' || COALESCE(s.staff_patronymic, '') AS executor_fio
                FROM completed_works cw
                LEFT JOIN staff s ON cw.staff_id = s.staff_id
                WHERE cw.order_id = @id
                ORDER BY cw.complworks_datetime";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryWorks, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@id", orderId);
                    DataTable dtWorks = new DataTable("CompletedWorks");
                    adapter.Fill(dtWorks);
                    ds.Tables.Add(dtWorks);
                }
            }

            return ds;
        }

        private string GetSaveFilePath()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF файлы (*.pdf)|*.pdf";
                sfd.FileName = "Отчет_по_заказу.pdf";
                sfd.Title = "Сохранить отчёт";
                return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
            }
        }

        private void CreatePdfReport(string filePath, DataSet dataSet, int orderId)
        {
            // Регистрируем провайдер кодировок перед использованием
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // Создаем шрифт с поддержкой кириллицы
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                if (!File.Exists(fontPath))
                    fontPath = "c:\\windows\\fonts\\arial.ttf";

                BaseFont baseFont;
                try
                {
                    baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                }
                catch
                {
                    // Резервный вариант
                    baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                }

                // Создаем шрифты
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 11);
                iTextSharp.text.Font normalFontBold = new iTextSharp.text.Font(baseFont, 11, iTextSharp.text.Font.BOLD);

                // Заголовок
                Paragraph title = new Paragraph($"Отчёт по заказу №{orderId}", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                DataTable orderInfo = dataSet.Tables["OrderInfo"];
                if (orderInfo.Rows.Count > 0)
                {
                    DataRow r = orderInfo.Rows[0];

                    // Информация о клиенте
                    document.Add(new Paragraph("Информация о клиенте", headerFont));
                    document.Add(new Paragraph($"ФИО: {GetSafeString(r["client_fio"])}", normalFont));
                    document.Add(new Paragraph($"Телефон: {GetSafeString(r["client_phone"])}", normalFont));
                    document.Add(new Paragraph($"Адрес: {GetSafeString(r["client_address"])}", normalFont));
                    document.Add(new Paragraph(Chunk.NEWLINE));

                    // Техника
                    document.Add(new Paragraph("Техника", headerFont));
                    document.Add(new Paragraph($"Марка и модель: {GetSafeString(r["technic_full"])}", normalFont));
                    document.Add(new Paragraph($"Серийный номер: {GetSafeString(r["technic_serialnumber"])}", normalFont));
                    document.Add(new Paragraph($"Пробег: {GetSafeString(r["order_mileage"])}", normalFont));
                    document.Add(new Paragraph($"Состояние: {GetSafeString(r["technic_condition"])}", normalFont));
                    document.Add(new Paragraph(Chunk.NEWLINE));

                    // Заказ
                    document.Add(new Paragraph("Заказ", headerFont));

                    // Дата создания
                    string creationDate = "Не указана";
                    if (r["order_datecreation"] != DBNull.Value && r["order_datecreation"] != null)
                    {
                        creationDate = ((DateTime)r["order_datecreation"]).ToString("dd.MM.yyyy");
                    }
                    document.Add(new Paragraph($"Дата создания: {creationDate}", normalFont));

                    // Дата завершения
                    string completionText = "Не завершён";
                    if (r["order_datecompletion"] != DBNull.Value && r["order_datecompletion"] != null)
                    {
                        completionText = ((DateTime)r["order_datecompletion"]).ToString("dd.MM.yyyy");
                    }
                    document.Add(new Paragraph($"Дата завершения: {completionText}", normalFont));

                    document.Add(new Paragraph($"Статус: {GetSafeString(r["status_text"])}", normalFont));
                    document.Add(new Paragraph($"Ответственный: {GetSafeString(r["staff_fio"])} ({GetSafeString(r["staff_post"])})", normalFont));
                    document.Add(new Paragraph($"Описание: {GetSafeString(r["order_description"])}", normalFont));

                    // Стоимость
                    string costText = "0,00 руб.";
                    if (r["order_cost"] != DBNull.Value && r["order_cost"] != null)
                    {
                        if (decimal.TryParse(r["order_cost"].ToString(), out decimal cost))
                        {
                            costText = cost.ToString("N2") + " руб.";
                        }
                    }
                    document.Add(new Paragraph($"Стоимость: {costText}", normalFont));

                    document.Add(new Paragraph(Chunk.NEWLINE));

                    // Выполненные работы
                    document.Add(new Paragraph("Выполненные работы", headerFont));
                    DataTable works = dataSet.Tables["CompletedWorks"];

                    // ДЕБАГ: Выводим информацию о структуре таблицы
                    if (works != null)
                    {
                        // Проверяем доступные столбцы
                        string columnsInfo = "Доступные столбцы: ";
                        foreach (DataColumn col in works.Columns)
                        {
                            columnsInfo += col.ColumnName + ", ";
                        }
                        // Для отладки можно вывести в консоль
                        // System.Diagnostics.Debug.WriteLine(columnsInfo);
                    }

                    if (works == null || works.Rows.Count == 0)
                    {
                        document.Add(new Paragraph("Работы ещё не выполнены.", normalFont));
                    }
                    else
                    {
                        int workNumber = 1;
                        foreach (DataRow wr in works.Rows)
                        {
                            document.Add(new Paragraph($"Работа #{workNumber}", normalFontBold));

                            
                            string startDate = "Не указана";
                            object startValue = null;

                            // Пробуем разные возможные имена столбцов
                            if (works.Columns.Contains("start_date"))
                                startValue = wr["start_date"];
                            else if (works.Columns.Contains("complworks_datetime"))
                                startValue = wr["complworks_datetime"];
                            else if (works.Columns.Contains("Дата начала работы")) 
                                startValue = wr["Дата начала работы"];

                            if (startValue != null && startValue != DBNull.Value)
                            {
                                startDate = ((DateTime)startValue).ToString("dd.MM.yyyy HH:mm");
                            }
                            document.Add(new Paragraph($"Начало: {startDate}", normalFont));

                            // Дата завершения
                            string endDate = "Не завершено";
                            object endValue = null;

                            if (works.Columns.Contains("end_date"))
                                endValue = wr["end_date"];
                            else if (works.Columns.Contains("complworks_duration"))
                                endValue = wr["complworks_duration"];
                            else if (works.Columns.Contains("Дата завершения работы"))
                                endValue = wr["Дата завершения работы"];

                            if (endValue != null && endValue != DBNull.Value)
                            {
                                endDate = ((DateTime)endValue).ToString("dd.MM.yyyy HH:mm");
                            }
                            document.Add(new Paragraph($"Завершение: {endDate}", normalFont));

                            // Исполнитель
                            string executor = GetSafeString(wr["executor_fio"]);
                            if (string.IsNullOrEmpty(executor) && works.Columns.Contains("ФИО сотрудника"))
                            {
                                executor = GetSafeString(wr["ФИО сотрудника"]);
                            }
                            document.Add(new Paragraph($"Исполнитель: {executor}", normalFont));

                            // Содержание работ
                            string jobContent = GetSafeString(wr["job_content"]);
                            if (string.IsNullOrEmpty(jobContent) && works.Columns.Contains("complworks_jobcontent"))
                            {
                                jobContent = GetSafeString(wr["complworks_jobcontent"]);
                            }
                            else if (string.IsNullOrEmpty(jobContent) && works.Columns.Contains("Проведённые работы"))
                            {
                                jobContent = GetSafeString(wr["Проведённые работы"]);
                            }
                            document.Add(new Paragraph($"Содержание: {jobContent}", normalFont));

                            document.Add(new Paragraph(Chunk.NEWLINE));
                            workNumber++;
                        }
                    }
                }
                else
                {
                    document.Add(new Paragraph("Данные по заказу не найдены.", normalFont));
                }

                // Футер с датой создания отчета
                document.Add(new Paragraph(Chunk.NEWLINE));
                Paragraph footer = new Paragraph($"Отчёт создан: {DateTime.Now:dd.MM.yyyy HH:mm}",
                    new iTextSharp.text.Font(baseFont, 9, iTextSharp.text.Font.ITALIC));
                footer.Alignment = Element.ALIGN_RIGHT;
                document.Add(footer);

                document.Close();
            }
        }

        // Вспомогательный метод для безопасного получения строк
        private string GetSafeString(object value)
        {
            if (value == null || value == DBNull.Value || Convert.IsDBNull(value))
                return "Не указан";

            string str = value.ToString();
            return string.IsNullOrEmpty(str) ? "Не указан" : str;
        }


        public void GenerateMonthlyProfitReport(int year, int month)
        {
            MonthlyReportData reportData = GetMonthlyProfitData(year, month);
            string filePath = GetSaveFilePath($"Прибыль_за_{month:00}_{year}");
            if (string.IsNullOrEmpty(filePath)) return;

            CreateMonthlyProfitPdf(filePath, reportData, year, month);
            MessageBox.Show($"Отчёт за {GetMonthName(month)} {year} создан!\nФайл: {filePath}",
                          "Отчёт", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private MonthlyReportData GetMonthlyProfitData(int year, int month)
        {
            MonthlyReportData data = new MonthlyReportData();

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // 1. Общая статистика по выполненным заказам
                string queryCompletedOrders = @"
            SELECT 
                COUNT(*) as total_orders,
                COALESCE(SUM(CAST(order_cost AS numeric)), 0) as total_income,
                COALESCE(AVG(CAST(order_cost AS numeric)), 0) as avg_order_cost
            FROM ""Order"" 
            WHERE EXTRACT(YEAR FROM order_datecompletion) = @year 
              AND EXTRACT(MONTH FROM order_datecompletion) = @month
              AND order_status = 1";

                using (NpgsqlCommand cmd = new NpgsqlCommand(queryCompletedOrders, conn))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@month", month);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.TotalOrders = reader.GetInt32(0);
                            data.TotalIncome = reader.GetDecimal(1);
                            data.AverageOrderCost = reader.GetDecimal(2);
                        }
                        else
                        {
                            // Если нет данных, устанавливаем значения по умолчанию
                            data.TotalOrders = 0;
                            data.TotalIncome = 0;
                            data.AverageOrderCost = 0;
                        }
                    }
                }

                // 2. Расходы на запчасти за месяц
                string queryPartsCost = @"
            SELECT 
                COALESCE(SUM(CAST(supply_cost AS numeric)), 0) as total_parts_cost
            FROM histotysupply 
            WHERE EXTRACT(YEAR FROM supply_date) = @year 
              AND EXTRACT(MONTH FROM supply_date) = @month";

                using (NpgsqlCommand cmd = new NpgsqlCommand(queryPartsCost, conn))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@month", month);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.TotalPartsCost = reader.GetDecimal(0);
                        }
                        else
                        {
                            data.TotalPartsCost = 0;
                        }
                    }
                }

                // 3. Топ 5 самых дорогих заказов
                string queryTopOrders = @"
            SELECT 
                o.order_id,
                o.order_datecompletion,
                CAST(o.order_cost AS numeric) as order_cost,
                c.client_surname || ' ' || c.client_name as client_name,
                t.technic_mark || ' ' || t.technic_model as technic_info
            FROM ""Order"" o
            LEFT JOIN client c ON o.client_id = c.client_id
            LEFT JOIN technic t ON o.technic_id = t.technic_id
            WHERE EXTRACT(YEAR FROM o.order_datecompletion) = @year 
              AND EXTRACT(MONTH FROM o.order_datecompletion) = @month
              AND o.order_status = 1
            ORDER BY CAST(o.order_cost AS numeric) DESC NULLS LAST
            LIMIT 5";

                data.TopOrders = new DataTable("TopOrders");
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryTopOrders, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@year", year);
                    adapter.SelectCommand.Parameters.AddWithValue("@month", month);
                    adapter.Fill(data.TopOrders);
                }

                // 4. Количество заказов по дням
                string queryOrdersByDay = @"
            SELECT 
                EXTRACT(DAY FROM order_datecompletion) as day,
                COUNT(*) as order_count,
                COALESCE(SUM(CAST(order_cost AS numeric)), 0) as day_income
            FROM ""Order""
            WHERE EXTRACT(YEAR FROM order_datecompletion) = @year 
              AND EXTRACT(MONTH FROM order_datecompletion) = @month
              AND order_status = 1
            GROUP BY EXTRACT(DAY FROM order_datecompletion)
            ORDER BY day";

                data.OrdersByDay = new DataTable("OrdersByDay");
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryOrdersByDay, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@year", year);
                    adapter.SelectCommand.Parameters.AddWithValue("@month", month);
                    adapter.Fill(data.OrdersByDay);
                }

                // 5. Статистика по сотрудникам
                string queryStaffStats = @"
            SELECT 
                s.staff_surname || ' ' || s.staff_name as staff_name,
                COUNT(o.order_id) as completed_orders,
                COALESCE(SUM(CAST(o.order_cost AS numeric)), 0) as total_income
            FROM staff s
            LEFT JOIN ""Order"" o ON s.staff_id = o.staff_id 
                AND EXTRACT(YEAR FROM o.order_datecompletion) = @year 
                AND EXTRACT(MONTH FROM o.order_datecompletion) = @month
                AND o.order_status = 1
            GROUP BY s.staff_id, s.staff_surname, s.staff_name
            HAVING COUNT(o.order_id) > 0
            ORDER BY COALESCE(SUM(CAST(o.order_cost AS numeric)), 0) DESC";

                data.StaffStatistics = new DataTable("StaffStatistics");
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryStaffStats, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@year", year);
                    adapter.SelectCommand.Parameters.AddWithValue("@month", month);
                    adapter.Fill(data.StaffStatistics);
                }

                // 6. Статистика по маркам техники
                string queryTechnicStats = @"
            SELECT 
                t.technic_mark,
                COUNT(o.order_id) as order_count,
                COALESCE(SUM(CAST(o.order_cost AS numeric)), 0) as total_income
            FROM technic t
            LEFT JOIN ""Order"" o ON t.technic_id = o.technic_id 
                AND EXTRACT(YEAR FROM o.order_datecompletion) = @year 
                AND EXTRACT(MONTH FROM o.order_datecompletion) = @month
                AND o.order_status = 1
            GROUP BY t.technic_mark
            HAVING COUNT(o.order_id) > 0
            ORDER BY COALESCE(SUM(CAST(o.order_cost AS numeric)), 0) DESC";

                data.TechnicStatistics = new DataTable("TechnicStatistics");
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryTechnicStats, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@year", year);
                    adapter.SelectCommand.Parameters.AddWithValue("@month", month);
                    adapter.Fill(data.TechnicStatistics);
                }
            }

            // Рассчитываем чистую прибыль
            data.NetProfit = data.TotalIncome - data.TotalPartsCost;

            return data;
        }

        private void CreateMonthlyProfitPdf(string filePath, MonthlyReportData data, int year, int month)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // Шрифты
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                BaseFont baseFont = File.Exists(fontPath)
                    ? BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED)
                    : BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 11);
                iTextSharp.text.Font boldFont = new iTextSharp.text.Font(baseFont, 11, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font largeNumberFont = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD);

                // Заголовок
                Paragraph title = new Paragraph($"ОТЧЁТ О ПРИБЫЛИ\n{GetMonthName(month).ToUpper()} {year}", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 30;
                document.Add(title);

                // 1. Основные финансовые показатели
                document.Add(new Paragraph("ФИНАНСОВЫЕ ПОКАЗАТЕЛИ", headerFont));

                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 70, 30 });

                AddSummaryRow(summaryTable, "Всего выполненных заказов:", data.TotalOrders.ToString(), boldFont, normalFont);
                AddSummaryRow(summaryTable, "Общий доход:", data.TotalIncome.ToString("N2") + " руб.", boldFont, normalFont);
                AddSummaryRow(summaryTable, "Средняя стоимость заказа:", data.AverageOrderCost.ToString("N2") + " руб.", boldFont, normalFont);
                AddSummaryRow(summaryTable, "Затраты на запчасти:", data.TotalPartsCost.ToString("N2") + " руб.", boldFont, normalFont);

                // Определяем цвет для прибыли/убытка
                iTextSharp.text.Font profitFont;
                if (data.NetProfit >= 0)
                {
                    profitFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                    profitFont.Color = new BaseColor(0, 100, 0); // Зелёный
                }
                else
                {
                    profitFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                    profitFont.Color = new BaseColor(200, 0, 0); // Красный
                }

                AddSummaryRow(summaryTable, "ЧИСТАЯ ПРИБЫЛЬ:", data.NetProfit.ToString("N2") + " руб.", boldFont, profitFont);

                document.Add(summaryTable);
                document.Add(new Paragraph(Chunk.NEWLINE));

                // 2. Топ 5 самых дорогих заказов
                if (data.TopOrders.Rows.Count > 0)
                {
                    document.Add(new Paragraph("ТОП-5 САМЫХ ДОРОГИХ ЗАКАЗОВ", headerFont));

                    PdfPTable topOrdersTable = new PdfPTable(5);
                    topOrdersTable.WidthPercentage = 100;
                    topOrdersTable.SetWidths(new float[] { 15, 20, 25, 20, 20 });

                    // Заголовки таблицы
                    AddTableCell(topOrdersTable, "№ заказа", boldFont, true);
                    AddTableCell(topOrdersTable, "Дата завершения", boldFont, true);
                    AddTableCell(topOrdersTable, "Клиент", boldFont, true);
                    AddTableCell(topOrdersTable, "Техника", boldFont, true);
                    AddTableCell(topOrdersTable, "Стоимость", boldFont, true);

                    // Данные
                    foreach (DataRow row in data.TopOrders.Rows)
                    {
                        AddTableCell(topOrdersTable, row["order_id"].ToString(), normalFont, false);

                        string dateStr = "Не указана";
                        if (row["order_datecompletion"] != DBNull.Value)
                        {
                            dateStr = Convert.ToDateTime(row["order_datecompletion"]).ToString("dd.MM.yyyy");
                        }
                        AddTableCell(topOrdersTable, dateStr, normalFont, false);

                        AddTableCell(topOrdersTable, GetSafeString(row["client_name"]), normalFont, false);
                        AddTableCell(topOrdersTable, GetSafeString(row["technic_info"]), normalFont, false);

                        string costStr = "0,00 руб.";
                        if (row["order_cost"] != DBNull.Value)
                        {
                            decimal cost = Convert.ToDecimal(row["order_cost"]);
                            costStr = cost.ToString("N2") + " руб.";
                        }
                        AddTableCell(topOrdersTable, costStr, normalFont, false);
                    }

                    document.Add(topOrdersTable);
                    document.Add(new Paragraph(Chunk.NEWLINE));
                }

                // 3. Заказы по дням
                if (data.OrdersByDay.Rows.Count > 0)
                {
                    document.Add(new Paragraph("ЗАКАЗЫ ПО ДНЯМ МЕСЯЦА", headerFont));

                    PdfPTable daysTable = new PdfPTable(3);
                    daysTable.WidthPercentage = 100;
                    daysTable.SetWidths(new float[] { 33, 33, 34 });

                    AddTableCell(daysTable, "День", boldFont, true);
                    AddTableCell(daysTable, "Кол-во заказов", boldFont, true);
                    AddTableCell(daysTable, "Доход за день", boldFont, true);

                    foreach (DataRow row in data.OrdersByDay.Rows)
                    {
                        int day = Convert.ToInt32(row["day"]);
                        int orderCount = Convert.ToInt32(row["order_count"]);
                        decimal dayIncome = row["day_income"] != DBNull.Value ? Convert.ToDecimal(row["day_income"]) : 0;

                        AddTableCell(daysTable, day.ToString(), normalFont, false);
                        AddTableCell(daysTable, orderCount.ToString(), normalFont, false);
                        AddTableCell(daysTable, dayIncome.ToString("N2") + " руб.", normalFont, false);
                    }

                    document.Add(daysTable);
                    document.Add(new Paragraph(Chunk.NEWLINE));
                }

                // 4. Статистика по сотрудникам
                if (data.StaffStatistics.Rows.Count > 0)
                {
                    document.Add(new Paragraph("СТАТИСТИКА ПО СОТРУДНИКАМ", headerFont));

                    PdfPTable staffTable = new PdfPTable(3);
                    staffTable.WidthPercentage = 100;
                    staffTable.SetWidths(new float[] { 50, 25, 25 });

                    AddTableCell(staffTable, "Сотрудник", boldFont, true);
                    AddTableCell(staffTable, "Выполнено заказов", boldFont, true);
                    AddTableCell(staffTable, "Принесённый доход", boldFont, true);

                    foreach (DataRow row in data.StaffStatistics.Rows)
                    {
                        AddTableCell(staffTable, GetSafeString(row["staff_name"]), normalFont, false);
                        AddTableCell(staffTable, row["completed_orders"].ToString(), normalFont, false);

                        string incomeStr = "0,00 руб.";
                        if (row["total_income"] != DBNull.Value)
                        {
                            decimal income = Convert.ToDecimal(row["total_income"]);
                            incomeStr = income.ToString("N2") + " руб.";
                        }
                        AddTableCell(staffTable, incomeStr, normalFont, false);
                    }

                    document.Add(staffTable);
                    document.Add(new Paragraph(Chunk.NEWLINE));
                }

                // 5. Статистика по маркам техники
                if (data.TechnicStatistics.Rows.Count > 0)
                {
                    document.Add(new Paragraph("СТАТИСТИКА ПО МАРКАМ ТЕХНИКИ", headerFont));

                    PdfPTable technicTable = new PdfPTable(3);
                    technicTable.WidthPercentage = 100;
                    technicTable.SetWidths(new float[] { 50, 25, 25 });

                    AddTableCell(technicTable, "Марка техники", boldFont, true);
                    AddTableCell(technicTable, "Кол-во заказов", boldFont, true);
                    AddTableCell(technicTable, "Общий доход", boldFont, true);

                    foreach (DataRow row in data.TechnicStatistics.Rows)
                    {
                        AddTableCell(technicTable, GetSafeString(row["technic_mark"]), normalFont, false);
                        AddTableCell(technicTable, row["order_count"].ToString(), normalFont, false);

                        string incomeStr = "0,00 руб.";
                        if (row["total_income"] != DBNull.Value)
                        {
                            decimal income = Convert.ToDecimal(row["total_income"]);
                            incomeStr = income.ToString("N2") + " руб.";
                        }
                        AddTableCell(technicTable, incomeStr, normalFont, false);
                    }

                    document.Add(technicTable);
                }
                else
                {
                    document.Add(new Paragraph("Нет данных по статистике техники.", normalFont));
                }

                // Футер
                document.Add(new Paragraph(Chunk.NEWLINE));
                Paragraph footer = new Paragraph($"Отчёт сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}",
                    new iTextSharp.text.Font(baseFont, 9, iTextSharp.text.Font.ITALIC));
                footer.Alignment = Element.ALIGN_RIGHT;
                document.Add(footer);

                document.Close();
            }
        }

        // Вспомогательные методы
        private string GetMonthName(int month)
        {
            string[] months = {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
            "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };
            return months[month - 1];
        }

        private string GetSafeFilePath(string baseName)
        {
            string fileName = $"{baseName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        }

        private void AddSummaryRow(PdfPTable table, string label, string value, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = PdfPCell.NO_BORDER;
            labelCell.Padding = 5;

            PdfPCell valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = PdfPCell.NO_BORDER;
            valueCell.Padding = 5;
            valueCell.HorizontalAlignment = Element.ALIGN_RIGHT;

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private void AddTableCell(PdfPTable table, string text, iTextSharp.text.Font font, bool isHeader)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Padding = 5;
            if (isHeader)
            {
                cell.BackgroundColor = new BaseColor(240, 240, 240);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
            }
            table.AddCell(cell);
        }
        public class MonthlyReportData
        {
            public int TotalOrders { get; set; }
            public decimal TotalIncome { get; set; }
            public decimal AverageOrderCost { get; set; }
            public decimal TotalPartsCost { get; set; }
            public decimal NetProfit { get; set; }
            public DataTable TopOrders { get; set; }
            public DataTable OrdersByDay { get; set; }
            public DataTable StaffStatistics { get; set; }
            public DataTable TechnicStatistics { get; set; }
        }

        private string GetSaveFilePath(string reportName)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string defaultName = $"{reportName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                sfd.FileName = defaultName;
                sfd.Filter = "PDF файлы (*.pdf)|*.pdf";
                sfd.Title = "Сохранить отчёт";
                return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
            }
        }
    }
}

