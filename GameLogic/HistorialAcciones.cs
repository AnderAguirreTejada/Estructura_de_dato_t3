using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Historial de acciones del jugador.
///
/// Estructura clave: Stack<AccionJugador>.
/// El botón "Deshacer" siempre revierte la última acción realizada (LIFO),
/// que es exactamente la semántica de una Pila: Push al hacer, Pop al deshacer.
/// </summary>
public class HistorialAcciones
{
    private readonly Stack<AccionJugador> _pila = new();

    public int Cantidad => _pila.Count;
    public bool PuedeDeshacerse => _pila.Count > 0;

    public string? DescripcionPico => _pila.TryPeek(out var a) ? a.Descripcion : null;

    public void Agregar(AccionJugador accion) => _pila.Push(accion);

    public AccionJugador? Sacar() => _pila.TryPop(out var a) ? a : null;

    public void Limpiar() => _pila.Clear();
}
