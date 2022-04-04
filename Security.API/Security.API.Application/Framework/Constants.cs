using System;
using System.Collections.Generic;
using System.Text;

namespace Security.API.Application.Framework
{
    public class Constants
    {
        public const string DEFAULT_SCOPE = "any";
        public const string TOKEN_TYPE_BR = "bearer";
    }

    public class GRANT_TYPES
    {
        public const string PASSWORD = "password";
        public const string Refresh_Token = "refresh_token";
    }
}
