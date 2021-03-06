﻿namespace SFA.DAS.RoATPService.Application.Exceptions
{
    using System;

    public sealed class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() : base("") { }
        public ResourceNotFoundException(string message) : base(message) { }
    }
}
