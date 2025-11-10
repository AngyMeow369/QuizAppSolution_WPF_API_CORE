using QuizApp.Shared.DTOs;
using System.Collections.ObjectModel;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class QuestionViewModel : ObservableObject
    {
        private string _text = string.Empty;
        private int _categoryId;
        private string _categoryName = string.Empty;

        public int Id { get; set; }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public int CategoryId
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId, value);
        }

        public string CategoryName
        {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }

        public ObservableCollection<OptionDto> Options { get; set; } = new();

        public QuestionDto ToDto()
        {
            return new QuestionDto
            {
                Id = Id,
                Text = Text,
                CategoryId = CategoryId,
                CategoryName = CategoryName,
                Options = Options.ToList()
            };
        }

        public static QuestionViewModel FromDto(QuestionDto dto)
        {
            return new QuestionViewModel
            {
                Id = dto.Id,
                Text = dto.Text,
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName,
                Options = new ObservableCollection<OptionDto>(
                    dto.Options?.Select(o => new OptionDto
                    {
                        Id = o.Id,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect,
                        QuestionId = o.QuestionId
                    }) ?? new List<OptionDto>()
                )
            };
        }

    }
}
