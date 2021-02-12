using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace XO
{
    public partial class Form1 : Form
    {
        private Game game;
        private PictureBox[,] pbs = new PictureBox[3, 3];
        private Image Cross;    
        private Image Nought;
        // запуск игры на форме
        public Form1()
        {
            InitializeComponent();
            Init();

            game = new Game();
            Build(game);
        }
        // метод инициализации игровых структур
        void Init()
        {
            // импорт изображений с крестиком и ноликом
            Cross = Image.FromStream(new WebClient().OpenRead("https://raw.github.com/christkv/tic-tac-toe/master/public/img/cross.png"));
            Nought = Image.FromStream(new WebClient().OpenRead("https://raw.github.com/christkv/tic-tac-toe/master/public/img/circle.png"));
            // инициализация крестика и нолика
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    pbs[i, j] = new PictureBox { Parent = this, Size = new Size(100, 100), Top = i * 100, Left = j * 100, BorderStyle = BorderStyle.FixedSingle, Tag = new Point(i, j), Cursor = Cursors.Hand, SizeMode = PictureBoxSizeMode.StretchImage };
                    pbs[i, j].Click += pb_Click;
                }
            // создание кнопки респавна
            new Button { Parent = this, Top = 320, Text = "Reset" }.Click += delegate { game = new Game(); Build(game); };
        }
        // построение поля игры
        private void Build(Game game)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    pbs[i, j].Image = game.Items[i, j] == FieldState.Cross ? Cross : (game.Items[i, j] == FieldState.Nought ? Nought : null);
        }
        // определение победителя по исходу игры
        void pb_Click(object sender, EventArgs e)
        {
            game.MakeMove((Point)(sender as Control).Tag);
            Build(game);

            if (game.Winned)
                MessageBox.Show(string.Format("{0} is winner!", game.CurrentPlayer == 0 ? "Cross" : "Nought"));
        }
    }
    // класс, отвечающий за процесс игры
    class Game
    {
        // инициализация поля 3 на 3 в двумерном массиве
        public FieldState[,] Items = new FieldState[3, 3];   
        // подвязка игрока
        public int CurrentPlayer = 0;
        // победа/поражение
        public bool Winned;
        // метод определения процесса игры
        public void MakeMove(Point p)
        {
            if (Items[p.X, p.Y] != FieldState.Empty)
                return;

            if (Winned)
                return;

            Items[p.X, p.Y] = CurrentPlayer == 0 ? FieldState.Cross : FieldState.Nought;
            if (CheckWinner(FieldState.Cross) || CheckWinner(FieldState.Nought))
            {
                Winned = true;
                return;
            }

            CurrentPlayer ^= 1;
        }
        // метод определения победителя
        private bool CheckWinner(FieldState state)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Items[i, 0] == state && Items[i, 1] == state && Items[i, 2] == state)
                    return true;
                if (Items[0, i] == state && Items[1, i] == state && Items[2, i] == state)
                    return true;
            }

            if (Items[0, 0] == state && Items[1, 1] == state && Items[2, 2] == state)
                return true;

            if (Items[0, 2] == state && Items[1, 1] == state && Items[2, 0] == state)
                return true;

            return false;
        }
    }
    // перечисление, выдающее состояние поля (что было нанесено)
    enum FieldState
    {
        Empty,
        Cross,
        Nought
    }
}

