namespace Migration_Desktop_App
{
    public partial class Index : Form
    {
        Migration migrate = new Migration();
        CreateAndPostAzureIteration iteration;


        public Index()
        {
            InitializeComponent();
        }


        private void lastIssue_Click(object sender, EventArgs e)
        {
            sendQueryToMigrate("last_issue");
        }

        private void last10Issues_Click(object sender, EventArgs e)
        {
            sendQueryToMigrate("last_10_issues");
        }

        private void issuesFrom2022_Click(object sender, EventArgs e)
        {
            sendQueryToMigrate("issues_2022");
        }
        private void issuesFrom2021_Click(object sender, EventArgs e)
        {
            sendQueryToMigrate("issues_2021");
        }
        private void issuesFrom2020_Click(object sender, EventArgs e)
        {
            sendQueryToMigrate("issues_2020");
        }
        private void issuesFrom2019_Click(object sender, EventArgs e)
        {
            sendQueryToMigrate("mobility_sprint");
        }
        private void Iterations_Click(object sender, EventArgs e)
        {
            iteration = new CreateAndPostAzureIteration();
            iteration.migrateIteration(Result);
        }

        private void sendQueryToMigrate(string query)
        {
            // first we check the project 
            var confirmResult = MessageBox.Show($"Are you sure to migrate those items ?", "yes or no ?",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                Result.Text += "Migration Starting..." + Environment.NewLine;
                ProgressBar pBar = new ProgressBar();
                pBar.Name = "progressBar1";
                pBar.Width = 200;
                pBar.Height = 30;
                pBar.ForeColor = Color.FromArgb(112, 191, 155);
                Controls.Add(pBar);

                pBar.Dock = DockStyle.Bottom;
                pBar.Minimum = 0;
                migrate.launchMigration(query, Result, counter, pBar);
                pBar.Hide();
            }

        }
        private void Result_TextChanged(object sender, EventArgs e)
        {
            Result.SelectionStart = Result.Text.Length;
            Result.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetJsonWithTimeTrackerUsersID list = new GetJsonWithTimeTrackerUsersID();
            list.getRequest();
        }

        private void Index_Load(object sender, EventArgs e)
        {

        }
    }
}