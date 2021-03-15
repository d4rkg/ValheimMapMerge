using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ValheimMapMerge.Helpers;

namespace ValheimMapMerge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 	
	public partial class MainWindow : Window
    {        
        private MapMerge _mapMerge;       
        public MainWindow()
        {
            InitializeComponent();
            _mapMerge = new MapMerge();
        }

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files)
                {
                    LogLine(f);
                }
                InitProfiles(files);
            }
        }

        private void InitProfiles(string[] files)
        {
            _mapMerge.LoadProfilesFromDisk(files);
            if(_mapMerge.LocateSharedWorld())
            {
                LogLine($"Shared world found with id {_mapMerge.GetWorldId().ToString("X")}");
                btnMerge.Visibility = Visibility.Visible;
                btnReset.Visibility = Visibility.Visible;
            }
            else
            {
                LogLine($"No shared world found for these profiles");
                btnReset.Visibility = Visibility.Visible;
            }
        }

        public async Task Merge(bool bMergePins)
        {
            if (_mapMerge.CreateMergedWorldWithPins(out string createError))
            {
                List<PlayerProfile> _profiles = _mapMerge.GetProfiles();
                foreach (var pp in _profiles)
                {                   
                    if (pp.SaveToDisk(_mapMerge.GetWorldId(), _mapMerge.GetMergedMapData(pp.GetPlayerId(), bMergePins)))
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            LogLine($"Profile {pp.m_filename} was merged");
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
                            LogLine($"Failed to merge Profile {pp.m_filename}");
                            btnMerge.Content = "Merge";
                            btnMerge.Visibility = Visibility.Visible;
                        }));
                    }
                }
            }
            else
            {
                LogLine(createError);
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
            bool bMergPins = chkMergePins.IsChecked.HasValue ? chkMergePins.IsChecked.Value : false;
            var t = Task.Run(async () =>
            {                
                await Merge(bMergPins);
            }); 
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            btnReset.Visibility = Visibility.Collapsed;
            btnMerge.Visibility = Visibility.Collapsed;
            
            lbLog.Items.Clear();
            _mapMerge.Reset();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
