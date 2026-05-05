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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ИДЗБД
{
    public partial class FormDelete : Form
    {
        string sqlconnect = "Server=localhost;Port=5432;Database=servis; User Id =postgres; Password=RBHBkk2002;";
        public FormDelete()
        {
            InitializeComponent();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection sqlConnection = new NpgsqlConnection(sqlconnect))
                {
                    sqlConnection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {

                        command.Connection = sqlConnection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = $@" UPDATE ""{"Order"}"" Set staff_id ={textBox2.Text} , client_id = {textBox3.Text} , 
technic_id = {textBox4.Text} , order_datecreation = {textBox5.Text} , order_status = {textBox6.Text} , order_datecompletion = {textBox7.Text} , 
order_description = '{textBox8.Text}' , order_cost = {textBox9.Text} , order_mileage = {textBox10.Text} WHERE order_id = {textBox1.Text} ;";
 


                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные успешно добавлены.");
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить данные.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox6.Text = string.Empty;
            textBox7.Text = string.Empty;   
            textBox8.Text = string.Empty;
            textBox9.Text = string.Empty;
            textBox10.Text = string.Empty;
        }
    }
}
