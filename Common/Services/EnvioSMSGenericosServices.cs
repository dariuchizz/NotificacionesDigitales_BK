using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Common.Model.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services
{
    public class EnvioSMSGenericosServices : IEnvioSMSGenericosServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public EnvioSMSGenericosServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<long>> AddEnvioSMSGenericoAsync(EnvioSMSRequest dto)
        {
            //Chequear si exsite el Sistema
            var sistema = await _unitOfWork.SistemaRepository()
            .FindByAsync(f => f.IdSistema == dto.IdSistema);
            if (sistema is null) {
                return ServiceResponseFactory.CreateErrorResponse<long>(new[]
                {
                    new ServiceResponseError
                    {
                        Message = "El sistema no existe."
                    },
                });
            }
            //Chequear si existe el proceso de negocio relacionado con el sistema
            var procesoNegocio = await _unitOfWork.ProcesoNegocioRepository()
            .FindByAsync(f => f.IdSistema == dto.IdSistema && f.IdProcesoNegocio == dto.IdProcesoNegocio);
            if (procesoNegocio is null)
            {
                return ServiceResponseFactory.CreateErrorResponse<long>(new[]
                {
                    new ServiceResponseError
                    {
                        Message = "El proceso de negocio no existe."
                    },
                });
            }
            else {
                //chequeamos el limite diario del proceso de negocio
                IEnumerable<EnvioSmsGenerico> enviosGenericoSMS = await _unitOfWork.EnvioSMSGenericoRepository()
                .SearchByAsync(f => f.IdSistema == dto.IdSistema
                    && f.IdProcesoNegocio == dto.IdProcesoNegocio
                        && f.FechaCreacion.Day == DateTime.Now.Day
                        && f.FechaCreacion.Month == DateTime.Now.Month
                        && f.FechaCreacion.Year == DateTime.Now.Year
                        && f.Activo == true);
                int count = new List<EnvioSmsGenerico>(enviosGenericoSMS).Count;
                if (count >= procesoNegocio.TopeDiario)
                {
                    return ServiceResponseFactory.CreateErrorResponse<long>(new[]
                    {
                        new ServiceResponseError
                        {
                            Message = "Limite diario de envio del proceso de negocio."
                        },
                    });
                }               
            }

            var envioSmsGenerico = new EnvioSmsGenerico
            {
                NroCelular = dto.NroCelular,
                TextoMensaje = dto.TextoMensaje,
                IdSistema = dto.IdSistema,
                IdProcesoNegocio = dto.IdProcesoNegocio,
                Datos = dto.Datos,
                Autor = 2,
                FechaCreacion = System.DateTime.Now,
                Activo = true
            };
            await _unitOfWork.EnvioSMSGenericoRepository().AddAsync(envioSmsGenerico);
            await _unitOfWork.SaveChangeAsync();
            return ServiceResponseFactory.CreateOkResponse(envioSmsGenerico.IdEnvioSMSGenerico);
        }
    }
}