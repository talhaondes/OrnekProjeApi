using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ProjeApiBusiness.Service.Abstract;
using ProjeApiModel.DTOs.Data;
using ProjeApiModel.DTOs.IdentityDTO;
using ProjeApiModel.DTOs.JWT;
using ProjeApiModel.DTOs.Response;
using ProjeApiModel.ViewModel;
using ProjeApiModel.ViewModel.Identity;

namespace ProjeApiUIAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, IConfiguration configuration, IPasswordHasher<User> passwordHasher, ICategoryService categoryService, IMapper mapper)
        {
            _userService = userService;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        //giriş yap ve üye ol için methodlar oluştur
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginReuqirestDTO loginReuqirestDTO)
        {
            var userResponse = await _userService.GetByUserName(loginReuqirestDTO.UserName);

            if (userResponse.StatusCode != ProjeApiModel.Enums.StatusCode.Success || userResponse.Data == null)
            {
                return Unauthorized(new ResponseDTO<LoginResponseDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Kullanıcı adı veya şifre hatalı"
                });
            }

            var user = userResponse.Data;

            var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginReuqirestDTO.Password);
            if (passwordCheck != PasswordVerificationResult.Success)
            {
                return Unauthorized(new ResponseDTO<LoginResponseDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Kullanıcı adı veya şifre hatalı"
                });
            }

            // DTO map işlemi
            var userdto = _mapper.Map<UserDTO>(user);
            var token = generatetoken(userdto);

            var response = new LoginResponseDTO
            {
                tokenDTO = token,
                UserName = user.UserName,
                UserId = user.Id,
                Roles = userdto.Roles
            };

            return Ok(new ResponseDTO<LoginResponseDTO>
            {
                StatusCode = ProjeApiModel.Enums.StatusCode.Success,
                Data = response,
                Messages = "Giriş Başarılı"
            });
        }

        private TokenDTO generatetoken(UserDTO userdto)
        {
            var secretKey = _configuration["JWT:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("SecretKey Hatası");

            var key = Encoding.UTF8.GetBytes(secretKey);
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userdto.Id.ToString()),
        new Claim(ClaimTypes.Name, userdto.UserName),
        new Claim(ClaimTypes.Email, userdto.Email),
    };
            claims.AddRange(userdto.Roles.Select(x => new Claim(ClaimTypes.Role, x)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:ExpiryInMinutes"] ?? "60")),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenDTO
            {
                AccessToken = tokenHandler.WriteToken(token),
                AccessTokenExpiration = tokenDescriptor.Expires ?? DateTime.Now.AddMinutes(60),
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpiration = DateTime.Now.AddDays(7) // örneğin 7 gün

            };
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterResponseDTO registerresponsedto)
        {
            var userCheck = await _userService.GetAll();
            if (userCheck.StatusCode != ProjeApiModel.Enums.StatusCode.Success)
            {
                return BadRequest(userCheck);
            }

            var existingUser = userCheck.Data?.FirstOrDefault(x => x.UserName == registerresponsedto.UserName || x.Email == registerresponsedto.Email);
            if (existingUser != null)
            {
                return BadRequest(new ResponseDTO<LoginResponseDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Kullanıcı adı veya e-posta zaten kayıtlı.",
                });
            }

            // Entity'yi oluştur
            var userEntity = new User
            {
                UserName = registerresponsedto.UserName,
                Email = registerresponsedto.Email,
                NormalizedEmail = registerresponsedto.Email.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // Şifreyi hashle
            userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, registerresponsedto.PassWord);

            // DTO'ya maple
            var userDto = _mapper.Map<UserDTO>(userEntity);
            userDto.Roles = new List<string> { "User" };
            userDto.PasswordHash = userEntity.PasswordHash;

            // Kaydet
            var createResponse = await _userService.Create(userDto);

            if (createResponse.StatusCode != ProjeApiModel.Enums.StatusCode.Success)
            {
                return BadRequest(createResponse);
            }

            // Token üret
            var token = generatetoken(createResponse.Data!);
            var response = new LoginResponseDTO
            {
                tokenDTO = token,
                UserName = createResponse.Data.UserName,
                UserId = createResponse.Data.Id,
                Roles = createResponse.Data.Roles,
            };

            return Ok(new ResponseDTO<LoginResponseDTO>
            {
                StatusCode = ProjeApiModel.Enums.StatusCode.Success,
                Data = response,
                Messages = "Kayıt başarılı"
            });
        }
        [HttpPost("Category")]
        
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            var entity = new Category
            {
                CategoryName = categoryDto.CategoryName,
                CategoryDescription = categoryDto.CategoryDescription,
                CreateDate = DateTime.UtcNow,
                CreateBy = "Admin" // otomatik olarak admin ekledi diye atadım
            };

            var dto = _mapper.Map<CategoryDTO>(entity);
            var result = await _categoryService.Create(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



    }
}
