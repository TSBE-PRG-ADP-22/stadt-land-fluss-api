﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StadtLandFussApi.Models;
using StadtLandFussApi.Persistence;

namespace StadtLandFussApi.Controllers
{
    [ApiController]
    [Route("lobby")]
    public class LobbyController
    {

        private readonly AppDbContext _context;
        private readonly List<string> _names = new()
        {
            "Donkey",
            "Dario",
            "Jan",
            "Kieran",
            "Josh",
            "Liam",
            "Matt",
            "Sam",
            "Björn",
            "Bob",
            "Jonathan",
            "Keanu"
        };

        public LobbyController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new lobby in the database with an admin user.
        /// Generates a randomly generated code for the lobby on execution.
        /// </summary>
        /// <param name="lobby">The lobby to add to the database.</param>
        /// <returns>The freshly created lobby from the database.</returns>
        [HttpPost]
        public async Task<ActionResult<Lobby>> Post(Lobby lobby)
        {
            // Generate a lobby code.
            lobby.Code = Guid.NewGuid().ToString()[..8];
            // Add the admin user to it.
            var random = new Random();
            lobby.Users = new List<User>() {
                new User
                {
                    Guid = Guid.NewGuid().ToString(),
                    Admin = true,
                    Name = _names[random.Next(_names.Count)]
                }
            };
            // Set the start status.
            lobby.Status = Status.Pending;
            // Add the lobby to the database.
            await _context.Lobbies.AddAsync(lobby);
            await _context.SaveChangesAsync();
            // Mark the current user (the one who created the lobby) as such.
            lobby.Users[0].IsCurrentUser = true;
            // Return the newly created lobby.
            return lobby;
        }

        /// <summary>
        /// Creates a new user in the database for a lobby without admin rights.
        /// </summary>
        /// <param name="user">The user with the lobby id </param>
        /// <returns>The current lobby with all users in it.</returns>
        [HttpPost("{id}/user")]
        public async Task<ActionResult<Lobby>> Post(string id)
        {
            var random = new Random();
            var lobby = await _context.Lobbies.FirstOrDefaultAsync(l => l.Code == id);
            var guid = Guid.NewGuid().ToString();
            // Create a new user without admin rights for the given lobby.
            var user = new User()
            {
                LobbyId = lobby!.Id,
                Guid = guid,
                Admin = false,
                Name = _names[random.Next(_names.Count)]
            };
            // Add the user to the database.
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            // Get the updated lobby with all users.
            lobby = await _context.Lobbies.Include(l => l.Users).Include(l => l.Categories).FirstOrDefaultAsync(l => l.Code == id);
            // Mark the current user as such.
            var current = lobby!.Users!.FirstOrDefault(u => u.Guid == guid);
            current!.IsCurrentUser = true;
            lobby.Users![lobby.Users.IndexOf(current)] = current;
            // Return the lobby.
            return lobby;
        }

    }
}
