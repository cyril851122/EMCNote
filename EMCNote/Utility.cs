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

namespace EMCNote
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
				return ((Paragraph)block).Inlines
					.OfType<InlineUIContainer>()
					.Where(x => x.Child is System.Windows.Controls.Image)
					.Select(x => x.Child as System.Windows.Controls.Image);
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
			throw new InvalidOperationException("Unknown block type: " + block.GetType());
		}
	}
}
