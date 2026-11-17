using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.DAL.Models;

public class OwnershipEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public Guid TicketId { get; set; }

    public Ticket Ticket { get; set; } = null!;
    
    public Guid AgentId { get; set; }

    public User Agent { get; set; } = null!;
    
    public DateTime DateCreated { get; set; }
}