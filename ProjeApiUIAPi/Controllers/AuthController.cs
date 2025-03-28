using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(IUserService userService, IConfiguration configuration,
            IPasswordHasher<User> passwordHasher, ICategoryService categoryService, IMapper mapper)
        {
            _userService = userService;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _categoryService = categoryService;
            _mapper = mapper;
        }

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

            var userdto = _mapper.Map<UserDTO>(user);
            var token = GenerateToken(userdto);

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

        private TokenDTO GenerateToken(UserDTO userdto)
        {
            var secretKey = _configuration["JWT:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("SecretKey Hatası");

            var key = Encoding.UTF8.GetBytes(secretKey);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userdto.Id.ToString()),
                new Claim(ClaimTypes.Name, userdto.UserName),
                new Claim(ClaimTypes.Email, userdto.Email)
            };
            claims.AddRange(userdto.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:ExpiryInMinutes"] ?? "60")),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenDTO
            {
                AccessToken = tokenHandler.WriteToken(token),
                AccessTokenExpiration = tokenDescriptor.Expires ?? DateTime.Now.AddMinutes(60),
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpiration = DateTime.Now.AddDays(7)
            };
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterResponseDTO registerResponseDto)
        {
            var userCheck = await _userService.GetAll();
            if (userCheck.StatusCode != ProjeApiModel.Enums.StatusCode.Success)
            {
                return BadRequest(userCheck);
            }

            var existingUser = userCheck.Data?.FirstOrDefault(x =>
                x.UserName == registerResponseDto.UserName || x.Email == registerResponseDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new ResponseDTO<LoginResponseDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Kullanıcı adı veya e-posta zaten kayıtlı."
                });
            }

            var userEntity = new User
            {
                UserName = registerResponseDto.UserName,
                Email = registerResponseDto.Email,
                NormalizedEmail = registerResponseDto.Email.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, registerResponseDto.PassWord);
            var userDto = _mapper.Map<UserDTO>(userEntity);
            userDto.Roles = new List<string> { "User" };
            userDto.PasswordHash = userEntity.PasswordHash;
            var createResponse = await _userService.Create(userDto);
            if (createResponse.StatusCode != ProjeApiModel.Enums.StatusCode.Success)
            {
                return BadRequest(createResponse);
            }

            var token = GenerateToken(createResponse.Data!);
            var response = new LoginResponseDTO
            {
                tokenDTO = token,
                UserName = createResponse.Data.UserName,
                UserId = createResponse.Data.Id,
                Roles = createResponse.Data.Roles
            };

            return Ok(new ResponseDTO<LoginResponseDTO>
            {
                StatusCode = ProjeApiModel.Enums.StatusCode.Success,
                Data = response,
                Messages = "Kayıt başarılı"
            });
        }
        [HttpPost("CategoryAdd")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            // Yeni kategori oluşturuluyor.
            var entity = new Category
            {
                CategoryName = categoryDto.CategoryName,
                CategoryDescription = categoryDto.CategoryDescription,
                CreateDate = DateTime.UtcNow,
                CreateBy = "Admin" // Bu değeri dinamik hale getirebilirsiniz.
            };

            // Entity, AutoMapper kullanılarak DTO'ya dönüştürülüyor (ters map işlemi yapılırsa gerekli veriler güncellenebilir).
            var dto = _mapper.Map<CategoryDTO>(entity);

            // Servis katmanı aracılığıyla kategori ekleniyor.
            var result = await _categoryService.Create(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpGet("CategoryGet")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetAll();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("CategoryDelete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var existingCategory = await _categoryService.Get(id);

            if (!existingCategory.IsSuccess || existingCategory.Data == null)
            {
                return NotFound(new ResponseDTO<CategoryDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Silinmek istenen kategori bulunamadı."
                });
            }

            var deleteResult = await _categoryService.Delete(id);

            if (!deleteResult.IsSuccess)
            {
                return BadRequest(new ResponseDTO<CategoryDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Kategori silinirken bir hata oluştu."
                });
            }

            return Ok(new ResponseDTO<CategoryDTO>
            {
                StatusCode = ProjeApiModel.Enums.StatusCode.Success,
                Messages = "Kategori başarıyla silindi."
            });
        }


        [HttpPut("CategoryEdit/{id}")]
        public async Task<IActionResult> EditCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            var existingCategory = await _categoryService.Get(id);

            if (!existingCategory.IsSuccess || existingCategory.Data == null)
            {
                return NotFound(new ResponseDTO<CategoryDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Güncellenmek istenen kategori bulunamadı."
                });
            }

            existingCategory.Data.CategoryName = categoryDto.CategoryName;
            existingCategory.Data.CategoryDescription = categoryDto.CategoryDescription;

            var updateResult = await _categoryService.Update(existingCategory.Data, id);

            if (!updateResult.IsSuccess)
            {
                return BadRequest(new ResponseDTO<CategoryDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Kategori güncellenirken bir hata oluştu."
                });
            }

            return Ok(new ResponseDTO<CategoryDTO>
            {
                StatusCode = ProjeApiModel.Enums.StatusCode.Success,
                Data = updateResult.Data,
                Messages = "Kategori başarıyla güncellendi."
            });
        }

        [HttpGet("GetByIdCategory/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
             var result = await _categoryService.Get(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound(new ResponseDTO<CategoryDTO>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = "Kategori bulunamadı."
                });
            }
            return Ok(result);
        }




    }
}
