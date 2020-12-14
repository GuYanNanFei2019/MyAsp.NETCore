using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement_Tools.MiddleWare
{
	public class CustomIdentityErrorDesc:IdentityErrorDescriber
	{

		public override IdentityError DefaultError() => new IdentityError { Code = nameof(DefaultError), Description = $"发生了未知的故障。" };

		public override IdentityError ConcurrencyFailure() => new IdentityError { Code = nameof(ConcurrencyFailure), Description = "乐观并发失败，对象已被修改。" };

		public override IdentityError PasswordMismatch() => new IdentityError { Code = nameof(PasswordMismatch), Description = "密码错误" };

		public override IdentityError InvalidToken() => new IdentityError { Code = nameof(InvalidToken), Description = "无效的令牌." };

		public override IdentityError LoginAlreadyAssociated() => new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "具有此登录的用户已经存在." };

		public override IdentityError InvalidUserName(string userName) => new IdentityError { Code = nameof(InvalidUserName), Description = $"用户名'{userName}'无效，只能包含字母或数字." };

		public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError { Code = nameof(InvalidEmail), Description = $"Email '{email}' 无效." };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError { Code = nameof(DuplicateUserName), Description = $"用户名 '{userName}' 已存在." };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Email '{email}' 已存在." };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError { Code = nameof(InvalidRoleName), Description = $"角色名 '{role}' 无效." };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"角色名 '{role}' 已存在." };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "用户已设置密码." };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "未为此用户启用锁定." };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"用户已存在角色 '{role}'中." };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError { Code = nameof(UserNotInRole), Description = $"用户不在角色 '{role}'中." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError { Code = nameof(PasswordTooShort), Description = $"密码必须至少是{length}字符." };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "密码必须至少有一个非字母数字字符."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = $"密码必须至少有一个数字('0'-'9')." };
        }


        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = $"密码必须使用至少不同的{uniqueChars}字符。" };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "密码必须至少有一个小写字母('a'-'z')." };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "密码必须至少有一个大写字母('A'-'Z')." };
        }
    }
}
