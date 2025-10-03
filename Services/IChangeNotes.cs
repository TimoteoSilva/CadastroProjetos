namespace Falcare.Projetos.App.Services;

public interface IChangeNotes
{
    void SetStatusNote(int projetoId, string? note);
    string? PopStatusNote(int projetoId);
}