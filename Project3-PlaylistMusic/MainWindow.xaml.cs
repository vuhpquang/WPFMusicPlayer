using Gma.System.MouseKeyHook;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Project3_PlaylistMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer player = new MediaPlayer();
        BindingList<Song> list_song = new BindingList<Song>();
        DispatcherTimer timer;
        private IKeyboardMouseEvents m_GlobalHook;
        string path = "";
        bool isPlaying = false;
        int Index = -1;

        int kind_play = 0;
        //0 - tuan tu
        //1 - ngau nhien
        //2 - lap 
        //3 - lap vo tan

        public MainWindow()
        {
            InitializeComponent();

            // set timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;

            // set player
            player.MediaEnded += Next_Song;

            // set hook 
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyUp += KeyUp_hook;
        }

        private void PlaySongIndex(int index)
        {
            if(index >= 0 && index < list_song.Count)
            {
                player.Open(new Uri(path + '/' + list_song[index].Name, UriKind.Absolute));
                endtimeTb.Text = list_song[index].Time;


                // convert time to seconds
                var tokens = list_song[index].Time.Split(new string[] { ":" },
                    StringSplitOptions.None);
                int totalSeconds = int.Parse(tokens[0]) * 60 + int.Parse(tokens[1]);
                timeValueSlider.Maximum = totalSeconds;
                timeValueSlider.Value = 0;
                nameSongTb.Text = list_song[index].Name.Split(new string[] { "." },
                StringSplitOptions.None)[0];

                // play
                player.Play();
                timer.Start();

                // set index 
                Index = index;
            }
        }


        #region Button Click
        // previous song
        private void Anterior_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            if (kind_play == 0)
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == 0)
                {
                    return;
                }
                else
                {
                    index--;
                }

                PlayListView.SelectedIndex = index;

                PlaySongIndex(index);
            }
            else if(kind_play == 1)
            {
                PlayListView.SelectedIndex = Index;
                int index = rnd.Next(list_song.Count);
                PlayListView.SelectedIndex = index;
                PlaySongIndex(index);
            }
            else if(kind_play == 2)
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == 0)
                {
                    index = list_song.Count - 1;
                }
                else
                {
                    index--;
                }

                PlayListView.SelectedIndex = index; 

                PlaySongIndex(index);
            }
            else
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == 0)
                {
                    return;
                }
                else
                {
                    index--;
                }

                PlayListView.SelectedIndex = index;

                PlaySongIndex(index);
            }
        }

        // next song
        private void Proxima_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            if (kind_play == 0)
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == list_song.Count - 1)
                {
                    return;
                }
                else
                {
                    index++;
                }

                PlayListView.SelectedIndex = index;

                PlaySongIndex(index);
            }
            else if (kind_play == 1)
            {
                PlayListView.SelectedIndex = Index;

                if (Index == list_song.Count - 1)
                {
                    return;
                }

                int index = rnd.Next(list_song.Count);
                PlayListView.SelectedIndex = index;
                PlaySongIndex(index);
            }
            else if (kind_play == 2)
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == list_song.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }

                PlayListView.SelectedIndex = index;

                PlaySongIndex(index);
            }
            else
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == list_song.Count - 1)
                {
                    return;
                }
                else
                {
                    index++;
                }

                PlayListView.SelectedIndex = index;

                PlaySongIndex(index);
            }
        }

        // play again
        private void playAgainBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayListView.SelectedIndex = Index;
            int index = PlayListView.SelectedIndex;
            PlaySongIndex(index);
        }

        // kind play 
        private void kindPlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if(kind_play == 0)
            {
                KindPlay.Kind = MaterialDesignThemes.Wpf.PackIconKind.ShuffleVariant;
            }
            else if(kind_play == 1)
            {
                KindPlay.Kind = MaterialDesignThemes.Wpf.PackIconKind.Repeat;
            }
            else if(kind_play == 2)
            {
                KindPlay.Kind = MaterialDesignThemes.Wpf.PackIconKind.RepeatOne;
            }
            else
            {
                KindPlay.Kind = MaterialDesignThemes.Wpf.PackIconKind.Forward;
            }

            kind_play = (kind_play + 1) % 4;
        }

        private void addListBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list_song.Clear();
                path = diag.SelectedPath;  //selected folder path

                DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles();

                int i = 1;
                foreach(FileInfo file in Files)
                {
                    if (Utils.GetExtension(file.ToString()) == "mp3")
                    {
                        string name = file.ToString();

                        Mp3FileReader reader = new Mp3FileReader(path + '/' + file.ToString());
                        TimeSpan duration = reader.TotalTime;
                        int time = (int)duration.TotalSeconds;
                        string timeStr = $"{time / 60}:{time % 60}";

                        Song temp = new Song(); temp.STT = i++;temp.Name = name;temp.Time = timeStr;
                        list_song.Add(temp);
                    }
                }

                PlayListView.ItemsSource = list_song;
                PlayListView.SelectedIndex = 0;
                Index = 0;
                PlaySongIndex(Index);

                //pause
                timer.Stop();
                player.Pause();
            }
        }    

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Index == 0)
            {
                SetAmination();
            }

            if (!isPlaying)
            {
                if (list_song.Count == 0)
                {
                    System.Windows.MessageBox.Show("List song is empty !!!");
                    return;
                }

                player.Play();
                timer.Start();
                isPlaying = !isPlaying;

                // set icon 
                kindPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            else
            {
                // set icon 
                kindPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;

                // pause
                player.Pause();
                timer.Stop();
                isPlaying = !isPlaying;
            }
        }

        private void powerBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Timer and Player
        private void timer_Tick(object sender, EventArgs e)
        {
            if (player.Source != null)
            {
                var currentPos = player.Position.ToString(@"mm\:ss");
                startTimeTb.Text = currentPos.ToString();
                timeValueSlider.Value += 1;
            }
        }

        private void Next_Song(object sender, EventArgs e)
        {
            Random rnd = new Random();
            if (kind_play == 0)
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == list_song.Count - 1)
                {
                    return;
                }
                else
                {
                    index++;
                }

                PlayListView.SelectedIndex = index;

                PlaySongIndex(index);
            }
            else if (kind_play == 1)
            {
                PlayListView.SelectedIndex = Index;
                int index = rnd.Next(list_song.Count);

                if (Index == list_song.Count - 1)
                {
                    return;
                }

                PlayListView.SelectedIndex = index;
                PlaySongIndex(index);
            }
            else if (kind_play == 2)
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                if (index == list_song.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }

                PlayListView.SelectedIndex = index;

                PlaySongIndex(index);
            }
            else
            {
                PlayListView.SelectedIndex = Index;
                int index = PlayListView.SelectedIndex;
                PlaySongIndex(index);
            }
        }

        #endregion

        private void timeValueSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            int valueSeconds = (int)timeValueSlider.Value;
            TimeSpan temp = new TimeSpan(0, 0, valueSeconds);
            player.Position = temp;
        }

        private void PlayListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = PlayListView.SelectedIndex;
            PlaySongIndex(index);
        }

        private void SetAmination()
        {
            var doubleAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(10)));
            var rotateTransform = new RotateTransform();
            coverImage.RenderTransform = rotateTransform;
            coverImage.RenderTransformOrigin = new Point(0.5,0.5);
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);     
        }

        private void KeyUp_hook(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // tạm dừng/ chơi ctrl + shift + q
            if (e.Control && e.Shift && (e.KeyCode == Keys.Q))
            {
                if (!isPlaying)
                {
                    if (list_song.Count == 0)
                    {
                        System.Windows.MessageBox.Show("List song is empty !!!");
                        return;
                    }

                    player.Play();
                    timer.Start();
                    isPlaying = !isPlaying;

                    // set icon 
                    kindPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                }
                else
                {
                    // set icon 
                    kindPlayPause.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;

                    // pause
                    player.Pause();
                    timer.Stop();
                    isPlaying = !isPlaying;
                }
            }

            // previous
            if(e.Control && e.Shift && (e.KeyCode == Keys.W))
            {
                Random rnd = new Random();
                if (kind_play == 0)
                {
                    PlayListView.SelectedIndex = Index;
                    int index = PlayListView.SelectedIndex;
                    if (index == 0)
                    {
                        return;
                    }
                    else
                    {
                        index--;
                    }

                    PlayListView.SelectedIndex = index;

                    PlaySongIndex(index);
                }
                else if (kind_play == 1)
                {
                    PlayListView.SelectedIndex = Index;
                    int index = rnd.Next(list_song.Count);
                    PlayListView.SelectedIndex = index;
                    PlaySongIndex(index);
                }
                else if (kind_play == 2)
                {
                    PlayListView.SelectedIndex = Index;
                    int index = PlayListView.SelectedIndex;
                    if (index == 0)
                    {
                        index = list_song.Count - 1;
                    }
                    else
                    {
                        index--;
                    }

                    PlayListView.SelectedIndex = index;

                    PlaySongIndex(index);
                }
                else
                {
                    PlayListView.SelectedIndex = Index;
                    int index = PlayListView.SelectedIndex;
                    if (index == 0)
                    {
                        return;
                    }
                    else
                    {
                        index--;
                    }

                    PlayListView.SelectedIndex = index;

                    PlaySongIndex(index);
                }
            }

            // next
            if (e.Control && e.Shift && (e.KeyCode == Keys.E))
            {
                Random rnd = new Random();
                if (kind_play == 0)
                {
                    PlayListView.SelectedIndex = Index;
                    int index = PlayListView.SelectedIndex;
                    if (index == list_song.Count - 1)
                    {
                        return;
                    }
                    else
                    {
                        index++;
                    }

                    PlayListView.SelectedIndex = index;

                    PlaySongIndex(index);
                }
                else if (kind_play == 1)
                {
                    PlayListView.SelectedIndex = Index;

                    if (Index == list_song.Count - 1)
                    {
                        return;
                    }

                    int index = rnd.Next(list_song.Count);
                    PlayListView.SelectedIndex = index;
                    PlaySongIndex(index);
                }
                else if (kind_play == 2)
                {
                    PlayListView.SelectedIndex = Index;
                    int index = PlayListView.SelectedIndex;
                    if (index == list_song.Count - 1)
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }

                    PlayListView.SelectedIndex = index;

                    PlaySongIndex(index);
                }
                else
                {
                    PlayListView.SelectedIndex = Index;
                    int index = PlayListView.SelectedIndex;
                    if (index == list_song.Count - 1)
                    {
                        return;
                    }
                    else
                    {
                        index++;
                    }

                    PlayListView.SelectedIndex = index;

                    PlaySongIndex(index);
                }
            }
        }
    }
}
