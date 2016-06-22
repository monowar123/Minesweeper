using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Media;
using System.Diagnostics;
using System.Threading;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private Button[,] myButton;
        private Label[,] myLabel;
        private Stopwatch time = new Stopwatch();
        private int mineNo = 10;

        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SoundPlayer player = new SoundPlayer(Properties.Resources.start);
            player.Play();

            myButton = new Button[9,9];
            myLabel = new Label[9,9];

            panelTime.BackColor = Color.Green;
            panelMine.BackColor = Color.Green;

            //initializa button and label
            for (int j = 0, x = 0, y = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    myButton[i,j] = new Button();
                    myButton[i,j].Width = 40;
                    myButton[i,j].Height = 40;
                    myButton[i,j].BackColor = Color.Green;
                    myButton[i, j].AutoSize = true;

                    myButton[i,j].Click += new EventHandler(myButton_Click);
                    myButton[i, j].MouseDown += new MouseEventHandler(myButton_MouseDown);

                    myLabel[i,j] = new Label();
                    myLabel[i,j].Width = 37;
                    myLabel[i,j].Height = 37;
                    myLabel[i,j].Font = new Font("Verdana", 18);
                    myLabel[i,j].Font = new Font(myLabel[i,j].Font, FontStyle.Bold);
                    myLabel[i,j].Text = "0";
                    myLabel[i,j].BackColor = Color.Honeydew;

                    myLabel[i,j].Click += new EventHandler(myLabel_Click);
                    //myLabel[i,j].Visible = false;

                    myButton[i,j].Location = new Point(x, y);
                    myLabel[i, j].Location = new Point(x + 2, y + 2);

                    x = x + 40;
                    if (x >= 360)
                    {
                        x = 0;
                        y = y + 40;
                    }

                    
                    panel1.Controls.Add(myButton[i,j]);
                    panel1.Controls.Add(myLabel[i,j]);
                }
                Thread.Sleep(10);
            }


            //Assign the mine
            Random random = new Random();
            int[] mine = new int[10];
            int temp;
            for (int i = 0; i < 10; i++)
            {
            aa:
                temp = random.Next(0, 80);
                if (mine.Contains(temp))   //to remove duplicacy
                {
                    goto aa;
                }
                mine[i] = temp;
            }

            for (int i = 0; i < 10; i++)
            {
                int a = mine[i] / 9;
                int b = mine[i] % 9;
                myLabel[a, b].Text = "";
                myLabel[a, b].Image = Properties.Resources.mine1;
            }


            //assign the mine number surrunded it
            int count;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (myLabel[i, j].Text == "0")
                    {
                        count = 0;
                        for (int m = i - 1; m < i + 2; m++)
                        {
                            for (int n = j - 1; n < j + 2; n++)
                            {
                                if (m < 0 || m > 8 || n < 0 || n > 8)
                                    continue;
                                if (myLabel[m, n].Image != null)
                                {
                                    count++;
                                }
                            }
                        }
                        if (count > 0)
                            myLabel[i, j].Text = count.ToString();
                        else
                            myLabel[i, j].Text = "";
                    }
                }
            }
       
        }

        private void myButton_Click(object sender, EventArgs e)
        {
            timer1.Start();
            time.Start();   //start the stopwatch to measure the time

            List<myClass> myList = new List<myClass>();

            bool[,] visited = new bool[9, 9];

            if (((Button)sender).Image == null)
            {
                ((Button)sender).Visible = false;           // this is for the label that's text is not null 
            }
            int x = ((Button)sender).Location.X / 40;   //get the position of the clicked button
            int y = ((Button)sender).Location.Y / 40;

            if (myLabel[x, y].Text == "" && myLabel[x, y].Image == null)  //for blanck label
            {

                SoundPlayer player = new SoundPlayer(Properties.Resources.beep_1);
                player.Play();

            repetation:   //iterate between adjacent position
                for (int i = x - 1; i < x + 2; i++)
                {
                    for (int j = y - 1; j < y + 2; j++)
                    {
                        if (i < 0 || i > 8 || j < 0 || j > 8)    //condition for border value
                            continue;
                        if (myButton[i, j].Image == null)
                        {
                            myButton[i, j].Visible = false;
                        }
                        if (myLabel[i, j].Text == "" && myLabel[i, j].Image == null)
                        {
                            if (visited[i, j] == false)  //if the position is not visited then the position
                            {                            //will be added into the list for furtur calculation                   
                                myList.Add(new myClass(i, j));
                                visited[i, j] = true;
                            }
                        }
                        
                    }
                }
                if (myList.Count != 0)   //iterate until the list is empty
                {
                    x = myList[0].xPosition;
                    y = myList[0].yPosition;                    
                    myList.RemoveAt(0);
                    goto repetation;
                }
            }
            else if (myLabel[x, y].Image != null)
            {
                myLabel[x, y].Image = Properties.Resources.mine2;
            }
        }

        private void myButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (((Button)sender).Image == null)
                {
                    ((Button)sender).Image = Properties.Resources.flag;
                    mineNo--;
                }
                else
                {
                    ((Button)sender).Image = null;
                    mineNo++;
                }
                lblMine.Text = string.Format("{0:00}", mineNo);
            }
        }

        private void myLabel_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("hello");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i <= 360; i += 40)
            {
                e.Graphics.FillRectangle(Brushes.YellowGreen, 2, i, 360, 2);
                e.Graphics.FillRectangle(Brushes.YellowGreen, i, 2, 2, 360);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = string.Format("{0:000}", time.Elapsed.TotalSeconds);
            if (this.WindowState == FormWindowState.Minimized)
            {
                time.Stop();
            }
            else
            {
                time.Start();
            }
        }

    }
}
