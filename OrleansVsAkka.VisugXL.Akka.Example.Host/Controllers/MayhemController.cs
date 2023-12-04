using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Actors;
using OrleansVsAkka.VisugXL.Akka.Example.Host.Messages;

namespace OrleansVsAkka.VisugXL.Akka.Example.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MayhemController : ControllerBase
    {
        private readonly ActorSystem _system;

        public MayhemController(ActorSystem system)
        {
            _system = system;
        }

        [HttpPost("start")] 
        public IActionResult Start()
        {
            var props = MayhemActor.CreateProps();
            var actor = _system.ActorOf(props);
            actor.Tell(new StartMayhem());
            return Ok();
        }
    }
}
