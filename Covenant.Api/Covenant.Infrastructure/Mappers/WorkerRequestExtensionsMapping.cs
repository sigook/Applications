using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Mappers
{
    public static class WorkerRequestExtensionsMapping
    {
        public static Expression<Func<WorkerRequestNote, NoteModel>> SelectNote = s => new NoteModel
        {
            Id = s.NoteId,
            Note = s.Note.Note,
            Color = s.Note.Color,
            CreatedAt = s.Note.CreatedAt,
            CreatedBy = s.Note.CreatedBy
        };
    }
}
