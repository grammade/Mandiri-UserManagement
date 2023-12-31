﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Effortless.Net.Encryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Dtos;
using UserManagement.Entities;
using UserManagement.Persistence;

namespace UserManagement.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly Context _context;
    private readonly IConfiguration _configuration;

    public UsersController(Context context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserRec>> Register(UserRec user)
    {
        var exists = await _context.Users
            .Where(u => u.UserName.Equals(user.username))
            .FirstOrDefaultAsync();
        if (exists is not null)
            return BadRequest("Username already exists");

        var key = _configuration.GetValue<string>("Encryption:Key");
        var iv = Bytes.GenerateIV();

        //encrypt password with symetric enc alg, using effortless enc.
        var enc = Strings.Encrypt(user.password, Encoding.ASCII.GetBytes(key), iv);
        var passId = Guid.NewGuid();
        await _context.Passwords.AddAsync(new Password
        {
            id = passId,
            hash = enc,
            salt = iv
        });
        await _context.Users.AddAsync(new User
        {
            UserName = user.username,
            Email = user.email,
            PasswordId = passId,
            Created = DateTime.Now
        });
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> login(LoginRec loginRec)
    {
        var key = _configuration.GetValue<string>("Encryption:Key");
        var issuer = _configuration.GetValue<string>("JWT:Issuer");
        var aud = _configuration.GetValue<string>("JWT:Audience");

        var user = await _context.Users
            .Include(u => u.Password)
            .Where(u => u.UserName.Equals(loginRec.username))
            .FirstOrDefaultAsync();

        if (user is null)
            return BadRequest("Invalid Cred");

        var decPass = Strings.Decrypt(user.Password.hash, Encoding.ASCII.GetBytes(key), user.Password.salt);
        if (!decPass.Equals(loginRec.password))
            return BadRequest("Invalid Cred");


        var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetValue<string>("JWT:Key")!));
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("UserId", user.Id.ToString())
        };
        var cred = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: cred,
            issuer: issuer,
            audience: aud
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }

    // just exist as developmental feature
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var key = _configuration.GetValue<string>("Encryption:Key");

        var r = await _context.Users
            .AsNoTracking()
            .Include(u => u.Password)
            .OrderByDescending(u => u.Created)
            .Select(u =>
                new UserDto
                {
                    Email = u.Email,
                    UserName = u.UserName,
                    Password = Strings.Decrypt(u.Password.hash, Encoding.ASCII.GetBytes(key), u.Password.salt)
                })
            .ToListAsync();
        return Ok(r);
    }


}
