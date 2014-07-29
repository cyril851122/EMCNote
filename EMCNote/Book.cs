/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 6/25/2014
 * Time: 2:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
namespace EMCNote
{
	/// <summary>
	/// Description of Book.
	/// </summary>
	public class Book
	{
		string path;
		int level;
		Book parent;
		BindingList<Note> allnoteitems;
		
		public Book(String Name)
		{
			initialize(Name);
			FindAndSetPath();
		}
		public Book(String Name, Book Parent)
		{
			initialize(Name);
			setParent(Parent);
			FindAndSetPath();
			
		}
		private void initialize(String Name)
		{
			this.Name=Name;
			this.BookItems =new BindingList<Book>();
			this.NoteItems =new BindingList<Note>();
			this.allnoteitems=new BindingList<Note>();
		}
		public String Name
		{
			get;set;
		}
		public int Level {
			get
			{
				return this.level;
			}
		}
		public bool HasChild
		{
			get{
				if(NoteItems.Count + BookItems.Count!=0)
				{
					return true;
				}else{
					return false;
				}
			}
		}
		
		public void UpdateAllNoteItems()
		{
			allnoteitems.Clear();
			AddSubNote(this);
		}
		private void AddSubNote(Book b)
		{
			foreach( Note n in b.NoteItems)
			{
				allnoteitems.Add(n);
			}
			foreach (Book subb in b.BookItems)
			{
				AddSubNote(subb);
			}
		}
		
		
		public BindingList<Note> AllNoteItems
		{
			get{
				UpdateAllNoteItems();
				return allnoteitems;
			}
			set{
				//UpdateAllNoteItems();
			}
		}
		public BindingList<Note> NoteItems
		{
			//---TODO--- Readonly//
			get;set;
		}

		public BindingList<Book> BookItems
		{
			//---TODO--- Readonly//
			get;set;
		}
		
		public Book Parent
		{
			get{
				return this.parent;
			}
		}
		public String Path
		{
			get { return path; }
		}
		
		private void setParent(Book parent)
		{
			this.parent=parent;
		}
		private void setLevel(int level)
		{
			this.level=level;
		}
		private void setPath(String path)
		{
			this.path=path;
		}
		private void Sort()
		{
			
		}
		private void FindAndSetPath()
		{
			
			String myPath="/";
			Book myBook=this;
			UInt16 depth=1;
			while(myBook.Parent!=null){
				depth++;
			
				myBook=myBook.Parent;
				myPath="/"+myBook.Name+myPath;
			}
			this.setLevel(depth);
			this.setPath(myPath);
		}
	}
	public class IndentConverter:System.Windows.Data.IValueConverter
	{
		private const int Indent=15;
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var level = System.Convert.ToInt16(value);
			return new System.Windows.Thickness(Indent* level -15, 0, 0, 0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo  culture)
		{
			throw new NotImplementedException();
		}
	}
}
