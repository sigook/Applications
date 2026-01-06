using Covenant.Common.Entities.Candidate;
using Covenant.Common.Models;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Mappers
{
    public static class CandidateExtensionsMapping
    {
        public static Expression<Func<CandidateNote, NoteModel>> SelectNote = n => new NoteModel
        {
            Id = n.NoteId,
            Note = n.Note.Note,
            Color = n.Note.Color,
            CreatedAt = n.Note.CreatedAt,
            CreatedBy = n.Note.CreatedBy
        };
    }
}
