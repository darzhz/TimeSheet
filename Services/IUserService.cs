﻿using TimeSheet.Models;
using TimeSheet.Models.Payload;

namespace TimeSheet.Services;

public interface IUserService
{
    string GenerateJwtToken(String userid);
    Task<AuthResponce> PerformAuthentication(UserLogin user);
    Task<StandardResponce> AddUserInParts(Phase phase,User user);

    Task<StandardResponce> AddQualificationDetails(QualificationDetails qa);
    Task<StandardListResponce> GetQualificationDetails(int userid);
}
