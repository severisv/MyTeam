using MyTeam.Models.Enums;

namespace MyTeam.Models.Structs
{
    public static class JsonResponse
    {
        public static JsonResponseMessage Success(string message = "") => new JsonResponseMessage { Success = true, SuccessMessage = message };
        public static JsonResponseMessage Failure => new JsonResponseMessage { Success = false };
        public static JsonResponseMessage ValidationFailed(string message) => new JsonResponseMessage { Success = false, ValidationMessage = message };


    }
}