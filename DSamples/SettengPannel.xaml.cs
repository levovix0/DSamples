using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSamples
{
    static class Global
    {
        public enum Theme
        {
            Dark,
            Light
        }
        public enum Lang
        {
            En,
            Ru
        }

        private static Theme _theme = Theme.Light;
        private static Lang _lang = Lang.Ru;
        public static Theme theme
        {
            get => _theme;
            set
            {
                if (_theme == value) return;
                _theme = value;
                ThemeChanged?.Invoke(_theme);
            }
        }
        public static Lang lang
        {
            get => _lang;
            set
            {
                if (_lang == value) return;
                _lang = value;
                LangChanged?.Invoke(_lang);
            }
        }

        public delegate void ThemeChangedEvent(Theme theme);
        public delegate void LangChangedEvent(Lang lang);
        public delegate void RefreshEvent();

        public static event ThemeChangedEvent ThemeChanged;
        public static event LangChangedEvent LangChanged;

        public static string server_ip = "";
        public static string folder = "~/samples";
        public static string login = "";
        public static string password = "";

        public static event RefreshEvent OnRefresh;

        public static string Address => "ftp://" + server_ip + "/";

        public static void Refresh()
        {
            OnRefresh?.Invoke();
        }

        public static void save_to_file(string filename)
        {
            string pass = "";
            Random rns = new Random(284621);
            Random rnd = new Random(rns.Next() * password.Length);

            foreach (var v in password)
            {
                pass += (char)(v + rnd.Next(1300));
            }

            string[] x =
            {
                theme.ToString(),
                lang.ToString(),
                server_ip,
                folder,
                login,
                pass,
            };
            File.WriteAllLines(filename, x);
        }

        public static void read_from_file(string filename)
        {
            try
            {
                string[] x = File.ReadAllLines(filename);
                if (x.Length < 6) return;

                if (x[0] == "Light") theme = Theme.Light;
                else if (x[0] == "Dark") theme = Theme.Dark;
                if (x[1] == "En") lang = Lang.En;
                else if (x[1] == "Ru") lang = Lang.Ru;

                server_ip = x[2];
                folder = x[3];
                login = x[4];
                password = x[5];

                string pass = "";
                Random rns = new Random(284621);
                Random rnd = new Random(rns.Next() * password.Length);

                foreach (var v in password)
                {
                    pass += (char)(v - rnd.Next(1300));
                }

                password = pass;
            }
            catch { }
        }
    }

    /// <summary>
    /// Логика взаимодействия для SettengPannel.xaml
    /// </summary>
    public partial class SettengPannel : UserControl
    {
        private Dictionary<object, Thickness> margins = new Dictionary<object, Thickness>();

        public SettengPannel()
        {
            InitializeComponent();

            LightButton.MouseEnter += Button_MouseEnter;
            DarkButton.MouseEnter += Button_MouseEnter;
            EnButton.MouseEnter += Button_MouseEnter;
            RuButton.MouseEnter += Button_MouseEnter;
            AboutDTeamLabel.MouseEnter += Button_MouseEnter;

            LightButton.MouseLeave += Button_MouseLeave;
            DarkButton.MouseLeave += Button_MouseLeave;
            LightButton.MouseUp += Button_MouseLeave;
            DarkButton.MouseUp += Button_MouseLeave;
            EnButton.MouseLeave += Button_MouseLeave;
            RuButton.MouseLeave += Button_MouseLeave;
            EnButton.MouseUp += Button_MouseLeave;
            RuButton.MouseUp += Button_MouseLeave;
            AboutDTeamLabel.MouseLeave += Button_MouseLeave;
            AboutDTeamLabel.MouseUp += Button_MouseLeave;

            LightButton.MouseDown += Button_MouseDown;
            DarkButton.MouseDown += Button_MouseDown;
            EnButton.MouseDown += Button_MouseDown;
            RuButton.MouseDown += Button_MouseDown;
            AboutDTeamLabel.MouseDown += Button_MouseDown;

            margins[LightButton] = LightButton.Margin;
            margins[DarkButton] = DarkButton.Margin;
            margins[EnButton] = EnButton.Margin;
            margins[RuButton] = RuButton.Margin;
            margins[MainSettings] = MainSettings.Margin;
            margins[ServerSettings] = ServerSettings.Margin;

            IpTB.MouseEnter += TB_MouseEnter;
            IpTB.TextChanged += TB_TextChanged;
            FolderTB.MouseEnter += TB_MouseEnter;
            FolderTB.TextChanged += TB_TextChanged;
            LoginTB.MouseEnter += TB_MouseEnter;
            LoginTB.TextChanged += TB_TextChanged;

            SetBtnState(LightButton, true);
            SetBtnState(DarkButton, false);
            SetBtnState(EnButton, false);
            SetBtnState(RuButton, true);

            Global.ThemeChanged += delegate(Global.Theme e)
            {
                SetBtnState(LightButton, e == Global.Theme.Light);
                SetBtnState(DarkButton, e == Global.Theme.Dark);
            };

            Global.LangChanged += delegate (Global.Lang e)
            {
                SetBtnState(EnButton, e == Global.Lang.En);
                SetBtnState(RuButton, e == Global.Lang.Ru);
            };
        }

        void SetBtnState(FrameworkElement o, bool b)
        {
            var t_a = TimeSpan.FromSeconds(0.15);

            if (b)
            {
                var s_animation = new DoubleAnimation
                {
                    From = 12,
                    To = 20,
                    Duration = t_a,
                    FillBehavior = FillBehavior.Stop
                };
                var o_animation = new DoubleAnimation
                {
                    From = 0.7,
                    To = 1,
                    Duration = t_a,
                    FillBehavior = FillBehavior.Stop
                };
                var m_animation = new ThicknessAnimation
                {
                    From = new Thickness(margins[o].Left + 4, margins[o].Top + 4, margins[o].Right, margins[o].Bottom),
                    To = margins[o],
                    Duration = t_a,
                    FillBehavior = FillBehavior.Stop
                };
                s_animation.Completed += delegate
                {
                    o.Width = 20;
                    o.Height = 20;
                    o.Opacity = 1;
                    o.Margin = margins[o];
                };

                o.BeginAnimation(WidthProperty, s_animation);
                o.BeginAnimation(HeightProperty, s_animation);
                o.BeginAnimation(OpacityProperty, o_animation);
                o.BeginAnimation(MarginProperty, m_animation);
            }
            else
            {
                var s_animation = new DoubleAnimation
                {
                    From = 20,
                    To = 12,
                    Duration = t_a,
                    FillBehavior = FillBehavior.Stop
                };
                var o_animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0.7,
                    Duration = t_a,
                    FillBehavior = FillBehavior.Stop
                };
                var m_animation = new ThicknessAnimation
                {
                    From = margins[o],
                    To = new Thickness(margins[o].Left + 4, margins[o].Top + 4, margins[o].Right, margins[o].Bottom),
                    Duration = t_a,
                    FillBehavior = FillBehavior.Stop
                };
                s_animation.Completed += delegate
                {
                    o.Width = 12;
                    o.Height = 12;
                    o.Opacity = 0.7;
                    o.Margin = new Thickness(margins[o].Left + 4, margins[o].Top + 4, margins[o].Right, margins[o].Bottom);
                };

                o.BeginAnimation(WidthProperty, s_animation);
                o.BeginAnimation(HeightProperty, s_animation);
                o.BeginAnimation(OpacityProperty, o_animation);
                o.BeginAnimation(MarginProperty, m_animation);
            }
        }

        double opacityOf(object o)
        {
            if (ReferenceEquals(o, LightButton))
                return Global.theme == Global.Theme.Light ? 10 : 0.7;
            if (ReferenceEquals(o, DarkButton))
                return Global.theme == Global.Theme.Dark ? 10 : 0.7;
            if (ReferenceEquals(o, EnButton))
                return Global.lang == Global.Lang.En ? 10 : 0.7;
            if (ReferenceEquals(o, RuButton))
                return Global.lang == Global.Lang.Ru ? 10 : 0.7;
            return 1;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Opacity = opacityOf(sender) * 0.7;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Opacity = opacityOf(sender) * 1;
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Opacity = opacityOf(sender) * 0.5;
        }

        private void LightButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Global.theme = Global.Theme.Light;
        }

        private void DarkButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Global.theme = Global.Theme.Dark;
        }

        private void EnButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Global.lang = Global.Lang.En;
        }

        private void RuButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Global.lang = Global.Lang.Ru;
        }

        private void ServerButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Opacity = 1;
        }

        private void ServerButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((FrameworkElement)sender).Opacity = 0;
        }

        private void ServerButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((FrameworkElement)sender).Opacity = 0.5;
        }

        private bool isServerSettingOpen = false;

        public void CloseServerSettings()
        {
            isServerSettingOpen = false;
            MainSettings.Margin = margins[MainSettings];
            ServerSettings.Margin = margins[ServerSettings];
            RenderRect.Height = MainSettings.ActualHeight;
            BackgroundRect.Height = MainSettings.ActualHeight;
        }
        public void OpenServerSettings()
        {
            isServerSettingOpen = true;
            MainSettings.Margin = new Thickness(margins[MainSettings].Left - 190, margins[MainSettings].Top, margins[MainSettings].Right + 190, margins[MainSettings].Bottom);
            ServerSettings.Margin = new Thickness(margins[ServerSettings].Left - 190, margins[ServerSettings].Top, margins[ServerSettings].Right + 190, margins[ServerSettings].Bottom);
            RenderRect.Height = ServerSettings.ActualHeight;
            BackgroundRect.Height = ServerSettings.ActualHeight;
        }

        private void ServerButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isServerSettingOpen) return;
            isServerSettingOpen = true;
            var a_t = TimeSpan.FromSeconds(0.15);
            var l_anim = new ThicknessAnimation
            {
                From = margins[MainSettings],
                To = new Thickness(margins[MainSettings].Left - 190, margins[MainSettings].Top, margins[MainSettings].Right + 190, margins[MainSettings].Bottom),
                Duration = a_t,
                FillBehavior = FillBehavior.Stop
            };
            var r_anim = new ThicknessAnimation
            {
                From = margins[ServerSettings],
                To = new Thickness(margins[ServerSettings].Left - 190, margins[ServerSettings].Top, margins[ServerSettings].Right + 190, margins[ServerSettings].Bottom),
                Duration = a_t,
                FillBehavior = FillBehavior.Stop
            };
            var rc_anim = new DoubleAnimation
            {
                From = MainSettings.ActualHeight,
                To = ServerSettings.ActualHeight,
                Duration = a_t,
                FillBehavior = FillBehavior.Stop
            };

            l_anim.Completed += delegate { OpenServerSettings(); };

            MainSettings.BeginAnimation(MarginProperty, l_anim);
            ServerSettings.BeginAnimation(MarginProperty, r_anim);

            RenderRect.BeginAnimation(HeightProperty, rc_anim);
            BackgroundRect.BeginAnimation(HeightProperty, rc_anim);
        }

        private void ServerBackButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isServerSettingOpen) return;
            isServerSettingOpen = false;
            var a_t = TimeSpan.FromSeconds(0.15);
            var l_anim = new ThicknessAnimation
            {
                From = new Thickness(margins[MainSettings].Left - 190, margins[MainSettings].Top, margins[MainSettings].Right + 190, margins[MainSettings].Bottom),
                To = margins[MainSettings],
                Duration = a_t,
                FillBehavior = FillBehavior.Stop
            };
            var r_anim = new ThicknessAnimation
            {
                From = new Thickness(margins[ServerSettings].Left - 190, margins[ServerSettings].Top, margins[ServerSettings].Right + 190, margins[ServerSettings].Bottom),
                To = margins[ServerSettings],
                Duration = a_t,
                FillBehavior = FillBehavior.Stop
            };
            var rc_anim = new DoubleAnimation
            {
                From = ServerSettings.ActualHeight,
                To = MainSettings.ActualHeight,
                Duration = a_t,
                FillBehavior = FillBehavior.Stop
            };
            l_anim.Completed += delegate { CloseServerSettings(); };

            MainSettings.BeginAnimation(MarginProperty, l_anim);
            ServerSettings.BeginAnimation(MarginProperty, r_anim);

            RenderRect.BeginAnimation(HeightProperty, rc_anim);
            BackgroundRect.BeginAnimation(HeightProperty, rc_anim);
        }

        private void AboutDTeamLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://zxx.ru");
        }

        private void TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox) sender;
            if (tb.Text == "")
                tb.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            else
                tb.CaretBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        private void TB_MouseEnter(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            tb.CaretBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }
        private void IpTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            Global.server_ip = tb.Text;
        }
        private void FolderTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            Global.folder = tb.Text;
        }

        private void LoginTB_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            Global.login = tb.Text;
        }

        private void PasswordTB_TextChanged(object sender, RoutedEventArgs e)
        {
            var tb = (PasswordBox)sender;
            Global.password = tb.Password;
            if (tb.Password == "")
            {
                PasswordTBT.Visibility = Visibility.Visible;
                tb.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            }
            else
            {
                PasswordTBT.Visibility = Visibility.Hidden;
                tb.CaretBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void PasswrdTB_MouseEnter(object sender, MouseEventArgs e)
        {
            var tb = (PasswordBox)sender;
            tb.CaretBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }
    }
}
