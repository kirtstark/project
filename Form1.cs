using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;




namespace Groceries
{
    
    public partial class Form1 : Form
    {
        
        static string MyConnection = "Server=localhost;Database=Meals;Uid=root;Pwd=Brazen123;";
        MySqlConnection conn = new MySqlConnection(MyConnection);
        MySqlDataReader rdr;
        MySqlCommand cmd;
        string cmdText;
        int temp;
        int temp2;
        int temp3;
        int iter;
        int sum;
        int randomNumber;
        int adjustment;
        string theWeekDay;
        Menu dayMeal;
        ListViewItem item;
        Random random = new Random();
        
        
        List<Menu> menu = new List<Menu>();
        List<DishIngredients> dishIngredients = new List<DishIngredients>();
        List<Menu> thisWeek = new List<Menu>();

        public Form1()
        {
            InitializeComponent();

            
            listView1.View = View.Details;

            listView1.Columns.Add("Name");
            listView1.Columns.Add("Meal");
            listView1.Columns.Add("Category");
            listView1.Columns.Add("Day");
            listView1.Columns.Add("Day Num");
            
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("We cannot connect to the database", ex.Message);
                return;
            }
            Updates.updateSource(conn);
            Updates.updateDays(conn);
            Updates.updateCuisine(conn);
            Updates.updateMeal(conn);
            Updates.updateCategory(conn);
            Updates.updateSeason(conn);
            Updates.updateUnits(conn);
            Updates.updateIngredients(conn);
            
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (conn != null)
            {
                try
             {
                conn.Close();
             }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }


        }

        // creates the weekly menu and shopping list and saves it in mysql
        private void button1_Click(object sender, EventArgs e)
        {
            //Find the date text for each weekday, from Monday through Sunday, for the upcoming week
            string[] weekDay = new string[7];
            bool enufBreakfast = true;
            bool enufOther = true;
            List<string> breakfastName = new List<string>();
            List<int> breakfastPop = new List<int>();
            List<string> DinnerName = new List<string>();
            List<int> DinnerPop = new List<int>();
            List<string> LunchName = new List<string>();
            List<int> LunchPop = new List<int>();
            List<string> AnyName = new List<string>();
            List<int> AnyPop = new List<int>();
            List<string> LunchDinnerName = new List<string>();
            List<int> LunchDinnerPop = new List<int>();
            List<string> sideDishes = new List<string>();
            List<string> deserts = new List<string>();
            thisWeek.Clear();
            int theDay = 0;
            cmdText = "SELECT WEEKDAY (CURDATE());";
            
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Int32.TryParse(rdr[0].ToString(), out theDay);                
            }
            rdr.Close();
            
            theDay = 7 - theDay;
            if (theDay == 7)
                theDay = 0;
            adjustment = theDay;

