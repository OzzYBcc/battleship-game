using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Navmaxia
{
    public partial class Form1 : Form
    {
        string connectionString = "Data source=scores.db; Version=3";
        SQLiteConnection connection;

        private const int gridSize = 10;
        private Button[,] playerGrid = new Button[gridSize, gridSize];
        private Button[,] computerGrid = new Button[gridSize, gridSize];
        private Random random = new Random();

        private int[,] computerShips = new int[gridSize, gridSize];
        private int[,] playerShips = new int[gridSize, gridSize];

        private int playerHits = 0;
        private int computerHits = 0;
        private int totalShipParts = 5 + 4 + 3 + 2;
        private int playerClicks = 0;
        private int timerTicks = 0;

        private int wins = 0;
        private int losses = 0;
        private int restarts = 0;

        private Dictionary<int, string> shipNames = new Dictionary<int, string>
        {
            { 5, "Αεροπλανοφόρο" },
            { 4, "Αντιτορπιλικό" },
            { 3, "Πολεμικό" },
            { 2, "Υποβρύχιο" }
        };

        private string playerName;

        public Form1()
        {
            InitializeComponent();
            InitializeGrids();
            PlacePlayerShips();
            PlaceComputerShips();
            timer1.Start();
            playerClicks = 0;
            timerTicks = 0;
            this.FormClosing += Form1_FormClosing;
        }

        private void InitializeGrids()
        {
            CreateGrid(playerGrid, 50, 100, false);
            CreateGrid(computerGrid, 400, 100, true);
        }

        private void CreateGrid(Button[,] grid, int startX, int startY, bool clickable)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(30, 30),
                        Location = new Point(startX + j * 30, startY + i * 30),
                        BackColor = Color.LightGray,
                        Tag = new Point(i, j)
                    };

                    if (clickable)
                        btn.Click += new EventHandler(Attack_Check);

                    this.Controls.Add(btn);
                    grid[i, j] = btn;
                }
            }
        }

        private void PlaceComputerShips()
        {
            PlaceShip(computerShips, playerShips, 5);
            PlaceShip(computerShips, playerShips, 4);
            PlaceShip(computerShips, playerShips, 3);
            PlaceShip(computerShips, playerShips, 2);
        }

        private void PlacePlayerShips()
        {
            PlaceShip(playerShips, computerShips, 5);
            PlaceShip(playerShips, computerShips, 4);
            PlaceShip(playerShips, computerShips, 3);
            PlaceShip(playerShips, computerShips, 2);

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (playerShips[i, j] != 0)
                    {
                        playerGrid[i, j].BackColor = Color.Blue;
                    }
                }
            }
        }

        private bool CanPlaceShip(int[,] shipGrid, int[,] otherGrid, int row, int col, int size, bool horizontal)
        {
            if (horizontal)
            {
                if (col + size > gridSize) return false;
                for (int i = 0; i < size; i++)
                {
                    if (shipGrid[row, col + i] != 0 || otherGrid[row, col + i] != 0)
                        return false;
                }
            }
            else
            {
                if (row + size > gridSize) return false;
                for (int i = 0; i < size; i++)
                {
                    if (shipGrid[row + i, col] != 0 || otherGrid[row + i, col] != 0)
                        return false;
                }
            }
            return true;
        }

        private void PlaceShip(int[,] shipGrid, int[,] otherGrid, int size)
        {
            bool placed = false;
            while (!placed)
            {
                int row = random.Next(gridSize);
                int col = random.Next(gridSize);
                bool horizontal = random.Next(2) == 0;

                if (CanPlaceShip(shipGrid, otherGrid, row, col, size, horizontal))
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (horizontal)
                            shipGrid[row, col + i] = size;
                        else
                            shipGrid[row + i, col] = size;
                    }
                    placed = true;
                }
            }
        }

        private void ComputerAttack()
        {
            bool attacked = false;

            while (!attacked)
            {
                int row = random.Next(gridSize);
                int col = random.Next(gridSize);
                Button btn = playerGrid[row, col];

                if (btn.Enabled)
                {
                    if (playerShips[row, col] > 0)
                    {
                        btn.BackColor = Color.Red;
                        btn.Text = "X";
                        computerHits++;

                        playerShips[row, col] = -playerShips[row, col];

                        if (IsShipSunk(playerShips, Math.Abs(playerShips[row, col])))
                        {
                            MessageBox.Show($"Βυθίστηκε το {shipNames[Math.Abs(playerShips[row, col])]} σου!");
                        }

                        if (computerHits == totalShipParts)
                        {
                            losses++;
                            string message = $"Ο υπολογιστής βύθισε όλα τα πλοία σου!\nΠροσπάθειες: {playerClicks}\nΧρόνος (seconds): {timerTicks}";
                            MessageBox.Show(message);
                            SaveGameResult("Computer", timerTicks);
                            DialogResult result = MessageBox.Show("Νέο παιχνίδι;", "Title", MessageBoxButtons.YesNo);

                            if (result == DialogResult.Yes)
                            {
                                button2.PerformClick();
                            }
                            else
                            {
                                button3.PerformClick();
                            }
                        }
                    }
                    else
                    {
                        btn.BackColor = Color.Green;
                        btn.Text = "-";
                    }

                    btn.Enabled = false;
                    attacked = true;
                }
            }
        }

        private void Attack_Check(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag is Point point)
            {
                playerClicks++;
                int row = point.X;
                int col = point.Y;

                if (computerShips[row, col] > 0)
                {
                    btn.BackColor = Color.Red;
                    btn.Text = "X";
                    playerHits++;

                    computerShips[row, col] = -computerShips[row, col];

                    int shipSize = Math.Abs(computerShips[row, col]);
                    if (IsShipSunk(computerShips, shipSize))
                    {
                        MessageBox.Show($"Βύθισες το {shipNames[shipSize]} του αντιπάλου!");
                    }

                    if (playerHits == totalShipParts)
                    {
                        wins++;
                        string message = $"Συγχαρητήρια! Βύθισες όλα τα πλοία!\nΠροσπάθειες: {playerClicks}\nΧρόνος (seconds): {timerTicks}";
                        MessageBox.Show(message);
                        SaveGameResult(playerName, timerTicks);
                        DialogResult result = MessageBox.Show("Νέο παιχνίδι;", "Title", MessageBoxButtons.YesNo);

                        if (result == DialogResult.Yes)
                        {
                            button2.PerformClick();
                        }
                        else
                        {
                            button3.PerformClick();
                        }
                    }
                }
                else
                {
                    btn.BackColor = Color.Green;
                    btn.Text = "-";
                }

                btn.Enabled = false;
                ComputerAttack();
            }
        }

        private bool IsShipSunk(int[,] shipGrid, int shipSize)
        {
            int count = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (Math.Abs(shipGrid[i, j]) == shipSize)
                    {
                        if (shipGrid[i, j] < 0)
                            count++;
                        else
                            return false;
                    }
                }
            }
            return count == shipSize;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            restarts++;

            string statsMessage = $"Νίκες: {wins}\nΉττες: {losses}";
            MessageBox.Show(statsMessage, "Στατιστικά Παιχνιδιού");

            Form1 newGame = new Form1();
            newGame.wins = this.wins;
            newGame.losses = this.losses;
            newGame.restarts = this.restarts;
            newGame.playerName = this.playerName;

            this.Hide();
            newGame.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (restarts >= 1)
            {
                string message = $"Νίκες: {wins}\nΉττες: {losses}";
                MessageBox.Show(message, "Στατιστικά", MessageBoxButtons.OK);
            }

            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AlignButtons();

            connection = new SQLiteConnection(connectionString);

            InitializeDatabase();

            this.StartPosition = FormStartPosition.CenterScreen;

            int width = 50 + gridSize * 30 + 300 + 50;
            int height = 100 + gridSize * 30 + 50;
            this.Size = new Size(width, height);

            int centerX = (this.ClientSize.Width - width) / 2;
            int centerY = (this.ClientSize.Height - height) / 2;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    playerGrid[i, j].Location = new Point(50 + j * 30 + centerX, 100 + i * 30 + centerY);
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    computerGrid[i, j].Location = new Point(400 + j * 30 + centerX, 100 + i * 30 + centerY);
                }
            }

            if (string.IsNullOrEmpty(playerName))
            {
                playerName = Interaction.InputBox("Εισάγετε το όνομα σας:", "Όνομα");
                if (!string.IsNullOrEmpty(playerName))
                {
                    MessageBox.Show("Όνομα: " + playerName);
                }
                else
                {
                    MessageBox.Show("Πρέπει να εισάγετε ένα όνομα! Κλείνει η εφαρμογή...");
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("Όνομα: " + playerName);
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "CREATE TABLE IF NOT EXISTS scores (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, winner TEXT NOT NULL, time INTEGER NOT NULL)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing database: " + ex.Message);
            }
        }

        private void SaveGameResult(string winner, int timeInSeconds)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO scores (name, winner, time) VALUES (@name, @winner, @time)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", playerName);
                        cmd.Parameters.AddWithValue("@winner", winner);
                        cmd.Parameters.AddWithValue("@time", timeInSeconds);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving game result: " + ex.Message);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timerTicks++;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                DialogResult result = MessageBox.Show("Θέλετε να κλείσετε την εφαρμογή;", "Έξοδος", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        public class HelpButton
        {
            public void ShowMessage(string message)
            {
                MessageBox.Show(message, "Μήνυμα", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public class RulesButton
        {
            public void Redirect(string url)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Σφάλμα κατά το άνοιγμα του συνδέσμου: {ex.Message}", "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AlignButtons()
        {
            Alignment aligner = new Alignment();
            aligner.AlignToRight(this, button6, button4);
        }

        public class Alignment
        {
            public void AlignToRight(Form form, params Control[] controls)
            {
                foreach (Control control in controls)
                {
                    int newX = form.ClientSize.Width - control.Width - 10;
                    control.Location = new System.Drawing.Point(newX, control.Location.Y);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RulesButton redirect = new RulesButton();
            redirect.Redirect("https://www.officialgamerules.org/board-games/battleship");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HelpButton message = new HelpButton();
            message.ShowMessage("Καλύτερα μην παίξεις.");
        }
    }
}