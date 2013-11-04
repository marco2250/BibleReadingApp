using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using BibleReading.Common45.Root.Data.Transactions;

using BibleReading.BS;
using BibleReading.Model;

namespace BibleReading.UI
{
    public partial class FrmMain : Form
    {
        private int BibleWords { get; set; }

        private int BibleVerses { get; set; }

        private DataTable Books { get; set; }

        private DataTable CurrentBook { get; set; }

        private IList<DataRow> Verses { get; set; }
        
        private DataRow CurrentVerse { get; set; }

        private DataRow FirstVerseRead
        {
            get
            {
                return Verses.Count > 0 ? Verses[0] : null;
            }
        }

        private DataRow LastVerseRead
        {
            get
            {
                return Verses.Count > 0 ? Verses[Verses.Count - 1] : null;
            }
        }

        private DataRow User { get; set; }

        private DateTime StartedAt { get; set; }
        private DateTime FinishedAt { get; set; }
        private int TotalSeconds { get; set; }

        private bool IsReading { get; set; }

        private bool IsPaused
        {
            get
            {
                return HasStarted && !IsReading;
            }
        }

        private bool HasStarted { get; set; }

        private int RemainingWords
        {
            get
            {
                return BibleWords - User.Field<int>("ReadWordCount");
            }
        }

        //private Timer _tmrVerseReadingTime;
        private Timer _tmrPlannedReadingTime;

        private int PlannedReadingTime { get; set; }

        readonly CultureInfo _ci = new CultureInfo("pt-BR");

        public FrmMain()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    // left arrow key pressed
                    LoadPreviousVerse();
                    return true;
                case Keys.Right:
                    LoadNextVerse();
                    return true;
                case Keys.Up:
                    // up arrow key pressed
                    return true;
                case Keys.Down:
                    // down arrow key pressed
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Focus();

            Init();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop(true);
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            Application.DoEvents();
        }

        private void Init()
        {
            Verses = new List<DataRow>();
            _ci.NumberFormat.NumberDecimalDigits = 0;
            PlannedReadingTime = int.Parse(ConfigurationManager.AppSettings["PlannedReadingTime"]);

            SetupControls();
            SetupTimer();

            Books = new Book().GetBook();
            BibleWords = Books.AsEnumerable().Sum(p => p.Field<int>("Words"));
            BibleVerses = Books.AsEnumerable().Sum(p => p.Field<int>("Verses"));

            LoadStartInformation();
        }

        private void LoadUser()
        {
            User = new User().GetUserById(1).Rows[0];
        }

        private void SetupControls()
        {
            lblVerse.MinimumSize = new Size(0, 170);
            lblVerse.MaximumSize = new Size((int) (Width*0.5), 170);

            lblPaused.Width = (int) (Width*0.5);
            lblPaused.Height = lblVerse.Height;
            lblPaused.Top = (Height - lblPaused.Height) / 2;
            lblPaused.Left = (Width - lblPaused.Width) / 2;

            AdjustTimeRemainingPosition();

            SetupInitialState();
        }

        private void AdjustVerseLabelsPosition()
        {
            lblVerse.Top = (Height - lblVerse.Height) / 2;
            lblVerse.Left = (Width - lblVerse.Width) / 2;

            lblReference.Top = lblVerse.Top - lblReference.Height - 20;

            lblReference.Left = (Width - lblReference.Width) / 2;
        }

        private void AdjustTimeRemainingPosition()
        {
            lblTimeRemaining.Left = (Width - lblTimeRemaining.Width) / 2;
        }

        private void SetupInitialState()
        {
            lblVerse.Visible = false;
            lblReference.Visible = false;
            lblPaused.Visible = true;

            btnStartPause.Text = @"&START";
            btnStop.Enabled = false;

            lblTimeRemaining.Text = @"00:00:00";
            lblTimeRemaining.ForeColor = Color.Black;
            AdjustTimeRemainingPosition();

            IsReading = false;
            HasStarted = false;
        }
        
