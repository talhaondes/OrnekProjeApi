using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProjeApiDal.Context;
using ProjeApiDal.Repository.Abstract;
using ProjeApiModel.Core;
using ProjeApiModel.DTOs.Response;
using ProjeApiModel.Enums;

namespace ProjeApiDal.Repository.Concrete
{
    public class RepositoryDTO<Tentity, TentityDTO> : IRepositoryDTO<TentityDTO>
        where Tentity : class, new()
        where TentityDTO : class, new()
    {
        private readonly ApiContext _apiContext;
        protected readonly DbSet<Tentity> _table;
        protected readonly DbSet<TentityDTO> _dbSetDTO;
        private readonly IMapper _mapper;

        public RepositoryDTO(ApiContext apiContext, DbSet<Tentity> dbSet, DbSet<TentityDTO> dbSetDTO, IMapper mapper)
        {
            _apiContext = apiContext;
            _table = dbSet;
            _dbSetDTO = dbSetDTO;
            _mapper = mapper;
        }

        public async Task<ResponseDTO<TentityDTO>> Create(TentityDTO entity)
        {
            try
            {
                var entitydb = _mapper.Map<Tentity>(entity);
                if (entitydb is EntityBase entityBase)
                {
                    entityBase.CreateDate = DateTime.UtcNow;
                    entityBase.CreateBy = "Admin";
                }
                await _table.AddAsync(entitydb);
                await _apiContext.SaveChangesAsync();
                var dto = _mapper.Map<TentityDTO>(entitydb);
                return new ResponseDTO<TentityDTO>
                {
                    Data = dto,
                    IsSuccess = true,
                    Messages = "Ekleme Başarılı"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"Ekleme Başarısız {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"Genel Ekleme Başarısız {e.Message}"
                };
            }

        }
        public virtual async Task<ResponseDTO<TentityDTO>> Delete(int id)
        {
            try
            {
                var entityType = typeof(Tentity);
                var entityName = entityType.Name;

                var idProperty = entityType.GetProperty("Id") ?? entityType.GetProperty($"{entityName}Id");

                if (idProperty == null || idProperty.PropertyType != typeof(int))
                    throw new InvalidOperationException($"Entity {entityName} does not have an integer 'Id' or '{entityName}Id' property.");

                var entityDb = await _table.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => EF.Property<int>(e, idProperty.Name) == id);

                if (entityDb == null)
                {
                    return new ResponseDTO<TentityDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Messages = "Entity Bulunamadı."
                    };
                }

                if (entityDb is EntityBase entityBase)
                {
                    entityBase.IsDeleted = true;
                    entityBase.DeleteDate = DateTime.UtcNow;
                    entityBase.DeleteBy = "Admin";
                }

                await _apiContext.SaveChangesAsync();

                var deletedDto = _mapper.Map<TentityDTO>(entityDb);

                return new ResponseDTO<TentityDTO>
                {
                    Data = deletedDto,
                    IsSuccess = true,
                    Messages = "Silme Başarılı"
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"İşlem Başarısız. {e.Message}"
                };
            }
        }

        public virtual async Task<ResponseDTO<TentityDTO>> Get(int id)
        {
            try
            {
                // Tentity türünü alıyoruz.
                var entityType = typeof(Tentity);
                var entityName = entityType.Name;

                // "Id" veya "{entityName}Id" özelliğini arıyoruz.
                var idProperty = entityType.GetProperty("Id") ?? entityType.GetProperty($"{entityName}Id");

                // Eğer "Id" veya "{entityName}Id" özelliği yoksa veya tipi integer değilse, hata fırlatıyoruz.
                if (idProperty == null || idProperty.PropertyType != typeof(int))
                    throw new InvalidOperationException($"Entity {entityName} does not have an integer 'Id' or '{entityName}Id' property.");

                // Veritabanında, verilen ID'ye sahip entity'yi arıyoruz (query filtrelerini yoksayarak).
                var entity = await _table.IgnoreQueryFilters()
                     .FirstOrDefaultAsync(e => EF.Property<int>(e, idProperty.Name) == id);
                if (entity == null)
                {
                    return new ResponseDTO<TentityDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Messages = "Entity Bulunamadı."
                    };
                }

                // 3) Entity’yi DTO’ya dönüştür
                var dto = _mapper.Map<TentityDTO>(entity);

                return new ResponseDTO<TentityDTO>
                {
                    Data = dto,
                    IsSuccess = true,
                    Messages = "Getirme Başarılı"
                };
            }
            catch (Exception e)
            {
                // Hata durumunda, loglamak isterseniz e.Message’i kullanabilirsiniz
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"İşlem Başarısız. {e.Message}"
                };
            }
        }

        public async Task<ResponseDTO<List<TentityDTO>>> GetAll()
        {
            try
            {
                var entities = await _table.AsNoTracking().ToListAsync();
                var dtos = _mapper.Map<List<TentityDTO>>(entities);
                return new ResponseDTO<List<TentityDTO>>
                {
                    Data = dtos,
                    IsSuccess = true,
                    Messages = "Liste Başarılı",
                    StatusCode = ProjeApiModel.Enums.StatusCode.Success
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO<List<TentityDTO>>
                {
                    Data = null,
                    IsSuccess = false,
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ResponseDTO<List<TentityDTO>>> GetAllIgnoeFilters()
        {
            try
            {
                var entities = await _table.IgnoreQueryFilters().AsNoTracking().ToListAsync();
                var dtos = _mapper.Map<List<TentityDTO>>(entities);
                return new ResponseDTO<List<TentityDTO>>
                {
                    Data = dtos,
                    IsSuccess = true,
                    Messages = "Liste Başarılı",
                    StatusCode = ProjeApiModel.Enums.StatusCode.Success
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO<List<TentityDTO>>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = "Kayıt Alınırken Hata oluştu"
                };

            }
        }
        public virtual async Task<ResponseDTO<TentityDTO>> Update(TentityDTO entityDto, int id)
        {
            try
            {
                var entityType = typeof(Tentity);
                var entityName = entityType.Name;

                var idProperty = entityType.GetProperty("Id") ?? entityType.GetProperty($"{entityName}Id");

                if (idProperty == null || idProperty.PropertyType != typeof(int))
                    throw new InvalidOperationException($"Entity {entityName} does not have an integer 'Id' or '{entityName}Id' property.");

                var entityDb = await _table.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => EF.Property<int>(e, idProperty.Name) == id);

                if (entityDb == null)
                {
                    return new ResponseDTO<TentityDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Messages = "Entity Bulunamadı."
                    };
                }

                _mapper.Map(entityDto, entityDb);

                if (entityDb is EntityBase entityBase)
                {
                    entityBase.UpdateDate = DateTime.UtcNow;
                    entityBase.UpdateBy = "Admin";
                }

                await _apiContext.SaveChangesAsync();

                var updatedDto = _mapper.Map<TentityDTO>(entityDb);

                return new ResponseDTO<TentityDTO>
                {
                    Data = updatedDto,
                    IsSuccess = true,
                    Messages = "Güncelleme Başarılı"
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"İşlem Başarısız. {e.Message}"
                };
            }
        }



    }
}
