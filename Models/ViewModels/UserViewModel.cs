using System;
using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Models.ViewModels;

public class UserViewModel
{
    public UserLoginDto userLoginDto { get; set; }
    public UserDto userDto { get; set; }
    public List<UserDto> userDtos { get; set; }

    public UserViewModel()
    {
        this.userLoginDto = new UserLoginDto();
        userDto = new UserDto();
        this.userDtos = new List<UserDto>();
    }
}
