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
			XmlDocument xd=XMLize();
			xd.Save(MyProfile.FileName);
			
		}
		public void Load()
		{
			this.MyProfile.Clear();
			LoadFromXMLFile(this.MyProfile.FileName);
		}
		private XmlDocument XMLize()
		{
			XmlDocument xd= new XmlDocument();
			xd.AppendChild(xd.CreateXmlDeclaration("1.0","utf-8","yes"));
			XmlElement profile_node= xd.CreateElement("Profile");
			profile_node.SetAttribute("Name",MyProfile.ProfileName);
			xd.AppendChild(profile_node);
			foreach (Book b in MyProfile.BookItems)
			{
				CreateBookNode(profile_node, b);
			}
			return xd;
			
		}
		
		private void CreateBookNode(XmlNode x, Book b)
		{
			XmlElement created_node=x.OwnerDocument.CreateElement("Book");
			created_node.SetAttribute("Name",b.Name);
			x.AppendChild(created_node);
			foreach(Book childbook in b.BookItems)
			{
				CreateBookNode(created_node,childbook);
			}
			foreach(Note childnote in b.NoteItems)
			{
				CreateNoteNode(created_node,childnote);
			}
		}
		
		private void CreateNoteNode(XmlNode x, Note n)
		{
			XmlElement created_node=x.OwnerDocument.CreateElement("Note");
			XmlElement document_node=x.OwnerDocument.CreateElement("Document");
			created_node.SetAttribute("Title",n.Title);
			x.AppendChild(created_node);
			created_node.AppendChild(document_node);
			document_node.InnerXml=System.Windows.Markup.XamlWriter.Save(n.Document);
			
		}
		
		private void LoadFromXMLFile(String filename)
		{
			XmlDocument xd=new XmlDocument();
			xd.Load(filename);
			XmlNode profile_node=xd.SelectSingleNode("/Profile");
			
			this.MyProfile.ProfileName=profile_node.Attributes.GetNamedItem("Name").Value;
			
			foreach (XmlNode x in profile_node.ChildNodes)
			{
				if(x.Name=="Book")
				{
					BuildBookTree(x);
				}
			}
		}
		private void BuildBookTree(XmlNode x)
		{
			Book created_book=this.MyProfile.newBook(x.Attributes.GetNamedItem("Name").Value);
			foreach(XmlNode cx in x.ChildNodes)
			{
				if(cx.Name=="Book")
				{
					BuildBookTree(created_book,cx);
				}else if(cx.Name=="Note")
				{
					BuildNote(created_book,cx);
				}
			}
		}
		private void BuildBookTree(Book b, XmlNode x)
		{
			Book created_book=this.MyProfile.newBook(x.Attributes.GetNamedItem("Name").Value,b);
			foreach(XmlNode cx in x.ChildNodes)
			{
				if(cx.Name=="Book")
				{
					BuildBookTree(created_book,cx);
				}else if(cx.Name=="Note")
				{
					BuildNote(created_book,cx);
				}
			}
		}
		private void  BuildNote(Book b,XmlNode x)
		{
			Note n=new Note(x.Attributes.GetNamedItem("Title").Value,b);
			XmlNode docnode=x.SelectSingleNode("./Document");
			if(docnode!=null){
				n.Content=docnode.InnerText;
				n.Document=System.Windows.Markup.XamlReader.Parse(docnode.InnerXml) as System.Windows.Documents.FlowDocument;
				
			}
		}
	}
}