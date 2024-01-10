using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace CheckSumTerminal.IView
{
    public interface ICreateDescriptionWindow
    {
        BackgroundWorker BackgroundWorker { get; set; }
        ProgressBar ProgressBar { get; set; }
        RichTextBox RichTextBox { get; set; }
        string NumberOfVersion { get; set; }
        List<string> FileNames { get; set; }
        Dictionary<string, int> FilesToUpdate { get; set; }
        Dictionary<string, int> FilesToDelete { get; set; }
        TextBlock WhatHppend { get; set; }

        event EventHandler<DoWorkEventArgs> DoWork;
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        event EventHandler Start;
        event EventHandler Complete;

        void BlockControls();
        void Close();
    }
}