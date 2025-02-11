using System;
using System.CodeDom;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userRepository.GetUsersAsync();
        var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
        return Ok(usersToReturn);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUsers(string username)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();
        var userToReturn = _mapper.Map<MemberDto>(user);
        return userToReturn;

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUsers(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return user;

    }
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return BadRequest("No username found in token");
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user == null) return BadRequest("Could not find user");
        _mapper.Map(memberUpdateDto, user);
        if (await _userRepository.SaveAllAsync()) return NoContent();
        return BadRequest("Failed to update User");
    }
}