        private void SetupVerseLabels()
        {
            if (IsReading)
            {
                lblVerse.Visible = true;
                lblReference.Visible = true;
                lblPaused.Visible = false;
            }
            else
            {
                lblVerse.Visible = false;
                lblReference.Visible = false;
                lblPaused.Visible = true;
            }
        }

        private void LoadInformationAlreadyRead()
        {
            LoadBook((BookEnum)User.Field<int>("BookId"));
            CurrentVerse =
                CurrentBook.AsEnumerable().First(x => x.Field<int>("VerseId") == User.Field<int>("NextVerseId"));
        }

        #region Book Methods

        private void LoadBook(BookEnum book)
        {
            CurrentBook = new Verse().GetVerseByBook(book);
        }

        private void LoadNextBook()
        {
            if ((BookEnum)CurrentBook.AsEnumerable().First().Field<int>("BookId") == BookEnum.Revelation)
            {
                LoadBook(BookEnum.Genesis);
                return;
            }

            LoadBook((BookEnum)CurrentBook.AsEnumerable().First().Field<int>("BookId") + 1);
            SetCurrentVerseAsFirstOne();
            UpdateVerse();
        }

        /*
        private void LoadPreviousBook()
        {
            if ((BookEnum)CurrentBook.AsEnumerable().First().Field<int>("BookId") == BookEnum.Genesis)
            {
                LoadBook(BookEnum.Revelation);
                return;
            }

            LoadBook((BookEnum)CurrentBook.AsEnumerable().First().Field<int>("BookId") - 1);
            SetCurrentVerseAsFirstOne();
            UpdateVerse();
        }
        */

        private void LoadPreviousVerse()
        {
            if (!HasStarted)
                return;

            StopTimer();

            if (CurrentVerse.Field<int>("SequenceInBook") == 1)
                return; // Não vai de Gênesis para Apocalipse

            CurrentVerse = GetPreviousVerse(CurrentVerse);

            UpdateVerse();
        }

        private void LoadNextVerse()
        {
            if (!HasStarted)
                return;

            StopTimer();

            FinishedVerse(CurrentVerse);

            if (CurrentVerse.Field<int>("SequenceInBook") == CurrentBook.AsEnumerable().Max(p => p.Field<int>("SequenceInBook")))
            {
                if ((BookEnum)CurrentVerse.Field<int>("BookId") == BookEnum.Revelation)
                {
                    // Finalizou a leitura. Terminamos.
                    Stop(false);
                    return;
                }

                // Terminou a leitura do livro atual. Vamos para o próximo livro.
                LoadNextBook();
                return;
            }

            // Próximo versículo do livro atual.
            CurrentVerse = GetNextVerse(CurrentVerse);

            UpdateVerse();
        }

        private DataRow GetPreviousVerse(DataRow verse)
        {
            if ((BookEnum)verse.Field<int>("BookId") == BookEnum.Genesis && verse.Field<int>("SequenceInBook") == 1)
                throw new InvalidOperationException();

            return
                CurrentBook.AsEnumerable()
                    .First(
                        x =>
                            x.Field<int>("SequenceInBook") == verse.Field<int>("SequenceInBook") - 1);
        }

        private DataRow GetNextVerse(DataRow verse)
        {
            return
                CurrentBook.AsEnumerable()
                    .FirstOrDefault(
                        x =>
                            x.Field<int>("SequenceInBook") == verse.Field<int>("SequenceInBook") + 1);
        }

        private DataRow GetNextBibleVerse(DataRow verse)
        {
            return
                CurrentBook.AsEnumerable()
                    .FirstOrDefault(
                        x =>
                            x.Field<int>("SequenceInBook") == verse.Field<int>("SequenceInBook") + 1);
        }

