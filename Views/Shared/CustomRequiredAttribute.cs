using System.ComponentModel.DataAnnotations;

namespace ContaFacil.Attributes
{
    public class CustomRequiredAttribute : RequiredAttribute
    {
        public CustomRequiredAttribute() : base()
        {
            ErrorMessage = "Este campo es obligatorio.";
        }
    }
}