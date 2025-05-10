using Siska.Admin.Database;
using Siska.Admin.Model.DTO;
using Siska.Admin.Model.DTO.System;

namespace Siska.Admin.Application.Services.System
{
    public interface IUserService
    {
        Task<UserSignInDTO> SignInAsync(UserLoginDTO credentials);
        Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO, CancellationToken cancellationToken);
        Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, CancellationToken cancellationToken);
        Task<PagedList<UserDTO>> GetList(ListDataDTO listDataDTO, CancellationToken cancellationToken);
        Task<UserDTO> Get(Guid id, CancellationToken cancellationToken);
        Task<UserDTO> Add(UserDTO userDTO, CancellationToken cancellationToken);
        Task<bool> Update(UserDTO userDTO, CancellationToken cancellationToken);
        Task<bool> Delete(Guid id, CancellationToken cancellationToken);
        Task<string> UploadImage(Guid id, MemoryStream stream, string fileName, CancellationToken cancellationToken);
        Task<(MemoryStream, string)> GetImage(Guid id, CancellationToken cancellationToken);
    }
}
