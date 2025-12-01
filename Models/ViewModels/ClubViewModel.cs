using System;
using epl_backend.Models.DTOs;

namespace epl_backend.Models.ViewModels;

public class ClubViewModel
{
    public ClubDto clubDto { get; set; }
    public List<ClubDto> clubDtos { get; set; }

    public ClubViewModel()
    {
        this.clubDto = new();
        this.clubDtos = new();
    }
}
