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
                        tb.Text = "♖";
                        tb.FontSize = 40;
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.Foreground = ((x + y) % 2 == 0) ? Brushes.Black : Brushes.White;

                        contentGrid.Children.Add(tb);
                    }


                    int mIndex = -1;
                    if(mushroomPath != null)
                    {
                        for(int i = 0; i < mushroomPath.Count; i++)
                        {
                            if (mushroomPath[i].X == x && mushroomPath[i].Y == y)
                            {
                                mIndex = i;
                                break;
                            }
                        }
                    }

                    if(mIndex != -1 && mIndex >= currentTargetIndex)
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

            Button clickedCell = (Button)sender;

            dynamic p = clickedCell.Tag;

            //if(isValidMove(currentX, currentY, targetX, targetY))
            //{
                //if(currentX != targetX || currentY != targetY)
                //{
                //    currentX = targetX;
                 //   currentY = targetY;
                //    DrawBoard();
                //}
            //}

            Point target = mushroomPath[currentTargetIndex];

            if (p.X == target.X && p.Y == target.Y)
            {
                currentX = (int)p.X;
                currentY = (int)p.Y;
                currentTargetIndex++;
                StatusImage.Source = imgHappy;

                if (currentTargetIndex >= mushroomPath.Count)
                {
                    InfoText.Text = "GYŐZELEM!";
                }

                DrawBoard();
            }
            else
            {

                StatusImage.Source = imgSad;
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




        //Bastya
        private bool isValidMove(int x1, int y1, int x2, int y2)
        {
            return (x1 == x2) || (y1 == y2);
        }


    }
}