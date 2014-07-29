using System;
using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using HTMLConverter;
using System.Text.RegularExpressions;

namespace EMCNote
{
	/// <summary>
	/// Description of PastedHtml.
	/// </summary>
	public class PastedHtml
	{
		
	
		String xamlsource;
		TextElement elem;
		
		public PastedHtml(String HtmlCode)
		{
			String html=pre(HtmlCode);
			xamlsource=HtmlToXamlConverter.ConvertHtmlToXaml(html,false);
			elem=System.Windows.Markup.XamlReader.Parse(xamlsource) as TextElement;
		}
		
		private String pre(String HtmlCode)
		{
			HtmlCode=HtmlCode.Replace("&nbsp;"," ");
			HtmlCode=HtmlCode.Replace('\r',' ');
			HtmlCode=HtmlCode.Replace('\n',' ');
			HtmlCode=Regex.Match(HtmlCode,"<html.*$",RegexOptions.Multiline|RegexOptions.IgnoreCase).Value;
			return HtmlCode;
		}
		public TextElement Paste{
			get{
				return elem;
			}
		}
		
	}
}
