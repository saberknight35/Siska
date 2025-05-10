using System.ComponentModel.DataAnnotations;

namespace Siska.Admin.Model.Extensions
{
    public static class DataValidatorExtensions
    {
        public static void Validate(this object model)
        {
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);
        }
    }
}
