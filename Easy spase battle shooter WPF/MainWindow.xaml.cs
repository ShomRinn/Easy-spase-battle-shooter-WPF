using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Easy_spase_battle_shooter_WPF
{

    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();
        bool MoveRight, MoveLeft;
        List<Rectangle> itemRemover = new List<Rectangle>();

        Random rand = new Random();

        int EnemySpriteCounter = 0; //
        int EnemyCounter = 100;
        int PlayerSpeed = 10;
        int limit = 50;
        int score = 0;
        int damage = 0;
        int enemySpeed = 10; //

        Rect PlayerHitBox;


        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            MyCanvas.Focus();
            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/purple.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            player.Fill = playerImage;


        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                MoveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                MoveRight = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                MoveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                MoveRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);

                MyCanvas.Children.Add(newBullet);

            }

        }

        private void MakeEnemies()
        {

            ImageBrush EnemySprite = new ImageBrush();

            EnemySpriteCounter = rand.Next(1, 5);

            switch (EnemySpriteCounter)
            {

                case 1:
                    EnemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/1.png"));
                    break;
                case 2:
                    EnemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/2.png"));
                    break;
                case 3:
                    EnemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/3.png"));
                    break;
                case 4:
                    EnemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/4.png"));
                    break;
                case 5:
                    EnemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/5.png"));
                    break;

            }

            Rectangle newEnemy = new Rectangle
            {

                Tag = "enemy",
                Height = 50,
                Width = 56,
                Fill = EnemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rand.Next(30, 430));
            MyCanvas.Children.Add(newEnemy);



        }


        private void GameLoop(object sender, EventArgs e)
        {
            PlayerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            EnemyCounter -= 1;  //!

            ScoreText.Content = "Score" + score;
            DamageText.Content = "danage" + damage;

            if (EnemyCounter < 0)
            {
                MakeEnemies();
                EnemyCounter = limit;
            }

            if (MoveLeft == true && Canvas.GetLeft(player) > 0)
            {

                Canvas.SetLeft(player, Canvas.GetLeft(player) - PlayerSpeed);

            }

            if (MoveRight == true && Canvas.GetLeft(player) + 90 < Application.Current.MainWindow.Width)
            {

                Canvas.SetLeft(player, Canvas.GetLeft(player) + PlayerSpeed);

            }


            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {

                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemRemover.Add(x);
                    }

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect EnemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(EnemyHit))
                            {
                                itemRemover.Add(x);
                                itemRemover.Add(y);
                                score++;
                            }
                        }
                    }

                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);

                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        damage += 10;
                    }
                }

                Rect EmenyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if (PlayerHitBox.IntersectsWith(EmenyHitBox))
                {
                    itemRemover.Add(x);
                    damage += 5;
                }


            }

            foreach (Rectangle i in itemRemover)
            {

                MyCanvas.Children.Remove(i);

            }

            if (score > 5)
            {
                limit = 20;
                enemySpeed = 15;
            }
            if (damage > 99)
            {
                gameTimer.Stop();
                DamageText.Content = "Damage: 100";
                MessageBox.Show("Capitan we faild!" + Environment.NewLine + "You have destroyed " + score + " Alien ships");

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }







    }
}
