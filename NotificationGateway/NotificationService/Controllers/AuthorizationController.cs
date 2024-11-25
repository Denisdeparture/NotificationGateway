using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Requests.Auth;
using BuisnesLogic.Realization;
using BuisnesLogic.Realization.Auth;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using Data.interfaces;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IMetrics _metrics;
        private readonly IJwtManager _jwtManager;
        private readonly IServiceAuthOperation _serviceOperation;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly MapServices _mapServices;
        public AuthorizationController(IServiceAuthOperation serviceOperation, IMetrics metrics, ILogger<AuthorizationController> logger, IJwtManager jwtManager, MapServices mapServices)
        {
            _serviceOperation = serviceOperation ?? throw new ArgumentNullException(nameof(serviceOperation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jwtManager = jwtManager ?? throw new ArgumentNullException(nameof(jwtManager));
            _mapServices = mapServices ?? throw new ArgumentNullException(nameof(mapServices));
            _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        }
        [Route("[action]")]
        [HttpPost]
        public IActionResult Registration([FromQuery] AuthRequest request)
        {
            
            try
            {
                if(string.IsNullOrWhiteSpace(request.Password)) return BadRequest($"Password was null");
                if (string.IsNullOrWhiteSpace(request.Login)) return BadRequest($"Login was null");
                if (request is null) return BadRequest($"{nameof(request)} was null");
                var model = _mapServices.MapFromDbAuthModel(request);
                if(model is null) throw new ArgumentNullException($"{DateTime.UtcNow}: Model was null {nameof(Registration)}");
                _serviceOperation.Register(model, Password =>
                {
                    var models = _serviceOperation.GetAll().Where(p => p.Password.PasswordsIsEquals(p.SaltForPassword, p.Password));
                    return models is not null;
                });
                _logger.LogInformation($"{DateTime.UtcNow}: Service {request.Login} & {request.Password} registered");
                var token = JwtCreator.CreateToken(model, _jwtManager);
                _metrics.CounterRequestsRegistrtion!.Add(1);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.UtcNow}: Service {request.Login} & {request.Password} didn`t register, because {ex.Message} {ex.StackTrace}");
                return BadRequest();
            }
        }
        [Route("[action]")]
        [HttpPost]
        public IActionResult Login([FromQuery] AuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password)) return BadRequest($"Password was null");
            if (string.IsNullOrWhiteSpace(request.Login)) return BadRequest($"Login was null");
            if (request is null) return BadRequest($"{nameof(request)} was null");
            try
            {
                var model = _serviceOperation.GetModel(request.Login, request.Password, Password =>
                {
                    var models = _serviceOperation.GetAll().Where(p => p.Password.PasswordsIsEquals(p.SaltForPassword, p.Password));
                    return models is not null;
                });
                if (model is null)
                {
                    _logger.LogError($"{DateTime.UtcNow}: Service don`t register");
                    return Unauthorized();
                }
                var token = JwtCreator.CreateToken(model, _jwtManager);
                _metrics.CounterRequestsLogin!.Add(1);
                return Ok(token);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
           
        }

    }
}
