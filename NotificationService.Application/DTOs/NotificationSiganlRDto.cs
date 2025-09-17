using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.DTOs
{
    public sealed record NotificationSiganlRDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = "";

        public string Message { get; init; } = "";

        public DateTime CreatedAt { get; init; }
    }
}
