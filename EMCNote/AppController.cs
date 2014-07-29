/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 6/25/2014
 * Time: 2:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Linq;
namespace EMCNote
{
	/// <summary>
	/// Description of AppController.
	/// </summary>
	public sealed class AppController
	{
		private static AppController instance;
		private Profile current_profile;
		
		public static AppController GetInstance() {
			if (AppController.instance == null)
			{
				instance=new AppController();
			}
			return instance;
			
		}
		
		private AppController()
		{
			LoadDefaultProfile();
			
		}
		private void LoadDefaultProfile()
		{
			String filename=Application.StartupPath+"\\default.enp";
			if(File.Exists(filename))
			{
				current_profile=LoadProfile(filename);
			}else{
				current_profile=NewProfile(filename);
			}
			
		}
		private Profile NewProfile(String filename)
		{
			return new Profile(filename);
		}
		
		public void BindBookTree(System.Windows.Controls.TreeView tv)
		{
			tv.ItemsSource=(current_profile.BookItems);			
		}
		public void BindNoteList(System.Windows.Controls.ListView lv)
		{
			lv.ItemsSource=(current_profile.AllNotes);
			if(lv.HasItems==true)
			{
				lv.SelectedIndex=0;
			}
		}
		
		public void newNote(String Title, Book b)
		{
			current_profile.newNote(Title,b);
		}
		
		public void newBook(String Name, Book Parent)
		{
			current_profile.newBook(Name,Parent);
		}
		
		public void deleteBook(Book b)
		{
			try{
				current_profile.deleteBook(b);
			}catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		
		public void deleteNote(Note n)
		{
			try{
				current_profile.deleteNote(n);
				
			}catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		
		public void newBook(String Name)
		{
			current_profile.newBook(Name);
		}
		
		public Profile LoadProfile(String filename)
		{
			
			Profile myprofile=new Profile(filename);
			try{
				myprofile.Helper.Load();
			}
			catch(Exception e)
			{
				MessageBox.Show("Load Profile Error:\r\n"+e.Message,"Load Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			return myprofile;
		}
		public void SaveDefaultProfile()
		{
			String filename=Application.StartupPath+"\\default.enp";
			SaveProfile(filename);
		}
		public void SaveProfile(String filename)
		{
			current_profile.Helper.Save();
		}
	}
}
