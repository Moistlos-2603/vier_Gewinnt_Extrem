using Field;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VierGewinntExtrem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //sql
        private SQLHandler handler;
        //constants to avoid typos
        private const string mode_normal = "Normal", mode_3x3 = "3 x 3";
        private const char player1 = '1', player2 = '2';
        private Regex valid_name;
        private Ellipse[] visual_field;
        private Button[] controls;

        //0 = player1, 1 = player2
        private byte gamestate;


#pragma warning disable IDE0052 // Ungelesene private Member entfernen
        private Field.Field game;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public MainWindow()
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        {
            InitializeComponent();
            valid_name = new(@"(\w|[0-9])+", RegexOptions.Compiled);
            //make everything invisible for later
            GameTypeSelector.Items.Add(mode_normal);
            GameTypeSelector.Items.Add(mode_3x3);
            Title = "VierGewinnt";
            handler = new SQLHandler();
            //This should give the data to the data context, meaning: the table gets content.
            DataBaseGrid.DataContext = handler.DataSet;
            //wait for startbutton to be pressed
            StartGame();
        }

        private void StartGame()
        {
            //make everything except start button invisible.
            DataBaseGrid.Visibility = Visibility.Collapsed;
            GameTypeSelector.Visibility = Visibility.Collapsed;
            P1NameGetter.Visibility = Visibility.Collapsed;
            P2NameGetter.Visibility = Visibility.Collapsed;
            NameSubmitButton.Visibility = Visibility.Collapsed;
            PlayerTurnDisplay.Visibility = Visibility.Collapsed;
            GameEndMSG.Visibility = Visibility.Collapsed;
            ReplayButton.Visibility = Visibility.Collapsed;
            Player1Color.Visibility = Visibility.Collapsed;
            Player2Color.Visibility = Visibility.Collapsed;

            //make start button and database button visible.
            StartButton.Visibility = Visibility.Visible;
            ToDatabase.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Make gametype selection possible, hide previous UI element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //make start button disapear + game selector visible
            StartButton.Visibility = Visibility.Collapsed;
            ToDatabase.Visibility = Visibility.Collapsed;
            GameTypeSelector.Visibility = Visibility.Visible;

            //wait for game be selected
            GameTypeSelector.SelectionChanged -= GameTypeSelector_SelectionChanged;
            GameTypeSelector.SelectedIndex = -1;
            GameTypeSelector.SelectionChanged += GameTypeSelector_SelectionChanged;
        }

        /// <summary>
        /// Check playername for validity to minimize security riscs + get the names.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NameErrorMSG.Content = "";
            bool is_name_valid = true;

            if (valid_name.Match(P1NameGetter.Text).Length != P1NameGetter.Text.Length)
            {
                NameErrorMSG.Content += "Player 1 name is not valid, must match: (\\w|[0-9])+\n";
                is_name_valid = false;
            }
            if (valid_name.Match(P2NameGetter.Text).Length != P2NameGetter.Text.Length)
            {
                NameErrorMSG.Content += "Player 2 name is not valid, must match: (\\w|[0-9])+";
                is_name_valid = false;
            }
            if(P1NameGetter.Text == P2NameGetter.Text)
            {
                NameErrorMSG.Content += "Names must be unequal.";
                is_name_valid = false;
            }
            if (!is_name_valid)
            {

                return;
            }

            NameSubmitButton.Visibility = Visibility.Collapsed;
            P1NameGetter.Visibility = Visibility.Collapsed;
            P2NameGetter.Visibility = Visibility.Collapsed;

            //TODO: Make player into Database!

            //Game actually gets initialized
            InitializeGame();
        }

        /// <summary>
        /// Game type gets chosen and wait for submit button to be pushed. Makes "name getters" visible and hides game type selector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = (string)GameTypeSelector.SelectedItem;
            switch (s)
            {
                case mode_normal:
                    game = new(7, 6, 4);
                    break;

                case mode_3x3:
                    game = new(3, 3, 3);
                    break;
            }

            //wait for names typed in and make required things visible
            GameTypeSelector.Visibility = Visibility.Collapsed;
            P1NameGetter.Visibility = Visibility.Visible;
            P2NameGetter.Visibility = Visibility.Visible;
            Player1Color.Visibility = Visibility.Visible;
            Player2Color.Visibility = Visibility.Visible;
            NameSubmitButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Adds buttons and ellipses to the game. The quantity is determined by the game type. Sets up the start of the game. Makes visible whose turn it is.
        /// </summary>
        private void InitializeGame()
        {
            GameGrid.Visibility = Visibility.Visible;

            (int, int) values = game.Dimensions;
            visual_field = new Ellipse[values.Item1 * values.Item2];
            controls = new Button[values.Item1];


            for (int i = 0; i < values.Item1; i++)
            {
                GameGrid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });

                for (int j = 0; j < values.Item2; j++)
                {
                    GameGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });

                    visual_field[i + j * values.Item1] = new Ellipse
                    {
                        Width = GameGrid.Width / values.Item1,
                        Height = GameGrid.Height / values.Item2
                    };

                    visual_field[i + j * values.Item1].SetValue(Grid.ColumnProperty, i);
                    visual_field[i + j * values.Item1].SetValue(Grid.RowProperty, j);
                    visual_field[i + j * values.Item1].Fill = Brushes.LightGray;
                    GameGrid.Children.Add(visual_field[i + j * values.Item1]);
                }

                //Add Butten to this row
                ButtonGrid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
                controls[i] = new();
                controls[i].Width = ButtonGrid.Width / values.Item1;
                controls[i].Height = ButtonGrid.Height;
                controls[i].SetValue(Grid.ColumnProperty, i);
                controls[i].Click += GenericControlButton_Clicked;
                ButtonGrid.Children.Add(controls[i]);
            }
            PlayerTurnDisplay.Content = P1NameGetter.Text;
            gamestate = 0;
            PlayerTurnDisplay.Visibility = Visibility.Visible;
            PlayerTurnDisplay.Foreground = Brushes.Red;
        }

        /// <summary>
        /// Translates the index of the button to a row for the field. Adds player piece on that row. Repaints the whole field and checks for winner.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void GenericControlButton_Clicked(object sender, RoutedEventArgs e)
        {
            int row = 0;
            if (sender is Button)
            {
                if (((Button)sender).Parent is Grid)
                {
                    row = ((Grid)((Button)sender).Parent).Children.IndexOf((Button)sender);
                    if (row == -1)
                        throw new Exception("Invalid Button");
                }
            }
            if (gamestate < 2)
            {
                if (game.Push(row, gamestate == 0 ? Player.player1 : Player.player2))
                {
                    gamestate ^= 1;
                }
            }
            RePaint();
            EvaluateWinner();
        }

        /// <summary>
        /// Iterates thru the field and paints the ellipses based on the player character at the given index. Updates whose turn it is.
        /// </summary>
        private void RePaint()
        {
            string field_symbolic = game.ToString1D();

            for (int i = 0; i < field_symbolic.Length; i++)
            {
                SolidColorBrush b;
                switch (field_symbolic[i])
                {
                    case Field.Field.Player1:
                        b = Brushes.Red;
                        break;
                    case Field.Field.Player2:
                        b = Brushes.Yellow;
                        break;
                    default:
                        b = Brushes.LightGray;
                        break;
                }
                visual_field[i].Fill = b;

            }

            PlayerTurnDisplay.Content = gamestate == 0 ? P1NameGetter.Text : P2NameGetter.Text;
            PlayerTurnDisplay.Foreground = gamestate == 0 ? Brushes.Red : Brushes.Yellow;
        }

        /// <summary>
        /// Returns back normally if no one won. Stops if a tie or a win happens.
        /// </summary>
        private void EvaluateWinner()
        {
            char winner = game.CheckWinner();
            
            if (!game.ToString1D().Contains(' ') && winner == ' ')
            {
                Tie();
                return;
            }
            if (winner == ' ')
            {
                return;
            }

            GameWon(winner == Field.Field.Player1 ? P1NameGetter.Text : P2NameGetter.Text);
            GameGrid.Visibility = Visibility.Collapsed;
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
            ReplayButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///Displays player name and hides game.
        /// </summary>
        private void GameWon(string name)
        {
            Player1Color.Visibility = Player2Color.Visibility = Visibility.Collapsed;
            PlayerTurnDisplay.Visibility = Visibility.Collapsed;
            foreach (UIElement uie in controls)
                uie.Visibility = Visibility.Collapsed;

            GameEndMSG.Content = $"Congratulations,\n'{name}' won.";
            GameEndMSG.Visibility = Visibility.Visible;
            
             
            
            //TODO: database entry for the win
            handler.Execute("INSERT INTO `spiele` (`idSpiele`, `spieler`, `spieler1`, `Gewinner`) VALUES ('', '" + P1NameGetter.Text + "', '" + P2NameGetter.Text +"', '" + name + "')");

            ReplayButton.Visibility = Visibility.Visible;
            DeleteVisualField();
        }

        /// <summary>
        /// Go to the data base.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToDatabase_Click(object sender, RoutedEventArgs e)
        {
            //Make everything invisible.
            StartButton.Visibility = Visibility.Collapsed;
            ToDatabase.Visibility = Visibility.Collapsed;
            TableGrid2.Visibility = Visibility.Visible;
            TableGrid3.Visibility = Visibility.Visible;

            //Make the grid visible.
            DataBaseGrid.Visibility = Visibility.Visible;
            ToMainMenu.Visibility = Visibility.Visible;
            Clear.Visibility = Visibility.Visible;

            //Querry the attributes for each table and update the corresponding DataFieldn
            //Update immediatly since the data is in a state, which is overwritten after the next command execution.
            handler.Execute("SELECT * FROM `spiele`");
            DataBaseGrid.ItemsSource = handler.DataTable?.DefaultView;

            handler.Execute("SELECT * FROM `spieleliga`");
            TableGrid2.ItemsSource = handler.DataTable?.DefaultView;

            handler.Execute("SELECT * FROM `spielerliga`");
            TableGrid3.ItemsSource = handler.DataTable?.DefaultView;
        }

        private void ToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            //Hide database.
            DataBaseGrid.Visibility = Visibility.Collapsed;
            TableGrid2.Visibility = Visibility.Collapsed;
            TableGrid3.Visibility = Visibility.Collapsed;

            //Make contents of the main menu visible.
            StartButton.Visibility = Visibility.Visible;
            ToDatabase.Visibility = Visibility.Visible;

            //Hide Buttons that are not in the main menu.
            ToMainMenu.Visibility = Visibility.Collapsed;
            Clear.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// It clears all tabels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //Execute the clear command.
            handler.Execute("DELETE FROM `spieleliga`");
            handler.Execute("DELETE FROM `spielerliga`");
            handler.Execute("DELETE FROM `spiele`");

            //Reconnect to avoid errors; if not done the change is not applied. 
            handler = new();

            //Update the tables.
            handler.Execute("SELECT * FROM `spiele`");
            DataBaseGrid.ItemsSource = handler.DataTable?.DefaultView;

            handler.Execute("SELECT * FROM `spieleliga`");
            TableGrid2.ItemsSource = handler.DataTable?.DefaultView;

            handler.Execute("SELECT * FROM `spielerliga`");
            TableGrid2.ItemsSource = handler.DataTable?.DefaultView;
        }

        /// <summary>
        /// Displays game end and hides the game.
        /// </summary>
        private void Tie()
        {
            Player1Color.Visibility = Player2Color.Visibility = Visibility.Collapsed;
            PlayerTurnDisplay.Visibility = Visibility.Collapsed;
            foreach (UIElement uie in controls)
                uie.Visibility = Visibility.Collapsed;

            GameEndMSG.Content = $"No one won.";
            GameEndMSG.Visibility = Visibility.Visible;

            //TODO: database entry for this match
            handler.Execute("INSERT INTO `spiele` (`idSpiele`, `spieler`, `spieler1`, `Gewinner`) VALUES ('', '" + P1NameGetter.Text + "', '" + P2NameGetter.Text + "', '')");

            ReplayButton.Visibility = Visibility.Visible;
            DeleteVisualField();
        }

        /// <summary>
        /// Deletes all ellipses and "row buttons" cleanly.
        /// </summary>
        private void DeleteVisualField()
        {
            GameGrid.Children.Clear();
            ButtonGrid.Children.Clear();
        }
    }
}
