using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UAE_Pass_Poc.Entities
{
    public interface IEntity
    {

    }

    public interface IDeleted
    {
        public bool Deleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public abstract class EntityBase : IEntity
    {
        /// <summary>
        /// Autogenrated ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]        
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public abstract class EntityRel : EntityBase
    {
        protected EntityRel()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Timestamp for CreatedAt
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    public abstract class Entity : EntityRel, IDeleted
    {
        protected Entity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Timestamp for UpdatedAt
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}