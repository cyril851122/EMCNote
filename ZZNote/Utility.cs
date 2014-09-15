/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 05/30/2014
 * Time: 13:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Linq;
using System.Collections.Generic;
using System.Xml;

namespace ZZNote
{
	/// <summary>
	/// Description of Util.
	/// </summary>
	public static class Utility
	{
		
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		public static BitmapImage BitmapToBitmapImage(Bitmap source){
			BitmapImage bitmapImage;
			using(MemoryStream memory = new MemoryStream())
			{
				source.Save(memory, ImageFormat.Png);
				memory.Position = 0;
				bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
			}
			return bitmapImage;
		}
		
		public static String ImageSourceToBase64String(BitmapSource source)
		{
			Bitmap bmp=Utility.BitmapSourceToBitmap(source);
			MemoryStream ms = new MemoryStream();
			bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
			byte[] arr = new byte[ms.Length];
			ms.Position = 0;
			ms.Read(arr, 0, (int)ms.Length);
			ms.Close();
			String strbaser64 = Convert.ToBase64String(arr);
			return strbaser64;
		}
		public static BitmapSource Base64StringToImageSource(String str)
		{
			byte[] arr = Convert.FromBase64String(str);
			MemoryStream ms=new MemoryStream(arr);
			Bitmap bmp = new Bitmap(ms);
			return Utility.BitmapToBitmapSource(bmp);
		}
		public static System.Drawing.Bitmap BitmapSourceToBitmap(BitmapSource s)
		{
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s.PixelWidth, s.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			s.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
			bmp.UnlockBits(data);
			return bmp;
		}
		public static BitmapSource BitmapToBitmapSource(Bitmap bmp)
		{
			IntPtr ip = bmp.GetHbitmap();
			BitmapSource bs=System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,IntPtr.Zero,Int32Rect.Empty,System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
			DeleteObject(ip);
			return bs;
		}
		
		public static IEnumerable<System.Windows.Controls.Image> FindImages(FlowDocument document)
		{
			return document.Blocks.SelectMany(block => FindImages(block));
		}

		
		public static IEnumerable<System.Windows.Controls.Image> FindImages(TextElement elem)
		{
			if(elem is Inline)
			{
				return FindImages(elem as Span);
			}else
			{
				return FindImages(elem as Section);
			}
		}
		public static IEnumerable<System.Windows.Controls.Image> FindImages(Block block)
		{
			if (block is Table)
			{
				return ((Table)block).RowGroups
					.SelectMany(x => x.Rows)
					.SelectMany(x => x.Cells)
					.SelectMany(x => x.Blocks)
					.SelectMany(innerBlock => FindImages(innerBlock));
			}
			if (block is Paragraph)
			{
				return ((Paragraph)block).Inlines.SelectMany(x => FindImages(x));
			}
			if (block is BlockUIContainer)
			{
				System.Windows.Controls.Image i = ((BlockUIContainer)block).Child as System.Windows.Controls.Image;
				return i == null
					? new List<System.Windows.Controls.Image>()
					: new List<System.Windows.Controls.Image>(new[] { i });
			}
			if (block is List)
			{
				return ((List)block).ListItems.SelectMany(listItem => listItem
				                                          .Blocks
				                                          .SelectMany(innerBlock => FindImages(innerBlock)));
			}
			if (block is Section)
			{
				return ((Section)block).Blocks.SelectMany(x => FindImages(x));
			}
			return new List<System.Windows.Controls.Image>();
		}
		public static IEnumerable<System.Windows.Controls.Image> FindImages(Inline inline)
		{
			if(inline is Span)
			{
				return (inline as Span).Inlines.SelectMany(x => FindImages(x));
			}
			if(inline is InlineUIContainer)
			{
				System.Windows.Controls.Image i = ((InlineUIContainer)inline).Child as System.Windows.Controls.Image;
				return i == null
					? new List<System.Windows.Controls.Image>()
					: new List<System.Windows.Controls.Image>(new[] { i });
			}
			
			return new List<System.Windows.Controls.Image>();
			
			
		}
		
		static public Dictionary<String,String> getConfig()
		{
			Dictionary<String,String> d=new Dictionary<string, string>();
			String iniFile=System.Windows.Forms.Application.StartupPath+"\\zzConf.ini";
			if(!System.IO.File.Exists(iniFile))
			{
				System.IO.File.WriteAllText(iniFile,"DataFile:\\\\cncd2fs1\\HOME\\zznote.znd");
			}
			String[] lines=System.IO.File.ReadAllLines(iniFile);
			foreach(String line in lines)
			{
				String[] kwp=line.Split(':');
				if(kwp.Count()==2){
					d.Add(kwp[0].Trim(),kwp[1].Trim());
				}else{
					throw new Exception("Please Check zzConf.ini");
				}
			}
			return d;
		}
	}
}
