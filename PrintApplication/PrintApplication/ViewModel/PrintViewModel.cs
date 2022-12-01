using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.IO;
using System;
using PrintApplication;
using PrintApplication.Model;
using Microsoft.Win32;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.Text;
using System.Threading;

namespace PrintApplication.ViewModel
{
    public class PrintViewModel : INotifyPropertyChanged
    {
        public string fileName = Directory.GetCurrentDirectory() + "\\PrintText";
        public static string InputName;
        public double sizeKb;
        private string filePath;
        private bool isLoading = false;
        Thread th1;
        ThreadStart ths1;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        private int currentProgress = 0;
        public int CurrentProgress
        {
            get { return currentProgress; }
            private set
            {
                if (currentProgress != value)
                {
                    currentProgress = value;
                    OnPropertyChanged("CurrentProgress");
                }
            }
        }
        private int maxValue = 1;

        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }
        private bool Abort_visibility;

        public bool AbortVisibility
        {
            get { return Abort_visibility; }
            set
            {
                Abort_visibility = value;
                OnPropertyChanged("AbortVisibility");
            }
        }
        private bool print_visibility;

        public bool PrintVisibility
        {
            get { return print_visibility; }
            set
            {
                print_visibility = value;
                OnPropertyChanged("PrintVisibility");
            }
        }
        private bool pause_visibility;

        public bool PauseVisibility
        {
            get { return pause_visibility; }
            set
            {
                pause_visibility = value;
                OnPropertyChanged("PauseVisibility");
            }
        }
        private bool resume_visibility;

        public bool ResumeVisibility
        {
            get { return resume_visibility; }
            set
            {
                resume_visibility = value;
                OnPropertyChanged("ResumeVisibility");
            }
        }

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged("FilePath");
            }
        }
        public ICommand BrowseImage
        {
            get;
            set;
        }
        public ICommand PrintImageCommand
        {
            get;
            set;
        }
        public ICommand OKCloseCommand
        {
            get;
            set;
        }
        public Action CloseWindowAction
        {
            get;
            set;
        }
        public ICommand PauseImageCommand
        {
            get;
            set;
        }
        public ICommand AbortImageCommand
        {
            get;
            set;
        }
        public ICommand ResumeImageCommand
        {
            get;
            set;
        }
        private bool CanSelectCabinets(object arg)
        {
            return true;
        }
        public PrintViewModel()
        {

            BrowseImage = new DelegateCommand(BrowseImageFile, CanSelectCabinets);
            PrintImageCommand = new DelegateCommand(PrintImage, CanSelectCabinets);
            AbortImageCommand = new DelegateCommand(AbortImage, CanSelectCabinets);
            PauseImageCommand = new DelegateCommand(PauseImage, CanSelectCabinets);
            ResumeImageCommand = new DelegateCommand(ResumeImage, CanSelectCabinets);
            OKCloseCommand = new DelegateCommand(CloseApplicationOK, CanSelectCabinets);
        }

        public void AbortImage(object obj)
        {
            if (th1.ThreadState == ThreadState.Running || th1.ThreadState == ThreadState.WaitSleepJoin)
            {
                th1.Abort();
                AbortVisibility = false;
                PauseVisibility = false;
                ResumeVisibility = false;
            }
        }
        public void PauseImage(object obj)
        {
            if (th1.ThreadState.Equals(ThreadState.Running) || th1.ThreadState == ThreadState.WaitSleepJoin)
            {
                th1.Suspend();
                PauseVisibility = false;
                ResumeVisibility = true;
                AbortVisibility = false;
            }
            else
            {
                System.Windows.MessageBox.Show("Thread is not user-suspended; it cannot be resumed.");
            }
        }
        public void ResumeImage(object obj)
        {
            if (th1.ThreadState == ThreadState.Suspended || th1.ThreadState == ThreadState.Stopped)
            {
                th1.Resume();
                PauseVisibility = true;
                ResumeVisibility = false;
                AbortVisibility = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Thread is not user-suspended; it cannot be resumed.");
            }
        }
        public void PrintImage(Object obj)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                if (th1.ThreadState != ThreadState.Running || th1.ThreadState != ThreadState.WaitSleepJoin)
                {
                    th1.Start();
                    IsLoading = true;
                    AbortVisibility = true;
                    PrintVisibility = false;
                    PauseVisibility = true;
                    // ResumeVisibility = true;
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        private void PrintText()
        {
            MaxValue = (int)sizeKb;
            using (FileStream fs = File.Create(fileName))
            {
                // Add some text to file    
                double size;
                int Scans = (int)sizeKb / 100;
                double percent = (double)100 / Scans;
                double Addvalue = percent;
                for (int i = 1; i <= Scans; i++)
                {
                    if (i == Scans)
                    {
                        size = Math.Ceiling(percent);
                        if (size > 100)
                        { size = 100.00; }
                    }
                    else
                    {
                        size = percent;
                    }

                    Byte[] title = new UTF8Encoding(true).GetBytes(InputName + "Completed" + "  ");
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes(size + "%" + "---" + DateTime.Now + Environment.NewLine);
                    fs.Write(author, 0, author.Length);
                    CurrentProgress = i * 100;
                    percent = percent + Addvalue;
                    System.Threading.Thread.Sleep(200);

                }
            }
            System.Windows.MessageBox.Show("Done");
            AbortVisibility = false;
            PauseVisibility = false;
            IsLoading = false;
            th1.Abort();

        }

        public void CloseApplicationOK(object obj)
        {
            if (th1 != null)
            {
                if ((th1.ThreadState == ThreadState.Running) && !(th1.ThreadState == ThreadState.Aborted))
                    th1.Suspend();
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(InputName + " image is printing do you want to stop printing? ", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //th1.Abort();
                    App.Current.Shutdown();
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do Nothing
                    if (th1.ThreadState == ThreadState.Suspended || th1.ThreadState == ThreadState.Stopped)
                        th1.Resume();
                    ResumeVisibility = false;
                    AbortVisibility = true;
                    if (th1.ThreadState == ThreadState.Running)
                        PauseVisibility = true;
                }
            }
            else
            {
                App.Current.Shutdown();
            }
        }
        public void BrowseImageFile(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select a Image file";
            //dlg.Filter = "Image file (*.PNG)|*.png";
            dlg.ShowDialog();

            var extension = Path.GetExtension(dlg.FileName).ToUpper();

            if (extension == ".PNG" || extension == ".JPG" || extension == ".JPEG")
            {
                FilePath = dlg.FileName;
                InputName = Path.GetFileName(FilePath);
                FileInfo fileInfo = new FileInfo(FilePath);
                double fileSizeKB = fileInfo.Length / 1024;
                sizeKb = fileSizeKB;
                double fileSizeMB = fileInfo.Length / (1024 * 1024);
                PrintVisibility = true;
                AbortVisibility = false;
                ResumeVisibility = false;
                PauseVisibility = false;
                ThreadStart childref = new ThreadStart(PrintText);
                Thread km = new Thread(childref);
                th1 = km;
            }
            else
            {
                System.Windows.MessageBox.Show("Please upload a Image File");
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
