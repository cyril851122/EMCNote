﻿/*
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
		
		private Note selectedNote;
		private Book selectedBook;
		
		
		
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
			
			DataObject.AddPastingHandler(rtb_note,rtb_note_Paste);
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
			if(selectedBook!=null)
			{
				appctr.newNote("New Note",selectedBook);
			}else{
				MessageBox.Show("You must select a notebook first.");
			}
		}
		void menuNewBookClick(object sender, System.Windows.RoutedEventArgs e)
		{
			
			NewBook n=new NewBook();
			n.Owner=this;
			if(n.ShowDialog()==true)
			{
				String bookname=n.tb_bookname.Text;
				if(selectedBook!=null)
				{
					appctr.newBook(bookname,selectedBook);
				}else{
					appctr.newBook(bookname);
				}
			}
			
		}
		
		void SelectBook(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi=e.OriginalSource as TreeViewItem;
			selectedBook=tvi.Header as Book;
			
			lv_note.ItemsSource=selectedBook.NoteItems;

		}
		
		void SelectNote(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			
			//Save First
			update_note_source();
			// load new
			ListView lv= e.OriginalSource as ListView;
			Note n = lv.SelectedItem as Note;
			if(n!=null)
			{
				selectedNote=n;
				if(n.Document!=null)
				{
					rtb_note.Document=n.Document;
				}else
				{
					rtb_note.Document=new FlowDocument();
				}
				tb_title.Text=n.Title;
				grid_noteview.Visibility=Visibility.Visible;
			}else
			{
				selectedNote =null;
				grid_noteview.Visibility=Visibility.Hidden;
			}
		}
		void SaveNoteClick(object sender, RoutedEventArgs e)
		{
			appctr.SaveDefaultProfile();

		}
		void Tb_title_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			update_note_source();
		}
		void Rtb_note_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			update_note_source();
		}
		
		void update_note_source()
		{
			if(selectedNote !=null)
			{
				selectedNote.Document=rtb_note.Document;
				System.Xml.XmlDocument xd=new System.Xml.XmlDocument();
				xd.LoadXml(System.Windows.Markup.XamlWriter.Save(rtb_note.Document));
				selectedNote.Content=xd.InnerText;
				selectedNote.Title=tb_title.Text;
			}
		}

		
		
		
		
		
		void deleteBookClick(object sender, RoutedEventArgs e)
		{
			if (selectedBook !=null)
			{
				String msg="";
				if(selectedBook.HasChild)
				{
					msg="The Notebook is not empty. ";
				}
				msg+="Are you sure you want to delete the Notebook: <"+selectedBook.Name+">?";
				if (MessageBox.Show(msg,"Confirm",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
				{
					appctr.deleteBook(selectedBook);
				}
			}else{
				MessageBox.Show("You didn't select any notebook.");
			}
		}
		void deleteNoteClick(object sender, RoutedEventArgs e)
		{
			if(selectedNote != null)
			{
				String msg="Are you sure you want to delete the Note: <"+selectedNote.Title+">?";
				if (MessageBox.Show(msg,"Confirm",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
				{
					appctr.deleteNote(selectedNote);
				}
			}else{
				MessageBox.Show("You didn't select any note.");
			}
		}
		void deleteButtonClick(object sender, RoutedEventArgs e)
		{
			Button b=e.OriginalSource as Button;
			b.ContextMenu.PlacementTarget=b;
			b.ContextMenu.Placement=System.Windows.Controls.Primitives.PlacementMode.Bottom;
			b.ContextMenu.IsOpen=true;
			
		}

		void changeColor(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.ColorDialog cd =new System.Windows.Forms.ColorDialog();
			cd.ShowDialog();
			TextRange range = new TextRange(rtb_note.Selection.Start, rtb_note.Selection.End);
			range.ApplyPropertyValue(FlowDocument.ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(cd.Color.A,cd.Color.R,cd.Color.G,cd.Color.B)));
			
		}
		
		void rtb_note_Paste(object sender, DataObjectPastingEventArgs e)
		{
			e.Handled = true;
			e.CancelCommand();
			TextRange range = new TextRange(rtb_note.Selection.Start, rtb_note.Selection.End);
			if(Clipboard.ContainsImage())
			{
				System.Windows.Media.Imaging.BitmapSource img=Clipboard.GetImage();
				String s=Utility.ImageSourceToBase64String(img);
				rtb_note.AppendText(s);
				
			}
			
			
			range.Text=Clipboard.GetText();
			rtb_note.Selection.Select(rtb_note.Selection.End,rtb_note.Selection.End);
		}
	}
	
}