﻿using TimeSheet.Models;
using TimeSheet.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using TimeSheet.Models.Payload;
namespace TimeSheet.Services;
using System.Data;
using System.Data.Common;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CheckUserExists(User user)
    {
        try
        {
            var existingUser = await _repository.GetUserByEmailAsyc(user.Email);
            return existingUser != null?true:false;
        }
        catch (System.Exception)
        {
            throw;
        }

    }

    public async Task<StandardResponce> AddUserInParts(Phase phase, User user)
    {
        StandardResponce resp = new();
        switch(phase){
            case Phase.Personal:
                try{
                    if(await CheckUserExists(user)){
                        //  await _repository.UpdateUserAsyc(user);
                         resp.Message = $"Email Already Exists";
                         resp.status = HttpStatusCode.UnprocessableEntity;

                    }else{
                         await _repository.AddUserAsyc(user);
                         resp.Message = $"successfully added user details";
                         resp.status = HttpStatusCode.OK;
                    }
                    resp.User = user;
                }catch(Exception ex){
                    resp.Message = ex.Message;
                    resp.User = null;
                    resp.status = HttpStatusCode.ServiceUnavailable;
                }
            break;
            default:
                resp.Message = "That Field doesnot exist in the system please re verify";
                resp.User = null;
            break;
        }
        return  resp;
    }
    public async Task<StandardResponce> AddQualificationDetails(QualificationDetails qa) {
        StandardResponce resp = new();
        try{
                await _repository.AddQualificationDetails(qa);
                resp.Message = "Successfully added Qualification";
                resp.status = HttpStatusCode.OK;
        }catch(Exception ex){
                resp.Message = $"Something Went Wrong {ex.InnerException?.Message}";
                resp.status = HttpStatusCode.BadGateway;
        }
        return resp;
    }

    public string GenerateJwtToken(string userid)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("this is my super secret key that is exposed");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
            new Claim(ClaimTypes.NameIdentifier, userid)
            ]),
            Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<AuthResponce> PerformAuthentication(UserLogin user)
    {
        AuthResponce resp = new();
        var UserfromDb = await _repository.GetUserByEmailAsyc(user.Email);
        if (UserfromDb == null)
            {
                resp.Message = "Invalid Email or Password";
                resp.Token = null;
                return resp;
            }else{
                if(UserfromDb.Password == user.Password){
                resp.Message = "User LoggedIn successfully";
                resp.Token = GenerateJwtToken(user.Email);
                return resp;
                }else{
                resp.Message = "Invalid Email or Password";
                resp.Token = null;
                return resp;
                }
            }
    }
     public async Task<StandardListResponce> GetQualificationDetails(int userid){
        var payload = new StandardListResponce();
        var data = await _repository.GetQualificationDetails(userid);
        if (data != null && data.Count != 0)
            {
                payload.Data = data;
                payload.Message = "Successfully retrieved data";
                payload.Status = System.Net.HttpStatusCode.OK;
            }
            else
            {
                payload.Message = "No data found";
                payload.Status = System.Net.HttpStatusCode.NoContent;
            }
            return payload;    
     }

}
