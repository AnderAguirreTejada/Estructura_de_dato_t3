using TowerDefenseWPF.EstructurasDeDatos;
using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;
public class GestorOleadas
{
    private readonly Lista<Oleada> _oleadas;
    private double _tiempoHastaProximaGeneracion;

    public Cola<GeneracionEnemigo> ColaActual { get; private set; } = new();
    public int ÍndiceOleadaActual { get; private set; } = -1;

    public int NúmeroOleadaActual => ÍndiceOleadaActual + 1;
    public int TotalOleadas => _oleadas.Cantidad;

    public bool EsOleadaActiva => ColaActual.Cantidad > 0;
    public bool HayMásOleadas => ÍndiceOleadaActual < _oleadas.Cantidad - 1;
    public bool TodasLasOleadasTerminadas => ÍndiceOleadaActual >= _oleadas.Cantidad - 1 && ColaActual.Cantidad == 0;

    public Oleada? OleadaActual => ÍndiceOleadaActual >= 0 && ÍndiceOleadaActual < _oleadas.Cantidad
        ? _oleadas[ÍndiceOleadaActual]
        : null;

    public GestorOleadas(Lista<Oleada> oleadas)
    {
        _oleadas = oleadas;
    }

    public bool IntentarIniciarSiguienteOleada()
    {
        if (EsOleadaActiva || !HayMásOleadas) return false;
        ÍndiceOleadaActual++;

        // Cargar las generaciones de la oleada en la cola propia
        ColaActual = new Cola<GeneracionEnemigo>();
        foreach (var gen in _oleadas[ÍndiceOleadaActual].Generaciones)
            ColaActual.Encolar(gen);

        _tiempoHastaProximaGeneracion = 0;
        return true;
    }

    public TipoEnemigo? Tick(double dt)
    {
        if (ColaActual.Cantidad == 0) return null;
        _tiempoHastaProximaGeneracion -= dt;
        if (_tiempoHastaProximaGeneracion > 0) return null;

        var generacion = ColaActual.Desencolar();
        _tiempoHastaProximaGeneracion = generacion.EsperaAntes;
        return generacion.Tipo;
    }
}
