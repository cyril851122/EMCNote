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
	/// Description of Note.
	/// </summary>
	public class Note
	{
		public Note()
		{
		}
		public String Title
		{
			get;set;
		}
		public String Content
		{
			get;set;
		}
		public DateTime ModifiedAt
		{
			get;set;
		}
		public Book BelongTo
		{
			get;set;
		}
		

	}
	
	public class NoteSorter:IComparer<Note>
	{
		public int Compare(Note x, Note y)
		{
			return -x.ModifiedAt.CompareTo(y.ModifiedAt);
		}
	}
}
