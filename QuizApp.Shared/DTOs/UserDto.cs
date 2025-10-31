﻿namespace QuizApp.Shared.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AssignedQuizCount { get; set; }
        public int AttemptedQuizCount { get; set; }

    }
}