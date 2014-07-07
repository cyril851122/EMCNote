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
using System.Windows.Documents;
using System.Collections.Generic; 

namespace EMCNote
{
	/// <summary>
	/// Description of Note.
	/// </summary>
	public class Note: INotifyPropertyChanged
	{
		Book notebook;
		private String content;
		private String title;
		public Note(String title, Book notebook)
		{
			this.Title=title;
			this.SetBook(notebook);
		}
		private void SetBook(Book notebook)
		{
			this.notebook=notebook;
			notebook.NoteItems.Add(this);
		}
		public String Title
		{
			get
			{
				return title;
			}
			set
			{
				title=value; OnPropertyChanged(new PropertyChangedEventArgs("Title"));;
			}
		}
		public FlowDocument Document
		{
			get;set;
		}
		public String Content
		{
			set{
				this.content=value;OnPropertyChanged(new PropertyChangedEventArgs("Brief"));
			}
			get{
				return this.content;
			}
		}
		public DateTime ModifiedAt
		{
			get;set;
		}
		public Book NoteBook
		{
			get
			{
				return this.notebook;
			}
		}
		public String Brief
		{
			get
			{
				String pure=System.Text.RegularExpressions.Regex.Replace(Content,"\\<\\/?[\\w]+\\>","");
				pure=System.Text.RegularExpressions.Regex.Replace(pure,"\\r|\\n|\\t","");
				return pure.Substring(0,pure.Length>100?100:pure.Length);
			}
		}
		
		#region INotifyPropertyChanged Members
		
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}
		#endregion

	}
	
	public class NoteSorter:IComparer<Note>
	{
		public int Compare(Note x, Note y)
		{
			return -x.ModifiedAt.CompareTo(y.ModifiedAt);
		}
	}
}
