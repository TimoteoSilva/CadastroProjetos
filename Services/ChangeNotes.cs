using System.Collections.Concurrent;

namespace Falcare.Projetos.App.Services;

public class ChangeNotes : IChangeNotes
{
    // thread-safe por garantia
    private readonly ConcurrentDictionary<int, string?> _statusNotes = new();

    public void SetStatusNote(int projetoId, string? note)
        => _statusNotes[projetoId] = note;

    public string? PopStatusNote(int projetoId)
    {
        _statusNotes.TryRemove(projetoId, out var note);
        return note;
    }
}
