using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Controla el spawn de enemigos.
///
/// Estructura clave: Queue<GeneracionEnemigo>.
/// Cada oleada llena la cola con sus enemigos en orden. En cada tick
/// del game loop, si pasó suficiente tiempo, se hace Dequeue() y se
/// genera el siguiente enemigo. Esto garantiza orden FIFO de salida,
/// que es el comportamiento natural de una oleada.
///
/// El catálogo total de oleadas se guarda en una List<Oleada>.
/// </summary>
public class GestorOleadas
{
    private readonly List<Oleada> _oleadas;
    private double _tiempoHastaProximaGeneracion;

    public Queue<GeneracionEnemigo> ColaActual { get; private set; } = new();
    public int ÍndiceOleadaActual { get; private set; } = -1;

    public int NúmeroOleadaActual => ÍndiceOleadaActual + 1;
    public int TotalOleadas => _oleadas.Count;

    public bool EsOleadaActiva => ColaActual.Count > 0;
    public bool HayMásOleadas => ÍndiceOleadaActual < _oleadas.Count - 1;
    public bool TodasLasOleadasTerminadas => ÍndiceOleadaActual >= _oleadas.Count - 1 && ColaActual.Count == 0;

    public Oleada? OleadaActual => ÍndiceOleadaActual >= 0 && ÍndiceOleadaActual < _oleadas.Count
        ? _oleadas[ÍndiceOleadaActual]
        : null;

    public GestorOleadas(List<Oleada> oleadas)
    {
        _oleadas = oleadas;
    }

    public bool IntentarIniciarSiguienteOleada()
    {
        if (EsOleadaActiva || !HayMásOleadas) return false;
        ÍndiceOleadaActual++;
        ColaActual = new Queue<GeneracionEnemigo>(_oleadas[ÍndiceOleadaActual].Generaciones);
        _tiempoHastaProximaGeneracion = 0;
        return true;
    }

    public TipoEnemigo? Tick(double dt)
    {
        if (ColaActual.Count == 0) return null;
        _tiempoHastaProximaGeneracion -= dt;
        if (_tiempoHastaProximaGeneracion > 0) return null;

        var generacion = ColaActual.Dequeue();
        _tiempoHastaProximaGeneracion = generacion.EsperaAntes;
        return generacion.Tipo;
    }
}
