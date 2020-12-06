using LiarsDiceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LiarsDiceApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private LiarsDiceDbContext _ldContext;

        public List<Player> playerList;
        public PlayerController(LiarsDiceDbContext ldContext)
        {
            _ldContext = ldContext;
            playerList = _ldContext.Players.ToList();
        }

        // GET api/<PlayerController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Find players in current gaming session and return
                var findPlayers = playerList.Find(p => p.GameId == HttpContext.Session.GetString("GameId"));
                return new JsonResult(findPlayers);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // GET api/<PlayerController>/bob
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            try
            {
                // Find player in current gaming session and return
                var findPlayer = playerList.Find(p => p.GameId == HttpContext.Session.GetString("GameId")
                && p.Name == name);
                return new JsonResult(findPlayer);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // POST api/id/<PlayerController>
        [HttpPost]
        public IActionResult Post([FromBody] Player player)
        {
            try
            {
                // Read JSON body and map to Player object
                _ldContext.Players.Add(player);
                _ldContext.SaveChanges();
                playerList = _ldContext.Players.ToList();
                return Created("OK", new JsonResult(player));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // PUT api/<PlayerController>/bob
        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromBody] Player player)
        {
            try
            {
                // Find player in current gaming session and update
                var editPlayer = _ldContext.Players.FirstOrDefault(p => p.GameId == HttpContext.Session.GetString("GameId")
                && p.Name == name);
                editPlayer.Name = player.Name;
                editPlayer.NumberOfDiceGuess = player.NumberOfDiceGuess;
                editPlayer.ValueOfDieGuess = player.ValueOfDieGuess;
                editPlayer.CalledLiar = player.CalledLiar;
                editPlayer.DiceList = player.DiceList;
                _ldContext.SaveChanges();
                playerList = _ldContext.Players.ToList();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        // PUT api/<PlayerController>/bob
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            try
            {
                // Find player in current gaming session and delete
                var deletePlayer = _ldContext.Players.FirstOrDefault(p => p.GameId == HttpContext.Session.GetString("GameId")
                && p.Name == name);
                _ldContext.Players.Remove(deletePlayer);
                _ldContext.SaveChanges();
                playerList = _ldContext.Players.ToList();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
