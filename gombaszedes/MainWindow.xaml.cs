using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gombaszedes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const int BoardSize = 8;

        int currentX = 0;
        int currentY = 0;

        List<Point> mushroomPath;
        int currentTargetIndex = 0;

        BitmapImage imgMushroom = new BitmapImage(new Uri("kepek/gomba.png", UriKind.Relative));
        BitmapImage imgHappy = new BitmapImage(new Uri("kepek/happy.png", UriKind.Relative));
        BitmapImage imgSad = new BitmapImage(new Uri("kepek/sad.png", UriKind.Relative));

        string currentPiece = "rook";

        public MainWindow()
        {
            InitializeComponent();
            startNewGame();
        }
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            startNewGame();
        }

        private void startNewGame()
        {
            Random rnd = new Random();

            currentX = rnd.Next(BoardSize);
            currentY = rnd.Next(BoardSize);

            mushroomPath = generatePath(currentX, currentY, 5);
            currentTargetIndex = 0;

            DrawBoard();
        }

        private void DrawBoard()
        {
            ChessBoard.Children.Clear();

            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    Button cell = new Button();
                    cell.Tag = new Point { X = x, Y = y };
                    cell.Click += handleCellClick;

                    if ((x + y) % 2 == 0)
                    {
                        cell.Background = Brushes.Beige;
                    }
                    else
                    {
                        cell.Background = Brushes.SaddleBrown;
                    }

                    Grid contentGrid = new Grid();
                    if(x == currentX && y == currentY)
                    {
                        TextBlock tb = new TextBlock();

                        string pieceChar = "♖";

                        if (currentPiece == "rook")
                            pieceChar = "♖";
                        else if (currentPiece == "bishop")
                            pieceChar = "♗";
                        else if (currentPiece == "queen")
                            pieceChar = "♕";
                        else if (currentPiece == "knight")
                            pieceChar = "♘";

                        tb.Text = pieceChar;

                        tb.FontSize = 40;
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.Foreground = ((x + y) % 2 == 0) ? Brushes.Black : Brushes.White;

                        contentGrid.Children.Add(tb);
                    }

                    bool isThereMushroom = false;
                    if(mushroomPath != null)
                    {
                        foreach(Point p in mushroomPath)
                        {
                            if (p.X == x && p.Y == y)
                            {
                                isThereMushroom = true;
                                break;
                            }
                        }
                    }

                    if(isThereMushroom)
                    {
                        Image img = new Image();
                        img.Source = imgMushroom;
                        img.Width = 50;
                        img.Height = 50;
                        contentGrid.Children.Add(img);
                    }

                    cell.Content = contentGrid;
                    ChessBoard.Children.Add(cell);
                }
            }
        }
        
        private void handleCellClick(object sender, RoutedEventArgs e)
        {
            if (currentTargetIndex >= mushroomPath.Count) return;
            StatusImage.Source = imgHappy;

            Button clickedCell = (Button)sender;
            dynamic tag = clickedCell.Tag;

            int targetX = (int)tag.X;
            int targetY = (int)tag.Y;

            int mushroomIndex = -1;

            for (int  i = 0; i < mushroomPath.Count; i++)
            {
                if (mushroomPath[i].X == targetX && mushroomPath[i].Y == targetY)
                {
                    mushroomIndex =  i;
                    break;
                }
            }

            if (mushroomIndex == -1 || !isValidMove(currentX, currentY, targetX, targetY))
            {
                StatusImage.Source = imgSad;
                return;
            }

            List<Point> remaining = new List<Point>();
            for (int i = 0; i < mushroomPath.Count; i++)
            {
                if (i != mushroomIndex)
                    remaining.Add(mushroomPath[i]);
            }

            int newX = targetX;
            int newY = targetY;

            bool okMove = CanCollectAll(newX, newY, remaining);

            if (!okMove)
            {
                StatusImage.Source = imgSad;
                return;
            }

            currentX = newX;
            currentY = newY;
            mushroomPath.RemoveAt(mushroomIndex);
            DrawBoard();

            if (mushroomPath.Count == 0)
            {
                MessageBox.Show("Minden gombát felszedtél.");
            }
        }

        private List<Point> generatePath(int startX,  int startY, int lenght)
        {
            Random rnd = new Random();
            List<Point> path = new List<Point>();

            int simX = startX;
            int simY = startY;

            List<Point> usedPositions = new List<Point>();
            usedPositions.Add(new Point(startX, startY));

            for (int i = 0; i < lenght; i++)
            {
                List<Point> validMoves = new List<Point>();

                for (int x = 0; x < BoardSize; x++)
                {
                    for (int y = 0; y < BoardSize; y++)
                    {
                        if(isValidMove(simX, simY, x, y))
                        {
                            bool visited = false;
                            foreach (Point p in usedPositions)
                            {
                                if(p.X == x &&  p.Y == y)
                                {
                                    visited = true;
                                }
                            }
                            if (!visited)
                            {
                                validMoves.Add(new Point(x, y));
                            }

                        }
                    }
                }

                if(validMoves.Count == 0)
                {
                    return generatePath(startX, startY, lenght);
                }

                Point nextMove = validMoves[rnd.Next(validMoves.Count)];
                path.Add(nextMove);
                usedPositions.Add(nextMove);

                simX = (int)nextMove.X;
                simY = (int)nextMove.Y;
            }
            return path;
        }


        private bool CanCollectAll(int fromX, int fromY, List<Point> remaining)
        {
            if (remaining.Count == 0) return true;

            for(int i  = 0; i < remaining.Count; i++)
            {
                Point target = remaining[i];

                if(isValidMove(fromX, fromY, (int)target.X, (int)target.Y))
                {
                    List<Point> nextReamining = new List<Point>();
                    for(int j = 0; j < remaining.Count; j++)
                    {
                        if(j != i)
                        {
                            nextReamining.Add(remaining[j]);
                        }
                    }

                    if(CanCollectAll((int)target.X, (int)target.Y, nextReamining))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool isValidMove(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);

            if (x1 == x2 && y1 == y2)
                return false;

            if (currentPiece == "rook")
            {
                // Bástya
                return (x1 == x2) || (y1 == y2);
            }
            else if (currentPiece == "bishop")
            {
                // Futó
                return dx == dy;
            }
            else if (currentPiece == "queen")
            {
                // Vezér
                bool rookMove = (x1 == x2) || (y1 == y2);
                bool bishopMove = (dx == dy);
                return rookMove || bishopMove;
            }
            else if (currentPiece == "knight")
            {
                // Huszár
                return (dx == 1 && dy == 2) || (dx == 2 && dy == 1);
            }
            return false;
        }

        private void Rook_Click(object sender, RoutedEventArgs e)
        {
            currentPiece = "rook";
            startNewGame();
        }

        private void Bishop_Click(object sender, RoutedEventArgs e)
        {
            currentPiece = "bishop";
            startNewGame();
        }

        private void Queen_Click(object sender, RoutedEventArgs e)
        {
            currentPiece = "queen";
            startNewGame();
        }

        private void Knight_Click(object sender, RoutedEventArgs e)
        {
            currentPiece = "knight";
            startNewGame();
        }
    }
}