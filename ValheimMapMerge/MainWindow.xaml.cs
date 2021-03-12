using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ValheimMapMerge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 	
	public partial class MainWindow : Window
    {
        private long _worldId;
        private List<PlayerProfile> _profiles = new List<PlayerProfile>();
        public MainWindow()
        {
            InitializeComponent();          
        }

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                InitProfiles((string[])e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void InitProfiles(string [] files)
        {
            LoadProfilesFromDisk(files);
            if(LocateSharedWorld())
            {
                LogLine($"Shared world found with id {_worldId.ToString("X")}");                
                btnMerge.Visibility = Visibility.Visible;
                btnReset.Visibility = Visibility.Visible;
            }
            else
            {
                LogLine($"No shared world found for these profiles");                
                btnReset.Visibility = Visibility.Visible;
            }
        }

        private void LoadProfilesFromDisk(string[] files)
        {
            foreach (string file in files)
            {
                lbLog.Items.Add(new Label { Content = file, Padding = new Thickness(0) });
                PlayerProfile mpp = new PlayerProfile(file);
                if (!mpp.LoadPlayerFromDisk(out string error))
                {
                    lbLog.Items.Add(new Label { Content = error, Padding = new Thickness(0) });
                    return;
                }

                foreach (var w in mpp.m_worldData)
                {
                    LogLine($"world: {w.Key.ToString("X")}");
                }

                _profiles.Add(mpp);
            }
        }

        private bool LocateSharedWorld()
        {
            Dictionary<long, int> _worlds = new Dictionary<long, int>();           
            foreach(var pp in _profiles)
            {
                foreach (var w in pp.m_worldData)
                {
                    if (_worlds.ContainsKey(w.Key))                    
                        _worlds[w.Key]++;                    
                    else                    
                        _worlds.Add(w.Key, 1);                    
                }
            }
            _worldId = _worlds.SingleOrDefault(x => x.Value == _profiles.Count()).Key;
            return _worldId > 0;
        }

        public async Task Merge()
        {
            foreach (var p in _profiles)
            {
                if (p.MergeToDisk(_worldId, _profiles))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        LogLine($"Profile {p.m_filename} was merged");                        
                        btnMerge.Content = "Merge";
                        btnMerge.Visibility = Visibility.Collapsed;
                        btnClose.Visibility = Visibility.Visible;
                        btnReset.Visibility = Visibility.Collapsed;
                    }));                    
                }
                else
                {                    
                    Dispatcher.Invoke(new Action(() =>
                    {
                        LogLine($"Failed to merge Profile {p.m_filename}");                       
                        btnMerge.Content = "Merge";
                        btnMerge.Visibility = Visibility.Visible;
                    }));
                }
            }
        }

        private void LogLine(string line)
        {
            lbLog.Items.Add(new Label { Content = line, Padding = new Thickness(0) });

            lbLog.Items.MoveCurrentToLast();
            lbLog.ScrollIntoView(lbLog.Items.CurrentItem);
        }

        private void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            btnMerge.Content = "Please wait...";           
            var t = Task.Run(async () =>
            {                
                await Merge();
            }); 
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            btnReset.Visibility = Visibility.Collapsed;
            btnMerge.Visibility = Visibility.Collapsed;

            _worldId = 0;
            _profiles = new List<PlayerProfile>();
            lbLog.Items.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
