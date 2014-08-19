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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using HTMLConverter;

using ZZNote;

namespace ZZNote
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 
	public partial class MainWindow : Window
	{
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenu notificationMenu;

		private Note selectedNote;
		private Book selectedBook;
		AppController appctr;
		public static MainWindow instance;
		HotKey k;
		
		private void WindowMouseDown(object sender, MouseEventArgs e)
		{
			this.DragMove();
		}
		private void WindowResize(object sender,SizeChangedEventArgs e)
		{

		}
		public MainWindow()
		{
			MainWindow.instance=this;
			InitializeComponent();
			this.Closing += WindowHide;
			this.Closed+=WindowClose;
			this.StateChanged+=WindowShowHide;
			//this.MouseLeftButtonDown+=WindowMouseDown;
			this.SizeChanged+=WindowResize;
			this.Loaded+=WindowLoad;
			NotifyIcon();
			this.k= new HotKey(Key.Q, KeyModifier.Ctrl | KeyModifier.Alt, OnHotKeyHandler);
		}
		
		private void OnHotKeyHandler(HotKey hotKey)
		{
			menuMainClick(hotKey,new EventArgs());
		}
		
		private void WindowLoad(object sender, RoutedEventArgs e)
		{
			// do something
			appctr=AppController.GetInstance();
			appctr.BindBookTree(tv_book);
			appctr.BindNoteList(lv_note);
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
			Icon ico=new Icon(System.Reflection.Assembly.Load("ZZNote").GetManifestResourceStream("ZZNote.res.N.ico"));
			notifyIcon.Icon = ico;
			notifyIcon.ContextMenu = notificationMenu;
			notifyIcon.Visible=true;
		}
		
		
		private System.Windows.Forms.MenuItem[] InitializeMenu()
		{
			System.Windows.Forms.MenuItem[] menu = new System.Windows.Forms.MenuItem[] {
				new System.Windows.Forms.MenuItem("ZZNote", menuMainClick),
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
		
		private void AboutClick(object sender, EventArgs e)
		{
			
		}
		
		private void menuExitClick(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
			notificationMenu.Dispose();
			notifyIcon.Dispose();
		}
		void menuNewNoteClick(object sender, System.Windows.RoutedEventArgs e)
		{
			selectedBook =tv_book.SelectedItem as Book;
			if(selectedBook!=null)
			{
				appctr.newNote("New Note",selectedBook);
				selectedBook.AllNoteItems=selectedBook.AllNoteItems;
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
			lv_note.ItemsSource=selectedBook.AllNoteItems;
			
			
			
		}
		
		void SelectNote(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (lv_note.SelectedIndex == -1)
			{
				return;
			}
			//Save First
			update_note_source();
			// load new
			
			Note n = lv_note.SelectedItem as Note;
			if(n!=null)
			{
				selectedNote=n;
				ShowNote(n);
				rtb_note.Focus();
				
			}else
			{
				selectedNote =null;
				grid_noteview.Visibility=Visibility.Hidden;
			}
		}
		
		void ShowNote(Note n)
		{
			if(n!=null){
				if(n.Document!=null)
				{
					rtb_note.Document=n.Document;
				}else
				{
					rtb_note.Document=new FlowDocument();
				}
				tb_title.Text=n.Title;
				grid_noteview.Visibility=Visibility.Visible;
			}else{
				grid_noteview.Visibility=Visibility.Hidden;
			}
		}
		void SaveNoteClick(object sender, RoutedEventArgs e)
		{
			update_note_source();
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
				TextRange range=new TextRange(rtb_note.Document.ContentStart,rtb_note.Document.ContentEnd);
				selectedNote.Content=range.Text;
				selectedNote.Title=tb_title.Text;
			}
		}


		void deleteBookClick(object sender, RoutedEventArgs e)
		{
			selectedBook=tv_book.SelectedItem as Book;
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
				appctr.BindNoteList(lv_note);
				selectedBook=null;
			}else{
				MessageBox.Show("You didn't select any notebook.");
			}
		}
		void deleteNoteClick(object sender, RoutedEventArgs e)
		{
			if(lv_note.SelectedItems.Count>0){
				String msg="Are you sure you want to delete "+lv_note.SelectedItems.Count.ToString()+ " notes?";
				if (MessageBox.Show(msg,"Confirm",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
				{
					foreach (Note n in lv_note.SelectedItems)
					{
						appctr.deleteNote(n);
					}
				}
				if(selectedBook!=null)
				{
					selectedBook.AllNoteItems=selectedBook.AllNoteItems;
				}else{
					appctr.BindNoteList(lv_note);
					selectedNote=null;
					ShowNote(selectedNote);
				}
			}else{
				
				MessageBox.Show("You didn't select any note.");
			}
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
			
			TextRange range = new TextRange(rtb_note.Selection.Start, rtb_note.Selection.End);
			range.Text="";
			if(Clipboard.ContainsImage())
			{
				e.Handled = true;
				e.CancelCommand();
				System.Windows.Media.Imaging.BitmapSource img=Clipboard.GetImage();
				if(selectedNote==null)
				{
					throw new Exception("Not selecting any Note.");
				}
				Int32 id=0;
				while(selectedNote.Attachments.ContainsKey(id))
				{
					id++;
				}
				selectedNote.Attachments.Add(id ,img);
				System.Windows.Controls.Image i=new System.Windows.Controls.Image();
				i.Width=img.Width;
				i.Height=img.Height;
				i.Source=img;
				i.Tag=id;
				InlineUIContainer iuc=new InlineUIContainer(i,rtb_note.CaretPosition);
			}else if(Clipboard.ContainsData("HTML Format"))
			{
				e.Handled = true;
				e.CancelCommand();
				String HtmlCode=Clipboard.GetData("HTML Format").ToString();
				HtmlCode=HtmlCode.Replace("&nbsp;"," ");
				HtmlCode=HtmlCode.Replace('\r',' ');
				HtmlCode=HtmlCode.Replace('\n',' ');
				
				HtmlCode=Regex.Match(HtmlCode,"<html.*$",RegexOptions.Multiline|RegexOptions.IgnoreCase).Value;
				
				String xamlsource=HtmlToXamlConverter.ConvertHtmlToXaml(HtmlCode,false);
				TextElement elem=System.Windows.Markup.XamlReader.Parse(xamlsource) as TextElement;
				//
				foreach (System.Windows.Controls.Image img in Utility.FindImages(elem))
				{
					Int32 id=0;
					while(selectedNote.Attachments.ContainsKey(id))
					{
						id++;
					}
					img.Tag=id;
					selectedNote.Attachments.Add(id ,img.Source as System.Windows.Media.Imaging.BitmapSource);
					
				}
				
				if (elem is Inline)
				{
					TextPointer tp=rtb_note.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
					Span s=new Span(elem as Span,tp);
				}else if (elem is Block)
				{
					rtb_note.CaretPosition.InsertParagraphBreak();
					rtb_note.Document.Blocks.InsertAfter( rtb_note.CaretPosition.Paragraph,elem as Section);
				}
			}else if(Clipboard.ContainsData("XamlPackage"))
			{
				e.Handled = true;
				e.CancelCommand();
				System.IO.MemoryStream ms=Clipboard.GetData("XamlPackage") as System.IO.MemoryStream;
				ms.Position=0;
				range.Load(ms,"XamlPackage");
				foreach (System.Windows.Controls.Image img in Utility.FindImages(rtb_note.Document))
				{
					if(img.Tag==null)
					{
						Int32 id=0;
						while(selectedNote.Attachments.ContainsKey(id))
						{
							id++;
						}
						img.Tag=id;
						selectedNote.Attachments.Add(id ,img.Source as System.Windows.Media.Imaging.BitmapSource);
					}
				}
				
			}
			Debug.Print(String.Join(",",Clipboard.GetDataObject().GetFormats()));
			range.Select(range.End,range.End);
		}
		
		private void UnselectTreeViewItem(TreeView pTreeView)
		{
			if(pTreeView.SelectedItem == null)
				return;

			if(pTreeView.SelectedItem is TreeViewItem)
			{
				(pTreeView.SelectedItem as TreeViewItem).IsSelected = false;
			}
			else
			{
				TreeViewItem item = pTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
				if (item != null)
				{
					item.IsSelected = true;
					item.IsSelected = false;
				}
			}
			selectedBook=null;
		}

		void Tv_book_MouseUp(object sender, MouseButtonEventArgs e)
		{
			
			if(e.ChangedButton==MouseButton.Left)
			{
				e.Handled=true;
				if( !(e.OriginalSource is Border)&&!(e.OriginalSource is System.Windows.Controls.Image) && !(e.OriginalSource is TextBlock))
				{
					UnselectTreeViewItem(tv_book);
					appctr.BindNoteList(lv_note);
				}
			}
			
		}
	}
	public partial class MenuTemplate : ResourceDictionary
	{

		public void AboutClick(object sender, RoutedEventArgs e)
		{
			About about_win=new About();
			about_win.Owner=MainWindow.instance;
			about_win.ShowDialog();
		}

	}
	
	public partial class TreeViewTemplate : ResourceDictionary
	{
		public void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs  e)
		{
			
			var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
			
			if (treeViewItem != null)
			{
				treeViewItem.IsSelected=true;
				treeViewItem.Focus();
				e.Handled = true;
			}
		}
		static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
		{
			while (source != null && source.GetType() != typeof(T))
				source = VisualTreeHelper.GetParent(source);

			return source;
		}
	}
}