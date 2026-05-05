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
    public partial class FormAddClient : Form
    {
        private readonly string sqlconnect = "Server=localhost;Port=5432;Database=servis;User Id=postgres;Password=RBHBkk2002;";

        public FormAddClient()
        {
            InitializeComponent();
            this.Text = "Добавление клиента";
            ConfigureControls();
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

            // Подсказка в поле телефона
            textBoxPhone.GotFocus += (s, e) =>
            {
                if (textBoxPhone.Text == "79123456789")
                    textBoxPhone.Text = "";
            };
            textBoxPhone.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBoxPhone.Text))
                    textBoxPhone.Text = "79123456789";
            };
            textBoxPhone.Text = "79123456789"; // Пример
        }

        private void buttonAdd_Click(object sender, EventArgs e)
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
                            INSERT INTO client 
                            (client_name, client_surname, client_patronymic, client_phonenumber, client_adress)
                            VALUES (@name, @surname, @patronymic, @phone, @address)";

                        cmd.Parameters.AddWithValue("@name", textBoxName.Text.Trim());
                        cmd.Parameters.AddWithValue("@surname", textBoxSurname.Text.Trim());
                        cmd.Parameters.AddWithValue("@patronymic", string.IsNullOrWhiteSpace(textBoxPatronymic.Text) ? (object)DBNull.Value : textBoxPatronymic.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", textBoxPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(textBoxAddress.Text) ? (object)DBNull.Value : textBoxAddress.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении клиента:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormAddClient_Load(object sender, EventArgs e)
        {

        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