        private void UpdateVerse()
        {
            lblReference.Text = string.Format("{0} {1}:{2}"
                , CurrentVerse.Field<string>("Name")
                , CurrentVerse.Field<int>("ChapterNumber")
                , CurrentVerse.Field<int>("VerseNumber"));


            lblVerse.Text = string.Format("{0}", CurrentVerse.Field<string>("Verse"));

            UpdateInterval();
            AdjustVerseLabelsPosition();

            //SystemSounds.Beep.Play();

            StartTimer();
        }

        private void SetCurrentVerseAsFirstOne()
        {
            CurrentVerse = CurrentBook.Rows[0];
        }

        private void FinishedVerse(DataRow verse)
        {
            if (verse.Field<int>("VerseId") < User.Field<int>("NextVerseId"))
                return;

            if (Verses.All(p => p.Field<int>("VerseId") != verse.Field<int>("VerseId")))
                Verses.Add(verse);
        }

        #endregion

        #region Timer / Frequency

        private void SetupTimer()
        {
            //_tmrVerseReadingTime = new Timer();
            //_tmrVerseReadingTime.Tick += _tmrVerseReadingTime_Tick;

            _tmrPlannedReadingTime = new Timer();
            _tmrPlannedReadingTime.Interval = 50;
            _tmrPlannedReadingTime.Tick += _tmrPlannedReadingTime_Tick;
        }

        void _tmrPlannedReadingTime_Tick(object sender, EventArgs e)
        {
            _tmrPlannedReadingTime.Stop();

            Application.DoEvents();

            var totalSeconds = (int)DateTime.Now.Subtract(StartedAt).TotalSeconds + TotalSeconds;

            if (totalSeconds >= PlannedReadingTime*60)
            {
                Stop(true);
                return;
            }

            var timeRemaining = TimeSpan.FromSeconds(PlannedReadingTime * 60 - totalSeconds);
            lblTimeRemaining.Text = GetFormattedTime(timeRemaining);

            if (timeRemaining.TotalSeconds <= 5)
                lblTimeRemaining.ForeColor = Color.Red;

            AdjustTimeRemainingPosition();

           _tmrPlannedReadingTime.Start();
        }

        void _tmrVerseReadingTime_Tick(object sender, EventArgs e)
        {
            //LoadNextVerse();
        }

        private void StartTimer()
        {
            //_tmrVerseReadingTime.Start();
        }

        private void StopTimer()
        {
            //_tmrVerseReadingTime.Stop();
        }

        //private void 

        private void UpdateInterval()
        {
            // Calcula o intervalo baseado na quantidade de palavras por minuto e na quantidade de palavras que o versículo possui
            //_tmrVerseReadingTime.Interval = 60 * CurrentVerse.Field<int>("WordCount") / User.Field<int>("WordsPerMinute") * 1000;
        }

        private int CalculateWordsPerMinute()
        {
            if (TotalSeconds == 0)
                return -1;

            return Verses.Sum(p => p.Field<int>("WordCount"))*60/TotalSeconds;
        }

        private void IncrementTotalSeconds()
        {
            TotalSeconds += (int)FinishedAt.Subtract(StartedAt).TotalSeconds;
        }

