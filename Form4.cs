using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


//This form adds a cuisine to the list
namespace Groceries
{
    public partial class Form4 : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;

        public Form4(MySqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string cuisine in Updates.getCuisines())
            {
                if (cuisine == textBox1.Text)
                {
                    MessageBox.Show("This cuisine is already in the list");
                    button1.Enabled = false;
                    textBox1.Text = null;
                    textBox1.Select();
                    return;
                }
            }
            string theCmd = "INSERT INTO Cuisine (name) VALUES (\'" + textBox1.Text + "\');";
            cmd = new MySqlCommand(theCmd, conn);
            cmd.ExecuteNonQuery();
            Updates.updateCuisine(conn);
            MessageBox.Show("This cuisine is added to the list");
            textBox1.Text = null;
            button1.Enabled = false;
            textBox1.Select();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != null) && (textBox1.Text != ""))
            {
                button1.Enabled = true;
            }
            else
                button1.Enabled = false;
        }
    }
}
