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

// Form used to add a new ingredient to the database

namespace Groceries
{
    public partial class Form2 : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;

        BindingList<string> bl;

        string source = null;
        string category = null;
        string staple = null;


        


        public Form2(MySqlConnection conn)
        {
            InitializeComponent();
   
            this.conn = conn;
            bl = new BindingList<string>(Updates.getCatagories());
            comboBox1.DataSource = bl;
            comboBox1.SelectedIndex = -1;
            comboBox2.DataSource = Updates.getStores();
            comboBox2.SelectedIndex = -1;
        }

        // Adds ingredient
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Ingredient ing in Updates.getIngredients())
            {
                if (ing.name == textBox1.Text)
                {
                    MessageBox.Show("This ingredient is already in the list");
                    textBox1.Text = null;
                    radioButton1.Checked = false;
                    button1.Enabled = false;
                    numericUpDown1.Value = 0;
                    comboBox1.Text = "";
                    comboBox2.Text = "";
                    textBox1.Select();
                    return;
                }
            }

            if (comboBox1.Text != String.Empty)
            {
                bool inList = false;

                foreach (string s in Updates.getCatagories())
                {
                    if (s == comboBox1.Text)
                    {
                        inList = true;
                        break;
                    }
                }

                if (!inList)
                {
                    category = null;
                    MessageBox.Show("You must select a category from the list or add it.");
                    comboBox1.Select();
                    return;
                }
                if (inList)
                {
                    category = comboBox1.Text;
                }
            }
            else
            {
                category = null;
            }

            if (comboBox2.Text != String.Empty)
            {
                bool inList = false;

                foreach (string s in Updates.getStores())
                {
                    if (s == comboBox2.Text)
                    {
                        inList = true;
                        break;
                    }
                }

                if (!inList)
                {
                    source = null;
                    MessageBox.Show("You must select a store from the list or add it.");
                    comboBox2.Select();
                    return;
                }

                if (inList)
                {
                    source = comboBox2.Text;
                }
            }
            else
            {
                source = null;
            }


            if (radioButton1.Checked)
                staple = "true";
            else
                staple = "false";

            string cmdText = "SET TRANSACTION READ WRITE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string theCmd = "CALL addIngredient(\'" + textBox1.Text + "\', \'" + category + "\', \'" + source + "\', " + staple + ", " + numericUpDown1.Value.ToString() + ");" ;
            cmd = new MySqlCommand(theCmd, conn);
            cmd.ExecuteNonQuery();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            Updates.updateIngredients(conn);   
            MessageBox.Show("This ingredient is added to the list");
            textBox1.Text = null;
            button1.Enabled = false;
            numericUpDown1.Value = 0;
            comboBox1.Text = "";
            comboBox2.Text = "";
            radioButton1.Checked = false;
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


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_Leave_1(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_Leave(object sender, EventArgs e)
        {
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Add category button
        private void button3_Click(object sender, EventArgs e)
        {
            foreach (string cat in Updates.getCatagories())
            {
                if (cat == comboBox1.Text)
                {
                    MessageBox.Show("This ingredient category is already in the list");
                    return;
                }
            }
            string theCmd = "INSERT INTO Category (name) VALUES (\'" + comboBox1.Text + "\');";
            cmd = new MySqlCommand(theCmd, conn);
            cmd.ExecuteNonQuery();
            Updates.updateCategory(conn);
            MessageBox.Show("This category is added to the list");
        }

        // Add store button
        private void button4_Click(object sender, EventArgs e)
        {
            foreach (string store in Updates.getStores())
            {
                if (store == comboBox2.Text)
                {
                    MessageBox.Show("This store is already in the list");
                    return;
                }
            }
            string theCmd = "INSERT INTO Source (name) VALUES (\'" + comboBox2.Text + "\');";
            cmd = new MySqlCommand(theCmd, conn);
            cmd.ExecuteNonQuery();
            Updates.updateSource(conn);
            MessageBox.Show("This store is added to the list");         
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