        #endregion

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop(true);
        }

        private void btnStartPause_Click(object sender, EventArgs e)
        {
            StartPause();
        }

        private void StartPause()
        {
            if (btnStartPause.Text == @"&START")
            {
                Start();
                btnStartPause.Text = @"&PAUSE";
            }
            else
            {
                Pause();
                btnStartPause.Text = @"&START";
            }
        }

        private void Start()
        {
            if (!HasStarted)
            {
                LoadStartInformation();
            }

            StartedAt = DateTime.Now;
            IsReading = true;
            HasStarted = true;

            SetupVerseLabels();

            btnStop.Enabled = true;

            _tmrPlannedReadingTime.Start();

            UpdateVerse();
        }

        private void Pause()
        {
            StopTimer();
            HandleNotReadingAnymore();

            SetupVerseLabels();
        }

        private void Stop(bool manualStop)
        {
            StopTimer();

            if (!HasStarted)
                return;

            SetupInitialState();

            HandleNotReadingAnymore();

            if (manualStop)
            {
                if (MessageBox.Show(@"Você leu o versículo atual até o fim?", @"Pergunta", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Verses.Add(CurrentVerse);
                }
            }

            if (LastVerseRead == null)
                return;

            var wordsPerMinute = CalculateWordsPerMinute();
            var totalVerses = Verses.Count;
            var totalWords = Verses.AsEnumerable().Sum(p => p.Field<int>("WordCount"));

            try
            {
                using (var scope = TransactionScopeUtility.GetTransactionScope())
                {
                    var nextVerse = GetNextBibleVerse(LastVerseRead);

                    if (nextVerse != null)
                        new User().UpdateNextVerseId(User.Field<int>("UserId"), nextVerse.Field<int>("VerseId"));

                    new Reading().InsertReading(StartedAt,
                        FinishedAt,
                        FirstVerseRead.Field<int>("VerseId"),
                        LastVerseRead.Field<int>("VerseId"),
                        TotalSeconds,
                        wordsPerMinute,
                        totalVerses,
                        totalWords,
                        User.Field<int>("UserId"));

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                // Gravar o XML para carregar para o banco depois
            }

            LoadStartInformation();
        }

        private void LoadStartInformation()
        {
            LoadUser();
            LoadInformationAlreadyRead();

            UpdateStatistics();
        }

        private void HandleNotReadingAnymore()
        {
            FinishedAt = DateTime.Now;

            _tmrPlannedReadingTime.Stop();

            IsReading = false;

            IncrementTotalSeconds();
        }

        private void UpdateStatistics()
        {
            var s = string.Empty;

// ReSharper disable JoinDeclarationAndInitializer
            int wordsRemaining;
            int secondsRemaining;
            TimeSpan timeToFinish;
            //int versesRemaining;
// ReSharper restore JoinDeclarationAndInitializer

            wordsRemaining = BibleWords - User.Field<int>("ReadWordCount");
            secondsRemaining = 60*wordsRemaining/User.Field<int>("WordsPerMinute");
            timeToFinish = TimeSpan.FromSeconds(secondsRemaining);

            s += "Bible\n";
            s += "Words Count: " + GetFormattedNumber(BibleWords) + "\n";
            s += "Words Read: " + GetFormattedNumber(User.Field<int>("ReadWordCount")) + "\n";
            s += "Words Remaining: " + GetFormattedNumber(wordsRemaining) + "\n";
            s += string.Format("Chapters Remaining: \n");
            s += string.Format("Verses Remaining: \n");

            // http://stackoverflow.com/questions/463642/what-is-the-best-way-to-convert-seconds-into-hourminutessecondsmilliseconds
            s += string.Format("ETF: {0}\n", GetFormattedTime(timeToFinish));

            s += "\n";
            s += "Old Testament:\n";

            s += "\n";
            s += "New Testament:\n";

            wordsRemaining =
                Books.AsEnumerable()
                    .First(p => p.Field<int>("BookId") == CurrentVerse.Field<int>("BookId"))
                    .Field<int>("Words") - User.Field<int>("ReadWordCount");

            secondsRemaining = 60 * wordsRemaining / User.Field<int>("WordsPerMinute");
            timeToFinish = TimeSpan.FromSeconds(secondsRemaining);

            s += "\n";
            s += "Current Book:\n";
            s += string.Format("Words Remaining: " + GetFormattedNumber(wordsRemaining)) + "\n";
            s += GetFormattedTime(timeToFinish);

            lblStatistics.Text = s;
        }

        private string GetFormattedTime(TimeSpan timeToFinish)
        {
            return Math.Floor((decimal)timeToFinish.TotalHours) + "h" + timeToFinish.ToString(@"mm\m\:ss\s");
            //s += string.Format("ETF: {0}\n", timeToFinish.ToString(@"dd\d\:hh\h\:mm\m\:ss\s"));
        }

        private string GetFormattedNumber(int number)
        {
            return number.ToString("N", _ci);
        }
    }
}
