using Common.Enums;
using System.Collections;

namespace Common.Model.Services
{
    public class ServiceResponseFactory
    {
        public static ServiceResponse<T> CreateResponse<T>(ServiceResponseStatus status, T result)
        {
            return new ServiceResponse<T>
            {
                Status = status,
                Result = result,
                Errors = new ServiceResponseError[] { }
            };
        }

        public static ServiceResponse<T> CreateOkResponse<T>(T result)
        {
            return new ServiceResponse<T>
            {
                Status = ServiceResponseStatus.Ok,
                Result = result,
                Errors = new ServiceResponseError[] { }
            };
        }

        public static ServiceResponse<T> CreateErrorResponse<T>(ServiceResponseError[] errors)
        {
            return new ServiceResponse<T>
            {
                Status = ServiceResponseStatus.Error,
                Result = default(T),
                Errors = errors
            };
        }

        public static ServiceResponse<object> CreateErrorResponse(System.Exception exception)
        {
            var response = new ServiceResponse<object>
            {
                Status = ServiceResponseStatus.Error,
                Errors = new[]
                {
                    new ServiceResponseError {Message = exception.Message, InnerError = null,}
                }
            };

            System.Exception innerException = exception.InnerException;
            ServiceResponseError error = response.Errors[0];
            while (innerException != null)
            {
                error.InnerError = new ServiceResponseError
                {
                    Message = exception.Message,
                    InnerError = null,
                };
                innerException = innerException.InnerException;
                error = error.InnerError;
            }

            return response;
        }

        public static ServiceResponse<object> CreateValidationErrorResponse(IEnumerable modelStateErrors)
        {
            return new ServiceResponse<object>
            {
                Status = ServiceResponseStatus.ValidationError,
                Result = null,
                Errors = new ServiceResponseError[] {
                    new ServerValidationErrorResponse {
                        Message = "Surgió un error al validar los datos del request.",
                        ModelState = modelStateErrors
                    }
                }
            };
        }
    }
}
