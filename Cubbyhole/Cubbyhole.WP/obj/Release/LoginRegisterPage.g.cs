﻿#pragma checksum "C:\Users\koenigs\Documents\Visual Studio 2012\Projects\Cubbyhole\Cubbyhole.WP\LoginRegisterPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5453874BBCE0F9C71685394C6AE892FA"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.34003
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Cubbyhole.WP {
    
    
    public partial class LoginRegisterPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.ProgressBar ProgressBarDataLoaded;
        
        internal System.Windows.Controls.TextBox LoginEmailTextBox;
        
        internal System.Windows.Controls.TextBox LoginPasswordTextBox;
        
        internal System.Windows.Controls.Button LoginButton;
        
        internal System.Windows.Controls.TextBlock Email;
        
        internal System.Windows.Controls.TextBox RegisterEmailTextBox;
        
        internal System.Windows.Controls.PasswordBox RegisterPasswordBox;
        
        internal System.Windows.Controls.PasswordBox RegisterPasswordBoxConfirmation;
        
        internal System.Windows.Controls.ListBox PlanListBox;
        
        internal System.Windows.Controls.Button RegisterButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Cubbyhole.WP;component/LoginRegisterPage.xaml", System.UriKind.Relative));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ProgressBarDataLoaded = ((System.Windows.Controls.ProgressBar)(this.FindName("ProgressBarDataLoaded")));
            this.LoginEmailTextBox = ((System.Windows.Controls.TextBox)(this.FindName("LoginEmailTextBox")));
            this.LoginPasswordTextBox = ((System.Windows.Controls.TextBox)(this.FindName("LoginPasswordTextBox")));
            this.LoginButton = ((System.Windows.Controls.Button)(this.FindName("LoginButton")));
            this.Email = ((System.Windows.Controls.TextBlock)(this.FindName("Email")));
            this.RegisterEmailTextBox = ((System.Windows.Controls.TextBox)(this.FindName("RegisterEmailTextBox")));
            this.RegisterPasswordBox = ((System.Windows.Controls.PasswordBox)(this.FindName("RegisterPasswordBox")));
            this.RegisterPasswordBoxConfirmation = ((System.Windows.Controls.PasswordBox)(this.FindName("RegisterPasswordBoxConfirmation")));
            this.PlanListBox = ((System.Windows.Controls.ListBox)(this.FindName("PlanListBox")));
            this.RegisterButton = ((System.Windows.Controls.Button)(this.FindName("RegisterButton")));
        }
    }
}
