using Npgsql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace beauty_salon
{
    public partial class AdditionalForm : Form
    {
        private MainForm mainForm;
        private const int BaseCashback = 10;
        private const int DiscountCashback = 20;

        public AdditionalForm(MainForm mainForm)
        {
            InitializeComponent();
            InitializeHeader();
            this.mainForm = mainForm;
            LoadVisitorsWithCashback();
        }

        private void InitializeHeader()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.Pink,
            };

            var promoLabel = new Label
            {
                Text = "Акция! Скидка 20% в будни 12:00-14:00 | Кешбэк 10% на все услуги",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            headerPanel.Controls.Add(promoLabel);
            Controls.Add(headerPanel);
            items_panel.AutoScroll = true;
        }

        private void LoadVisitorsWithCashback()
        {
            const string query = @"
                SELECT 
                    v.visitors_name, 
                    v.phone, 
                    v.bonus_points,
                    EXISTS (
                        SELECT 1 
                        FROM reservations r 
                        WHERE r.visitor_id = v.visitor_id 
                            AND EXTRACT(DOW FROM r.datetime) BETWEEN 1 AND 5 
                            AND r.datetime::time BETWEEN '12:00' AND '14:00'
                    ) as has_discount
                FROM visitors v
                ORDER BY v.bonus_points DESC";

            try
            {
                using var conn = new NpgsqlConnection(Constants.ConnectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                int yPos = 20;
                while (reader.Read())
                {
                    var cashback = reader.GetBoolean(3) ? DiscountCashback : BaseCashback;
                    var card = CreateVisitorCard(
                        reader["visitors_name"].ToString(),
                        reader["phone"].ToString(),
                        Convert.ToInt32(reader["bonus_points"]),
                        cashback,
                        yPos
                    );
                    items_panel.Controls.Add(card);
                    yPos += card.Height + 15;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private Panel CreateVisitorCard(string name, string phone, int points, int cashbackPercent, int top)
        {
            var card = new Panel
            {
                Size = new Size(720, 80),
                Location = new Point(20, top),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            var nameLabel = new Label
            {
                Text = name,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var phoneLabel = new Label
            {
                Text = phone,
                Location = new Point(10, 40),
                AutoSize = true
            };

            var pointsLabel = new Label
            {
                Text = $"Баллы: {points}",
                Location = new Point(500, 10),
                AutoSize = true
            };

            var cashbackLabel = new Label
            {
                Text = $"Кешбэк: {cashbackPercent}%",
                ForeColor = cashbackPercent == DiscountCashback ? Color.Green : Color.DarkBlue,
                Location = new Point(500, 40),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { nameLabel, phoneLabel, pointsLabel, cashbackLabel });
            return card;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            mainForm?.Show();
        }

        private void go_back_button_Click(object sender, EventArgs e) => Close();
    }
}