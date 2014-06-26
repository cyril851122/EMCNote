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
using System.Windows.Forms;
namespace EMCNote
{
	/// <summary>
	/// Description of AppController.
	/// </summary>
	public sealed class AppController
	{
		private static AppController instance = new AppController();
		private Profile current_profile;
		
		public static AppController Instance {
			get {
				return instance;
			}
		}
		
		private AppController()
		{
			LoadDefaultProfile();
		}
		private void LoadDefaultProfile()
		{
			String filename=App.Current.StartupUri.AbsolutePath+"\\default.enp";
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
		public Profile LoadProfile(String filename)
		{
			//--TODO-- Load Profile from file
			Profile myprofile=new Profile(filename);
			try{
				myprofile.Helper.Load();
			}
			catch(Exception e)
			{
				MessageBox.Show("Cannot load profile: "+e.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				myprofile=new Profile(filename);
			}
			return new Profile(filename);
		}
		public void SaveProfile(String filename)
		{
			
		}
	}
}
