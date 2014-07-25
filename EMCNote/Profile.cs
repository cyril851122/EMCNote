/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 6/25/2014
 * Time: 2:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace EMCNote
{
	/// <summary>
	/// Description of Profile.
	/// </summary>
	public class Profile
	{
		private String filename;
		private ProfileHelper helper;
		private  BindingList<Note> allnotes;
		public Profile(String filename)
		{
			this.filename=filename;
			this.helper=new ProfileHelper(this);

			this.BookItems=new BindingList<Book>();
			this.allnotes=new BindingList<Note>();
		}
		public ProfileHelper Helper
		{
			get{
				return this.helper;
			}
		}
		public String ProfileName
		{
			get;set;
		}
		public String FileName
		{
			get{
				return this.filename;
			}
		}
		public BindingList<Book> BookItems
		{
			get;set;
		}
		private void UpdateAllNotes()
		{
			this.allnotes.Clear();
			foreach(Book b in BookItems)
			{
				AddSubNote(b);
			}
		}
		private void AddSubNote(Book b)
		{
			foreach (Note n in b.NoteItems)
			{
				this.allnotes.Add(n);
			}
			foreach (Book subb in b.BookItems)
			{
				AddSubNote(subb);
			}
		}
		
		public BindingList<Note> AllNotes
		{
			get{
				UpdateAllNotes();
				return this.allnotes;
			}
			set{
				UpdateAllNotes();
			}
		}

		
		public void deleteBook(Book b)
		{
			if(b.Parent!=null)
			{
				b.Parent.BookItems.Remove(b);
			}else{
				if(this.BookItems.Contains(b))
				{
					this.BookItems.Remove(b);
				}else{
					throw new Exception("Cannot locate the selected notebook.");
				}
			}
		}
		
		public void deleteNote(Note n)
		{
			if(n.NoteBook.NoteItems.Contains(n))
			{
				n.NoteBook.NoteItems.Remove(n);
			}else{
					throw new Exception("Cannot locate the selected note.");
			}
		}
		
		public Note newNote(String title,Book Parent)
		{
			Note n=new Note(title,Parent);
			return n;
		}
		public Book newBook(String Name)
		{
			Book mybook=new Book(Name);
			BookItems.Add(mybook);
			return mybook;
		}
		public Book newBook(String Name, Book Parent)
		{
			Book mybook=new Book(Name,Parent);
			Parent.BookItems.Add(mybook);
			return mybook;
		}
		public Book FindBookByPath(String Path)
		{
			return null;
		}
		public Note FindNoteByPath(String Path)
		{
			return null;
		}
		public void Clear()
		{
			this.BookItems.Clear();
			GC.Collect();
		}
		
	}
}
