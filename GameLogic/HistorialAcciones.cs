using TowerDefenseWPF.EstructurasDeDatos;
using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Historial de acciones del jugador.
///
/// Estructura clave: Pila&lt;AccionJugador&gt; (LIFO propia).
/// El botón "Deshacer" siempre revierte la última acción realizada,
/// que es exactamente la semántica de una Pila: Apilar al hacer, Desapilar al deshacer.
/// </summary>
public class HistorialAcciones
{
    private readonly Pila<AccionJugador> _pila = new();

    public int Cantidad => _pila.Cantidad;
    public bool PuedeDeshacerse => _pila.PuedeDeshacerse;

    public string? DescripcionPico => _pila.IntentarVerCima(out var a) ? a.Descripcion : null;

    public void Agregar(AccionJugador accion) => _pila.Apilar(accion);

    public AccionJugador? Sacar() => _pila.IntentarDesapilar(out var a) ? a : null;

    public void Limpiar() => _pila.Limpiar();
}
