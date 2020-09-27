using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSamples
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            TitleBackground0.MouseUp += MinimizeWhenMMB;
            TitleBackground.MouseUp += MinimizeWhenMMB;

            SettingsPannel.Visibility = Visibility.Visible;
            SettingsPannel.Opacity = 0;
            SettingsPannel.IsHitTestVisible = false;

            Global.ThemeChanged += delegate(Global.Theme theme)
            {
                if (theme == Global.Theme.Light)
                {
                    Application.Current.Resources["Background1"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    Application.Current.Resources["Background2"] = new SolidColorBrush(Color.FromRgb(244, 244, 244));
                    Application.Current.Resources["Foreground1"] = new SolidColorBrush(Color.FromRgb(80, 80, 80));
                    Application.Current.Resources["Foreground2"] = new SolidColorBrush(Color.FromRgb(137, 137, 137));
                }
                else if (theme == Global.Theme.Dark)
                {
                    Application.Current.Resources["Background1"] = new SolidColorBrush(Color.FromRgb(32, 32, 32));
                    Application.Current.Resources["Background2"] = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                    Application.Current.Resources["Foreground1"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    Application.Current.Resources["Foreground2"] = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                }
            };

            Global.LangChanged += delegate(Global.Lang lang)
            {
                if (lang == Global.Lang.En)
                {
                    Application.Current.Resources["StrLanguage"] = "Language";
                    Application.Current.Resources["StrTheme"] = "Theme";
                    Application.Current.Resources["StrServer"] = "Server";
                    Application.Current.Resources["StrAboutDTeam"] = "About DTeam";
                    Application.Current.Resources["ThereIsNothing"] = "Here is nothing";
                    Application.Current.Resources["StrThisPC"] = "This PC";
                    Application.Current.Resources["StrFolder"] = "Folder";
                    Application.Current.Resources["StrNo"] = "no";
                    Application.Current.Resources["StrLogin"] = "Login";
                    Application.Current.Resources["StrPassword"] = "Password";
                }
                else if (lang == Global.Lang.Ru)
                {
                    Application.Current.Resources["StrLanguage"] = "Язык";
                    Application.Current.Resources["StrTheme"] = "Тема";
                    Application.Current.Resources["StrServer"] = "Сервер";
                    Application.Current.Resources["StrAboutDTeam"] = "О DTeam";
                    Application.Current.Resources["ThereIsNothing"] = "Тут пусто";
                    Application.Current.Resources["StrThisPC"] = "Этот компьютер";
                    Application.Current.Resources["StrFolder"] = "Папка";
                    Application.Current.Resources["StrNo"] = "нет";
                    Application.Current.Resources["StrLogin"] = "Логин";
                    Application.Current.Resources["StrPassword"] = "Пароль";
                }
            };

            Global.lang = Global.Lang.En;

            Global.read_from_file("config.txt");
            SettingsPannel.IpTB.Text = Global.server_ip;
            SettingsPannel.FolderTB.Text = Global.folder;
            SettingsPannel.LoginTB.Text = Global.login;
            SettingsPannel.PasswordTB.Password = Global.password;
        }

        ~MainWindow()
        {
            Global.save_to_file("config.txt");
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;
            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private bool isDraggingWindow;
        private Point dragPrevPos;

        private void begin_drag()
        {
            if (isDraggingWindow) return;
            isDraggingWindow = true;
            dragPrevPos = PointToScreen(Mouse.GetPosition(this));

            Mouse.OverrideCursor = Cursors.SizeAll;

            CaptureMouse();
        }

        private void end_drag()
        {
            if (!isDraggingWindow) return;
            isDraggingWindow = false;

            Mouse.OverrideCursor = null;

            ReleaseMouseCapture();
        }


        private bool isResizingWindowY;

        private void begin_resize_y()
        {
            if (isResizingWindowY) return;
            isResizingWindowY = true;
            dragPrevPos = PointToScreen(Mouse.GetPosition(this));

            Mouse.OverrideCursor = Cursors.SizeNS;

            CaptureMouse();
        }

        private void end_resize_y()
        {
            if (!isResizingWindowY) return;
            isResizingWindowY = false;

            Mouse.OverrideCursor = null;

            ReleaseMouseCapture();
        }

        private void Minimize()
        {
            WindowState = WindowState.Minimized;
        }

        private void ToBeginDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                begin_drag();
        }
        private void MinimizeWhenMMB(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                Minimize();
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingWindow)
            {
                var dragCurPos = PointToScreen(Mouse.GetPosition(this));
                var delta = dragCurPos - dragPrevPos;
                Left += delta.X;
                Top += delta.Y;

                dragPrevPos = dragCurPos;
            }
            if (isResizingWindowY)
            {
                var dragCurPos = PointToScreen(Mouse.GetPosition(this));
                var delta = dragCurPos - dragPrevPos;
                if (Height + delta.Y >= 520)
                {
                    Height += delta.Y;
                    dragPrevPos = dragCurPos;
                }
                else
                {
                    Height = 520;
                    dragPrevPos = dragCurPos;
                    dragPrevPos.Y = PointToScreen(new Point(0, 510)).Y;
                }

                AllGrid.Height = Height;
                BackgroundRect.Height = Height - BackgroundRect.Margin.Top - BackgroundRect.Margin.Bottom;
                ApplicationRect.Height = Height - BackgroundRect.Margin.Top - BackgroundRect.Margin.Bottom;
            }

            if (is_click && !isDraggingWindow && !isResizingWindowY)
            {
                begin_drag();
            }
            is_click = false;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDraggingWindow)
                end_drag();
            if (isResizingWindowY)
                end_resize_y();
            CloseButton.Opacity = 1;
            MinimizeButton.Opacity = 1;
            TopButton.Opacity = 1;
            SettingsButton.Opacity = 1;
        }

        private bool is_click;

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                is_click = true;
                (sender as Image).Opacity = 0.5;
            }
        }

        private bool isSettingsOpened = false;

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (is_click)
                {
                    if (ReferenceEquals(sender, CloseButton))
                    { 
                        Close();
                    }
                    else if (ReferenceEquals(sender, MinimizeButton))
                    {
                        Minimize();
                    }
                    else if (ReferenceEquals(sender, TopButton))
                    {
                        Topmost = !Topmost;
                        TopButton.Source = new BitmapImage(new Uri(Topmost ? @"TopButton0.png" : @"TopButton1.png", UriKind.Relative));
                    }
                    else if (ReferenceEquals(sender, SettingsButton))
                    {
                        setSettingsOpened(!isSettingsOpened);
                    }
                }
                is_click = false;
            }
        }

        void setSettingsOpened(bool b)
        {
            if (isSettingsOpened == b) return;

            var dur_a = TimeSpan.FromSeconds(0.15);
            isSettingsOpened = b;
            if (isSettingsOpened)
            {
                var o_animation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = dur_a
                };
                var p_animation = new ThicknessAnimation
                {
                    From = new Thickness(130, 10, 0, 0),
                    To = new Thickness(130, 40, 0, 0),
                    Duration = dur_a
                };
                var i_animation = new ThicknessAnimation
                {
                    From = new Thickness(0, 35, 82, 0),
                    To = new Thickness(0, 35, 72, 0),
                    Duration = dur_a
                };
                var i2_animation = new DoubleAnimation
                {
                    From = 0,
                    To = 20,
                    Duration = dur_a
                };
                SettingsPannel.BeginAnimation(OpacityProperty, o_animation);
                SettingsPannel.BeginAnimation(MarginProperty, p_animation);
                SettingsIndicator.BeginAnimation(MarginProperty, i_animation);
                SettingsIndicator.BeginAnimation(WidthProperty, i2_animation);
                SettingsPannel.IsHitTestVisible = true;
            }
            else
            {
                var o_animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = dur_a
                };
                var p_animation = new ThicknessAnimation
                {
                    From = new Thickness(130, 40, 0, 0),
                    To = new Thickness(130, 60, 0, 0),
                    Duration = dur_a
                };
                var i_animation = new ThicknessAnimation
                {
                    From = new Thickness(0, 35, 72, 0),
                    To = new Thickness(0, 35, 82, 0),
                    Duration = dur_a
                };
                var i2_animation = new DoubleAnimation
                {
                    From = 20,
                    To = 0,
                    Duration = dur_a
                };
                o_animation.Completed += delegate
                {
                    SettingsPannel.CloseServerSettings();
                };

                SettingsPannel.BeginAnimation(OpacityProperty, o_animation);
                SettingsPannel.BeginAnimation(MarginProperty, p_animation);
                SettingsIndicator.BeginAnimation(MarginProperty, i_animation);
                SettingsIndicator.BeginAnimation(WidthProperty, i2_animation);
                SettingsPannel.IsHitTestVisible = false;
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 0.7;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Opacity = 1;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            begin_drag();
            setSettingsOpened(false);
        }

        private void DownSizeBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            begin_resize_y();
        }
    }
}
