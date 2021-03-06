﻿using CSharpFunctionalExtensions;
using MediatR;

namespace FakeSurveyGenerator.Application.Users.Queries.IsUserRegistered
{
    public sealed class IsUserRegisteredQuery : IRequest<Result<UserRegistrationStatusModel>>
    {
        public string UserId { get; }

        public IsUserRegisteredQuery(string userId)
        {
            UserId = userId;
        }
    }
}
