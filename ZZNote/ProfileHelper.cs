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
using System.Windows.Documents;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ZZNote
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
			XmlElement attachement_node=x.OwnerDocument.CreateElement("Attachment");
			created_node.SetAttribute("Title",n.Title);
			x.AppendChild(created_node);
			created_node.AppendChild(document_node);
			created_node.AppendChild(attachement_node);
			
			// Clear ImageSource
			foreach (System.Windows.Controls.Image img in Utility.FindImages(n.Document))
			{
				img.Source=null;
			}
			// Save to file
			document_node.InnerXml=System.Windows.Markup.XamlWriter.Save(n.Document);
			// Set back Imagesource
			foreach (System.Windows.Controls.Image img in Utility.FindImages(n.Document))
			{
				BitmapSource bms=n.Attachments[Convert.ToInt32(img.Tag)];
				img.Source=bms;
				SaveAttachment(attachement_node,img.Tag.ToString(),bms);
			}
			//SaveAttachments(attachement_node,n.Attachments);
			
			
		}
		private void SaveAttachment(XmlNode att_node, String key, BitmapSource bmp)
		{
			XmlElement image_xe=att_node.OwnerDocument.CreateElement("Img");
			image_xe.SetAttribute("key",key);
			XmlCDataSection cdata=image_xe.OwnerDocument.CreateCDataSection(Utility.ImageSourceToBase64String(bmp));
			image_xe.AppendChild(cdata);
			att_node.AppendChild(image_xe);
			
		}
		private void SaveAttachments(XmlNode att_node,Dictionary<int,BitmapSource> att)  //discarded
		{
			//
			foreach (KeyValuePair<int,BitmapSource> kw_pair in att)
			{
				XmlElement image_xe=att_node.OwnerDocument.CreateElement("Img");
				image_xe.SetAttribute("key",kw_pair.Key.ToString());
				XmlCDataSection cdata=image_xe.OwnerDocument.CreateCDataSection(Utility.ImageSourceToBase64String(kw_pair.Value));
				image_xe.AppendChild(cdata);
				att_node.AppendChild(image_xe);
			}
			
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
			XmlNode attnode=x.SelectSingleNode("./Attachment");
			if(docnode!=null){
				n.Content=docnode.InnerText;
				
				n.Document=System.Windows.Markup.XamlReader.Parse(docnode.InnerXml) as System.Windows.Documents.FlowDocument;
				
				// find all Images and set source
				foreach(System.Windows.Controls.Image img in Utility.FindImages(n.Document))
				{
					String base64str=attnode.SelectSingleNode("./Img[@key='"+img.Tag+"']").InnerText;
					BitmapSource bms=Utility.Base64StringToImageSource(base64str);
					
					img.Source=bms;
					n.Attachments.Add(Convert.ToInt32(img.Tag),bms);
				}
			}
		}
	}
}