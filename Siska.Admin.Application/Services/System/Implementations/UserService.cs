using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Security.Authentication;
using Siska.Admin.Application.Constants;
using Siska.Admin.Application.Enums;
using Siska.Admin.Application.Exceptions;
using Siska.Admin.Database;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;
using Siska.Admin.Model.Entities;
using Siska.Admin.Model.Extensions;
using Siska.Admin.Model.Mappings;
using Siska.Admin.Storage;
using Siska.Admin.Utility;

namespace Siska.Admin.Application.Services.System.Implementations
{
    public class UserService : IUserService
    {
        private SignInManager<Users> signInManager;
        private UserManager<Users> users;
        private TokenGenerator tokens;
        private IExtractUser extractUser;
        private IStorage storage;
        private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };
        private readonly long fileSizeLimit;

        public UserService(
            SignInManager<Users> signInManager,
            UserManager<Users> users,
            TokenGenerator tokens,
            IExtractUser extractUser,
            IStorage storage,
            IConfiguration config)
        {
            this.signInManager = signInManager;
            this.users = users;
            this.tokens = tokens;
            this.extractUser = extractUser;
            this.storage = storage;
            fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }

        #region Main CRUD
        public async Task<UserDTO> Add(UserDTO userDTO, CancellationToken cancellationToken)
        {
            userDTO.Validate();

            userDTO.Id = Guid.NewGuid().ToString();
            userDTO.DataStatus = 1;

            var User = UserMapping.DTOtoEntities(userDTO);

            User.Roles = null;

            var result = await users.CreateAsync(User, User.Email);

            if (result != null)
            {
                if (result.Succeeded)
                {
                    await users.AddToRolesAsync(User, userDTO.Roles);
                }
                else
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            return UserMapping.EntitiesToDTO(User);
        }

        public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (extractUser.Id == id.ToString())
                throw new Exception("Can not change own status");

            var data = await users.Users.FirstAsync(x => x.Id == id);

            NotFoundException.ThrowIfNull(data, IPSEntityType.Users);

            if (data != null)
            {
                data.DataStatus = data.DataStatus == 1 ? 0 : 1;

                var result = await users.UpdateAsync(data);

                if (result != null)
                {
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    return true;
                }
            }

            return false;
        }

        public async Task<UserDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var data = await users.Users.Include(x => x.Roles).FirstAsync(x => x.Id == id, cancellationToken);

            NotFoundException.ThrowIfNull(data, IPSEntityType.Users);

            var dataDTO = UserMapping.EntitiesToDTO(data);

            dataDTO.Roles = await users.GetRolesAsync(data);

