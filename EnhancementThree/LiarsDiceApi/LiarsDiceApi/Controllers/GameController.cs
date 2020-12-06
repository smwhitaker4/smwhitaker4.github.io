using LiarsDiceApi.Helpers;
using LiarsDiceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiarsDiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private LiarsDiceDbContext _ldContext;

        public List<Game> gameList;

        public static List<Player> listOfPlayers;
        public GameController(LiarsDiceDbContext ldContext)
        {

            _ldContext = ldContext;

            gameList = _ldContext.Games.ToList();

            listOfPlayers = _ldContext.Players.ToList();
        }

        // GET api/<Game>/
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Get current game info
                var currentGame = gameList.Find(g => g.GameId == HttpContext.Session.GetString("GameId"));
                return new JsonResult(currentGame);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // POST api/<Game>
        [HttpPost]
        public IActionResult Post([FromBody] Game game)
        {
            try
            {
                // Read JSON body and map to Player object
                _ldContext.Games.Add(game);
                _ldContext.SaveChanges();
                gameList = _ldContext.Games.ToList();
                return Created("OK", new JsonResult(game));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE api/<Game>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                // Find gaming session and delete
                var deleteGame = gameList.Find(g => g.GameId == id);
                _ldContext.Games.Remove(deleteGame);
                _ldContext.SaveChanges();
                gameList = _ldContext.Games.ToList();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE api/<Game>/star
        [Route("api/[controller]/start")]
        [HttpPost]
        public IActionResult StartGame(string id)
        {
            try
            {
                var currentGame = gameList.Find(g => g.GameId == HttpContext.Session.GetString("GameId"));
                GameHelper.RollDice(currentGame, listOfPlayers);

                do
                {
                    // First player starts with a guess before entering the loop
                    GameHelper.TakeGuess(listOfPlayers);
                } while (!GameHelper.EndGame(GameHelper.CallLiar(listOfPlayers), currentGame, listOfPlayers));
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
