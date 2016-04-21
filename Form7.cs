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

// This form adds a recipe for a dish or desert

namespace Groceries
{
    public partial class Form7 : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        string sql;
        MySqlDataReader rdr;
        ListViewItem item;
        string whole;
        string fraction;

        public Form7(MySqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
            checkedListBox1.CheckOnClick = true;
            listBox1.DataSource = Updates.getCuisines();
            listBox2.DataSource = Updates.getSeasons();
            numericUpDown1.Value = 1;
            listBox3.DataSource = Updates.getIngredientsList();
            listBox4.DataSource = Updates.getUnits();
            listView1.View = View.Details;
            listView1.Columns.Add("Name");
            listView1.Columns.Add("Amount");
            listView1.Columns.Add("Measurement");

            //make most controls invisible
            makeVisible(false);
            
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                checkedListBox1.Visible = true;
                label2.Visible = true;
            }
            else
            {
                checkedListBox1.Visible = false;
                label2.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 10)
                numericUpDown1.Value = 10;
            if (numericUpDown1.Value < 1)
                numericUpDown1.Value = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            whole = numericUpDown2.Value.ToString();
            fraction = numericUpDown3.Value.ToString();
            item = new ListViewItem();
            item.Text = listBox3.SelectedItem.ToString();

            item.SubItems.Add(whole + "." + fraction);
            item.SubItems.Add(listBox4.SelectedItem.ToString());

            listView1.Items.Add(item);
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null)
                button2.Enabled = true;

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            makeVisible(false);
        }

        public void makeVisible(bool there)
        {
            button3.Visible = !there;
            button4.Visible = there;
            label4.Visible = there;
            label5.Visible = there;
            label6.Visible = there;
            label7.Visible = there;
            label8.Visible = there;
            label9.Visible = there;
            label10.Visible = there;
            label11.Visible = there;
            label12.Visible = there;
            label13.Visible = there;
            listBox1.Visible = there;
            listBox2.Visible = there;
            listBox3.Visible = there;
            listBox4.Visible = there;
            button2.Visible = there;
            numericUpDown1.Visible = there;
            numericUpDown2.Visible = there;
            numericUpDown3.Visible = there;
            listView1.Visible = there;
            richTextBox1.Visible = there;
            radioButton1.Enabled = !there;
            radioButton2.Enabled = !there;
            radioButton3.Enabled = !there;
            checkedListBox1.Enabled = !there;
        }

        // This button adds the new menu item recipe
        private void button4_Click(object sender, EventArgs e)
        {
            string menu = "";
            string menuIngredients = "";
            string meal = "";
            if (radioButton1.Checked || radioButton2.Checked)
            {
                int countCheck = 0;

                foreach (int indexChecked in checkedListBox1.CheckedIndices)
                    countCheck++;

                if (countCheck == 0 || countCheck == 3)
                {
                    meal = "Any";
                }
                else
                {
                    if (checkedListBox1.GetItemChecked(0) && !checkedListBox1.GetItemChecked(1) && !checkedListBox1.GetItemChecked(2))
                    {
                        meal = "Breakfast";
                    }
                    if (!checkedListBox1.GetItemChecked(0) && checkedListBox1.GetItemChecked(1) && !checkedListBox1.GetItemChecked(2))
                    {
                        meal = "Lunch";
                    }
                    if (!checkedListBox1.GetItemChecked(0) && !checkedListBox1.GetItemChecked(1) && checkedListBox1.GetItemChecked(2))
                    {
                        meal = "Dinner";
                    }
                    if (!checkedListBox1.GetItemChecked(0) && checkedListBox1.GetItemChecked(1) && checkedListBox1.GetItemChecked(2))
                    {
                        meal = "Lunch or Dinner";
                    }
                }
            }
            if (radioButton1.Checked)
            {
                menu = "Entrees";
                menuIngredients = "EntreeIngredients";
            }
            if (radioButton2.Checked)
            {
                menu = "SideDishes";
                menuIngredients = "SideDishIngredients";
            }
            if (radioButton3.Checked)
            {
                menu = "Deserts";
                menuIngredients = "DesertIngredients";
            }

            string theCmd = "";

            string cmdText = "SET TRANSACTION READ WRITE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            

            theCmd = "INSERT INTO " + menu + " VALUES (\'" + textBox1.Text + "\', \'" + listBox1.SelectedItem.ToString() + "\', \'" +
                listBox2.SelectedItem.ToString() + "\', null, \'" + richTextBox1.Text + "\', " + numericUpDown1.Value.ToString();

            if (radioButton1.Checked ||radioButton2.Checked)
            {
                theCmd = theCmd + ", \'" + meal + "\'";
            }

            theCmd = theCmd + ");";
            cmd = new MySqlCommand(theCmd, conn);
            cmd.ExecuteNonQuery();

            foreach (ListViewItem item in listView1.Items)
            {
                theCmd = "INSERT INTO " + menuIngredients + " VALUES (\'" + item.SubItems[0].Text + "\', " + item.SubItems[1].Text + ", \'" + item.SubItems[2].Text
                     + "\', \'" + textBox1.Text + "\');";
                cmd = new MySqlCommand(theCmd, conn);
                cmd.ExecuteNonQuery();
            }
            listView1.Clear();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            makeVisible(false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int exists = 0;
            bool result = false;
            string menu = "Entrees";
            if (radioButton1.Checked)
                menu = "Entrees";
            if (radioButton2.Checked)
                menu = "SideDishes";
            if (radioButton3.Checked)
                menu = "Deserts";

            string cmdText = "SET TRANSACTION READ ONLY;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            sql = "SELECT COUNT(name) FROM " + menu + " WHERE name = \'" + textBox1.Text + " \';";

            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                result = Int32.TryParse((rdr[0].ToString()), out exists);
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            if (exists > 0)
            {
                MessageBox.Show("This item is already on the menu");
                //make most controls invisible
                makeVisible(false);
                textBox1.Text = "";
                return;
            }
            if (!result)
            {
                MessageBox.Show("There was an error in trying to test the item. \nPlease try again.");
                //make most controls invisible
                makeVisible(false);
                textBox1.Text = "";
                return;
            }

            //make everything visible if we can add to the menu items
            makeVisible(true);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                checkedListBox1.Visible = true;
                label2.Visible = true;
            }
            else
            {
                checkedListBox1.Visible = false;
                label2.Visible = false;
            }
        }
    }
}
