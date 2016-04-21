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

namespace Groceries
{
    public partial class Form3 : Form
    {
       MySqlConnection conn;
       MySqlCommand cmd;

       public Form3(MySqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;            
        }

        private void Form3_Load(object sender, EventArgs e)
        {

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
            foreach (string store in Updates.getStores())
            {
                if (store == textBox1.Text)
                {
                    MessageBox.Show("This store is already in the list");
                    textBox1.Text = null;
                    button1.Enabled = false;
                    return;
                }                
            }
            string theCmd = "INSERT INTO Source (name) VALUES (\'" + textBox1.Text + "\');";
            cmd = new MySqlCommand(theCmd, conn);
            cmd.ExecuteNonQuery();
            Updates.updateSource(conn);
            MessageBox.Show("This store is added to the list");
            textBox1.Text = null;
            button1.Enabled = false;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
