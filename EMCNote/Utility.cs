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
using System.Xml;

namespace EMCNote
{
	/// <summary>
	/// Description of Util.
	/// </summary>
	public static class Utility
	{
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
			int bufferSize = 1000;
			byte[] buffer = new byte[bufferSize];
			int readBytes = 0;
			Bitmap bmp=Utility.BitmapSourceToBitmap(source);
			MemoryStream ms=new MemoryStream();
			MemoryStream result=new MemoryStream();
			bmp.Save(ms,ImageFormat.Png);
			ms.Position=0;
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.CloseOutput = false;
			using (XmlWriter writer = XmlWriter.Create(result,settings)) {
				writer.WriteStartElement("Image");
				BinaryReader br = new BinaryReader(ms);
				do {
					readBytes = br.Read(buffer, 0, bufferSize);
					writer.WriteBase64(buffer, 0, readBytes);
				} while (bufferSize <= readBytes);
				br.Close();
				writer.WriteEndElement();
			}

			StreamReader sr=new StreamReader(result);
			result.Position=0;
			return sr.ReadToEnd();
		}
		
		public static System.Drawing.Bitmap BitmapSourceToBitmap(BitmapSource s)
		{
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s.PixelWidth, s.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			s.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
			bmp.UnlockBits(data);
			return bmp;
		}
	}
}