            for (int i = 0; i < 7; i++)
            {
                cmdText = "SELECT DATE_ADD(CURDATE(), INTERVAL " + theDay.ToString() + " DAY);";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    weekDay[i] = rdr[0].ToString();
                }
                rdr.Close();
                theDay++;
            }
            theDay = theDay - 7;
            // Find an interval of time where there are enough breakfasts
            int interval = 31 + theDay;
            int counter = 0;
            int numberOfMeals = 0;

            do
            {
                interval = interval - counter * 7;
                cmdText = "SELECT COUNT(name) FROM ENTREES WHERE meal = 'Breakfast' AND lastHad < DATE_SUB(CURDATE(), INTERVAL " + interval.ToString() + " DAY);";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Int32.TryParse(rdr[0].ToString(), out numberOfMeals);
                }
                rdr.Close();
                if (interval < 7)
                {
                    enufBreakfast = false;
                    interval = 7;
                    break;
                }
                counter++;
            } while (numberOfMeals < 10);

            //load in the breakfast information
            cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Breakfast' AND lastHad < DATE_SUB(CURDATE(), INTERVAL " + interval.ToString() + " DAY);";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                breakfastName.Add(rdr[0].ToString());
                Int32.TryParse(rdr[1].ToString(), out temp);
                breakfastPop.Add(temp);
            }
            rdr.Close();

            if (breakfastName.Count < 10)
            {
                cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Breakfast';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    breakfastName.Add(rdr[0].ToString());
                    Int32.TryParse(rdr[1].ToString(), out temp);
                    breakfastPop.Add(temp);
                }
                rdr.Close();
            }

            // Find an interval of time where there are enough dinners and lunches
            interval = 31 + theDay;
            counter = 0;
            numberOfMeals = 0;
            do
            {
                interval = interval - counter * 7;
                cmdText = "SELECT COUNT(name) FROM ENTREES WHERE lastHad < DATE_SUB(CURDATE(), INTERVAL " + interval.ToString() + " DAY) AND meal != 'Breakfast' AND meal != 'Snack';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Int32.TryParse(rdr[0].ToString(), out numberOfMeals);
                }
                rdr.Close();
                if (interval < 7)
                {
                    enufOther = false;
                    interval = 7;
                    break;
                }
                counter++;
            } while (numberOfMeals < 20);

            //load in the dinner information
            cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Dinner' AND lastHad < DATE_SUB(CURDATE(), INTERVAL " + interval.ToString() + " DAY);";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();
            
            while (rdr.Read())
            {
                DinnerName.Add(rdr[0].ToString());
                Int32.TryParse(rdr[1].ToString(), out temp);
                DinnerPop.Add(temp);
            }
            rdr.Close();

            if (DinnerName.Count < 10)
            {
                cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Dinner';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    DinnerName.Add(rdr[0].ToString());
                    Int32.TryParse(rdr[1].ToString(), out temp);
                    DinnerPop.Add(temp);
                }
                rdr.Close();
            }

            //load in the Lunch information
            cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Lunch' AND lastHad < DATE_SUB(CURDATE(), INTERVAL " + interval.ToString() + " DAY);";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                LunchName.Add(rdr[0].ToString());
                Int32.TryParse(rdr[1].ToString(), out temp);
                LunchPop.Add(temp);
            }
            rdr.Close();
            if (LunchName.Count < 10)
            {
                cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Lunch';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    LunchName.Add(rdr[0].ToString());
                    Int32.TryParse(rdr[1].ToString(), out temp);
                    LunchPop.Add(temp);
                }
                rdr.Close();
            }

            //load in the lunch or dinner information
            cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Lunch or Dinner' AND lastHad < DATE_SUB(CURDATE(), INTERVAL " + interval.ToString() + " DAY);";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                LunchDinnerName.Add(rdr[0].ToString());
                Int32.TryParse(rdr[1].ToString(), out temp);
                LunchDinnerPop.Add(temp);
            }
            rdr.Close();
            if (LunchDinnerName.Count < 10)
            {
                cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Lunch or Dinner';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    LunchDinnerName.Add(rdr[0].ToString());
                    Int32.TryParse(rdr[1].ToString(), out temp);
                    LunchDinnerPop.Add(temp);
                }
                rdr.Close();
            }

            //load in the Any information
            cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Any' AND lastHad < DATE_SUB(CURDATE(), INTERVAL " + interval.ToString() + " DAY);";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                AnyName.Add(rdr[0].ToString());
                Int32.TryParse(rdr[1].ToString(), out temp);
                AnyPop.Add(temp);
            }
            rdr.Close();
            if (AnyName.Count < 10)
            {
                cmdText = cmdText = "SELECT name, popularity FROM ENTREES WHERE meal = 'Any';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AnyName.Add(rdr[0].ToString());
                    Int32.TryParse(rdr[1].ToString(), out temp);
                    AnyPop.Add(temp);
                }
                rdr.Close();

            }
            for (int i = 0; i < 7; i++)
            {
                temp = 0;
                temp2 = 0;
                temp3 = 0;
                iter = 0;
                sum = 0;
                switch (i)
                {
                    case 0: theWeekDay = "Monday";
                        break;
                    case 1: theWeekDay = "Tuesday";
                        break;
                    case 2: theWeekDay = "Wednesday";
                        break;
                    case 3: theWeekDay = "Thursday";
                        break;
                    case 4: theWeekDay = "Friday";
                        break;
                    case 5: theWeekDay = "Saturday";
                        break;
                    case 6: theWeekDay = "Sunday";
                        break;
                    default: theWeekDay = "";
                        break;

                }
                
                foreach (int pop in breakfastPop)
                {
                    temp += pop;
                    temp2 += pop;
                }
                foreach (int pop in AnyPop)
                {
                    temp2 += pop;
                }
                randomNumber = random.Next(0, temp2);
                if (randomNumber >= temp)
                {
                    sum = temp;
                    for (int j = 0; j < AnyName.Count; j++)
                    {                        
                        sum += AnyPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }
                    dayMeal = new Menu(AnyName[iter], "Breakfast", "Entrees", theWeekDay, i);
                    
                    if (enufBreakfast)
                    {
                        AnyName.RemoveAt(iter);
                        AnyPop.RemoveAt(iter);
                    }
                    
                }
                else
                {
                    sum = 0;
                    for (int j = 0; j < breakfastName.Count; j++)
                    {
                        sum += breakfastPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }
                    dayMeal = new Menu(breakfastName[iter], "Breakfast", "Entrees", theWeekDay, i);
                    if (enufBreakfast)
                    {
                        breakfastName.RemoveAt(iter);
                        breakfastPop.RemoveAt(iter);
                    }
                }

                thisWeek.Add(dayMeal);
                
                // choose some side dishes for breakfast
                sideDishes.Clear();
                cmdText = cmdText = "SELECT name FROM SideDishes WHERE meal = 'Breakfast';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    sideDishes.Add(rdr[0].ToString());
                }
                rdr.Close();
                randomNumber = random.Next(0, sideDishes.Count);

                dayMeal = new Menu(sideDishes[randomNumber], "Breakfast", "SideDishes", theWeekDay, i);
                thisWeek.Add(dayMeal);

                temp = 0;
                temp2 = 0;
                temp3 = 0;
                iter = 0;
                sum = 0;
                foreach (int pop in LunchPop)
                {
                    temp += pop;
                    temp2 += pop;
                    temp3 += pop;
                }
                foreach (int pop in AnyPop)
                {
                    temp2 += pop;
                    temp3 += pop;
                }
                foreach (int pop in LunchDinnerPop)
                {
                    temp3 += pop;
                }
                randomNumber = random.Next(0, temp3);
                if (randomNumber >= temp2)
                {
                    sum = temp2;
                    for (int j = 0; j < LunchDinnerName.Count; j++)
                    {
                        sum += LunchDinnerPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }

                    dayMeal = new Menu(LunchDinnerName[iter], "Lunch", "Entrees", theWeekDay, i);
                    if (enufOther)
                    {
                        LunchDinnerPop.RemoveAt(iter);
                        LunchDinnerName.RemoveAt(iter);
                    }
                }
                else if (randomNumber >= temp)
                {
                    sum = temp;
                    for (int j = 0; j < AnyName.Count; j++)
                    {
                        sum += AnyPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }
                    dayMeal = new Menu(AnyName[iter], "Lunch", "Entrees", theWeekDay, i);
                    if (enufOther)
                    {
                        AnyPop.RemoveAt(iter);
                        AnyName.RemoveAt(iter);
                    }

                }
                else
                {
                    sum = 0;
                    for (int j = 0; j < LunchName.Count; j++)
                    {
                        sum += LunchPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }
                    dayMeal = new Menu(LunchName[iter], "Lunch", "Entrees", theWeekDay, i);
                    if (enufOther)
                    {
                        LunchPop.RemoveAt(iter);
                        LunchName.RemoveAt(iter);
                    }
                }

                thisWeek.Add(dayMeal);

                // choose some side dishes for Lunch
                sideDishes.Clear();
                cmdText = cmdText = "SELECT name FROM SideDishes WHERE meal = 'Lunch' OR meal = 'Any' OR meal = 'Lunch or Dinner';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    sideDishes.Add(rdr[0].ToString());
                }
                rdr.Close();
                randomNumber = random.Next(0, sideDishes.Count);

                dayMeal = new Menu(sideDishes[randomNumber], "Lunch", "SideDishes", theWeekDay, i);
                thisWeek.Add(dayMeal);

                temp = 0;
                temp2 = 0;
                temp3 = 0;
                iter = 0;
                sum = 0;
                foreach (int pop in DinnerPop)
                {
                    temp += pop;
                    temp2 += pop;
                    temp3 += pop;
                }
                foreach (int pop in AnyPop)
                {
                    temp2 += pop;
                    temp3 += pop;
                }
                foreach (int pop in LunchDinnerPop)
                {
                    temp3 += pop;
                }
                randomNumber = random.Next(0, temp3);
                if (randomNumber >= temp2)
                {
                    sum = temp2;
                    for (int j = 0; j < LunchDinnerName.Count; j++)
                    {
                        sum += LunchDinnerPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }
                    dayMeal = new Menu(LunchDinnerName[iter], "Dinner", "Entrees", theWeekDay, i);
                    if (enufOther)
                    {
                        LunchDinnerPop.RemoveAt(iter);
                        LunchDinnerName.RemoveAt(iter);
                    }
                }
                else if (randomNumber >= temp)
                {
                    sum = temp;
                    for (int j = 0; j < AnyName.Count; j++)
                    {
                        sum += AnyPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }
                    dayMeal = new Menu(AnyName[iter], "Dinner", "Entrees", theWeekDay, i);
                    if (enufOther)
                    {
                        AnyPop.RemoveAt(iter);
                        AnyName.RemoveAt(iter);
                    }

                }
                else
                {
                    sum = 0;
                    for (int j = 0; j < DinnerName.Count; j++)
                    {
                        sum += DinnerPop[j];
                        if (sum > randomNumber)
                            break;
                        iter = j;
                    }
                    dayMeal = new Menu(DinnerName[iter], "Dinner", "Entrees", theWeekDay, i);
                    if (enufOther)
                    {
                        DinnerPop.RemoveAt(iter);
                        DinnerName.RemoveAt(iter);
                    }
                }

                thisWeek.Add(dayMeal);

                // choose some side dishes for Dinner
                sideDishes.Clear();
                cmdText = cmdText = "SELECT name FROM SideDishes WHERE meal = 'Dinner' OR meal = 'Any' OR meal = 'Lunch or Dinner';";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    sideDishes.Add(rdr[0].ToString());
                }
                rdr.Close();
                randomNumber = random.Next(0, sideDishes.Count);

                dayMeal = new Menu(sideDishes[randomNumber], "Dinner", "SideDishes", theWeekDay, i);
                thisWeek.Add(dayMeal);

                // choose some deserts for Dinner
                deserts.Clear();
                cmdText = cmdText = "SELECT name FROM Deserts;";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    deserts.Add(rdr[0].ToString());
                }
                rdr.Close();
                randomNumber = random.Next(0, deserts.Count);

                dayMeal = new Menu(deserts[randomNumber], "Dinner", "Deserts", theWeekDay, i);
                thisWeek.Add(dayMeal);
            }

            listView1.Clear();
            listView1.View = View.Details;

            listView1.Columns.Add("Name");
            listView1.Columns.Add("Meal");
            listView1.Columns.Add("Category");
            listView1.Columns.Add("Day");
            listView1.Columns.Add("Day Num");
            listView1.Visible = true;

            foreach (Menu food in thisWeek)
            {
                item = new ListViewItem();
                item.Text = food.item;
                item.SubItems.Add(food.meal);
                item.SubItems.Add(food.category);
                item.SubItems.Add(food.weekday);
                item.SubItems.Add(food.dayNum.ToString());

                listView1.Items.Add(item);
            }
            button11.Visible = true;
            
            MessageBox.Show("Menu is complete and ready for use");
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // add new store
        private void button7_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3(conn);
            f3.ShowDialog();
        }

        //Add a new ingredient
        private void button5_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(conn);
            f2.ShowDialog();
        }

        // add a new cuisine
        private void button8_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4(conn);
            f4.ShowDialog();
        }

        // add a new ingredient category
        private void button9_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5(conn);
            f5.ShowDialog();
        }

        // get a recipe
        private void button6_Click(object sender, EventArgs e)
        {
            Form6 f6 = new Form6(conn);
            f6.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Add a new dish
        private void button4_Click(object sender, EventArgs e)
        {
            Form7 f7 = new Form7(conn);
            f7.ShowDialog();
        }

        // Select to accept and store the produced menu
        private void button11_Click(object sender, EventArgs e)
        {
            if (thisWeek.Count == 0)
            {
                MessageBox.Show("There is nothing in the menu right now");
                button11.Visible = false;
                return;
            }

            cmdText = "SET TRANSACTION READ WRITE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            cmdText = "DELETE FROM Menu;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            foreach (Menu food in thisWeek)
            {
                cmdText = "INSERT INTO Menu VALUES (\'" + food.item + "\', \'" + food.category + "\', \'" + food.meal + "\', \'" + food.weekday + "\', " + food.dayNum + ");";
                cmd = new MySqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();

                temp = food.dayNum + adjustment;
                cmdText = "UPDATE " + food.category + " SET lastHad = ADDDATE(CURDATE(), " + temp.ToString() + ") WHERE name = \'" + food.item + "\';";
                cmd = new MySqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
            }

            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            MessageBox.Show("The menu has been updated");
            button11.Visible = false;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int totalItems = 0;

            cmdText = "SET TRANSACTION READ ONLY;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            cmdText = "SELECT COUNT(item) as total FROM Menu;";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();
            rdr.Read();            
            totalItems = rdr.GetInt32("total");
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            if (totalItems < 1)
            {
                MessageBox.Show("The menu is empty.");
                return;
            }

            cmdText = "SET TRANSACTION READ ONLY;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
                cmdText = "SELECT * FROM Menu ORDER BY dayNum, meal;";
                cmd = new MySqlCommand(cmdText, conn);
                rdr = cmd.ExecuteReader();
                listView1.Clear();
                listView1.View = View.Details;

                listView1.Columns.Add("Name");
                listView1.Columns.Add("Meal");
                listView1.Columns.Add("Category");
                listView1.Columns.Add("Day");
                listView1.Columns.Add("Day Num");
                listView1.Visible = true;

                while (rdr.Read())
                {
                    item = new ListViewItem();
                    item.Text = rdr.GetString("item");
                    item.SubItems.Add(rdr.GetString("meal"));
                    item.SubItems.Add(rdr.GetString("category"));
                    item.SubItems.Add(rdr.GetString("weekDay"));
                    item.SubItems.Add(rdr.GetString("dayNum"));

                    listView1.Items.Add(item);
                }
                rdr.Close();
                cmdText = "COMMIT;";
                cmd = new MySqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
        }

        // This button retrieves the shopping list from the database
        private void button2_Click(object sender, EventArgs e)
        {
            int totalItems = 0;
            cmdText = "SET TRANSACTION READ ONLY;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SELECT COUNT(name) as total FROM ShoppingList;";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            totalItems = rdr.GetInt32("total");
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            if (totalItems < 1)
            {
                MessageBox.Show("The shopping list is empty.");
                return;
            }

            cmdText = "SET TRANSACTION READ WRITE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

            cmdText = cmdText + "CALL ShoppingList();";
            cmd = new MySqlCommand(cmdText, conn);
            rdr = cmd.ExecuteReader();
            listView1.Clear();
            listView1.View = View.Details;

            listView1.Columns.Add("Name");
            listView1.Columns.Add("Source");
            listView1.Columns.Add("Isle");
            listView1.Visible = true;

            while (rdr.Read())
            {
                item = new ListViewItem();
                item.Text = rdr.GetString("name");
                item.SubItems.Add(rdr.GetString("source"));
                item.SubItems.Add(rdr.GetString("isle"));
                
                listView1.Items.Add(item);
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }
        
    }

    public class Updates
    {
        private static ObservableCollection<string> stores = new ObservableCollection<string>();
        private static ObservableCollection<string> days = new ObservableCollection<string>();
        private static ObservableCollection<string> cuisines = new ObservableCollection<string>();
        private static ObservableCollection<string> meals = new ObservableCollection<string>();
        private static ObservableCollection<string> categories = new ObservableCollection<string>();
        private static ObservableCollection<string> seasons = new ObservableCollection<string>();
        private static ObservableCollection<string> units = new ObservableCollection<string>();
        private static ObservableCollection<Ingredient> ingredients = new ObservableCollection<Ingredient>();
        private static ObservableCollection<string> ingredientsList = new ObservableCollection<string>();

        public static ObservableCollection<string> getStores()
        {
            return stores;
        }

        public static ObservableCollection<string> getDays()
        {
            return days;
        }

        public static ObservableCollection<string> getCuisines()
        {
            return cuisines;
        }

        public static ObservableCollection<string> getMeals()
        {
            return meals;
        }

        public static ObservableCollection<string> getCatagories()
        {
            return categories;
        }

        public static ObservableCollection<string> getSeasons()
        {
            return seasons;
        }

        public static ObservableCollection<string> getUnits()
        {
            return units;
        }

        public static ObservableCollection<Ingredient> getIngredients()
        {
            return ingredients;
        }

        public static ObservableCollection<string> getIngredientsList()
        {
            return ingredientsList;
        }

        public static void updateSource(MySqlConnection conn)
        {
            MySqlDataReader rdr;
            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Source;";

            cmd = new MySqlCommand(cmdText2, conn);
            rdr = cmd.ExecuteReader();
            stores.Clear();
            while (rdr.Read())
            {
                stores.Add(rdr[0].ToString());
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }

        public static void updateDays(MySqlConnection conn)
        {
            MySqlDataReader rdr;
            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Day;";

            cmd = new MySqlCommand(cmdText2, conn);
            rdr = cmd.ExecuteReader();
            days.Clear();
            while (rdr.Read())
            {
                days.Add(rdr[0].ToString());
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }

        public static void updateCuisine(MySqlConnection conn)
        {
            MySqlDataReader rdr;

            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Cuisine;";

            cmd = new MySqlCommand(cmdText2, conn);
            rdr = cmd.ExecuteReader();
            cuisines.Clear();
            while (rdr.Read())
            {
                cuisines.Add(rdr[0].ToString());
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }
        public static void updateMeal(MySqlConnection conn)
        {
            MySqlDataReader rdr;

            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Meal;";

            cmd = new MySqlCommand(cmdText2, conn);
            rdr = cmd.ExecuteReader();
            meals.Clear();
            while (rdr.Read())
            {
                meals.Add(rdr[0].ToString());
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }
        public static void updateCategory(MySqlConnection conn)
        {
            MySqlDataReader rdr;

            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Category;";

            cmd = new MySqlCommand(cmdText2, conn);
            rdr = cmd.ExecuteReader();
            categories.Clear();
            while (rdr.Read())
            {
                categories.Add(rdr[0].ToString());
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }
        public static void updateSeason(MySqlConnection conn)
        {
            MySqlDataReader rdr;

            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Season;";

            cmd = new MySqlCommand(cmdText2, conn);
            rdr = cmd.ExecuteReader();
            seasons.Clear();
            while (rdr.Read())
            {
                seasons.Add(rdr[0].ToString());
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();

        }
        public static void updateUnits(MySqlConnection conn)
        {
            MySqlDataReader rdr;

            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Units;";

            cmd = new MySqlCommand(cmdText2, conn);
            rdr = cmd.ExecuteReader();
            units.Clear();
            while (rdr.Read())
            {
                units.Add(rdr[0].ToString());
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }
        public static void updateIngredients(MySqlConnection conn)
        {
            string name = null;
            string category = null;
            string source = null;
            string staple = null;
            string isle = null;
            bool bStaple = false;
            int iIsle = 0;
            MySqlDataReader rdr;
            

            string cmdText = "SET TRANSACTION READ ONLY;";
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            cmdText = "START TRANSACTION;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
            string cmdText2 = "SELECT * FROM Ingredients;";
            cmd = new MySqlCommand(cmdText2, conn);
            
            rdr = cmd.ExecuteReader();
            ingredients.Clear();
            while (rdr.Read())
            {
                name = (rdr[0].ToString());
                category = (rdr[1].ToString());
                source = (rdr[2].ToString());
                staple = (rdr[3].ToString());
                isle = (rdr[4].ToString());

                if ((staple == "true") || (staple == "TRUE") || (staple == "True"))
                    bStaple = true;
                else
                    bStaple = false;
                if (!Int32.TryParse(isle, out iIsle))
                    iIsle = 0;
                ingredients.Add(new Ingredient(name, category, source, bStaple, iIsle));
                ingredientsList.Add(name);
            }
            rdr.Close();
            cmdText = "COMMIT;";
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }

    }

    public class Ingredient
    {
        public string name { get; private set; }
        public string category { get; private set; }
        public string source { get; private set; }
        public bool staple { get; set; }
        public int isle { get; set; }

        public Ingredient(string name, string category, string source, bool staple, int isle)
        {
            if (Updates.getIngredients().Count > 0)
            {
                setName(name);
            }
            else
            {
                this.name = name;
            }

            setCategory(category);
            setSource(source);
            this.staple = staple;
            this.isle = isle;
        }
        public void setCategory(string category)
        {
            bool check = false;
            foreach (string cat in Updates.getCatagories())
            {
                if (category == cat)
                {
                    check = true;
                }
            }
            if (check)
                this.category = category;
            else
                this.category = null;
        }
        public void setName(string name)
        {
            bool check = true;
            foreach (Ingredient ing in Updates.getIngredients())
            {
                if (name == ing.name)
                {
                    check = false;
                }
            }
            if (check)
            {
                this.name = name;
            }
            else
                this.name = null;
        }
        public void setSource(string source)
        {
            bool check = false;
            foreach (string store in Updates.getStores())
            {
                if (source == store)
                {
                    check = true;
                }
            }
            if (check)
                this.source = source;
            else
                this.source = null;
        }
    }

    public class Menu
    {
        public string item { get; private set; }
        public string meal { get; private set; }
        public string category { get; private set; }
        public string weekday { get; private set; }
        public int dayNum { get; private set; }

        public Menu(string item, string meal, string category, string weekday, int dayNum)
        {
            this.item = item;
            this.meal = meal;
            this.category = category;
            this.weekday = weekday;
            this.dayNum = dayNum;
        }

    }

    public class Dish
    {
        public string name { get; set; }
        public string cuisine { get; private set; }
        public string season { get; private set; }
        public string lastHad { get; private set; }
        public string recipe { get; set; }
        public int popularity { get; set; }
        public string meal { get; private set; }
    }

    public class DishIngredients
    {
        public string name { get; private set; }
        public double amount { get; set; }
        public string units { get; private set; }
        public string dishName { get; private set; }
    }


}
