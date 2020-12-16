using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StudentManagement_Tools.CustomUtil
{
	public class ValidEmailDomainAttribute:ValidationAttribute
	{
		private readonly string AllowDomain;

		public ValidEmailDomainAttribute(string allowdomain)
		{
			AllowDomain = allowdomain;
		}

		public override bool IsValid(object value)
		{
			string[] strings = value.ToString().Split('@');

			return strings[1].ToUpper() == AllowDomain.ToUpper();
		}
	}
}
