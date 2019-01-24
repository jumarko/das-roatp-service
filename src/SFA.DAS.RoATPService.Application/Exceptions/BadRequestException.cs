﻿namespace SFA.DAS.RoATPService.Application.Exceptions
{
    using System;

    public sealed class BadRequestException : Exception
    {
        public BadRequestException() : base("") { }
        public BadRequestException(string message) : base(message) { }
    }
}
