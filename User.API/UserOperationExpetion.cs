using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace User.API
{
    public class UserOperationExpetion : Exception
    {
        public UserOperationExpetion()
        {
        }

        public UserOperationExpetion(string message) : base(message)
        {
        }

        public UserOperationExpetion(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}