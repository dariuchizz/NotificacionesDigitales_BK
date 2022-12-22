using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Services;
using Common.Validations;

namespace Common.Services
{
    public class CsvCampaniaServices: ICsvCampaniaServices
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkNotificacion _unitOfWork;

        public CsvCampaniaServices(IMapper mapper, IUnitOfWorkNotificacion unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CsvCampaniaDto> GetFirstRowAsync(long idCampania)
        {
            var response = await _unitOfWork.CsvCampaniaRepository().GetFirstRowAsync(idCampania);
            var final = _mapper.Map<CsvCampaniaDto>(response);
            return final;
        }       
    }
}
