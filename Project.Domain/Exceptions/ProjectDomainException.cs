using System;

namespace Project.Domain.Exceptions
{
    public class ProjectDomainException : Exception
    {
        public ProjectDomainException()
        {
        }

        public ProjectDomainException(string message) : base(message)
        {
        }

        public ProjectDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}