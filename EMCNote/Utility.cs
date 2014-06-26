/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 05/30/2014
 * Time: 13:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace EMCNote
{
	/// <summary>
	/// Description of Util.
	/// </summary>
	public static class Utility
	{
		static BitmapImage ToBitmapImage(Bitmap source){
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
	}
}
