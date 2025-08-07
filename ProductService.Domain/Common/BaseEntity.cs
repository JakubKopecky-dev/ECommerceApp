﻿namespace ProductService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
