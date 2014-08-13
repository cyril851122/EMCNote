/*
 * Created by SharpDevelop.
 * User: jiangz3
 * Date: 07/08/2014
 * Time: 16:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ZZNote
{
	/// <summary>
	/// Interaction logic for NewBook.xaml
	/// </summary>
	public partial class NewBook : Window
	{
		public NewBook()
		{
			InitializeComponent();
		}
		
		void OK_Click(object sender, RoutedEventArgs e)
		{
			if(tb_bookname.Text.Trim()!="")
			{
				this.DialogResult=true;
				this.Close();
			}else
			{
				MessageBox.Show("Invalid Name");
			}
		}
		void Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult=false;
			this.Close();
		}
		void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			tb_bookname.Focus();
		}
	}
}