using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ИДЗБД
{
    public partial class FormEditClient : Form
    {
        private readonly string sqlconnect = "Server=localhost;Port=5432;Database=servis;User Id=postgres;Password=RBHBkk2002;";
        private int clientId;  // ID клиента для UPDATE

        public FormEditClient(DataGridViewRow row)
        {
            InitializeComponent();
            this.Text = "Редактирование клиента";
            ConfigureControls();
            FillFields(row);
        }

        private void ConfigureControls()
        {
            // Номер телефона — только цифры, ровно 11 символов
            textBoxPhone.MaxLength = 11;
            textBoxPhone.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    e.Handled = true;
            };
        }

        private void FillFields(DataGridViewRow row)
        {
            clientId = Convert.ToInt32(row.Cells["client_id"].Value);  // Или скрытая колонка client_id

            textBoxName.Text = row.Cells["Имя"].Value?.ToString();
            textBoxSurname.Text = row.Cells["Фамилия"].Value?.ToString();
            textBoxPatronymic.Text = row.Cells["Отчество"].Value?.ToString() ?? "";
            textBoxPhone.Text = row.Cells["Номер телефона"].Value?.ToString();
            textBoxAddress.Text = row.Cells["Адрес проживания"].Value?.ToString() ?? "";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxSurname.Text) ||
                string.IsNullOrWhiteSpace(textBoxPhone.Text) ||
                textBoxPhone.Text.Length != 11)
            {
                MessageBox.Show(
                    "Заполните обязательные поля корректно:\n" +
                    "- Имя\n- Фамилия\n- Номер телефона (ровно 11 цифр, например: 79123456789)",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(sqlconnect))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = @"
                            UPDATE client 
                            SET client_name = @name,
                                client_surname = @surname,
                                client_patronymic = @patronymic,
                                client_phonenumber = @phone,
                                client_adress = @address
                            WHERE client_id = @id";

                        cmd.Parameters.AddWithValue("@id", clientId);
                        cmd.Parameters.AddWithValue("@name", textBoxName.Text.Trim());
                        cmd.Parameters.AddWithValue("@surname", textBoxSurname.Text.Trim());
                        cmd.Parameters.AddWithValue("@patronymic", string.IsNullOrWhiteSpace(textBoxPatronymic.Text) ? (object)DBNull.Value : textBoxPatronymic.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", textBoxPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(textBoxAddress.Text) ? (object)DBNull.Value : textBoxAddress.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Клиент успешно обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Запись не найдена.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении клиента:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormEditClient_Load(object sender, EventArgs e)
        {

        }
    }
}
