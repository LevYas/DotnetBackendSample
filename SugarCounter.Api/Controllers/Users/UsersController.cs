using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Auth;
using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Shared;
using SugarCounter.Core.Users;
using System.Threading.Tasks;

namespace SugarCounter.Api.Controllers.Users
{
    public class UsersController : BaseApiController
    {
        private readonly IUsersRepository _usersRepo;
        private readonly RequestContext _requestContext;

        public UsersController(RequestContext requestContext, IUsersRepository usersRepo)
        {
            _usersRepo = usersRepo;
            _requestContext = requestContext;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserInfoDto>> CreateUser(NewUserInfoDto input)
        {
            UserRole roleToUse = input.Role ?? UserRole.User;

            if (!UserRolesHelper.CanAssignRoleForNewUser(_requestContext.CurrentUserRaw, roleToUse))
                return Forbid();

            Res<UserInfo, CreateUserError> result = await _usersRepo.Create(input.Login, input.Password, roleToUse);

            return Match(result, onOk: u => new UserInfoDto(u),
                (CreateUserError.UserAlreadyExists, () => Conflict("User with this name is already created")),
                (CreateUserError.Unknown, () => Problem()));
        }

        [HttpGet]
        [AuthorizeFor(UserRole.Supervisor, UserRole.Admin)]
        public async Task<ActionResult<PaginatedListDto<UserInfoDto>>> GetUsers([FromQuery] UsersRequestDto? requestParams = null)
        {
            if (requestParams == null)
                requestParams = new UsersRequestDto();

            bool excludeAdmins = _requestContext.CurrentUser.Role != UserRole.Admin;

            PaginatedList<UserInfo>? items = await _usersRepo.GetList(excludeAdmins, requestParams.ToRefined());

            if (items == null)
                return Problem();
            else
                return items.ToDto(u => new UserInfoDto(u));
        }

        [HttpGet("me")]
        public Task<ActionResult<UserInfoDto>> GetCurrentUser() => GetUserById(_requestContext.CurrentUser.Id);

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserInfoDto>> GetUserById(int userId)
        {
            return await getUserOrError(userId)
                .ThenMatch(u => (ActionResult<UserInfoDto>)new UserInfoDto(u), err => err);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(int userId, UserEditsDto edits)
        {
            return await getUserOrError(userId)
                .ThenMap(userModel => tryUpdateUser(userModel, edits))
                .ThenGet();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            return await getUserOrError(userId)
                .ThenMap(async userModel =>
                {
                    if (!UserRolesHelper.CanUserBeDeleted(userModel))
                        return Forbid("User is not deletable");

                    return await _usersRepo.Delete(userModel) ? (ActionResult)NoContent() : Problem();
                })
                .ThenGet();
        }

        private async Task<ActionResult> tryUpdateUser(UserInfo userModel, UserEditsDto edits)
        {
            if (edits.Role.HasValue && userModel.Role != edits.Role)
            {
                if (UserRolesHelper.IsRoleChangeAllowed(_requestContext.CurrentUser, userModel, edits.Role.Value))
                    userModel.Role = edits.Role.Value;
                else
                    return Forbid();
            }

            return await _usersRepo.Update(userModel) ? (ActionResult)NoContent() : Problem();
        }

        private async Task<Res<UserInfo, ActionResult>> getUserOrError(int userId)
        {
            UserInfo? userModel = await _usersRepo.GetById(userId);

            // we do not want to share information that resource exists, so, in any case return 404
            if (userModel == null || UserRolesHelper.IsAssessForbidden(_requestContext.CurrentUser, userModel))
                return NotFound($"Item with id={userId} is not found");

            return userModel;
        }
    }
}
