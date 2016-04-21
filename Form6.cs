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
    public partial class Form6 : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        string sql;
        MySqlDataReader rdr;
        string menuList = "";
        string menuList2 = "";
        string menuList3 = "";
        string inBox = "";

        public Form6(MySqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
            checkedListBox1.CheckOnClick = true;
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Visible = true;
            label3.Visible = true;
            if (radioButton2.Checked)
            {
                menuList = " SideDishes ";
                sql = "SELECT name from SideDishes;";
            }
            else if (radioButton3.Checked)
            {
                menuList = " Deserts ";
                sql = "SELECT name from Deserts;";
            }
            else if (radioButton1.Checked)
            {
                bool adding = false;
                menuList = " Entrees ";
                sql = "SELECT name FROM Entrees";
                int countCheck = 0;
                
                foreach (int indexChecked in checkedListBox1.CheckedIndices)
                    countCheck++;

                if (countCheck == 0 || countCheck == 3)
                {

                }
                else  
                {
                    if (checkedListBox1.GetItemChecked(0))
                    {
                        sql = sql + " WHERE meal = \'Breakfast\'";
                        adding = true;
                    }
                    if (checkedListBox1.GetItemChecked(1))
                    {
                        if (!adding)
                        {
                            sql = sql + " WHERE meal LIKE \'%Lunch%\'";
                        }
                        else
                        {
                            sql = sql + " OR meal LIKE \'%Lunch%\'";
                        }
                        adding = true;
                    }
                    if (checkedListBox1.GetItemChecked(2))
                    {
                        if (!adding)
                        {
                            sql = sql + " WHERE meal LIKE \'%Dinner%\'";
                        }
                        else
                        {
                            sql = sql + " OR meal LIKE \'%Dinner%\'";
                        }
                    }
            
                }
                if (!adding)
                {
                    sql = sql + ";";
                }
                else
                {
                    sql = sql + " OR meal = \'Any\';";
                }
            }
            else
            {
                MessageBox.Show("You must select one of the available options");
                return;
            }
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            listBox1.Items.Clear();
            listBox1.BeginUpdate();           
            
            while (rdr.Read())
            {
               listBox1.Items.Add(rdr[0].ToString());
            }              
            
            listBox1.EndUpdate();
            rdr.Close();
            checkedListBox1.ClearSelected();
            checkedListBox1.Refresh();
        }

        //get recipe button
        private void button2_Click(object sender, EventArgs e)
        {
            string choosen = listBox1.SelectedItem.ToString();
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            radioButton3.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            button1.Visible = false;
            checkedListBox1.Visible = false;
            listBox1.Visible = false;
            label3.Visible = false;
            button2.Visible = false;
            richTextBox1.Visible = true;

            listView1.Visible = true;
            ListViewItem item;
            listView1.Visible = true;
            listView1.Clear();
            listView1.View = View.Details;
            listView1.Columns.Add("Name");
            listView1.Columns.Add("Amount");
            listView1.Columns.Add("Measurement");

            if (menuList == " Entrees ")
            {
                menuList2 = "EntreeIngredients";
                menuList3 = "entreeName";
            }
            if (menuList == " SideDishes ")
            {
                menuList2 = "SideDishIngredients";
                menuList3 = "sideDishName";
            }
            if (menuList == " Deserts ")
            {
                menuList2 = "DesertIngredients";
                menuList3 = "desertName";
            }

            string cmdText = "SET TRANSACTION READ ONLY;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            sql = "SELECT recipe FROM" + menuList + "WHERE name = \'" + choosen + "\';";
            
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                inBox = (rdr[0].ToString());
            }
            rdr.Close();
            richTextBox1.Text = inBox;

            sql = "SELECT name, amount, units FROM " + menuList2 + " WHERE " + menuList3 + " = \'" + choosen + "\';";
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                item = new ListViewItem();
                item.Text = rdr[0].ToString();
                item.SubItems.Add(rdr[1].ToString());
                item.SubItems.Add(rdr[2].ToString());

                listView1.Items.Add(item);
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
                button2.Enabled = true;
        }

    }
}
