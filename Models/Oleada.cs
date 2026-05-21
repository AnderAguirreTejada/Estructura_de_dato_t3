using TowerDefenseWPF.EstructurasDeDatos;

namespace TowerDefenseWPF.Models;

public class Oleada
{
    public int Numero { get; init; }
    public Lista<GeneracionEnemigo> Generaciones { get; init; } = new();
    public int BonusRecompensa { get; init; }
}
