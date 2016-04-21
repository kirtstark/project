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

// this form adds an ingredient category to the list
namespace Groceries
{
    public partial class Form5 : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;

        public Form5(MySqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string cat in Updates.getCatagories())
            {
                if (cat == textBox1.Text)
                {
                    MessageBox.Show("This ingredient category is already in the list");
                    button1.Enabled = false;
                    textBox1.Text = null;
                    textBox1.Select();
                    return;
                }
            }
            string theCmd = "INSERT INTO Category (name) VALUES (\'" + textBox1.Text + "\');";
            cmd = new MySqlCommand(theCmd, conn);
            cmd.ExecuteNonQuery();
            Updates.updateCategory(conn);
            MessageBox.Show("This category is added to the list");
            textBox1.Text = null;
            button1.Enabled = false;
            textBox1.Select();
        }
    }
}
