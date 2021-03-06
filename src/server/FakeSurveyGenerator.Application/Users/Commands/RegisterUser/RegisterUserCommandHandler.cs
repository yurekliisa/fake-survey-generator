﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using FakeSurveyGenerator.Application.Common.Errors;
using FakeSurveyGenerator.Application.Common.Identity;
using FakeSurveyGenerator.Application.Common.Persistence;
using FakeSurveyGenerator.Application.Users.Models;
using FakeSurveyGenerator.Domain.AggregatesModel.UserAggregate;
using FakeSurveyGenerator.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FakeSurveyGenerator.Application.Users.Commands.RegisterUser
{
    public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserModel, Error>>
    {
        private readonly IUserService _userService;
        private readonly ISurveyContext _surveyContext;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IUserService userService, ISurveyContext surveyContext, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _surveyContext = surveyContext ?? throw new ArgumentNullException(nameof(surveyContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<UserModel, Error>> Handle(RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var userInfo = await _userService.GetUserInfo(cancellationToken);

            if (await _surveyContext.Users.AnyAsync(user => user.ExternalUserId == userInfo.Id, cancellationToken))
                return Result.Failure<UserModel, Error>(Errors.General.UserAlreadyRegistered(userInfo.Id));

            var newUser = new User(NonEmptyString.Create(userInfo.DisplayName),
                NonEmptyString.Create(userInfo.EmailAddress), NonEmptyString.Create(userInfo.Id));

            await _surveyContext.Users.AddAsync(newUser, cancellationToken);

            await _surveyContext.SaveChangesAsync(cancellationToken);

            return Result.Success<UserModel, Error>(_mapper.Map<UserModel>(newUser));
        }
    }
}