            return dataDTO;
        }

        public async Task<PagedList<UserDTO>> GetList(ListDataDTO listDataDTO, CancellationToken cancellationToken)
        {
            if (listDataDTO.PageNumber <= 0) listDataDTO.PageNumber = 1;

            if (listDataDTO.PageSize <= 0) listDataDTO.PageSize = 10;
            var skipSize = (listDataDTO.PageNumber - 1) * listDataDTO.PageSize;

            int totalItems = 0;
            List<UserDTO> data = null;

            if (listDataDTO.Search != null && listDataDTO.Search.Count > 0)
            {
                Expression<Func<Users, bool>> exp;

                exp = ExpressionUtils.BuildCondition<Users>(listDataDTO.Search);

                totalItems = await users.Users.CountAsync(exp, cancellationToken: cancellationToken);
                data = UserMapping.ListEntitiesToDTO(await users.Users.Include(x => x.Roles).Where(exp).Skip(skipSize).Take(listDataDTO.PageSize).ToListAsync());
            }
            else if (!listDataDTO.SearchField.IsNullOrEmpty() && !listDataDTO.SearchString.IsNullOrEmpty())
            {
                Expression<Func<Users, bool>> exp;

                exp = ExpressionUtils.BuildPredicate<Users>(listDataDTO.SearchField, "==", listDataDTO.SearchString);

                totalItems = await users.Users.CountAsync(exp, cancellationToken: cancellationToken);
                data = UserMapping.ListEntitiesToDTO(await users.Users.Include(x => x.Roles).Where(exp).Skip(skipSize).Take(listDataDTO.PageSize).ToListAsync());
            }
            else
            {
                totalItems = await users.Users.CountAsync(cancellationToken: cancellationToken);
                data = UserMapping.ListEntitiesToDTO(await users.Users.Include(x => x.Roles).Skip(skipSize).Take(listDataDTO.PageSize).ToListAsync());
            }

            return new PagedList<UserDTO>(data, totalItems, listDataDTO.PageNumber, listDataDTO.PageSize);
        }

        public async Task<bool> Update(UserDTO userDTO, CancellationToken cancellationToken)
        {
            userDTO.Validate();

            var User = UserMapping.DTOtoEntities(userDTO);

            var data = await users.Users.Include(i => i.Roles).FirstAsync(x => x.Id == User.Id);

            NotFoundException.ThrowIfNull(data, IPSEntityType.Users);

            if (data != null)
            {
                data.FullName = User.FullName;
                data.Address = User.Address;
                data.Age = User.Age;
                data.UserTelegramId = User.UserTelegramId;
                data.Roles = null;

                var result = await users.UpdateAsync(data);

                if (result != null)
                {
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    await users.AddToRolesAsync(data, userDTO.Roles);

                    return true;
                }
            }

            return false;
        }

        public async Task<string> UploadImage(Guid id, MemoryStream stream, string fileName, CancellationToken cancellationToken)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.Length > fileSizeLimit)
                throw new Exception("File is bigger than maximum allowed (max 512 KB)");

            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                throw new Exception("File is not allowed (only image files jpg, png and bmp)");

            var filePath = StoragePaths.Users + id.ToString() + "/image/";

            storage.DeleteFolder(StoragePaths.ContainerName, filePath);

            return await storage.SaveFileAsync(stream.ToArray(), StoragePaths.ContainerName, filePath + fileName);
        }

        public async Task<(MemoryStream, string)> GetImage(Guid id, CancellationToken cancellationToken)
        {
            var filePath = StoragePaths.Users + id.ToString() + "/image/";

            var lFolder = storage.ListFileOnFolder(StoragePaths.ContainerName, filePath, 1, 0);

            if (lFolder == null || lFolder.Count == 0)
                return (null, null);

            var fileName = lFolder.FirstOrDefault().Name;

            var dataImage = await storage.GetFile(StoragePaths.ContainerName, filePath + fileName);

            return (new MemoryStream(dataImage), fileName);
        }
        #endregion Main CRUD

        #region User Auth
        public async Task<UserSignInDTO> SignInAsync(UserLoginDTO credentials)
        {
            if (credentials is null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            var isEmail = EmailAddressValidator.TryValidate(credentials.Login, out _);
            var user = isEmail
                ? await users.FindByEmailAsync(credentials.Login)
                : await users.FindByNameAsync(credentials.Login);

            //if (user is null || !await users.CheckPasswordAsync(user, credentials.Password))
            //{
            //    // Log invalid login attempt
            //    throw new AuthenticationException("Invalid login attempt.");
            //}

            if (user is null)
            {
                throw new AuthenticationException("Invalid login attempt.");
            }


            // Sign in with lockout on failure
            var result = await signInManager.CheckPasswordSignInAsync(user, credentials.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                throw new AuthenticationException("Your account is locked due to multiple failed login attempts. Please try again later.");
            }
            else if (!result.Succeeded)
            {
                throw new AuthenticationException("Invalid login attempt.");
            }

            if (user.DataStatus == 0)
            {
                // Log inactive user attempt
                throw new InactiveUserException("User account is inactive.");
            }

            var roles = await users.GetRolesAsync(user);
            var accessToken = tokens.GenerateAccessToken(user, roles);
            if (string.IsNullOrEmpty(accessToken))
            {
                // Log token generation failure
                throw new TokenGenerationException("Failed to generate access token.");
            }

            var userSignIn = new UserSignInDTO
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Name = user.FullName,
                Email = user.Email,
                Roles = roles,
                Token = accessToken
            };

            return userSignIn;
        }


        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO, CancellationToken cancellationToken)
        {
            resetPasswordDTO.Validate();

            var data = await users.Users.FirstAsync(x => x.Id == Guid.Parse(resetPasswordDTO.UserId));

            var result = await users.RemovePasswordAsync(data);  //users.ResetPasswordAsync(data, token, resetPasswordDTO.Password);

            if (result != null)
            {
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await users.AddPasswordAsync(data, resetPasswordDTO.Password);

                if (result != null)
                {
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    return true;
                }
            }

            return false;
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, CancellationToken cancellationToken)
        {
            if (extractUser.Id != changePasswordDTO.UserId)
                throw new Exception("Can not change other user");

            changePasswordDTO.Validate();

            var data = await users.Users.FirstAsync(x => x.Id == Guid.Parse(changePasswordDTO.UserId));

            var result = await users.ChangePasswordAsync(data, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

            if (result != null)
            {
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                return true;
            }

            return false;
        }
        #endregion User Auth
    }
}
