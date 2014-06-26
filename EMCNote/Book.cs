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
		Book parent;
		public Book(String Name)
		{
			this.Name=Name;
			FindAndSetPath();
		}
		public Book(String Name, Book Parent)
		{
			this.Name=Name;
			setParent(Parent);
			FindAndSetPath();
		}
		public String Name
		{
			get;set;
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
				if(depth>256){
					throw new Exception("The depth of Book had exceeded 256."); //by design
				}
				myBook=myBook.Parent;
				myPath="/"+myBook.Name+myPath;
			}
			this.setPath(myPath);
		}
	}
}
