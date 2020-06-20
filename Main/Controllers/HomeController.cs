using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Persistence;
using Persistence.Interfaces;
using Persistence.DTO;
using Export.Interfaces;
using Authentication.Interfaces;
using AutoMapper;

namespace Main.Controllers
{
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IFilingRepository _filingRepository;
        private readonly IExportService _exportService;
        private readonly IAuthenticationService _authenticationService; 
        private readonly IConfiguration _configuration;
        private IMapper _mapper;
        

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IFilingRepository filingRepository, IExportService exportService,
                              IAuthenticationService authenticationService, IConfiguration configuration, IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _filingRepository = filingRepository;
            _exportService = exportService;
            _authenticationService = authenticationService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/subscribe")]
        public IActionResult Subscribe(ViewModels.User userView)
        {
            IContextFactory contextFactory = new DapperContextFactory(_configuration.GetSection("ConnectionStrings").GetSection("DB").Value);
            Models.User user = new Models.User(userView.Email, userView.Password, userView.Status);
            Models.HashSalt hashSalt = _authenticationService.ComputeHashSalt(user);
            Console.WriteLine(hashSalt.Hash);
            Console.WriteLine(hashSalt.Salt);
            UserInsertDto dto = new UserInsertDto(userView.Email, hashSalt.Hash, hashSalt.Salt, userView.Status);
            using(IContext context = contextFactory.Create())
            {
                _userRepository.Insert(dto, context);
                context.Commit();
                return CreatedAtAction("subscribe", new{userView.Email});
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/login")]
        public IActionResult Login(ViewModels.User userView)
        {
            string secretKey = _configuration.GetSection("Secrets").GetSection("SecretKey").Value;
            IContextFactory contextFactory = new DapperContextFactory(_configuration.GetSection("ConnectionStrings").GetSection("DB").Value);
            Models.User user = new Models.User(userView.Email, userView.Password, userView.Status);
            using(IContext context = contextFactory.Create())
            {
                UserGetDto userGetDto = _userRepository.GetByEmail(user.Email, context)[0];
                byte[] hash = _authenticationService.ComputeHashFromSalt(userGetDto.Salt, Encoding.UTF8.GetBytes(user.Password));
                if(hash.SequenceEqual(userGetDto.Hash))
                {
                    return new ObjectResult(_authenticationService.GenerateToken(user, secretKey));
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        [HttpPost]
        [Route("api/download")]

        public IActionResult Download(ViewModels.FilingRequest filingRequestView)
        {
            IContextFactory contextFactory = new DapperContextFactory(_configuration.GetSection("ConnectionStrings").GetSection("DB").Value);
            string connectFS = _configuration.GetSection("ConnectionStrings").GetSection("FS").Value;
            string folder = "./Filings";
            Models.FilingRequest filingRequest = new Models.FilingRequest(filingRequestView.CIKS,
                                                                            filingRequestView.DateStart, 
                                                                            filingRequestView.DateEnd,
                                                                            filingRequestView.Filings,
                                                                            filingRequestView.Items);
            
            using(IContext context = contextFactory.Create())
            {
                IList<FilingGetDto> dtos = _filingRepository.GetByCIKDateFilingItem(filingRequest, context);
                IList<string> names = dtos.Select(dto => dto.FileName).ToList();
                IList<Stream> streams = _exportService.DownloadFromFileSystem(names, folder, connectFS);
                using(var ms = new MemoryStream())
                {
                    using(var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        for(int i = 0; i < streams.Count; i++)
                        {
                            ZipArchiveEntry entry = archive.CreateEntry(names[i], CompressionLevel.Fastest);
                            using(var zipStream = entry.Open())
                            {
                                streams[i].CopyTo(zipStream);
                            }
                            streams[i].Dispose();
                            
                        }
                    }
                    return File(ms.ToArray(), "application/zip", "Filings.zip"); 
                } 
            }  
        }

        [HttpPost]
        [Route("api/getCiks")]
        public IActionResult GetCiks()
        {
            IContextFactory contextFactory = new DapperContextFactory(_configuration.GetSection("ConnectionStrings").GetSection("DB").Value);
            using(IContext context = contextFactory.Create())
            {
                IList<int> ciks = _filingRepository.GetAllCiks(context);
                return new ObjectResult(ciks);
            }
        }
    }
}
