﻿using SugarCounter.Api.Controllers.Users;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Shared;
using SugarCounter.Core.Users;

namespace Integration.Api.Controllers.Users
{
    public class UsersControllerTestsBase : IntegrationTestsBase
    {
        protected readonly IUsersRepository UsersRepo;
        protected readonly UsersController UsersController;

        public UsersControllerTestsBase()
        {
            UsersRepo = GetRepository();
            UsersController = new UsersController(GetContextForAdmin(), UsersRepo);
        }

        protected UsersRequest DefaultRequestParams => new UsersRequest(new PaginationParams
        {
            ItemsPerPage = 50,
            PageNumber = 1
        });

        protected NewUserInfoDto CreateNewUserDto(string login) => new NewUserInfoDto { Login = login, Password = "123" };
    }
}
