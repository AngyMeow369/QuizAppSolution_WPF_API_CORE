using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace QuizApp.Shared.DTOs
{
    public class QuizDto : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private DateTime _startTime;
        private DateTime _endTime;
        private int _categoryId;
        private string _categoryName = string.Empty;
        private List<QuestionDto> _questions = new();

        public int Id { get; set; }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public DateTime StartTime
        {
            get => _startTime;
            set { _startTime = value; OnPropertyChanged(nameof(StartTime)); }
        }

        public DateTime EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(nameof(EndTime)); }
        }

        public int CategoryId
        {
            get => _categoryId;
            set { _categoryId = value; OnPropertyChanged(nameof(CategoryId)); }
        }

        public string CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(nameof(CategoryName)); }
        }

        public List<QuestionDto> Questions
        {
            get => _questions;
            set { _questions = value; OnPropertyChanged(nameof(Questions)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
