using TowerDefenseWPF.EstructurasDeDatos;
using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;


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
