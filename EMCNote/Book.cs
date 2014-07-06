/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 6/25/2014
 * Time: 2:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
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
		public Book(String Name)
		{
			this.Name=Name;
			this.BookItems =new List<Book>();
			this.NoteItems =new List<Note>();
			FindAndSetPath();
		}
		public Book(String Name, Book Parent)
		{
			this.Name=Name;
			this.BookItems =new List<Book>();
			this.NoteItems =new List<Note>();
			setParent(Parent);
			FindAndSetPath();
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
		public List<Note> NoteItems
		{
			//---TODO--- Readonly//
			get;set;
			
		}
		public List<Book> BookItems
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
			NoteItems.Sort(new NoteSorter());
		}
		private void FindAndSetPath()
		{
			
			String myPath="/";
			Book myBook=this;
			UInt16 depth=1;
			while(myBook.Parent!=null){
				depth++;
				if(depth>3){
					throw new Exception("The depth of Book had exceeded 3."); //by design
				}
				myBook=myBook.Parent;
				myPath="/"+myBook.Name+myPath;
			}
			this.setLevel(depth);
			this.setPath(myPath);
		}
	}
	public class IndentConverter:System.Windows.Data.IValueConverter 
    { 
		private const int Indent=20;
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
