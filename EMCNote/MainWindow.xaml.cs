/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 5/28/2014
 * Time: 4:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using EMCNote;

namespace EMCNote
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenu notificationMenu;
		AppController appctr;
		
		private void WindowMouseDown(object sender, MouseEventArgs e)
		{
			this.DragMove();
	   	}
		private void WindowResize(object sender,SizeChangedEventArgs e)
		{

		}
		public MainWindow()
		{
			InitializeComponent();
			this.Closing += WindowHide;
			this.Closed+=WindowClose;
			this.StateChanged+=WindowShowHide;
			this.MouseLeftButtonDown+=WindowMouseDown;
			this.SizeChanged+=WindowResize;
			this.Loaded+=WindowLoad;
			NotifyIcon();
		}
		

		
		private void WindowLoad(object sender, RoutedEventArgs e)
		{
			// do something
			appctr=AppController.GetInstance();
			appctr.BindBookTree(tv_book);
		}
		
		private void SelectBook(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi=e.OriginalSource as TreeViewItem;
			Book selectedBook=tvi.Header as Book;
			lv_note.ItemsSource=selectedBook.NoteItems;
			
			
			
			
			
		}
		
		private void WindowHide(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel=true;
			this.Hide();
			this.WindowState=WindowState.Minimized;
		}
		
		private void WindowShowHide(object sender, EventArgs e)
		{
			if(this.WindowState==WindowState.Minimized){
				this.ShowInTaskbar=false;
			}else{
				this.ShowInTaskbar=true;
			}
		}
		
		private void WindowClose(object sender, EventArgs e)
		{
			notifyIcon.Dispose();
			notificationMenu.Dispose();
		}
		
		private void NotifyIcon(){
			notifyIcon = new System.Windows.Forms.NotifyIcon();
			notificationMenu = new System.Windows.Forms.ContextMenu(InitializeMenu());
			notifyIcon.DoubleClick += menuMainClick;
			Icon ico=new Icon(System.Reflection.Assembly.Load("EMCNote").GetManifestResourceStream("EMCNote.res.N.ico"));
			notifyIcon.Icon = ico;
			notifyIcon.ContextMenu = notificationMenu;
			notifyIcon.Visible=true;
		}
		
		private System.Windows.Forms.MenuItem[] InitializeMenu()
		{
			System.Windows.Forms.MenuItem[] menu = new System.Windows.Forms.MenuItem[] {
				new System.Windows.Forms.MenuItem("EMC Note", menuMainClick),
				new System.Windows.Forms.MenuItem("Exit", menuExitClick)
			};
			menu[0].DefaultItem=true;
			return menu;
		}
		
		private void menuMainClick(object sender, EventArgs e)
		{
			if(this.WindowState == WindowState.Minimized)
			{
				this.Show();
				this.WindowState=WindowState.Normal;
				
			}else{
				this.Hide();
				this.WindowState=WindowState.Minimized;
			}
			
		}
		
		private void menuExitClick(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
			notificationMenu.Dispose();
			notifyIcon.Dispose();
		}
		void menuNewNoteClick(object sender, System.Windows.RoutedEventArgs e)
		{

		}
		void SelectNote(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			ListView lv= e.OriginalSource as ListView;
			Note n = lv.SelectedItem as Note;
			MessageBox.Show(n.Brief);
		}
	}
	
}