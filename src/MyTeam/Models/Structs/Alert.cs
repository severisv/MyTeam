using MyTeam.Models.Enums;

namespace MyTeam.Models.Structs
{
    public class Alert
    {
        public AlertType Type { get; }
        public string Message { get; }


        public Alert(AlertType type, string message)
        {
            Message = message;
            Type = type;
        }
        
        public string Div => $"<div class='alert alert-{Type.ToString().ToLower()}'>{Icon} {Message}<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button></div>";

        public string Icon {
            get
            {
                switch (Type)
                {
                    case AlertType.Success:
                        return "check";
                    case AlertType.Danger:
                        return "warning";
                    case AlertType.Info:
                        return "info";
                    case AlertType.Warning:
                        return "warning";
                    default:
                        return "info";

                }
            }
        }
    }
}