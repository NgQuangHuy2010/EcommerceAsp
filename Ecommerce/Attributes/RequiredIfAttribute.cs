using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfNewAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public RequiredIfNewAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var viewModel = validationContext.ObjectInstance as IViewModelWithId;

            // Kiểm tra nếu là sản phẩm mới và không có hình thì bắt lỗi required
            if (viewModel?.Id == 0 && value == null)
            {
                return new ValidationResult(ErrorMessage ?? "Vui lòng thêm hình cho sản phẩm này");
            }

            return ValidationResult.Success;
        }
    }

    public interface IViewModelWithId
    {
        int Id { get; }
    }
}
