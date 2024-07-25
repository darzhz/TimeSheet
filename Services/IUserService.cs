﻿using TimeSheet.Models;

namespace TimeSheet.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsyc();
    Task<StandardResponce> AddUserAsyc(User user);
    string GenerateJwtToken(String userid);
    Task<AuthResponce> PerformAuthentication(User user);
}