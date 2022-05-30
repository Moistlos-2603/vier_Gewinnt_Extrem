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
using System.Text.RegularExpressions;

namespace VierGewinntExtrem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            //wait for startbutton to be pressed

            StartGame();
        }

        private void StartGame()
        {
            GameTypeSelector.Visibility = Visibility.Collapsed;
            P1NameGetter.Visibility = Visibility.Collapsed;
            P2NameGetter.Visibility = Visibility.Collapsed;
            NameSubmitButton.Visibility = Visibility.Collapsed;
            PlayerTurnDisplay.Visibility = Visibility.Collapsed;
            GameEndMSG.Visibility = Visibility.Collapsed;
            ReplayButton.Visibility = Visibility.Collapsed;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //make start button disapear + game selector visible
            StartButton.Visibility = Visibility.Collapsed;
            GameTypeSelector.Visibility = Visibility.Visible;
            //wait for game be selected
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NameErrorMSG.Content = "";
            bool is_name_valid = true;

            if (valid_name.Match(P1NameGetter.Text).Length != P1NameGetter.Text.Length)
            {
                NameErrorMSG.Content += "Player 1 name is not valid, must match: (\\w|[0-9])+\n";
                is_name_valid = false;
            }
            if(valid_name.Match(P2NameGetter.Text).Length != P2NameGetter.Text.Length)
            {
                NameErrorMSG.Content += "Player 2 name is not valid, must match: (\\w|[0-9])+";
                is_name_valid = false;
            }
            if(!is_name_valid)
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

        private void GameTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = (string)GameTypeSelector.SelectedItem;
            switch(s)
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
            NameSubmitButton.Visibility = Visibility.Visible;
        }

        private void InitializeGame()
        {
            (int, int) values = game.Dimensions;
            visual_field = new Ellipse[values.Item1 * values.Item2];
            controls = new Button[values.Item1];
            
            
            for(int i = 0 ; i < values.Item1; i++)
            {
                GameGrid.ColumnDefinitions.Add(new() { Width = GridLength.Auto});

                for(int j = 0 ; j < values.Item2; j++)
                {
                    GameGrid.RowDefinitions.Add(new() { Height = GridLength.Auto});

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
                ButtonGrid.ColumnDefinitions.Add(new() { Width = GridLength.Auto});
                controls[i] = new();
                controls[i].Width = ButtonGrid.Width / values.Item1;
                controls[i].Height = ButtonGrid.Height;
                controls[i].SetValue(Grid.ColumnProperty, i);
                controls[i].Click += GenericControlButton_Clicked;
                ButtonGrid.Children.Add(controls[i]);
            }
            PlayerTurnDisplay.Content = P1NameGetter.Text;
            PlayerTurnDisplay.Visibility = Visibility.Visible;
        }

        private void GenericControlButton_Clicked(object sender, RoutedEventArgs e)
        {
            int row = 0;
            if(sender is Button)
            {
                if(((Button)sender).Parent is Grid)
                {
                    row = ((Grid)((Button)sender).Parent).Children.IndexOf((Button)sender);
                    if (row == -1)
                        throw new Exception("Invalid Button");
                }
            }
            if(gamestate < 2)
            {
                if(game.Push(row, gamestate == 0 ? player1 : player2))
                {
                    gamestate ^= 1;
                }
            }
            RePaint();
            EvaluateWinner();
        }

        private void RePaint()
        {
            for(int i = 0; i < game.Dimensions.Item1; i++)
            {
                for(int j = 0; j < game.Dimensions.Item2 ; j++)
                {
                    SolidColorBrush b;
                    switch(game.GetCharArray()[i, j])
                    {
                        case player1:
                            b = Brushes.Red;
                            break;
                        case player2:
                            b = Brushes.Yellow;
                            break;
                        default:
                            b = Brushes.LightGray;
                            break;
                    }
                    visual_field[i + (game.Dimensions.Item2 - j - 1) * game.Dimensions.Item1].Fill = b;
                }
            }
            
            PlayerTurnDisplay.Content = gamestate == 0 ? P1NameGetter.Text : P2NameGetter.Text;
            PlayerTurnDisplay.Foreground = gamestate == 0 ? Brushes.Red : Brushes.Yellow;
        }

        private void EvaluateWinner()
        {
            char winner = game.CheckWinner();
            if(winner == ' ')
            {
                foreach(char c in game.GetCharArray())
                {
                    if(c == ' ')
                        return;
                }

                Tie();
            }

            GameWon(gamestate == 0 ? P2NameGetter.Text : P1NameGetter.Text);
            GameGrid.Visibility = Visibility.Collapsed;
        }

        private void GameWon(string name)
        {
            GameEndMSG.Content = $"Congratulations,\n'{name}' won.";
            GameEndMSG.Visibility = Visibility.Visible;
            ReplayButton.Visibility = Visibility.Visible;
            //do database entry for the win
        }

        private void Tie()
        {
            GameEndMSG.Content = $"No one won.";
            ReplayButton.Visibility = Visibility.Visible;
        }
    }
}
