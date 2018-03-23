namespace MyTeam.Models.Structs
{
    public struct JsonResponseMessage
    {
        public bool Success { get; set; }
        public string ValidationMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
}