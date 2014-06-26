/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 6/26/2014
 * Time: 3:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;


namespace EMCNote
{
	/// <summary>
	/// Description of Profile.
	/// </summary>
	public class ProfileHelper
	{
		private Profile myprofile;
		public ProfileHelper(Profile myprofile)
		{
			this.myprofile=myprofile;
		}
		public Profile MyProfile
		{
			get {
				return this.myprofile;
			}
		}
		public void Save()
		{
			
		}
		public void Load()
		{
			
		}
		private XmlDocument XMLize()
		{
			return new XmlDocument();
		}
		private void LoadFromXMLFile()
		{
			
		}
	}
}