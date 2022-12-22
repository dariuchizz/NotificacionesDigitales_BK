using System.Collections.Generic;
using Common.IRepositories;
using Common.Model.NotificacionesDigitales;
using Dapper;
using System.Threading.Tasks;
using Common.Model.Dto;
using System;

namespace Common.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public StoreRepository(INotificacionesDigitalesDbContext context)
        {
            _context = context;
        }

        public async Task<dynamic> GetDataViewAsync(string viewName, int top)
        {
            var sql = $"SELECT TOP {top} * FROM {viewName} with (nolock)";
            var response = await _context.Connection().QueryAsync<dynamic>(sql);
            return response;
        }

        public async Task<dynamic> GetDataStoreAsync(string storeName, int top, int timeOut)
        {
            var sql = $"EXEC dbo.{storeName}";
            if (top > 0)
            {
                sql = $"{sql} {top}";
            }
            var response = await _context.Connection()
                .QueryAsync<dynamic>(sql, commandTimeout: timeOut);
            return response;
        }
        public async Task<dynamic> GetDataStoreAsync(string storeName, long idCampania, int top, int timeOut)
        {
            var sql = $"EXEC dbo.{storeName} {idCampania}";
            if (top > 0)
            {
                sql = $"{sql}, {top}";
            }
            var response = await _context.Connection()
                .QueryAsync<dynamic>(sql, commandTimeout: timeOut);
            return response;
        }
        public async Task<dynamic> GetDataStoreAsync(string storeName, int timeOut)
        {
            var sql = $"EXEC dbo.{storeName}";
            var response = await _context.Connection()
                .QueryAsync<dynamic>(sql, commandTimeout: timeOut);
            return response;
        }

        public async Task<dynamic> ExecuteAsync(string storeName, int timeOut = 240)
        {
            var sql = $"EXEC dbo.{storeName}";
            var response = await _context.Connection().ExecuteAsync(sql, commandTimeout: 0);
            return response;
        }
        public async Task<dynamic> ExecuteAsync(string storeName, long id, int timeOut = 240)
        {
            var sql = $"EXEC dbo.{storeName} {id}";
            var response = await _context.Connection().ExecuteAsync(sql, commandTimeout: 0);
            return response;
        }

        public async Task<IEnumerable<ConsultarCampaniaDto>> GetDatosCampaniaAsync(string storeName, long idCampania, int timeOut = 240)
        {
            try
            {
                var sql = $"EXEC dbo.{storeName} {idCampania}";
                var response = await _context.Connection()
                    .QueryAsync<ConsultarCampaniaDto>(sql, commandTimeout: timeOut);
                return response;
            }
            catch (Exception ex) { throw ex; }
            

        }
    }
}
