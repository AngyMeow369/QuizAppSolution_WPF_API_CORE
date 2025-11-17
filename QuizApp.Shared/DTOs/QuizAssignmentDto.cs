using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Shared.DTOs
{
    public class QuizAssignmentDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int UserId { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

