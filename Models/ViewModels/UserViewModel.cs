using System;
using epl_backend.Models.DTOs;

namespace epl_backend.Models.ViewModels;

public class UserViewModel
{
    public UserLoginDto userLoginDto { get; set; }

    public UserViewModel()
    {
        this.userLoginDto = new UserLoginDto();
    }
}
