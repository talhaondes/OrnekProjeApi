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
        public async Task<ResponseDTO<TentityDTO>> Delete(int id)
        {
            try
            {
                var sil = typeof(Tentity);
                var entityname = sil.Name;
                var idproperty = sil.GetProperty(entityname + "Id") ?? sil.GetProperty("Id");
                if (idproperty == null || idproperty.PropertyType != typeof(int))
                {
                    throw new Exception("Id property not found");
                }
                var entity = await _table.IgnoreQueryFilters().FirstOrDefaultAsync(x => (int)idproperty.GetValue(x)! == null);
                if (entity == null)
                {
                    return new ResponseDTO<TentityDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Errors = new List<string> { "Entity not found" }
                    };
                }
                if (entity is EntityBase entityBase)
                {
                    entityBase.IsDeleted = true;
                    entityBase.DeleteDate = DateTime.UtcNow;
                    entityBase.DeleteBy = "Admin";

                }

                await _apiContext.SaveChangesAsync();
                var delete = _mapper.Map<TentityDTO>(entity);
                return new ResponseDTO<TentityDTO>
                {
                    Data = delete,
                    IsSuccess = true,
                    Messages = "Silme Başaralı"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"Silme Başarısız {ex.Message}",
                    Errors = new List<string> { ex.Message }

                };
            }
            catch (Exception e)
            {
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"Silme Başarısız {e.Message}"
                };
            }
        }


        public async Task<ResponseDTO<TentityDTO>> Get(int id)
        {

            try
            {
                var sil = typeof(Tentity);
                var entityname = sil.Name;
                var idproperty = sil.GetProperty(entityname + "Id") ?? sil.GetProperty("Id");
                if (idproperty == null || idproperty.PropertyType != typeof(int))
                {
                    throw new Exception("Bu Ürünün Id'si ve ya İsmi yok");
                }
                var entity = await _table.IgnoreQueryFilters().FirstOrDefaultAsync(x => (int)idproperty.GetValue(x)! == id);
                if (entity == null)
                {
                    return new ResponseDTO<TentityDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Messages = "Entity Bulunamadı."
                    };
                }
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
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = "İşlem Başarısız."
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


        public async Task<ResponseDTO<TentityDTO>> Update(TentityDTO entity, int id)//güncelleme methodu typeof kullanarak yapılacak
        {
            try
            {
                var sil = typeof(Tentity);
                var entityname = sil.Name;
                var idproperty = sil.GetProperty(entityname + "Id") ?? sil.GetProperty("Id");
                if (idproperty == null || idproperty.PropertyType != typeof(int))
                {
                    throw new Exception("Id property not found");
                }
                var entitydb = await _table.IgnoreQueryFilters().FirstOrDefaultAsync(x => (int)idproperty.GetValue(x)! == id);
                if (entitydb == null)
                {
                    return new ResponseDTO<TentityDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Messages = "Entity Bulunamadı."
                    };
                }
                _mapper.Map(entity, entitydb);
                if (entitydb is EntityBase entityBase)
                {
                    entityBase.UpdateDate = DateTime.UtcNow;
                    entityBase.UpdateBy = "Admin";
                }
                _apiContext.Entry(entitydb).CurrentValues.SetValues(entitydb);//
                await _apiContext.SaveChangesAsync();
                var update = _mapper.Map<TentityDTO>(entitydb);
                return new ResponseDTO<TentityDTO>
                {
                    Data = entity,
                    IsSuccess = true,
                    Messages = "Güncelleme Başarılı"
                };
            }
            catch (DbUpdateException ex)
            {   
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = $"Güncelleme Başarısız {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO<TentityDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Messages = "İşlem Başarısız."
                };
            }



        }
    }
}
