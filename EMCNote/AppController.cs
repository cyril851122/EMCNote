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

		public Profile LoadProfile(String filename)
		{
			
			Profile myprofile=new Profile(filename);
			try{
				myprofile.Helper.Load();
			}
			catch(Exception e)
			{
				MessageBox.Show("Cannot load profile: "+e.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				
			}
			return myprofile;
		}
		public void SaveProfile(String filename)
		{
			
		}
		public void BuildTreeView()
		{
			
		}
	}
}
