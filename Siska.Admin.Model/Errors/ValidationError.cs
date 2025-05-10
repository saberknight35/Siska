using System.ComponentModel.DataAnnotations;

namespace Siska.Admin.Model.Errors
{
    public static class ValidationError
    {
        public static List<string> ValidationErrorMessage(this List<ValidationResult> result) 
        {
            List<string> message = new List<string>();

            foreach (var item in result)
            {
                message.Add(item.ErrorMessage);
            }

            return message;
        }
    }
}
